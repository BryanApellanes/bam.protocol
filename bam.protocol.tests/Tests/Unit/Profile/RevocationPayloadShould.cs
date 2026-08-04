using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("RevocationPayload Should", Selector = "rvp")]
public class RevocationPayloadShould : UnitTestMenuContainer
{
    [UnitTest]
    public void FrameHandleAndUuidUnambiguously()
    {
        // The length prefix fixes each field boundary, so shifting content across the handle/Uuid
        // boundary (which a delimiter-only scheme could confuse) yields distinct payloads.
        PublicKeySetData targetOne = new PublicKeySetData { KeySetHandle = "ab", PublicRsaKey = "rsa", Uuid = "c" };
        PublicKeySetData targetTwo = new PublicKeySetData { KeySetHandle = "a", PublicRsaKey = "rsa", Uuid = "bc" };

        When.A<string>("frames each field by length so a handle-to-Uuid boundary shift is unambiguous",
            () => "rvp",
            (_) => new PayloadComparisonOutcome(RevocationPayload.Compose(targetOne, null), RevocationPayload.Compose(targetTwo, null)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("a boundary shift across handle and Uuid produces different payloads", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BindTheTargetIdentity()
    {
        // Same handle and key material, different Uuid → different payloads, so a proof is bound
        // to the specific registration and cannot be replayed against a re-registration.
        PublicKeySetData targetOne = new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = "rsa", Uuid = "uuid-1" };
        PublicKeySetData targetTwo = new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = "rsa", Uuid = "uuid-2" };

        When.A<string>("binds the target Uuid so the same handle and key differ per registration",
            () => "rvp",
            (_) => new PayloadComparisonOutcome(RevocationPayload.Compose(targetOne, null), RevocationPayload.Compose(targetTwo, null)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("different Uuids produce different payloads", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BindTheAuthorizedSuccessor()
    {
        // The successor fingerprint is a signed field, so the same target with different successors
        // must produce different payloads — otherwise a captured proof could be re-purposed to
        // authorize a different successor (bam.protocol#21).
        PublicKeySetData target = new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = "rsa", Uuid = "uuid-1" };

        When.A<string>("binds the authorized successor so the same target with different successors differs",
            () => "rvp",
            (_) => new PayloadComparisonOutcome(
                RevocationPayload.Compose(target, "successor-a"),
                RevocationPayload.Compose(target, "successor-b")))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("different successors produce different payloads", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DistinguishBoundFromUnboundRevocation()
    {
        // An unbound revocation (null successor) must not compose to the same payload as one bound
        // to a successor, so a proof for an open re-registration cannot be re-read as a binding one.
        PublicKeySetData target = new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = "rsa", Uuid = "uuid-1" };

        When.A<string>("distinguishes an unbound revocation from one bound to a successor",
            () => "rvp",
            (_) => new PayloadComparisonOutcome(
                RevocationPayload.Compose(target, null),
                RevocationPayload.Compose(target, "successor-a")))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("an unbound and a bound revocation produce different payloads", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FrameTheSuccessorFieldByLength()
    {
        // The length prefix must fix the successor boundary too, so shifting content across the
        // Uuid/successor boundary yields distinct payloads (the framing is injective over all four
        // fields, not just the first three).
        PublicKeySetData targetOne = new PublicKeySetData { KeySetHandle = "h", PublicRsaKey = "rsa", Uuid = "ab" };
        PublicKeySetData targetTwo = new PublicKeySetData { KeySetHandle = "h", PublicRsaKey = "rsa", Uuid = "a" };

        When.A<string>("frames the successor field by length so a Uuid-to-successor boundary shift is unambiguous",
            () => "rvp",
            (_) => new PayloadComparisonOutcome(
                RevocationPayload.Compose(targetOne, "c"),
                RevocationPayload.Compose(targetTwo, "bc")))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("a boundary shift across Uuid and successor produces different payloads", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record PayloadComparisonOutcome(string ComposedOne, string ComposedTwo);
}
