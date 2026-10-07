using Bam.Encryption;
using Bam.Protocol.Server;
using Bam.Test;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("SignedActorTokenVerifier Should", Selector = "satv")]
public class SignedActorTokenVerifierShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions Options = new ActorAuthenticationOptions();

    [UnitTest]
    public void VerifyAClientSignedTokenAgainstTheRegisteredEccKey()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));

        When.A<SignedActorTokenVerifier>("accepts a token signed by the registered key",
            () => new SignedActorTokenVerifier(resolver, Options),
            (verifier) => verifier.Verify(TestKeys.ClientToken(pair, "alice")))
            .TheTest
            .ShouldPass<ActorTokenVerification>((because, verification) =>
            {
                because.ItsTrue("verification succeeded", verification.Success);
                because.ItsTrue("the actor is the handle", verification.Actor is not null && verification.Actor.Handle == "alice");
                because.ItsTrue("the registered ECC pem is returned", verification.EccPublicKeyPem == pair.PublicPem);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RejectTokensThatFailEachCheck()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        EccKeyPair otherPair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair)).Add(TestKeys.KeySet("revoked", pair));
        resolver.Revoke("revoked");

        When.A<SignedActorTokenVerifier>("rejects malformed, over-lifetime, expired, unknown, revoked and wrong-key tokens",
            () => new SignedActorTokenVerifier(resolver, Options),
            (verifier) =>
            {
                BamJwtToken expired = new BamJwtToken("sid", "alice", "client", TimeSpan.FromMinutes(2))
                {
                    IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
                    Expiry = DateTimeOffset.UtcNow.AddMinutes(-8),
                };
                return new RejectionOutcome(
                    verifier.Verify("not-a-token"),
                    verifier.Verify(TestKeys.ClientToken(pair, "alice", TimeSpan.FromMinutes(30))),
                    verifier.Verify(expired.Encode(pair.PrivateKey.Value)),
                    verifier.Verify(TestKeys.ClientToken(pair, "nobody")),
                    verifier.Verify(TestKeys.ClientToken(pair, "revoked")),
                    verifier.Verify(TestKeys.ClientToken(otherPair, "alice")));
            })
            .TheTest
            .ShouldPass<RejectionOutcome>((because, outcome) =>
            {
                because.ItsTrue("malformed is rejected", !outcome.Malformed.Success && outcome.Malformed.Messages[0].Contains("format"));
                because.ItsTrue("over-lifetime is rejected", !outcome.OverLifetime.Success && outcome.OverLifetime.Messages[0].Contains("lifetime"));
                because.ItsTrue("expired is rejected", !outcome.Expired.Success && outcome.Expired.Messages[0].Contains("expired"));
                because.ItsTrue("unknown handle is rejected", !outcome.Unknown.Success && outcome.Unknown.Messages[0].Contains("No active key set"));
                because.ItsTrue("revoked handle is rejected", !outcome.Revoked.Success && outcome.Revoked.Messages[0].Contains("No active key set"));
                because.ItsTrue("wrong key is rejected", !outcome.WrongKey.Success && outcome.WrongKey.Messages[0].Contains("signature"));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record RejectionOutcome(
        ActorTokenVerification Malformed,
        ActorTokenVerification OverLifetime,
        ActorTokenVerification Expired,
        ActorTokenVerification Unknown,
        ActorTokenVerification Revoked,
        ActorTokenVerification WrongKey);
}

[UnitTestMenu("ServerIssuedTokenVerifier Should", Selector = "sitv")]
public class ServerIssuedTokenVerifierShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions Options = new ActorAuthenticationOptions();

    private static InMemoryNamedKeyStorage StorageWithServerKey(EccKeyPair serverPair)
    {
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();
        storage.SaveNamedKey(Options.ServerKeyName, TestKeys.PairPem(serverPair));
        return storage;
    }

    [UnitTest]
    public void VerifyAnIssuedTokenWhoseBindingMatchesTheActiveKeySet()
    {
        EccKeyPair clientPair = TestKeys.NewEcc();
        EccKeyPair serverPair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", clientPair));
        InMemoryNamedKeyStorage storage = StorageWithServerKey(serverPair);
        ServerActorTokenIssuer issuer = new ServerActorTokenIssuer(storage, Options);

        When.A<ServerIssuedTokenVerifier>("accepts a server-issued token bound to the active key",
            () => new ServerIssuedTokenVerifier(resolver, storage, Options),
            (verifier) =>
            {
                ActorTokenVerification proof = new SignedActorTokenVerifier(resolver, Options).Verify(TestKeys.ClientToken(clientPair, "alice"));
                IssuedActorToken issued = issuer.Issue(proof.Actor!, proof.EccPublicKeyPem!);
                return verifier.Verify(issued.Token);
            })
            .TheTest
            .ShouldPass<ActorTokenVerification>((because, verification) =>
            {
                because.ItsTrue("verification succeeded", verification.Success);
                because.ItsTrue("the actor is the subject", verification.Actor is not null && verification.Actor.Handle == "alice");
                because.ItsTrue("the registered ECC pem is returned for body proofs", verification.EccPublicKeyPem == clientPair.PublicPem);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotatedRevokedUnboundAndForeignTokens()
    {
        EccKeyPair clientPair = TestKeys.NewEcc();
        EccKeyPair serverPair = TestKeys.NewEcc();
        EccKeyPair otherServerPair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", clientPair)).Add(TestKeys.KeySet("bob", clientPair));
        InMemoryNamedKeyStorage storage = StorageWithServerKey(serverPair);
        ServerActorTokenIssuer issuer = new ServerActorTokenIssuer(storage, Options);

        When.A<ServerIssuedTokenVerifier>("rejects rotated, revoked, unbound, wrong-issuer and wrong-server-key tokens",
            () => new ServerIssuedTokenVerifier(resolver, storage, Options),
            (verifier) =>
            {
                IssuedActorToken alice = issuer.Issue(new Bam.Protocol.Data.Common.ActorData { Handle = "alice", Name = "alice" }, clientPair.PublicPem);
                IssuedActorToken bob = issuer.Issue(new Bam.Protocol.Data.Common.ActorData { Handle = "bob", Name = "bob" }, clientPair.PublicPem);
                resolver.Rotate("alice", TestKeys.NewEcc());
                resolver.Revoke("bob");

                BamJwtToken unbound = new BamJwtToken("sid", "alice", Options.Issuer, TimeSpan.FromMinutes(5));
                string unboundToken = unbound.Encode(serverPair.PrivateKey.Value);

                BamJwtToken wrongIssuer = new BamJwtToken("sid", "alice", "someone-else", TimeSpan.FromMinutes(5)) { KeyFingerprint = alice.KeyFingerprint };
                string wrongIssuerToken = wrongIssuer.Encode(serverPair.PrivateKey.Value);

                BamJwtToken foreign = new BamJwtToken("sid", "alice", Options.Issuer, TimeSpan.FromMinutes(5)) { KeyFingerprint = alice.KeyFingerprint };
                string foreignToken = foreign.Encode(otherServerPair.PrivateKey.Value);

                return new HybridRejectionOutcome(
                    verifier.Verify(alice.Token),
                    verifier.Verify(bob.Token),
                    verifier.Verify(unboundToken),
                    verifier.Verify(wrongIssuerToken),
                    verifier.Verify(foreignToken));
            })
            .TheTest
            .ShouldPass<HybridRejectionOutcome>((because, outcome) =>
            {
                because.ItsTrue("a rotated key set invalidates the outstanding token", !outcome.Rotated.Success && outcome.Rotated.Messages[0].Contains("binding"));
                because.ItsTrue("a revoked key set invalidates the outstanding token", !outcome.Revoked.Success && outcome.Revoked.Messages[0].Contains("No active key set"));
                because.ItsTrue("a token without kfp is rejected in hybrid mode", !outcome.Unbound.Success && outcome.Unbound.Messages[0].Contains("kfp"));
                because.ItsTrue("a foreign issuer is rejected", !outcome.WrongIssuer.Success && outcome.WrongIssuer.Messages[0].Contains("issuer"));
                because.ItsTrue("a token signed by another server key is rejected", !outcome.Foreign.Success && outcome.Foreign.Messages[0].Contains("server key"));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record HybridRejectionOutcome(
        ActorTokenVerification Rotated,
        ActorTokenVerification Revoked,
        ActorTokenVerification Unbound,
        ActorTokenVerification WrongIssuer,
        ActorTokenVerification Foreign);
}

[UnitTestMenu("ServerActorTokenIssuer Should", Selector = "sati")]
public class ServerActorTokenIssuerShould : UnitTestMenuContainer
{
    [UnitTest]
    public void IssueTokensSignedByTheServerKeyAndBoundToTheActorKey()
    {
        ActorAuthenticationOptions options = new ActorAuthenticationOptions { ServerTokenLifetime = TimeSpan.FromMinutes(30) };
        EccKeyPair serverPair = TestKeys.NewEcc();
        EccKeyPair clientPair = TestKeys.NewEcc();
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();
        storage.SaveNamedKey(options.ServerKeyName, TestKeys.PairPem(serverPair));

        When.A<ServerActorTokenIssuer>("mints a server-signed, key-bound token",
            () => new ServerActorTokenIssuer(storage, options),
            (issuer) =>
            {
                IssuedActorToken issued = issuer.Issue(new Bam.Protocol.Data.Common.ActorData { Handle = "alice", Name = "alice" }, clientPair.PublicPem);
                BamJwtToken decoded = BamJwtToken.Decode(issued.Token);
                return new IssueOutcome(
                    BamJwtToken.Verify(issued.Token, serverPair.PublicKey.Value),
                    decoded.KeyFingerprint == Bam.Protocol.Profile.PublicKeyFingerprint.Of(clientPair.PublicPem),
                    decoded.ActorHandle,
                    decoded.Expiry - decoded.IssuedAt);
            })
            .TheTest
            .ShouldPass<IssueOutcome>((because, outcome) =>
            {
                because.ItsTrue("the token verifies with the server public key", outcome.VerifiesWithServerKey);
                because.ItsTrue("kfp is the canonical fingerprint of the actor key", outcome.KfpMatches);
                because.ItsTrue("sub is the actor handle", outcome.Subject == "alice");
                because.ItsTrue("lifetime honors the option", outcome.Lifetime == TimeSpan.FromMinutes(30));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void FailFastWithoutAServerKey()
    {
        When.A<ServerActorTokenIssuer>("refuses to issue when no server key is stored",
            () => new ServerActorTokenIssuer(new InMemoryNamedKeyStorage(), new ActorAuthenticationOptions()),
            (issuer) =>
            {
                try
                {
                    issuer.Issue(new Bam.Protocol.Data.Common.ActorData { Handle = "alice", Name = "alice" }, TestKeys.NewEcc().PublicPem);
                    return null;
                }
                catch (InvalidOperationException failure)
                {
                    return failure;
                }
            })
            .TheTest
            .ShouldPass<InvalidOperationException?>((because, failure) =>
            {
                because.ItsTrue("issuance threw", failure is not null);
                because.ItsTrue("the message names the key setting", failure!.Message.Contains("ServerKeyName"));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record IssueOutcome(bool VerifiesWithServerKey, bool KfpMatches, string Subject, TimeSpan Lifetime);
}
