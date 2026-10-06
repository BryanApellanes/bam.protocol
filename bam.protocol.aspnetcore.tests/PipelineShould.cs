using Bam.Encryption;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("ActorAuthenticationMiddleware Should", Selector = "aam")]
public class ActorAuthenticationMiddlewareShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions Options = new ActorAuthenticationOptions();

    private static readonly IActorAdmission Everyone = new ConfiguredActorAdmission(new ActorAuthenticationOptions { OpenEnrollment = true });

    private static async Task<MiddlewareOutcome> RunAsync(DefaultHttpContext context, IActorTokenVerifier verifier, IActorAdmission? admission = null)
    {
        bool nextCalled = false;
        ActorAuthenticationMiddleware middleware = new ActorAuthenticationMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        await middleware.InvokeAsync(context, verifier, new AnonymousActorProvider(), admission ?? Everyone);
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

    private static async Task<string> BodyOfRejectionAsync(DefaultHttpContext context, IActorTokenVerifier verifier, IActorAdmission? admission = null)
    {
        MemoryStream body = new MemoryStream();
        context.Response.Body = body;
        await RunAsync(context, verifier, admission);
        return context.Response.StatusCode + ":" + Encoding.UTF8.GetString(body.ToArray());
    }

    [UnitTest]
    public void AnswerEveryTokenFailureWithTheSameBody()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));
        SignedActorTokenVerifier verifier = new SignedActorTokenVerifier(resolver, Options);

        When.A<SignedActorTokenVerifier>("rejects an unknown handle and a bad signature identically",
            () => verifier,
            (subject) =>
            {
                DefaultHttpContext unknown = Requests.WithEndpoint();
                unknown.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(pair, "nobody");
                DefaultHttpContext forged = Requests.WithEndpoint();
                forged.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(TestKeys.NewEcc(), "alice");
                DefaultHttpContext missing = Requests.WithEndpoint();
                return new SameBodyOutcome(
                    BodyOfRejectionAsync(unknown, subject).GetAwaiter().GetResult(),
                    BodyOfRejectionAsync(forged, subject).GetAwaiter().GetResult(),
                    BodyOfRejectionAsync(missing, subject).GetAwaiter().GetResult());
            })
            .TheTest
            .ShouldPass<SameBodyOutcome>((because, outcome) =>
            {
                because.ItsTrue($"an unknown handle gets the generic 401 ({outcome.UnknownHandle})", outcome.UnknownHandle == "401:" + ActorAuthenticationMiddleware.FailureMessage);
                because.ItsTrue("a bad signature gets a byte-identical response", outcome.BadSignature == outcome.UnknownHandle);
                because.ItsTrue("a missing header gets the same response", outcome.Missing == outcome.UnknownHandle);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void AuthenticateOnlyOnEndpointsMappedWithoutRequireActorAccess()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));

        When.A<ActorAuthenticationOptions>("lets an admitted actor's valid token through an unmarked endpoint with no body signature",
            () => new ActorAuthenticationOptions { AdmittedHandles = new[] { "alice" } },
            (options) =>
            {
                DefaultHttpContext unmarked = Requests.WithEndpoint().WithBody("{\"unsigned\":true}");
                unmarked.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(pair, "alice");
                return RunAsync(unmarked, new SignedActorTokenVerifier(resolver, options), new ConfiguredActorAdmission(options)).GetAwaiter().GetResult();
            })
            .TheTest
            .ShouldPass<MiddlewareOutcome>((because, outcome) =>
            {
                because.ItsTrue("the documented behaviour: the middleware authenticates and continues", outcome.NextCalled);
                because.ItsTrue("the actor is recorded for any later check", outcome.ActorHandle == "alice");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseAnUnadmittedActorOnEveryNonAnonymousEndpoint()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        EccKeyPair carolPair = TestKeys.NewEcc();
        EccKeyPair bobPair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver()
            .Add(TestKeys.KeySet("alice", pair))
            .Add(TestKeys.KeySet("bob", bobPair))
            .Add(TestKeys.KeySet("carol", carolPair));
        string alicePin = PublicKeyFingerprint.Of(pair.PublicPem)!;
        string wrongPin = PublicKeyFingerprint.Of(TestKeys.NewEcc().PublicPem)!;

        When.A<ActorAuthenticationOptions>("checks admission, with any key pin, before continuing",
            () => new ActorAuthenticationOptions { AdmittedHandles = new[] { "alice@" + alicePin, "carol@" + wrongPin } },
            (options) =>
            {
                SignedActorTokenVerifier verifier = new SignedActorTokenVerifier(resolver, options);
                ConfiguredActorAdmission admission = new ConfiguredActorAdmission(options);
                DefaultHttpContext unadmitted = Requests.WithEndpoint();
                unadmitted.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(bobPair, "bob");
                DefaultHttpContext pinnedElsewhere = Requests.WithEndpoint();
                pinnedElsewhere.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(carolPair, "carol");
                DefaultHttpContext admitted = Requests.WithEndpoint();
                admitted.Request.Headers[Headers.Authorization] = "Bearer " + TestKeys.ClientToken(pair, "alice");
                return new UnadmittedOutcome(
                    BodyOfRejectionAsync(unadmitted, verifier, admission).GetAwaiter().GetResult(),
                    BodyOfRejectionAsync(pinnedElsewhere, verifier, admission).GetAwaiter().GetResult(),
                    RunAsync(admitted, verifier, admission).GetAwaiter().GetResult());
            })
            .TheTest
            .ShouldPass<UnadmittedOutcome>((because, outcome) =>
            {
                because.ItsTrue($"an unadmitted actor gets the generic 401 on an unmarked endpoint ({outcome.Unadmitted})", outcome.Unadmitted == "401:" + ActorAuthenticationMiddleware.FailureMessage);
                because.ItsTrue("an actor whose key doesn't match its pin gets the same 401", outcome.PinnedToAnotherKey == outcome.Unadmitted);
                because.ItsTrue("an admitted actor with its pinned key continues", outcome.Admitted.NextCalled && outcome.Admitted.ActorHandle == "alice");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record UnadmittedOutcome(string Unadmitted, string PinnedToAnotherKey, MiddlewareOutcome Admitted);

    private sealed record SameBodyOutcome(string UnknownHandle, string BadSignature, string Missing);

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
        ActorAuthenticationOptions writeOptions = new ActorAuthenticationOptions { EnrolledActorAccess = BamAccess.Execute, OpenEnrollment = true };
        ActorData actor = new ActorData { Handle = "alice", Name = "alice" };

        When.A<ActorAccessEndpointFilter>("compares held access against required access",
            () => new ActorAccessEndpointFilter(new ConfiguredActorAccessPolicy(writeOptions, new ConfiguredActorAdmission(writeOptions))),
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

    [UnitTest]
    public void RefuseAnUnadmittedActorAtTheAccessFilter()
    {
        ActorData fresh = new ActorData { Handle = "fresh", Name = "fresh" };

        When.A<ActorAuthenticationOptions>("forbids a freshly enrolled actor until it is admitted",
            () => new ActorAuthenticationOptions(),
            (closed) =>
            {
                ActorAuthenticationOptions listed = new ActorAuthenticationOptions { AdmittedHandles = new[] { "fresh" } };
                ActorAuthenticationOptions open = new ActorAuthenticationOptions { OpenEnrollment = true };
                return new AdmissionFilterOutcome(
                    StatusOf(RunAsync(Filter(closed), Execute(fresh)).GetAwaiter().GetResult()),
                    RunAsync(Filter(listed), Execute(fresh)).GetAwaiter().GetResult(),
                    RunAsync(Filter(open), Execute(fresh)).GetAwaiter().GetResult());
            })
            .TheTest
            .ShouldPass<AdmissionFilterOutcome>((because, outcome) =>
            {
                because.ItsTrue("an unadmitted actor gets 403 on an Execute endpoint", outcome.ClosedStatus == 403);
                because.ItsTrue("an allow-listed actor reaches the handler", Equals(outcome.Listed, "handled"));
                because.ItsTrue("open enrollment lets it through", Equals(outcome.Open, "handled"));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void EvaluateTheProofRuleOutsideTheFilter()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        string body = "{\"goal\":\"proxy hello\"}";
        BodySignatureProofVerifier proof = new BodySignatureProofVerifier();

        When.A<BodySignatureProofVerifier>("evaluates a request directly, as a proxy host would",
            () => proof,
            (subject) =>
            {
                DefaultHttpContext valid = Requests.WithEndpoint().WithBody(body);
                valid.Request.Headers[Headers.BodySignature] = TestKeys.BodySignature(pair, body);
                DefaultHttpContext missing = Requests.WithEndpoint().WithBody(body);
                DefaultHttpContext forged = Requests.WithEndpoint().WithBody(body);
                forged.Request.Headers[Headers.BodySignature] = TestKeys.BodySignature(TestKeys.NewEcc(), body);
                DefaultHttpContext algorithm = Requests.WithEndpoint().WithBody(body);
                algorithm.Request.Headers[Headers.BodySignature] = TestKeys.BodySignature(pair, body);
                algorithm.Request.Headers[Headers.BodySignatureAlgorithm] = "NONEwithECDSA";
                RequestProofResult validResult = RequestProofRule.EvaluateAsync(valid.Request, subject, pair.PublicPem).GetAwaiter().GetResult();
                string afterRead = new StreamReader(valid.Request.Body).ReadToEnd();
                return new EvaluateOutcome(
                    validResult.Success,
                    afterRead == body,
                    RequestProofRule.EvaluateAsync(missing.Request, subject, pair.PublicPem).GetAwaiter().GetResult().Success,
                    RequestProofRule.EvaluateAsync(forged.Request, subject, pair.PublicPem).GetAwaiter().GetResult().Success,
                    RequestProofRule.EvaluateAsync(algorithm.Request, subject, pair.PublicPem).GetAwaiter().GetResult().Success,
                    RequestProofRule.EvaluateAsync(valid.Request, subject, null).GetAwaiter().GetResult().Success);
            })
            .TheTest
            .ShouldPass<EvaluateOutcome>((because, outcome) =>
            {
                because.ItsTrue("a valid signature verifies", outcome.Valid);
                because.ItsTrue("the body is rewound for the handler", outcome.BodyRewound);
                because.ItsTrue("a missing signature fails", !outcome.Missing);
                because.ItsTrue("another key's signature fails", !outcome.Forged);
                because.ItsTrue("a disallowed algorithm fails", !outcome.DisallowedAlgorithm);
                because.ItsTrue("no registered key fails", !outcome.NoKey);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static ActorAccessEndpointFilter Filter(ActorAuthenticationOptions options)
    {
        return new ActorAccessEndpointFilter(new ConfiguredActorAccessPolicy(options, new ConfiguredActorAdmission(options)));
    }

    private static DefaultHttpContext Execute(ActorData actor)
    {
        DefaultHttpContext context = Requests.WithEndpoint(new RequiredAccessAttribute(BamAccess.Execute));
        context.SetActor(actor, null);
        return context;
    }

    private sealed record AdmissionFilterOutcome(int? ClosedStatus, object? Listed, object? Open);

    private sealed record EvaluateOutcome(bool Valid, bool BodyRewound, bool Missing, bool Forged, bool DisallowedAlgorithm, bool NoKey);

    private sealed record ProofRuleOutcome(bool Anonymous, bool Read, bool Execute, bool Write, bool ForcedRead, bool WaivedWrite, bool NoMetadata);

    private sealed record ProofFilterOutcome(object? Signed, int? UnsignedStatus, int? ForgedStatus, object? ReadOnly);

    private sealed record AccessOutcome(object? Allowed, int? ForbiddenStatus, int? UnauthenticatedStatus, object? AnonymousOk);
}
