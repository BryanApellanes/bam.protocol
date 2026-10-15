using Bam;
using Bam.Encryption;
using Bam.Protocol.Profile;
using Bam.Test;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
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

    [UnitTest]
    public void MatchOpensslForACompressedPoint()
    {
        When.A<string>("fingerprints the fixture key written with a compressed point",
            () => "pkfp",
            (_) => PublicKeyFingerprint.Of(EcKeyEncodingFixtures.CompressedPointPem))
        .TheTest
        .ShouldPass<string?>((because, fingerprint) =>
        {
            because.ItsTrue("the compressed point gives the openssl fingerprint", EcKeyEncodingFixtures.OpensslFingerprint == fingerprint);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CanonicaliseAnExplicitEncodingWithoutTheOptionalCofactor()
    {
        // X9.62 makes the cofactor optional; BouncyCastle parses a 5-element ECParameters with H null.
        // Without canonicalisation this is a third fingerprint for one key (#49 challenger blocker 1).
        When.A<string>("fingerprints the fixture key with explicit parameters and no cofactor",
            () => "pkfp",
            (_) =>
            {
                string withoutCofactor = ExplicitPemWithoutCofactor(EcKeyEncodingFixtures.NamedCurvePem);
                ECPublicKeyParameters parsed = (ECPublicKeyParameters)withoutCofactor.PemToKey();
                return new CofactorOutcome(parsed.Parameters.H == null, PublicKeyFingerprint.Of(withoutCofactor));
            })
        .TheTest
        .ShouldPass<CofactorOutcome>((because, outcome) =>
        {
            because.ItsTrue("the encoding really carries no cofactor", outcome.CofactorAbsent);
            because.ItsTrue("it fingerprints like the named-curve form", EcKeyEncodingFixtures.OpensslFingerprint == outcome.Fingerprint);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GiveOneFingerprintAcrossAliasedCurveOids()
    {
        // GostR3410-2001-CryptoPro-A and -XchA name identical parameters (#49 challenger should-fix 2).
        X9ECParameters curve = ECNamedCurveTable.GetByName("GostR3410-2001-CryptoPro-A");
        DerObjectIdentifier oidA = ECNamedCurveTable.GetOid("GostR3410-2001-CryptoPro-A");
        DerObjectIdentifier oidXchA = ECNamedCurveTable.GetOid("GostR3410-2001-CryptoPro-XchA");
        Org.BouncyCastle.Math.EC.ECPoint q = curve.G.Multiply(BigInteger.ValueOf(123456789)).Normalize();

        When.A<string>("fingerprints one point under two aliased OIDs and in explicit form",
            () => "pkfp",
            (_) =>
            {
                string underA = ToPem(new ECPublicKeyParameters("EC", q, oidA));
                string underXchA = ToPem(new ECPublicKeyParameters("EC", q, oidXchA));
                string explicitForm = ToPem(new ECPublicKeyParameters("EC", q, new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed())));
                return new AliasOutcome(
                    underA != underXchA,
                    PublicKeyFingerprint.Of(underA),
                    PublicKeyFingerprint.Of(underXchA),
                    PublicKeyFingerprint.Of(explicitForm));
            })
        .TheTest
        .ShouldPass<AliasOutcome>((because, outcome) =>
        {
            because.ItsTrue("the two named PEMs differ", outcome.PemsDiffer);
            because.ItsTrue("the aliased OIDs give one fingerprint", outcome.UnderA != null && outcome.UnderA == outcome.UnderXchA);
            because.ItsTrue("the explicit form gives the same one", outcome.UnderA == outcome.Explicit);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ListEveryPreCanonicalFingerprintOfAnEcKey()
    {
        // The legacy set must contain the pre-#47 stamp of every encoding a stored row could carry.
        EccPublicPrivateKeyPair bamKeyPair = new EccPublicPrivateKeyPair();
        ECPublicKeyParameters parsed = (ECPublicKeyParameters)bamKeyPair.PublicKeyPem.PemToKey();
        X9ECParameters p256 = ECNamedCurveTable.GetByName("prime256v1");
        string unseeded = ToPem(new ECPublicKeyParameters("EC", parsed.Q, new ECDomainParameters(p256.Curve, p256.G, p256.N, p256.H)));
        string noCofactor = ExplicitPemWithoutCofactor(ToPem(new ECPublicKeyParameters("EC", parsed.Q, SecObjectIdentifiers.SecP256r1)));

        When.A<string>("lists the legacy fingerprints",
            () => "pkfp",
            (_) =>
            {
                IReadOnlyList<string> legacy = PublicKeyFingerprint.LegacyExplicitFormsOf(bamKeyPair.PublicKeyPem);
                return new LegacyOutcome(
                    legacy.Contains(EcKeyEncodingFixtures.PreCanonicalFingerprint(bamKeyPair.PublicKeyPem)),
                    legacy.Contains(EcKeyEncodingFixtures.PreCanonicalFingerprint(unseeded)),
                    legacy.Contains(EcKeyEncodingFixtures.PreCanonicalFingerprint(noCofactor)),
                    legacy.Contains(PublicKeyFingerprint.Of(bamKeyPair.PublicKeyPem)!),
                    PublicKeyFingerprint.LegacyExplicitFormsOf(new RsaPublicPrivateKeyPair().PublicKeyPem).Count,
                    PublicKeyFingerprint.LegacyExplicitFormsOf("not a pem").Count);
            })
        .TheTest
        .ShouldPass<LegacyOutcome>((because, outcome) =>
        {
            because.ItsTrue("the seeded explicit stamp Bam wrote is listed", outcome.Seeded);
            because.ItsTrue("the unseeded explicit stamp is listed", outcome.Unseeded);
            because.ItsTrue("the cofactor-less explicit stamp is listed", outcome.NoCofactor);
            because.ItsTrue("the canonical fingerprint itself is not", !outcome.ContainsCanonical);
            because.ItsTrue("an RSA key has none", outcome.RsaCount == 0);
            because.ItsTrue("unparseable input has none", outcome.GarbageCount == 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static string ExplicitPemWithoutCofactor(string namedPem)
    {
        ECPublicKeyParameters key = (ECPublicKeyParameters)namedPem.PemToKey();
        ECDomainParameters domain = key.Parameters;
        Asn1Sequence full = (Asn1Sequence)new X9ECParameters(domain.Curve, new X9ECPoint(domain.G, false), domain.N, domain.H, domain.GetSeed()).ToAsn1Object();
        Asn1EncodableVector elements = new Asn1EncodableVector();
        for (int index = 0; index < 5; index++)
        {
            elements.Add(full[index]);
        }
        SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
            new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, new DerSequence(elements)),
            key.Q.GetEncoded(false));
        string base64 = Convert.ToBase64String(info.GetDerEncoded(), Base64FormattingOptions.InsertLineBreaks).Replace("\r\n", "\n");
        return "-----BEGIN PUBLIC KEY-----\n" + base64 + "\n-----END PUBLIC KEY-----\n";
    }

    private sealed record CofactorOutcome(bool CofactorAbsent, string? Fingerprint);

    private sealed record AliasOutcome(bool PemsDiffer, string? UnderA, string? UnderXchA, string? Explicit);

    private sealed record LegacyOutcome(bool Seeded, bool Unseeded, bool NoCofactor, bool ContainsCanonical, int RsaCount, int GarbageCount);

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
