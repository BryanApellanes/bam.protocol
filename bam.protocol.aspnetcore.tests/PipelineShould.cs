using Bam.Encryption;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("ActorAuthenticationMiddleware Should", Selector = "aam")]
public class ActorAuthenticationMiddlewareShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions Options = new ActorAuthenticationOptions();

    private static async Task<MiddlewareOutcome> RunAsync(DefaultHttpContext context, IActorTokenVerifier verifier)
    {
        bool nextCalled = false;
        ActorAuthenticationMiddleware middleware = new ActorAuthenticationMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        await middleware.InvokeAsync(context, verifier, new AnonymousActorProvider());
        return new MiddlewareOutcome(nextCalled, context.Response.StatusCode, context.GetActor()?.Handle, context.GetActorEccPublicKeyPem());
    }

    [UnitTest]
    public void PassAnonymousEndpointsThroughWithTheSentinel()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));

        When.A<ActorAuthenticationOptions>("lets anonymous endpoints through without a token",
            () => Options,
            (_) => RunAsync(Requests.WithEndpoint(new AnonymousAccessAttribute()), new SignedActorTokenVerifier(resolver, Options)).GetAwaiter().GetResult())
            .TheTest
            .ShouldPass<MiddlewareOutcome>((because, outcome) =>
            {
                because.ItsTrue("the pipeline continued", outcome.NextCalled);
                because.ItsTrue("the anonymous sentinel was recorded", outcome.ActorHandle == AnonymousActorProvider.AnonymousHandle);
                because.ItsTrue("no key pem is recorded for anonymous", outcome.EccPem is null);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RejectMissingBadAndUnverifiableTokensAndAcceptValidOnes()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));
        SignedActorTokenVerifier verifier = new SignedActorTokenVerifier(resolver, Options);

        When.A<ActorAuthenticationOptions>("401s without a valid bearer token and records the actor with one",
            () => Options,
            (_) =>
            {
                DefaultHttpContext missing = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute));
                DefaultHttpContext basic = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute));
                basic.Request.Headers[Headers.Authorization] = "Basic abc";
                DefaultHttpContext bad = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute));
                bad.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(TestKeys.NewEcc(), "alice");
                DefaultHttpContext good = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute));
                good.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(pair, "alice");
                return new GateOutcome(
                    RunAsync(missing, verifier).GetAwaiter().GetResult(),
                    RunAsync(basic, verifier).GetAwaiter().GetResult(),
                    RunAsync(bad, verifier).GetAwaiter().GetResult(),
                    RunAsync(good, verifier).GetAwaiter().GetResult());
            })
            .TheTest
            .ShouldPass<GateOutcome>((because, outcome) =>
            {
                because.ItsTrue("a missing header 401s without continuing", outcome.Missing.StatusCode == 401 && !outcome.Missing.NextCalled);
                because.ItsTrue("a non-Bearer scheme 401s", outcome.Basic.StatusCode == 401 && !outcome.Basic.NextCalled);
                because.ItsTrue("an unverifiable token 401s", outcome.Bad.StatusCode == 401 && !outcome.Bad.NextCalled);
                because.ItsTrue("a valid token continues with the actor recorded", outcome.Good.NextCalled && outcome.Good.ActorHandle == "alice" && outcome.Good.EccPem == pair.PublicPem);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record MiddlewareOutcome(bool NextCalled, int StatusCode, string? ActorHandle, string? EccPem);

    private sealed record GateOutcome(MiddlewareOutcome Missing, MiddlewareOutcome Basic, MiddlewareOutcome Bad, MiddlewareOutcome Good);
}

[UnitTestMenu("Endpoint filters Should", Selector = "epf")]
public class EndpointFiltersShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions Options = new ActorAuthenticationOptions();

    private static int? StatusOf(object? result)
    {
        return (result as IStatusCodeHttpResult)?.StatusCode;
    }

    private static async Task<object?> RunAsync(IEndpointFilter filter, DefaultHttpContext context)
    {
        DefaultEndpointFilterInvocationContext invocation = new DefaultEndpointFilterInvocationContext(context);
        return await filter.InvokeAsync(invocation, _ => ValueTask.FromResult<object?>("handled"));
    }

    [UnitTest]
    public void DeriveTheProofRequirementFromMetadataAndOptions()
    {
        When.A<ActorAuthenticationOptions>("derives proof requirements",
            () => Options,
            (options) => new ProofRuleOutcome(
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint(new AnonymousAccessAttribute()).GetEndpoint(), options),
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Read)).GetEndpoint(), options),
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute)).GetEndpoint(), options),
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Write)).GetEndpoint(), options),
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Read), new RequireRequestProofAttribute()).GetEndpoint(), options),
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Write), new RequireRequestProofAttribute(false)).GetEndpoint(), options),
                RequestProofEndpointFilter.RequiresProof(Requests.WithEndpoint().GetEndpoint(), options)))
            .TheTest
            .ShouldPass<ProofRuleOutcome>((because, outcome) =>
            {
                because.ItsTrue("anonymous endpoints never require proof", !outcome.Anonymous);
                because.ItsTrue("Read endpoints are token-only by default", !outcome.Read);
                because.ItsTrue("Execute endpoints require proof", outcome.Execute);
                because.ItsTrue("Write endpoints require proof", outcome.Write);
                because.ItsTrue("the attribute can force proof on a Read endpoint", outcome.ForcedRead);
                because.ItsTrue("the attribute can waive proof on a Write endpoint", !outcome.WaivedWrite);
                because.ItsTrue("an endpoint with no metadata defaults to Execute and requires proof", outcome.NoMetadata);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void EnforceRequestProofOverTheRawBody()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        string body = "{\"goal\":\"write hello\"}";
        ActorData actor = new ActorData { Handle = "alice", Name = "alice" };

        When.A<RequestProofEndpointFilter>("accepts a signed body and rejects missing or bad signatures",
            () => new RequestProofEndpointFilter(new BodySignatureProofVerifier(), Options),
            (filter) =>
            {
                DefaultHttpContext signed = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute)).WithBody(body);
                signed.SetActor(actor, pair.PublicPem);
                signed.Request.Headers[Headers.BodySignature] = TestKeys.BodySignature(pair, body);

                DefaultHttpContext unsigned = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute)).WithBody(body);
                unsigned.SetActor(actor, pair.PublicPem);

                DefaultHttpContext forged = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute)).WithBody(body);
                forged.SetActor(actor, pair.PublicPem);
                forged.Request.Headers[Headers.BodySignature] = TestKeys.BodySignature(TestKeys.NewEcc(), body);

                DefaultHttpContext readOnly = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Read)).WithBody(body);
                readOnly.SetActor(actor, pair.PublicPem);

                return new ProofFilterOutcome(
                    RunAsync(filter, signed).GetAwaiter().GetResult(),
                    StatusOf(RunAsync(filter, unsigned).GetAwaiter().GetResult()),
                    StatusOf(RunAsync(filter, forged).GetAwaiter().GetResult()),
                    RunAsync(filter, readOnly).GetAwaiter().GetResult());
            })
            .TheTest
            .ShouldPass<ProofFilterOutcome>((because, outcome) =>
            {
                because.ItsTrue("a correctly signed body reaches the handler", Equals(outcome.Signed, "handled"));
                because.ItsTrue("a missing signature is 401", outcome.UnsignedStatus == 401);
                because.ItsTrue("a signature by another key is 401", outcome.ForgedStatus == 401);
                because.ItsTrue("a Read endpoint needs no proof", Equals(outcome.ReadOnly, "handled"));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void EnforceRequiredAccessWithCalculatorSemantics()
    {
        ActorAuthenticationOptions writeOptions = new ActorAuthenticationOptions { EnrolledActorAccess = BamAccess.Execute };
        ActorData actor = new ActorData { Handle = "alice", Name = "alice" };

        When.A<ActorAccessEndpointFilter>("compares held access against required access",
            () => new ActorAccessEndpointFilter(new ConfiguredActorAccessPolicy(writeOptions)),
            (filter) =>
            {
                DefaultHttpContext allowed = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute));
                allowed.SetActor(actor, null);
                DefaultHttpContext forbidden = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Write));
                forbidden.SetActor(actor, null);
                DefaultHttpContext unauthenticated = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Read));
                DefaultHttpContext anonymousOk = Requests.WithEndpoint(new AnonymousAccessAttribute(), new RequiredAccessAttribute(BamAccess.Write));
                anonymousOk.SetActor(new AnonymousActorProvider().GetAnonymousActor(), null);
                return new AccessOutcome(
                    RunAsync(filter, allowed).GetAwaiter().GetResult(),
                    StatusOf(RunAsync(filter, forbidden).GetAwaiter().GetResult()),
                    StatusOf(RunAsync(filter, unauthenticated).GetAwaiter().GetResult()),
                    RunAsync(filter, anonymousOk).GetAwaiter().GetResult());
            })
            .TheTest
            .ShouldPass<AccessOutcome>((because, outcome) =>
            {
                because.ItsTrue("sufficient access reaches the handler", Equals(outcome.Allowed, "handled"));
                because.ItsTrue("insufficient access is 403", outcome.ForbiddenStatus == 403);
                because.ItsTrue("no actor on the context is 401", outcome.UnauthenticatedStatus == 401);
                because.ItsTrue("anonymous-marked endpoints bypass the access check", Equals(outcome.AnonymousOk, "handled"));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record ProofRuleOutcome(bool Anonymous, bool Read, bool Execute, bool Write, bool ForcedRead, bool WaivedWrite, bool NoMetadata);

    private sealed record ProofFilterOutcome(object? Signed, int? UnsignedStatus, int? ForgedStatus, object? ReadOnly);

    private sealed record AccessOutcome(object? Allowed, int? ForbiddenStatus, int? UnauthenticatedStatus, object? AnonymousOk);
}
