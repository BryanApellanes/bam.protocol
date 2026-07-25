using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("KeySetRotationPayload Should", Selector = "krp")]
public class KeySetRotationPayloadShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ComposeUnambiguouslyAcrossFieldBoundaries()
    {
        // The C1 finding: a newline-joined payload is malleable because the field values
        // themselves contain newlines (multi-line PEM). These two tuples move a newline across
        // the RSA/ECC boundary — a newline join would produce IDENTICAL bytes for both, letting
        // one signature authorize a different key set. The length-prefixed composition must not.
        PublicKeySetData splitOne = new PublicKeySetData
        {
            KeySetHandle = "victim",
            PublicRsaKey = "SEGMENT_A\nSEGMENT_B",
            PublicEccKey = "SEGMENT_C",
        };
        PublicKeySetData splitTwo = new PublicKeySetData
        {
            KeySetHandle = "victim",
            PublicRsaKey = "SEGMENT_A",
            PublicEccKey = "SEGMENT_B\nSEGMENT_C",
        };

        When.A<string>("composes a rotation payload that cannot be re-split into a different key set",
            () => "krp",
            (_) =>
            {
                string composedOne = KeySetRotationPayload.Compose("current-sha", splitOne);
                string composedTwo = KeySetRotationPayload.Compose("current-sha", splitTwo);
                return new PayloadComparisonOutcome(composedOne, composedTwo);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("the two re-split field tuples produce different payloads", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BindTheCurrentKeyHash()
    {
        PublicKeySetData proposed = new PublicKeySetData
        {
            KeySetHandle = "victim",
            PublicRsaKey = "rsa",
            PublicEccKey = "ecc",
        };

        When.A<string>("binds the current key hash so the same proposal differs per current key",
            () => "krp",
            (_) =>
            {
                string boundToKeyOne = KeySetRotationPayload.Compose("current-sha-1", proposed);
                string boundToKeyTwo = KeySetRotationPayload.Compose("current-sha-2", proposed);
                return new PayloadComparisonOutcome(boundToKeyOne, boundToKeyTwo);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PayloadComparisonOutcome>("the same proposal bound to different current keys differs", o => o.ComposedOne != o.ComposedTwo);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record PayloadComparisonOutcome(string ComposedOne, string ComposedTwo);
}
