using Bam;
using Bam.Encryption;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

/// <summary>
/// Computes a re-encoding-independent fingerprint of a PEM-encoded public key: the SHA-256 of
/// the parsed key's canonical DER <c>SubjectPublicKeyInfo</c> encoding.  This is the single shared
/// contract that both sides of successor-binding (bam.protocol#21) agree on — the admin computes a
/// successor's fingerprint when authorizing a revocation, and <see cref="PublicKeySetRegistrar"/>
/// recomputes a candidate's fingerprint when gating re-registration of a freed handle — so the two
/// must never drift.  It is also the equality basis for the one-handle-to-one-key-material invariant.
/// <para>
/// Ordinal PEM comparison is deliberately avoided: a trivial re-encoding (an appended newline, CRLF
/// line endings, trailing spaces) parses to the identical key while being string- and SHA-unequal,
/// which would let a revoked/compromised key slip past the blocklist, the uniqueness check, and the
/// successor gate (bam.protocol#18 review, condition C1 / T1).
/// </para>
/// <para>
/// An EC key's curve can be written as a named-curve OID (RFC 5480's required form, as openssl and .NET
/// write it) or as explicit curve parameters (as Bam's own <c>EccKeyPair</c> writes it), with or without
/// the optional cofactor and seed, and a few curves are known under more than one OID. BouncyCastle
/// re-encodes a parsed key in whichever form it was read, so every EC key whose parameters (curve, base
/// point and order, plus the cofactor when present) match a known named curve is rewritten with that
/// curve's OID before hashing; when several OIDs name the same parameters, the ordinally lowest OID wins,
/// so the choice does not depend on the table's order. Every encoding of one EC key therefore produces
/// one fingerprint (bam.protocol#47). The equivalent openssl command is
/// <c>openssl ec -pubin -in key.pem -param_enc named_curve -conv_form uncompressed -outform DER | sha256sum</c>.
/// </para>
/// </summary>
public static class PublicKeyFingerprint
{
    private static readonly Lazy<IReadOnlyList<KeyValuePair<DerObjectIdentifier, X9ECParameters>>> NamedCurves =
        new Lazy<IReadOnlyList<KeyValuePair<DerObjectIdentifier, X9ECParameters>>>(LoadNamedCurves);

    /// <summary>
    /// Returns the SHA-256 of the parsed key's canonical DER <c>SubjectPublicKeyInfo</c> encoding,
    /// or null when <paramref name="pem"/> is null, empty, or does not parse to a public key.  Two
    /// PEM strings that parse to the same key yield the same non-null fingerprint regardless of
    /// their textual encoding, including an EC key written with explicit curve parameters and the
    /// same key written with its named-curve OID.
    /// </summary>
    /// <param name="pem">The PEM-encoded public key, or null/empty.</param>
    /// <returns>The canonical fingerprint, or null when the input is empty or unparseable.</returns>
    public static string? Of(string? pem)
    {
        if (string.IsNullOrEmpty(pem))
        {
            return null;
        }
        try
        {
            AsymmetricKeyParameter parsedKey = pem.PemToKey();
            if (parsedKey == null)
            {
                return null;
            }
            return HashOf(ToNamedCurveForm(parsedKey));
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the other fingerprints an EC key could have been stamped with before bam.protocol#47, when
    /// fingerprints followed the encoding the key was registered in: the SHA-256 of each named form under
    /// an OID that names the same curve, and of the explicit-parameter form with and without the seed and
    /// the cofactor. An indexed lookup queries these as well as <see cref="Of"/> until a repair pass has
    /// restamped every row. Empty for a non-EC key, a key on no known named curve, or input that does
    /// not parse.
    /// </summary>
    /// <param name="pem">The PEM-encoded public key, or null/empty.</param>
    /// <returns>The distinct pre-#47 fingerprints of the key that differ from <see cref="Of"/>.</returns>
    internal static IReadOnlyList<string> LegacyExplicitFormsOf(string? pem)
    {
        List<string> fingerprints = new List<string>();
        if (string.IsNullOrEmpty(pem))
        {
            return fingerprints;
        }
        try
        {
            if (pem.PemToKey() is not ECPublicKeyParameters ecKey)
            {
                return fingerprints;
            }
            ECPublicKeyParameters named = (ECPublicKeyParameters)ToNamedCurveForm(ecKey);
            if (named.Parameters is not ECNamedDomainParameters)
            {
                return fingerprints;
            }
            ECDomainParameters domain = named.Parameters;
            string canonical = HashOf(named);
            List<AsymmetricKeyParameter> forms = new List<AsymmetricKeyParameter>();
            foreach (DerObjectIdentifier alias in MatchingCurves(domain))
            {
                forms.Add(new ECPublicKeyParameters(ecKey.AlgorithmName, named.Q, alias));
            }
            foreach (byte[]? seed in new[] { domain.GetSeed(), null })
            {
                forms.Add(new ECPublicKeyParameters(ecKey.AlgorithmName, named.Q, new ECDomainParameters(domain.Curve, domain.G, domain.N, domain.H, seed)));
            }
            foreach (AsymmetricKeyParameter form in forms)
            {
                string legacy = HashOf(form);
                if (legacy != canonical && !fingerprints.Contains(legacy))
                {
                    fingerprints.Add(legacy);
                }
            }
            foreach (byte[]? seed in new[] { domain.GetSeed(), null })
            {
                string? withoutCofactor = HashWithoutCofactor(named.Q, domain, seed);
                if (withoutCofactor != null && withoutCofactor != canonical && !fingerprints.Contains(withoutCofactor))
                {
                    fingerprints.Add(withoutCofactor);
                }
            }
        }
        catch (Exception)
        {
            fingerprints.Clear();
        }
        return fingerprints;
    }

    private static string HashOf(AsymmetricKeyParameter key)
    {
        return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key).GetDerEncoded().Sha256();
    }

    /// <summary>
    /// The SHA-256 of the explicit-parameter encoding of a key with the optional cofactor left out, the
    /// shape a 5-element X9.62 <c>ECParameters</c> sequence takes.
    /// </summary>
    private static string? HashWithoutCofactor(Org.BouncyCastle.Math.EC.ECPoint q, ECDomainParameters domain, byte[]? seed)
    {
        X9ECParameters full = new X9ECParameters(domain.Curve, new X9ECPoint(domain.G, false), domain.N, domain.H, seed);
        Asn1Sequence sequence = (Asn1Sequence)full.ToAsn1Object();
        if (sequence.Count < 6)
        {
            return null;
        }
        Asn1EncodableVector elements = new Asn1EncodableVector();
        for (int index = 0; index < sequence.Count; index++)
        {
            if (index != 5)
            {
                elements.Add(sequence[index]);
            }
        }
        AlgorithmIdentifier algorithm = new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, new DerSequence(elements));
        SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(algorithm, q.GetEncoded(false));
        return info.GetDerEncoded().Sha256();
    }

    /// <summary>
    /// Rewrites an EC public key with the OID of the named curve its parameters describe: the curve,
    /// base point and order must match, and the cofactor too when the encoding carries one (X9.62 makes
    /// it optional). When several OIDs name the same parameters, the ordinally lowest OID is used. This
    /// applies to keys written with explicit parameters and to keys already carrying an OID, so aliases
    /// collapse to one form. A key on no known curve, or a non-EC key, is returned unchanged.
    /// </summary>
    private static AsymmetricKeyParameter ToNamedCurveForm(AsymmetricKeyParameter key)
    {
        if (key is not ECPublicKeyParameters ecKey)
        {
            return key;
        }
        DerObjectIdentifier? canonicalOid = MatchingCurves(ecKey.Parameters).FirstOrDefault();
        if (canonicalOid == null)
        {
            return key;
        }
        if (ecKey.Parameters is ECNamedDomainParameters alreadyNamed && alreadyNamed.Name.Equals(canonicalOid))
        {
            return key;
        }
        return new ECPublicKeyParameters(ecKey.AlgorithmName, ecKey.Q, canonicalOid);
    }

    /// <summary>
    /// The OIDs of the known named curves whose parameters match <paramref name="domain"/>, ordinally
    /// lowest first. The cofactor is compared only when <paramref name="domain"/> carries one.
    /// </summary>
    private static IEnumerable<DerObjectIdentifier> MatchingCurves(ECDomainParameters domain)
    {
        return NamedCurves.Value
            .Where(curve => curve.Value.Curve.Equals(domain.Curve)
                && curve.Value.G.Equals(domain.G)
                && curve.Value.N.Equals(domain.N)
                && (domain.H == null || curve.Value.H.Equals(domain.H)))
            .Select(curve => curve.Key)
            .OrderBy(oid => oid.Id, StringComparer.Ordinal);
    }

    private static IReadOnlyList<KeyValuePair<DerObjectIdentifier, X9ECParameters>> LoadNamedCurves()
    {
        List<KeyValuePair<DerObjectIdentifier, X9ECParameters>> curves = new List<KeyValuePair<DerObjectIdentifier, X9ECParameters>>();
        HashSet<DerObjectIdentifier> seen = new HashSet<DerObjectIdentifier>();
        foreach (string name in ECNamedCurveTable.Names)
        {
            DerObjectIdentifier? oid = ECNamedCurveTable.GetOid(name);
            X9ECParameters? parameters = ECNamedCurveTable.GetByName(name);
            if (oid != null && parameters != null && seen.Add(oid))
            {
                curves.Add(new KeyValuePair<DerObjectIdentifier, X9ECParameters>(oid, parameters));
            }
        }
        return curves;
    }

    /// <summary>
    /// True when <paramref name="fingerprint"/> is a well-formed canonical fingerprint — exactly the
    /// shape <see cref="Of"/> produces: 64 lowercase hexadecimal characters (a SHA-256 digest).  The
    /// break-glass admin binding and the registrar's successor gate compare fingerprints ordinally, so
    /// a value in any other shape (uppercase hex, truncated, a raw PEM) could never match a candidate
    /// recomputed via <see cref="Of"/> and would silently brick the handle; the revocation policy
    /// (<c>IKeySetRevocation</c>) validates a bound successor with this before persisting it
    /// (bam.protocol#25 review B2 / Condition 3).
    /// </summary>
    /// <param name="fingerprint">The candidate fingerprint string, or null.</param>
    /// <returns>True when the value is 64 lowercase hex characters; false otherwise (including null/empty).</returns>
    public static bool IsCanonical(string? fingerprint)
    {
        if (string.IsNullOrEmpty(fingerprint) || fingerprint.Length != 64)
        {
            return false;
        }
        foreach (char c in fingerprint)
        {
            bool isLowerHex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
            if (!isLowerHex)
            {
                return false;
            }
        }
        return true;
    }
}
