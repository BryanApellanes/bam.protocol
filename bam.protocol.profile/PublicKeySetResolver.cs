using Bam.Data.Objects;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Default <see cref="IPublicKeySetResolver"/>: resolves key sets through the object store's
/// indexed equality queries (dictionary-form <c>Query</c>, which the store backs with its
/// search index and verifies against live values), matches key material by CANONICAL identity
/// via the server-stamped fingerprint properties (raw-PEM equality is bypassable by trivial
/// re-encoding — bam.protocol#18 C1; see <see cref="KeyMaterialIdentity"/>), skips revoked
/// tombstones on every resolve path (bam.protocol#13's revoked-skip), and orders every
/// candidate set by <c>Created</c> then <c>Id</c> so the earliest registration always wins over
/// any duplicate.  Shared by <see cref="PublicKeySetRegistrar"/> (admission checks) and
/// <see cref="EncryptedProfileRepository"/> (the key-to-profile read path) so registration-time
/// and resolution-time behavior can never disagree.
/// <para>
/// Material lookups union three indexed queries per key field — the canonical fingerprint
/// column (stamped rows), and the raw PEM columns (legacy rows stamped before fingerprints
/// existed, and tampered rows carrying unparseable material, whose identity is exact bytes) —
/// then re-verify each hit canonically, so stamped, legacy, and tampered rows all resolve
/// under one identity definition.
/// </para>
/// </summary>
public class PublicKeySetResolver : IPublicKeySetResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetResolver"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    public PublicKeySetResolver(ObjectDataRepository repository)
    {
        this.Repository = repository;
    }

    /// <summary>
    /// Gets the object-data repository the key sets are persisted in.
    /// </summary>
    protected ObjectDataRepository Repository { get; }

    /// <inheritdoc />
    public PublicKeySetData? ResolveByHandle(string keySetHandle)
    {
        if (string.IsNullOrEmpty(keySetHandle))
        {
            return null;
        }

        return QueryByProperty(nameof(PublicKeySetData.KeySetHandle), keySetHandle)
            .Where(keySet => keySet.RevokedUtc == null)
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .FirstOrDefault();
    }

    /// <inheritdoc />
    public PublicKeySetData? ResolveByKeyMaterial(string publicKeyPem)
    {
        if (string.IsNullOrEmpty(publicKeyPem))
        {
            return null;
        }

        PublicKeySetData candidate = new PublicKeySetData { PublicRsaKey = publicKeyPem };
        IReadOnlyList<string> candidateIdentities = KeyMaterialIdentity.CandidateIdentities(candidate);

        List<PublicKeySetData> carriers = FindMaterialCarriers(publicKeyPem, candidateIdentities)
            .Where(keySet => keySet.RevokedUtc == null)
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .ToList();

        if (carriers.Count == 0)
        {
            return null;
        }

        List<string> distinctHandles = carriers
            .Select(keySet => keySet.KeySetHandle)
            .Distinct()
            .ToList();
        if (distinctHandles.Count > 1)
        {
            Log.Warn(
                "Public key material resolves to {0} handles ({1}); the earliest-created registration ('{2}') wins. Run the public key-set audit to repair the duplicates.",
                distinctHandles.Count,
                string.Join(", ", distinctHandles),
                carriers[0].KeySetHandle);
        }

        return carriers[0];
    }

    /// <inheritdoc />
    public IEnumerable<PublicKeySetData> FindKeyMaterialClaims(PublicKeySetData candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        IReadOnlyList<string> candidateIdentities = KeyMaterialIdentity.CandidateIdentities(candidate);

        Dictionary<string, PublicKeySetData> claimsByUuid = new Dictionary<string, PublicKeySetData>();
        CollectMaterialCarriers(claimsByUuid, candidate.PublicRsaKey, candidateIdentities);
        CollectMaterialCarriers(claimsByUuid, candidate.PublicEccKey, candidateIdentities);

        return claimsByUuid.Values
            .Where(keySet => !(keySet.KeySetHandle == candidate.KeySetHandle && keySet.RevokedUtc == null))
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .ToList();
    }

    /// <summary>
    /// Finds every stored row carrying the specified material, by the union of indexed lookups
    /// (fingerprint column for stamped rows; raw PEM columns for legacy/tampered rows),
    /// re-verified canonically.
    /// </summary>
    private IEnumerable<PublicKeySetData> FindMaterialCarriers(string publicKeyPem, IReadOnlyList<string> candidateIdentities)
    {
        Dictionary<string, PublicKeySetData> carriersByUuid = new Dictionary<string, PublicKeySetData>();
        CollectMaterialCarriers(carriersByUuid, publicKeyPem, candidateIdentities);
        return carriersByUuid.Values;
    }

    /// <summary>
    /// Adds every row whose material shares canonical identity with <paramref name="publicKeyPem"/>
    /// to <paramref name="carriersByUuid"/>: indexed lookups on the fingerprint columns (when
    /// the material parses) and on the raw PEM columns (exact-bytes legacy/tampered match),
    /// each hit re-verified via <see cref="KeyMaterialIdentity.SharesMaterial"/>.  No-op for
    /// empty material.
    /// </summary>
    private void CollectMaterialCarriers(Dictionary<string, PublicKeySetData> carriersByUuid, string? publicKeyPem, IReadOnlyList<string> candidateIdentities)
    {
        if (string.IsNullOrEmpty(publicKeyPem))
        {
            return;
        }

        string? fingerprint = KeyMaterialIdentity.CanonicalKeyFingerprint(publicKeyPem);
        List<PublicKeySetData> hits = new List<PublicKeySetData>();
        if (fingerprint != null)
        {
            hits.AddRange(QueryByProperty(nameof(PublicKeySetData.PublicRsaKeyFingerprint), fingerprint));
            hits.AddRange(QueryByProperty(nameof(PublicKeySetData.PublicEccKeyFingerprint), fingerprint));
        }

        hits.AddRange(QueryByProperty(nameof(PublicKeySetData.PublicRsaKey), publicKeyPem));
        hits.AddRange(QueryByProperty(nameof(PublicKeySetData.PublicEccKey), publicKeyPem));

        foreach (PublicKeySetData hit in hits)
        {
            if (KeyMaterialIdentity.SharesMaterial(hit, candidateIdentities))
            {
                carriersByUuid[hit.Uuid] = hit;
            }
        }
    }

    /// <summary>
    /// Runs a single-property equality query through the repository's dictionary query form,
    /// which the store resolves via its search index (verified against live values) and which
    /// falls back to a scan on stores with no index.
    /// </summary>
    private IEnumerable<PublicKeySetData> QueryByProperty(string propertyName, string value)
    {
        return Repository.Query<PublicKeySetData>(new Dictionary<string, object>
        {
            { propertyName, value }
        });
    }
}
