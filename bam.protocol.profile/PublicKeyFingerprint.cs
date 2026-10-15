using Bam;
using Bam.Encryption;
using Org.BouncyCastle.Asn1;
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
/// An EC key's curve can be written two ways: as a named-curve OID (RFC 5480's required form, as
/// openssl and .NET write it) or as explicit curve parameters (as Bam's own <c>EccKeyPair</c> writes
/// it). BouncyCastle re-encodes a parsed key in whichever form it was read, so an EC key whose
/// explicit parameters match a known named curve is converted to the named-curve form before hashing.
/// Both encodings of one EC key therefore produce one fingerprint (bam.protocol#47). The equivalent
/// openssl command is <c>openssl ec -pubin -in key.pem -param_enc named_curve -outform DER | sha256sum</c>.
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
    /// Returns the fingerprints an EC key had before bam.protocol#47 when it was registered with
    /// explicit curve parameters: the SHA-256 of the explicit-parameter encoding, with and without
    /// the curve seed. Fingerprints stamped on stored rows before that change used whichever form the
    /// key was registered in, so an indexed lookup queries these as well as <see cref="Of"/> until a
    /// repair pass has restamped every row. Empty for a non-EC key, a key on no known named curve, or
    /// input that does not parse.
    /// </summary>
    /// <param name="pem">The PEM-encoded public key, or null/empty.</param>
    /// <returns>The distinct pre-#47 explicit-form fingerprints that differ from <see cref="Of"/>.</returns>
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
            foreach (byte[]? seed in new[] { domain.GetSeed(), null })
            {
                ECDomainParameters explicitDomain = new ECDomainParameters(domain.Curve, domain.G, domain.N, domain.H, seed);
                string legacy = HashOf(new ECPublicKeyParameters(ecKey.AlgorithmName, named.Q, explicitDomain));
                if (legacy != canonical && !fingerprints.Contains(legacy))
                {
                    fingerprints.Add(legacy);
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
    /// Converts an EC public key written with explicit curve parameters to the equivalent key
    /// carrying the named-curve OID, when the parameters match a known named curve. Any other key
    /// is returned unchanged.
    /// </summary>
    private static AsymmetricKeyParameter ToNamedCurveForm(AsymmetricKeyParameter key)
    {
        if (key is not ECPublicKeyParameters ecKey || ecKey.Parameters is ECNamedDomainParameters)
        {
            return key;
        }
        ECDomainParameters domain = ecKey.Parameters;
        foreach (KeyValuePair<DerObjectIdentifier, X9ECParameters> curve in NamedCurves.Value)
        {
            X9ECParameters named = curve.Value;
            if (named.Curve.Equals(domain.Curve) && named.G.Equals(domain.G) && named.N.Equals(domain.N) && named.H.Equals(domain.H))
            {
                return new ECPublicKeyParameters(ecKey.AlgorithmName, ecKey.Q, curve.Key);
            }
        }
        return key;
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
