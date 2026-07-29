using Bam.Encryption;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("PublicKeyFingerprint Should", Selector = "pkfp")]
public class PublicKeyFingerprintShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ProduceEqualFingerprintsForReEncodedVariantsOfOneKey()
    {
        // A trivial re-encoding (appended newline, CRLF line endings, trailing spaces) parses to the
        // identical key, so its canonical fingerprint must be equal — this is the property the
        // successor gate and the material blocklist both rely on (bam.protocol#18 C1 / #21).
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        string pem = keyPair.PublicKeyPem;

        When.A<string>("computes equal fingerprints for whitespace- and CRLF-re-encoded variants of one key",
            () => "pkfp",
            (_) =>
            {
                string canonical = PublicKeyFingerprint.Of(pem)!;
                string appendedNewline = PublicKeyFingerprint.Of(pem + "\n")!;
                string crlf = PublicKeyFingerprint.Of(pem.Replace("\n", "\r\n"))!;
                string trailingSpaces = PublicKeyFingerprint.Of(pem + "   ")!;
                return new FingerprintEqualityOutcome(
                    canonical,
                    canonical == appendedNewline,
                    canonical == crlf,
                    canonical == trailingSpaces);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<FingerprintEqualityOutcome>("the canonical fingerprint is non-empty", o => !string.IsNullOrEmpty(o.Canonical))
                .As<FingerprintEqualityOutcome>("an appended newline yields the same fingerprint", o => o.EqualsAppendedNewline)
                .As<FingerprintEqualityOutcome>("CRLF line endings yield the same fingerprint", o => o.EqualsCrlf)
                .As<FingerprintEqualityOutcome>("trailing spaces yield the same fingerprint", o => o.EqualsTrailingSpaces);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ProduceDifferentFingerprintsForDifferentKeys()
    {
        RsaPublicPrivateKeyPair keyPairOne = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair keyPairTwo = new RsaPublicPrivateKeyPair();

        When.A<string>("computes different fingerprints for different keys",
            () => "pkfp",
            (_) =>
            {
                string? one = PublicKeyFingerprint.Of(keyPairOne.PublicKeyPem);
                string? two = PublicKeyFingerprint.Of(keyPairTwo.PublicKeyPem);
                return new FingerprintDifferenceOutcome(one != null, two != null, one != two);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<FingerprintDifferenceOutcome>("the first key fingerprints", o => o.OneComputed)
                .As<FingerprintDifferenceOutcome>("the second key fingerprints", o => o.TwoComputed)
                .As<FingerprintDifferenceOutcome>("two different keys produce different fingerprints", o => o.Differ);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReturnNullForEmptyOrUnparseableInput()
    {
        When.A<string>("returns null for null, empty, and unparseable input",
            () => "pkfp",
            (_) => new NullFingerprintOutcome(
                PublicKeyFingerprint.Of(null) == null,
                PublicKeyFingerprint.Of(string.Empty) == null,
                PublicKeyFingerprint.Of("   ") == null,
                PublicKeyFingerprint.Of("not a pem") == null))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<NullFingerprintOutcome>("null input yields null", o => o.NullIsNull)
                .As<NullFingerprintOutcome>("empty input yields null", o => o.EmptyIsNull)
                .As<NullFingerprintOutcome>("whitespace input yields null", o => o.WhitespaceIsNull)
                .As<NullFingerprintOutcome>("non-PEM input yields null", o => o.GarbageIsNull);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record FingerprintEqualityOutcome(string Canonical, bool EqualsAppendedNewline, bool EqualsCrlf, bool EqualsTrailingSpaces);

    private sealed record FingerprintDifferenceOutcome(bool OneComputed, bool TwoComputed, bool Differ);

    private sealed record NullFingerprintOutcome(bool NullIsNull, bool EmptyIsNull, bool WhitespaceIsNull, bool GarbageIsNull);
}
