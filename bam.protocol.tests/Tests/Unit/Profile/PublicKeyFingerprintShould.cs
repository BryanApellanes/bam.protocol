using Bam;
using Bam.Encryption;
using Bam.Protocol.Profile;
using Bam.Test;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

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

    [UnitTest]
    public void ProduceOneFingerprintForTheNamedCurveAndExplicitParameterEncodingsOfOneEcKey()
    {
        // bam.protocol#47: Bam's EccKeyPair writes P-256 keys with explicit curve parameters, while
        // openssl and .NET write the named-curve OID. Both must fingerprint identically, or the
        // revocation blocklist and admission pins depend on how the actor happened to encode its key.
        EccPublicPrivateKeyPair bamKeyPair = new EccPublicPrivateKeyPair();
        string bamExplicitPem = bamKeyPair.PublicKeyPem;

        When.A<string>("fingerprints a Bam-written explicit-parameter key and its named-curve encoding",
            () => "pkfp",
            (_) =>
            {
                ECPublicKeyParameters parsed = (ECPublicKeyParameters)bamExplicitPem.PemToKey();
                ECPublicKeyParameters named = new ECPublicKeyParameters("EC", parsed.Q, SecObjectIdentifiers.SecP256r1);
                string namedPem = ToPem(named);
                return new EncodingOutcome(
                    namedPem != bamExplicitPem,
                    PublicKeyFingerprint.Of(bamExplicitPem),
                    PublicKeyFingerprint.Of(namedPem));
            })
        .TheTest
        .ShouldPass<EncodingOutcome>((because, outcome) =>
        {
            because.ItsTrue("the two PEMs really are different encodings", outcome.EncodingsDiffer);
            because.ItsTrue("the explicit-parameter key fingerprints", outcome.ExplicitFingerprint != null);
            because.ItsTrue("both encodings produce the same fingerprint", outcome.ExplicitFingerprint == outcome.NamedFingerprint);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void MatchTheDocumentedOpensslCommandForBothEncodings()
    {
        // Operators compute pins with `openssl ec -pubin -param_enc named_curve -outform DER | sha256sum`;
        // the fixture's expected value is that command's output for this key.
        When.A<string>("fingerprints one openssl-written key in both curve encodings",
            () => "pkfp",
            (_) => new OpensslOutcome(
                PublicKeyFingerprint.Of(EcKeyEncodingFixtures.NamedCurvePem),
                PublicKeyFingerprint.Of(EcKeyEncodingFixtures.ExplicitParametersPem)))
        .TheTest
        .ShouldPass<OpensslOutcome>((because, outcome) =>
        {
            because.ItsTrue("the named-curve PEM matches openssl", EcKeyEncodingFixtures.OpensslFingerprint == outcome.Named);
            because.ItsTrue("the explicit-parameter PEM matches openssl", EcKeyEncodingFixtures.OpensslFingerprint == outcome.Explicit);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static string ToPem(ECPublicKeyParameters key)
    {
        using StringWriter writer = new StringWriter();
        PemWriter pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(key);
        pemWriter.Writer.Flush();
        return writer.ToString();
    }

    private sealed record EncodingOutcome(bool EncodingsDiffer, string? ExplicitFingerprint, string? NamedFingerprint);

    private sealed record OpensslOutcome(string? Named, string? Explicit);

    private sealed record FingerprintEqualityOutcome(string Canonical, bool EqualsAppendedNewline, bool EqualsCrlf, bool EqualsTrailingSpaces);

    private sealed record FingerprintDifferenceOutcome(bool OneComputed, bool TwoComputed, bool Differ);

    private sealed record NullFingerprintOutcome(bool NullIsNull, bool EmptyIsNull, bool WhitespaceIsNull, bool GarbageIsNull);
}
