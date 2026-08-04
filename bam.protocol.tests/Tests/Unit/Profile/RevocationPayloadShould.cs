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
            (_) => new PayloadComparisonOutcome(RevocationPayload.Compose(targetOne), RevocationPayload.Compose(targetTwo)))
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
            (_) => new PayloadComparisonOutcome(RevocationPayload.Compose(targetOne), RevocationPayload.Compose(targetTwo)))
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

    private sealed record PayloadComparisonOutcome(string ComposedOne, string ComposedTwo);
}
