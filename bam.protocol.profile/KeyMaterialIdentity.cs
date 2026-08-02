using Bam.Protocol.Data.Profile;

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
/// <para>
/// The canonicalization itself is delegated to <see cref="PublicKeyFingerprint.Of"/> — the same
/// public contract the break-glass admin uses to compute a successor fingerprint and the
/// registrar's successor gate uses to check a candidate (bam.protocol#21).  Sharing one
/// implementation guarantees material identity here and successor identity there can never drift
/// apart.
/// </para>
/// </summary>
internal static class KeyMaterialIdentity
{
    /// <summary>
    /// Returns the SHA-256 of the parsed key's canonical DER <c>SubjectPublicKeyInfo</c>
    /// encoding, or null when the PEM is empty or does not parse.  Delegates to
    /// <see cref="PublicKeyFingerprint.Of"/> so the canonical fingerprint has exactly one
    /// definition across identity, admin binding, and the successor gate.
    /// </summary>
    internal static string? CanonicalKeyFingerprint(string? pem)
    {
        return PublicKeyFingerprint.Of(pem);
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
    /// Returns the identity keys a stored row's material answers to, ALWAYS recomputed from the
    /// material itself (canonical fingerprint, or the raw string for unparseable material) —
    /// never read from the row's stamped fingerprint properties.  The stamps exist solely to
    /// give the search index a canonical column to accelerate lookups; identity DECISIONS
    /// (uniqueness, blocklist, audit grouping, hit re-verification) must derive from ground
    /// truth, or a drifted/tampered stamp could hide byte-identical material from the blocklist
    /// or group a victim's row under material it does not carry
    /// (bam.protocol#24 review round 2, SF6).
    /// </summary>
    internal static IReadOnlyList<string> RowIdentities(PublicKeySetData row)
    {
        List<string> identities = new List<string>();
        AddIdentity(identities, row.PublicRsaKey);
        AddIdentity(identities, row.PublicEccKey);
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

    private static void AddIdentity(List<string> identities, string? pem)
    {
        if (string.IsNullOrEmpty(pem))
        {
            return;
        }

        string identity = CanonicalKeyFingerprint(pem) ?? pem;
        if (!identities.Contains(identity))
        {
            identities.Add(identity);
        }
    }
}
