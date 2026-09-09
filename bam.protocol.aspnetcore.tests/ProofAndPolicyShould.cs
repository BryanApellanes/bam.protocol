using Bam.Encryption;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Server;
using Bam.Test;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("BodySignatureProofVerifier Should", Selector = "bspv")]
public class BodySignatureProofVerifierShould : UnitTestMenuContainer
{
    [UnitTest]
    public void VerifyOnlyGenuineSignaturesOverTheExactBody()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        EccKeyPair otherPair = TestKeys.NewEcc();
        string body = "{\"goal\":\"say hello\"}";
        string signature = TestKeys.BodySignature(pair, body);

        When.A<BodySignatureProofVerifier>("accepts the genuine signature and rejects tampering, wrong keys and garbage",
            () => new BodySignatureProofVerifier(),
            (proof) => new ProofOutcome(
                proof.Verify(body, signature, null, pair.PublicPem),
                proof.Verify(body, signature, BodySignatureProofVerifier.DefaultAlgorithm, pair.PublicPem),
                proof.Verify(body + " ", signature, null, pair.PublicPem),
                proof.Verify(body, signature, null, otherPair.PublicPem),
                proof.Verify(body, "not base64!", null, pair.PublicPem),
                proof.Verify(body, signature, null, "not a pem")))
            .TheTest
            .ShouldPass<ProofOutcome>((because, outcome) =>
            {
                because.ItsTrue("the genuine signature verifies with the default algorithm", outcome.Genuine);
                because.ItsTrue("the genuine signature verifies with the explicit algorithm", outcome.GenuineExplicit);
                because.ItsTrue("a tampered body fails", !outcome.Tampered);
                because.ItsTrue("another key fails", !outcome.WrongKey);
                because.ItsTrue("a malformed signature fails without throwing", !outcome.Malformed);
                because.ItsTrue("an unparseable pem fails without throwing", !outcome.BadPem);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record ProofOutcome(bool Genuine, bool GenuineExplicit, bool Tampered, bool WrongKey, bool Malformed, bool BadPem);
}

[UnitTestMenu("ConfiguredActorAccessPolicy Should", Selector = "caap")]
public class ConfiguredActorAccessPolicyShould : UnitTestMenuContainer
{
    [UnitTest]
    public void GrantEnrolledActorsTheConfiguredLevelAndDenyTheAnonymousSentinel()
    {
        ActorAuthenticationOptions options = new ActorAuthenticationOptions { EnrolledActorAccess = BamAccess.Write };

        When.A<ConfiguredActorAccessPolicy>("maps enrolled and anonymous actors",
            () => new ConfiguredActorAccessPolicy(options),
            (policy) => new PolicyOutcome(
                policy.GetAccess(new ActorData { Handle = "alice", Name = "alice" }),
                policy.GetAccess(new AnonymousActorProvider().GetAnonymousActor())))
            .TheTest
            .ShouldPass<PolicyOutcome>((because, outcome) =>
            {
                because.ItsTrue("enrolled actors get the configured level", outcome.Enrolled == BamAccess.Write);
                because.ItsTrue("the anonymous sentinel is denied", outcome.Anonymous == BamAccess.Denied);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record PolicyOutcome(BamAccess Enrolled, BamAccess Anonymous);
}

[UnitTestMenu("BamJwtToken kfp Should", Selector = "kfp")]
public class BamJwtTokenKeyFingerprintShould : UnitTestMenuContainer
{
    [UnitTest]
    public void RoundTripTheOptionalKeyFingerprintClaim()
    {
        EccKeyPair pair = TestKeys.NewEcc();

        When.A<BamJwtToken>("encodes kfp only when set and decodes it back",
            () => new BamJwtToken("sid", "alice", "bam", TimeSpan.FromMinutes(5)) { KeyFingerprint = "abc123" },
            (bound) =>
            {
                string boundToken = bound.Encode(pair.PrivateKey.Value);
                BamJwtToken unbound = new BamJwtToken("sid", "alice", "bam", TimeSpan.FromMinutes(5));
                string unboundToken = unbound.Encode(pair.PrivateKey.Value);
                return new KfpOutcome(
                    BamJwtToken.Decode(boundToken).KeyFingerprint,
                    BamJwtToken.Decode(unboundToken).KeyFingerprint,
                    BamJwtToken.Verify(boundToken, pair.PublicKey.Value),
                    BamJwtToken.Verify(unboundToken, pair.PublicKey.Value));
            })
            .TheTest
            .ShouldPass<KfpOutcome>((because, outcome) =>
            {
                because.ItsTrue("a bound token round-trips its fingerprint", outcome.BoundFingerprint == "abc123");
                because.ItsTrue("an unbound token decodes with a null fingerprint", outcome.UnboundFingerprint is null);
                because.ItsTrue("the bound token still verifies", outcome.BoundVerifies);
                because.ItsTrue("the unbound token still verifies (backward compatible)", outcome.UnboundVerifies);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record KfpOutcome(string? BoundFingerprint, string? UnboundFingerprint, bool BoundVerifies, bool UnboundVerifies);
}
