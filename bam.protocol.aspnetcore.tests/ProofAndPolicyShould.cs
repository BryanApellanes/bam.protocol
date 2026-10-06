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

    [UnitTest]
    public void RefuseAnyAlgorithmTheOptionsDontAllow()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        string body = "{\"goal\":\"say hello\"}";
        string signature = TestKeys.BodySignature(pair, body);

        When.A<BodySignatureProofVerifier>("verifies only with an allowed algorithm",
            () => new BodySignatureProofVerifier(new ActorAuthenticationOptions()),
            (proof) => new AlgorithmOutcome(
                proof.Verify(body, signature, null, pair.PublicPem),
                proof.Verify(body, signature, "sha256withecdsa", pair.PublicPem),
                proof.Verify(body, signature, "NONEwithECDSA", pair.PublicPem),
                proof.Verify(body, signature, "SHA1WITHECDSA", pair.PublicPem),
                proof.Verify(body, signature, "NOT-AN-ALGORITHM", pair.PublicPem),
                new BodySignatureProofVerifier(new ActorAuthenticationOptions { AllowedBodySignatureAlgorithms = Array.Empty<string>() }).Verify(body, signature, null, pair.PublicPem)))
            .TheTest
            .ShouldPass<AlgorithmOutcome>((because, outcome) =>
            {
                because.ItsTrue("a missing header uses the default algorithm", outcome.Default);
                because.ItsTrue("the allowed algorithm matches without regard to case", outcome.ExplicitAllowed);
                because.ItsTrue("NONEwithECDSA is refused", !outcome.None);
                because.ItsTrue("SHA1WITHECDSA is refused", !outcome.Sha1);
                because.ItsTrue("an unknown name is refused", !outcome.Unknown);
                because.ItsTrue("an empty allow-list refuses everything", !outcome.EmptyList);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record ProofOutcome(bool Genuine, bool GenuineExplicit, bool Tampered, bool WrongKey, bool Malformed, bool BadPem);

    private sealed record AlgorithmOutcome(bool Default, bool ExplicitAllowed, bool None, bool Sha1, bool Unknown, bool EmptyList);
}

[UnitTestMenu("ConfiguredActorAccessPolicy Should", Selector = "caap")]
public class ConfiguredActorAccessPolicyShould : UnitTestMenuContainer
{
    [UnitTest]
    public void GrantEnrolledActorsTheConfiguredLevelAndDenyTheAnonymousSentinel()
    {
        ActorAuthenticationOptions options = new ActorAuthenticationOptions { EnrolledActorAccess = BamAccess.Write, OpenEnrollment = true };

        When.A<ConfiguredActorAccessPolicy>("maps enrolled and anonymous actors",
            () => new ConfiguredActorAccessPolicy(options, new ConfiguredActorAdmission(options)),
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

    [UnitTest]
    public void GrantNothingToEnrolledActorsUnlessAdmitted()
    {
        ActorData alice = new ActorData { Handle = "alice", Name = "alice" };
        ActorData bob = new ActorData { Handle = "bob", Name = "bob" };

        When.A<ActorAuthenticationOptions>("admits by switch or allow-list only",
            () => new ActorAuthenticationOptions(),
            (closed) =>
            {
                ActorAuthenticationOptions listed = new ActorAuthenticationOptions { AdmittedHandles = new[] { "alice" } };
                ActorAuthenticationOptions open = new ActorAuthenticationOptions { OpenEnrollment = true };
                return new AdmissionOutcome(
                    new ConfiguredActorAccessPolicy(closed, new ConfiguredActorAdmission(closed)).GetAccess(alice),
                    new ConfiguredActorAccessPolicy(listed, new ConfiguredActorAdmission(listed)).GetAccess(alice),
                    new ConfiguredActorAccessPolicy(listed, new ConfiguredActorAdmission(listed)).GetAccess(bob),
                    new ConfiguredActorAdmission(listed).IsAdmitted(new ActorData { Handle = "ALICE", Name = "ALICE" }),
                    new ConfiguredActorAccessPolicy(open, new ConfiguredActorAdmission(open)).GetAccess(bob));
            })
            .TheTest
            .ShouldPass<AdmissionOutcome>((because, outcome) =>
            {
                because.ItsTrue("by default an enrolled actor is denied", outcome.ClosedByDefault == BamAccess.Denied);
                because.ItsTrue("a listed handle holds the enrolled level", outcome.Listed == BamAccess.Execute);
                because.ItsTrue("an unlisted handle is denied", outcome.Unlisted == BamAccess.Denied);
                because.ItsTrue("the allow-list matches exactly", !outcome.DifferentCaseAdmitted);
                because.ItsTrue("open enrollment admits everyone", outcome.Open == BamAccess.Execute);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record PolicyOutcome(BamAccess Enrolled, BamAccess Anonymous);

    private sealed record AdmissionOutcome(BamAccess ClosedByDefault, BamAccess Listed, BamAccess Unlisted, bool DifferentCaseAdmitted, BamAccess Open);
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
