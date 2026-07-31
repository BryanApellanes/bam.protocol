using Bam.Encryption;
using Bam.Protocol.Data.Profile;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

/// <summary>
/// The single definition of public-key-material identity for key-set policy decisions:
/// SHA-256 of the parsed key's canonical DER <c>SubjectPublicKeyInfo</c>.  Ordinal PEM
/// comparison is bypassable — a trivial re-encoding (appended newline, CRLF endings) parses to
/// the identical key while being string-unequal, which would let revoked/duplicate material
/// slip past the blocklist and uniqueness checks (bam.protocol#18 review C1/T1) — so every
/// identity decision (registrar admission, resolver lookup, audit grouping) routes through the
/// fingerprints computed here.  Material that does not parse as a key (which the registrar
/// never admits, but tampered or legacy rows may carry) falls back to the raw string as its
/// identity — re-encoding attacks require parseable keys, so exact-bytes identity is sound for
/// unparseable blobs.
/// </summary>
internal static class KeyMaterialIdentity
{
    /// <summary>
    /// Returns the SHA-256 of the parsed key's canonical DER <c>SubjectPublicKeyInfo</c>
    /// encoding, or null when the PEM is empty or does not parse.
    /// </summary>
    internal static string? CanonicalKeyFingerprint(string? pem)
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
            byte[] canonicalDer = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(parsedKey).GetDerEncoded();
            return canonicalDer.Sha256();
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Stamps the server-owned canonical fingerprint properties from the key set's PEM fields
    /// (null when a field is empty or unparseable).  Called at registration and on every
    /// rotation update so the store's search index can serve canonical-identity lookups.
    /// </summary>
    internal static void StampFingerprints(PublicKeySetData keySet)
    {
        keySet.PublicRsaKeyFingerprint = CanonicalKeyFingerprint(keySet.PublicRsaKey);
        keySet.PublicEccKeyFingerprint = CanonicalKeyFingerprint(keySet.PublicEccKey);
    }

    /// <summary>
    /// Returns the identity keys of the candidate's non-empty material fields: the canonical
    /// fingerprint when the field parses, else the raw string (exact-bytes identity for
    /// unparseable material).
    /// </summary>
    internal static IReadOnlyList<string> CandidateIdentities(PublicKeySetData candidate)
    {
        List<string> identities = new List<string>();
        AddIdentity(identities, candidate.PublicRsaKey);
        AddIdentity(identities, candidate.PublicEccKey);
        return identities;
    }

    /// <summary>
    /// Returns the identity keys a stored row's material answers to, preferring stamped
    /// fingerprints and computing them (or falling back to raw strings) for legacy rows
    /// stamped before fingerprints existed.
    /// </summary>
    internal static IReadOnlyList<string> RowIdentities(PublicKeySetData row)
    {
        List<string> identities = new List<string>();
        AddIdentity(identities, row.PublicRsaKey, row.PublicRsaKeyFingerprint);
        AddIdentity(identities, row.PublicEccKey, row.PublicEccKeyFingerprint);
        return identities;
    }

    /// <summary>
    /// True when any of the row's material identities equals any of the candidate identities —
    /// deliberately field-agnostic: material is one identity regardless of which key field
    /// carries it.
    /// </summary>
    internal static bool SharesMaterial(PublicKeySetData row, IReadOnlyList<string> candidateIdentities)
    {
        if (candidateIdentities.Count == 0)
        {
            return false;
        }

        foreach (string rowIdentity in RowIdentities(row))
        {
            if (candidateIdentities.Contains(rowIdentity))
            {
                return true;
            }
        }

        return false;
    }

    private static void AddIdentity(List<string> identities, string? pem, string? stampedFingerprint = null)
    {
        if (string.IsNullOrEmpty(pem))
        {
            return;
        }

        string identity = stampedFingerprint ?? CanonicalKeyFingerprint(pem) ?? pem;
        if (!identities.Contains(identity))
        {
            identities.Add(identity);
        }
    }
}
