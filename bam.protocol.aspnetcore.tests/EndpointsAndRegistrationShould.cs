using System.Text;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.UserAccounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("Actor endpoints Should", Selector = "aep")]
public class ActorEndpointsShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions Options = new ActorAuthenticationOptions();

    private static int? StatusOf(IResult result)
    {
        return (result as IStatusCodeHttpResult)?.StatusCode;
    }

    [UnitTest]
    public void IssueAServerTokenForAProvenClientTokenAndRefuseOthers()
    {
        EccKeyPair clientPair = TestKeys.NewEcc();
        EccKeyPair serverPair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", clientPair));
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();
        storage.SaveNamedKey(Options.ServerKeyName, TestKeys.PairPem(serverPair));
        SignedActorTokenVerifier proof = new SignedActorTokenVerifier(resolver, Options);
        ServerActorTokenIssuer issuer = new ServerActorTokenIssuer(storage, Options);

        When.A<ActorAuthenticationOptions>("issues on a valid client token and 401s otherwise",
            () => Options,
            (_) =>
            {
                IResult ok = ActorTokenEndpoints.Issue(new ActorTokenRequest { ClientToken = TestKeys.ClientToken(clientPair, "alice") }, proof, issuer);
                IResult refused = ActorTokenEndpoints.Issue(new ActorTokenRequest { ClientToken = TestKeys.ClientToken(TestKeys.NewEcc(), "alice") }, proof, issuer);
                ActorTokenResponse? response = (ok as Ok<ActorTokenResponse>)?.Value;
                bool serverVerifies = response is not null && new ServerIssuedTokenVerifier(resolver, storage, Options).Verify(response.Token).Success;
                return new IssueEndpointOutcome(StatusOf(ok), serverVerifies, StatusOf(refused));
            })
            .TheTest
            .ShouldPass<IssueEndpointOutcome>((because, outcome) =>
            {
                because.ItsTrue("a proven client token yields 200", outcome.OkStatus == 200);
                because.ItsTrue("the issued token verifies in hybrid mode", outcome.IssuedVerifies);
                because.ItsTrue("an unproven client token yields 401", outcome.RefusedStatus == 401);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RunTheEnrollmentRoundTripOverTheExistingFlows()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        byte[] accepted = Encoding.UTF8.GetBytes("signed-ok");
        FakeAccountConfirmation confirmation = new FakeAccountConfirmation(accepted);
        FakeKeySetRegistrar registrar = new FakeKeySetRegistrar();
        FakeKeySetRegistrar refusing = new FakeKeySetRegistrar { Refuse = true };

        When.A<ActorAuthenticationOptions>("registers, challenges and confirms; refuses bad registrations and signatures",
            () => Options,
            (_) =>
            {
                ActorRegistrationRequest registration = new ActorRegistrationRequest { KeySetHandle = "alice", PublicRsaKeyPem = "rsa-pem", PublicEccKeyPem = pair.PublicPem };
                IResult registered = ActorEnrollmentEndpoints.Register(registration, registrar);
                IResult refused = ActorEnrollmentEndpoints.Register(registration, refusing);
                IResult challenged = ActorEnrollmentEndpoints.Challenge(new ActorChallengeRequest { PersonHandle = "alice" }, confirmation);
                ActorChallengeResponse? challenge = (challenged as Ok<ActorChallengeResponse>)?.Value;
                IResult confirmed = ActorEnrollmentEndpoints.Confirm(new ActorConfirmRequest { PersonHandle = "alice", SignatureBase64 = Convert.ToBase64String(accepted) }, confirmation);
                IResult denied = ActorEnrollmentEndpoints.Confirm(new ActorConfirmRequest { PersonHandle = "alice", SignatureBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("wrong")) }, confirmation);
                IResult garbage = ActorEnrollmentEndpoints.Confirm(new ActorConfirmRequest { PersonHandle = "alice", SignatureBase64 = "%%%" }, confirmation);
                return new EnrollmentOutcome(
                    StatusOf(registered),
                    registrar.Registered?.PublicEccKey == pair.PublicPem,
                    StatusOf(refused),
                    challenge is not null && challenge.ChallengeBase64 == Convert.ToBase64String(Encoding.UTF8.GetBytes("challenge-for-alice")),
                    StatusOf(confirmed),
                    StatusOf(denied),
                    StatusOf(garbage));
            })
            .TheTest
            .ShouldPass<EnrollmentOutcome>((because, outcome) =>
            {
                because.ItsTrue("registration returns 200", outcome.RegisteredStatus == 200);
                because.ItsTrue("the registrar received the ECC pem", outcome.RegistrarGotEcc);
                because.ItsTrue("a refused registration returns 400", outcome.RefusedStatus == 400);
                because.ItsTrue("the challenge carries the flow's bytes", outcome.ChallengeMatches);
                because.ItsTrue("a matching signature confirms", outcome.ConfirmedStatus == 200);
                because.ItsTrue("a wrong signature is 401", outcome.DeniedStatus == 401);
                because.ItsTrue("garbage base64 is 401 without throwing", outcome.GarbageStatus == 401);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record IssueEndpointOutcome(int? OkStatus, bool IssuedVerifies, int? RefusedStatus);

    private sealed record EnrollmentOutcome(int? RegisteredStatus, bool RegistrarGotEcc, int? RefusedStatus, bool ChallengeMatches, int? ConfirmedStatus, int? DeniedStatus, int? GarbageStatus);
}

[UnitTestMenu("ActorAuthenticationRegistration Should", Selector = "aar")]
public class ActorAuthenticationRegistrationShould : UnitTestMenuContainer
{
    private static IServiceCollection HostServices(FakeKeySetResolver resolver, InMemoryNamedKeyStorage storage)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<IPublicKeySetResolver>(resolver);
        services.AddSingleton<IPublicKeySetRegistrar>(new FakeKeySetRegistrar());
        services.AddSingleton<IAccountConfirmation>(new FakeAccountConfirmation(Array.Empty<byte>()));
        services.AddSingleton<INamedKeyStorage>(storage);
        return services;
    }

    [UnitTest]
    public void BindTheVerifierByModeInTheServiceCollection()
    {
        FakeKeySetResolver resolver = new FakeKeySetResolver();
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();

        When.A<ActorAuthenticationOptions>("binds hybrid and client-signed verifiers by mode",
            () => new ActorAuthenticationOptions(),
            (_) =>
            {
                ServiceProvider hybrid = HostServices(resolver, storage).AddActorAuthentication(new ActorAuthenticationOptions { Mode = ActorAuthenticationMode.Hybrid }).BuildServiceProvider();
                ServiceProvider client = HostServices(resolver, storage).AddActorAuthentication(new ActorAuthenticationOptions { Mode = ActorAuthenticationMode.ClientSignedOnly }).BuildServiceProvider();
                return new BindingOutcome(
                    hybrid.GetRequiredService<IActorTokenVerifier>(),
                    client.GetRequiredService<IActorTokenVerifier>(),
                    hybrid.GetRequiredService<IRequestProof>(),
                    hybrid.GetRequiredService<IActorAccessPolicy>(),
                    hybrid.GetRequiredService<IAnonymousActorProvider>());
            })
            .TheTest
            .ShouldPass<BindingOutcome>((because, outcome) =>
            {
                because.ItsTrue("hybrid binds the server-issued verifier", outcome.Hybrid is ServerIssuedTokenVerifier);
                because.ItsTrue("client-signed-only binds the signed verifier", outcome.Client is SignedActorTokenVerifier);
                because.ItsTrue("the body-signature proof is bound", outcome.Proof is BodySignatureProofVerifier);
                because.ItsTrue("the configured policy is bound", outcome.Policy is ConfiguredActorAccessPolicy);
                because.ItsTrue("an anonymous actor provider is supplied when the host has none", outcome.Anonymous is AnonymousActorProvider);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void FailFastOnMissingPrerequisitesAndMissingServerKey()
    {
        When.A<ActorAuthenticationOptions>("names missing contracts and the missing server key",
            () => new ActorAuthenticationOptions { Mode = ActorAuthenticationMode.Hybrid },
            (options) =>
            {
                ServiceProvider empty = new ServiceCollection().AddActorAuthentication(options).BuildServiceProvider();
                string? missingMessage = Catch(() => ActorAuthenticationRegistration.VerifyPrerequisites(empty, options));

                InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();
                ServiceProvider noKey = HostServices(new FakeKeySetResolver(), storage).AddActorAuthentication(options).BuildServiceProvider();
                string? noKeyMessage = Catch(() => ActorAuthenticationRegistration.VerifyPrerequisites(noKey, options));

                storage.SaveNamedKey(options.ServerKeyName, TestKeys.PairPem(TestKeys.NewEcc()));
                string? okMessage = Catch(() => ActorAuthenticationRegistration.VerifyPrerequisites(noKey, options));

                return new PrerequisiteOutcome(missingMessage, noKeyMessage, okMessage);
            })
            .TheTest
            .ShouldPass<PrerequisiteOutcome>((because, outcome) =>
            {
                because.ItsTrue("missing contracts are named", outcome.Missing is not null && outcome.Missing.Contains("IPublicKeySetResolver") && outcome.Missing.Contains("IAccountConfirmation"));
                because.ItsTrue("a missing server key is named", outcome.NoKey is not null && outcome.NoKey.Contains("ServerKeyName"));
                because.ItsTrue("a complete host passes", outcome.Ok is null);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ProvisionTheServerKeyOnceAndVerifyWithIt()
    {
        ActorAuthenticationOptions options = new ActorAuthenticationOptions();
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();
        EccKeyPair clientPair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", clientPair));

        When.A<ActorAuthenticationOptions>("generates a server key on first call, keeps it afterwards, and it signs verifiable tokens",
            () => options,
            (_) =>
            {
                bool first = ActorAuthenticationRegistration.EnsureServerKey(storage, options);
                byte[] afterFirst = storage.GetNamedKey(options.ServerKeyName)!;
                bool second = ActorAuthenticationRegistration.EnsureServerKey(storage, options);
                byte[] afterSecond = storage.GetNamedKey(options.ServerKeyName)!;
                IssuedActorToken issued = new ServerActorTokenIssuer(storage, options).Issue(new Bam.Protocol.Data.Common.ActorData { Handle = "alice", Name = "alice" }, clientPair.PublicPem);
                bool verifies = new ServerIssuedTokenVerifier(resolver, storage, options).Verify(issued.Token).Success;
                return new ProvisionOutcome(first, second, afterFirst.AsSpan().SequenceEqual(afterSecond), verifies);
            })
            .TheTest
            .ShouldPass<ProvisionOutcome>((because, outcome) =>
            {
                because.ItsTrue("the first call generates a key", outcome.First);
                because.ItsTrue("the second call is a no-op", !outcome.Second);
                because.ItsTrue("the stored key is unchanged by the second call", outcome.Unchanged);
                because.ItsTrue("tokens issued with the provisioned key verify", outcome.Verifies);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RegisterIntoTheBamtkServiceRegistry()
    {
        FakeKeySetResolver resolver = new FakeKeySetResolver();
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();

        When.A<ServiceRegistry>("resolves the adapter contracts from a ServiceRegistry",
            () =>
            {
                ServiceRegistry registry = ServiceRegistry.Create();
                registry.For<IPublicKeySetResolver>().UseSingleton<IPublicKeySetResolver>(resolver);
                registry.For<INamedKeyStorage>().UseSingleton<INamedKeyStorage>(storage);
                return registry.AddActorAuthentication(new ActorAuthenticationOptions { Mode = ActorAuthenticationMode.ClientSignedOnly });
            },
            (registry) => new RegistryOutcome(registry.Get<IActorTokenVerifier>(), registry.Get<IRequestProof>(), registry.Get<IActorAccessPolicy>()))
            .TheTest
            .ShouldPass<RegistryOutcome>((because, outcome) =>
            {
                because.ItsTrue("the verifier resolves per mode", outcome.Verifier is SignedActorTokenVerifier);
                because.ItsTrue("the proof resolves", outcome.Proof is BodySignatureProofVerifier);
                because.ItsTrue("the policy resolves", outcome.Policy is ConfiguredActorAccessPolicy);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static string? Catch(Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (InvalidOperationException failure)
        {
            return failure.Message;
        }
    }

    private sealed record BindingOutcome(IActorTokenVerifier Hybrid, IActorTokenVerifier Client, IRequestProof Proof, IActorAccessPolicy Policy, IAnonymousActorProvider Anonymous);

    private sealed record PrerequisiteOutcome(string? Missing, string? NoKey, string? Ok);

    private sealed record ProvisionOutcome(bool First, bool Second, bool Unchanged, bool Verifies);

    private sealed record RegistryOutcome(IActorTokenVerifier Verifier, IRequestProof Proof, IActorAccessPolicy Policy);
}
