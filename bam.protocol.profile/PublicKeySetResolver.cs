using Bam.Data.Objects;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Default <see cref="IPublicKeySetResolver"/>: matches key material by CANONICAL identity
/// (raw-PEM equality is bypassable by trivial re-encoding — bam.protocol#18 C1; see
/// <see cref="KeyMaterialIdentity"/>), skips revoked tombstones on every resolve path
/// (bam.protocol#13's revoked-skip), and orders every candidate set by <c>Created</c> then
/// <c>Id</c> so the earliest registration always wins over any duplicate.  Shared by
/// <see cref="PublicKeySetRegistrar"/> (admission checks) and <see cref="EncryptedProfileRepository"/>
/// (the key-to-profile read path) so registration-time and resolution-time behavior can never
/// disagree.
/// <para>
/// <b>Indexed-vs-scan contract (do not over-read as "always indexed").</b>  The object store
/// serves a <c>Query</c> from its search index only when a per-property index directory exists
/// for the queried column AND the criterion value is non-null; otherwise it falls back to a
/// full scan.  Key-material identity spans four columns (RSA/ECC fingerprint + raw RSA/ECC),
/// and a column has no index directory until some row stores a non-null value there — so in an
/// RSA-only store the ECC columns are never indexed.  A material lookup therefore does NOT
/// blindly query all four columns: it takes at most ONE full store pass
/// (bam.protocol#24 round-2 security condition 1) — index-served per column when the store is
/// indexed (skipping columns with no directory, which hold nothing), or a single
/// <c>RetrieveAll</c> filtered in memory otherwise.  Because the index is not authoritative,
/// the registrar's admission decisions confirm every empty indexed result against a full scan
/// (see <see cref="PublicKeySetRegistrar"/>); that scan-confirmation is load-bearing and must
/// not be removed on the assumption that the index covers it.
/// </para>
/// <para>
/// Every candidate hit — indexed or scanned — is re-verified canonically via
/// <see cref="KeyMaterialIdentity.SharesMaterial"/> against the row's MATERIAL, never its
/// stored fingerprint stamp, so stamped, legacy, and tampered rows all resolve under one
/// identity definition and a drifted stamp can neither hide nor fabricate a match.
/// </para>
/// </summary>
public class PublicKeySetResolver : IPublicKeySetResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetResolver"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    /// <param name="searchIndexer">
    /// The store's search indexer, used ONLY to decide whether a key-material column can be
    /// served from the index (<see cref="IObjectDataSearchIndexer.HasIndex(System.Type, string)"/>)
    /// so a material lookup never issues a query that silently degrades to a full store scan.
    /// When null (the convenience/default-policy construction path), a material lookup performs
    /// exactly ONE full store pass and filters in memory — never more.  See the class remarks on
    /// the indexed-vs-scan contract.  Production wiring supplies the registered indexer via the
    /// DI factory in <see cref="ProfileRepositoryServiceRegistration"/>.
    /// </param>
    public PublicKeySetResolver(ObjectDataRepository repository, IObjectDataSearchIndexer? searchIndexer = null)
    {
        this.Repository = repository;
        this.SearchIndexer = searchIndexer;
    }

    /// <summary>
    /// Gets the object-data repository the key sets are persisted in.
    /// </summary>
    protected ObjectDataRepository Repository { get; }

    /// <summary>
    /// Gets the search indexer used to gate index-served material lookups, or null when the
    /// resolver must resolve material by a single full scan (see the constructor remarks).
    /// </summary>
    protected IObjectDataSearchIndexer? SearchIndexer { get; }

    /// <inheritdoc />
    public PublicKeySetData? ResolveByHandle(string keySetHandle)
    {
        if (string.IsNullOrEmpty(keySetHandle))
        {
            return null;
        }

        // Handle is a single always-populated column, so this is one query (index-served when
        // the store is indexed, one scan otherwise) — a single store pass either way.
        return Repository.Query<PublicKeySetData>(new Dictionary<string, object>
            {
                { nameof(PublicKeySetData.KeySetHandle), keySetHandle }
            })
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

        List<PublicKeySetData> carriers = FindMaterialCarriers(new[] { publicKeyPem }, candidateIdentities)
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

        return FindMaterialCarriers(new[] { candidate.PublicRsaKey, candidate.PublicEccKey }, candidateIdentities)
            .Where(keySet => !(keySet.KeySetHandle == candidate.KeySetHandle && keySet.RevokedUtc == null))
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .ToList();
    }

    /// <summary>
    /// Finds every stored row whose material shares canonical identity with any of the given
    /// PEMs, in AT MOST ONE full store pass (bam.protocol#24 round-2 security condition 1 — this
    /// is on the anonymous pre-auth read path, so it must not fan out into several queries that
    /// each degrade to a full scan).  When the store's key-set search index is present, each
    /// material column is served from the index and columns with no index directory are skipped
    /// (a column with no directory has never held a non-null value, so nothing to find there);
    /// when there is no index (or no indexer was supplied), a single <c>RetrieveAll</c> is
    /// filtered in memory.  Every candidate hit is re-verified through
    /// <see cref="KeyMaterialIdentity.SharesMaterial"/> against the material — never the stored
    /// fingerprint stamp.
    /// </summary>
    private IReadOnlyList<PublicKeySetData> FindMaterialCarriers(IEnumerable<string?> publicKeyPems, IReadOnlyList<string> candidateIdentities)
    {
        List<string> pems = publicKeyPems.Where(pem => !string.IsNullOrEmpty(pem)).Select(pem => pem!).ToList();
        if (pems.Count == 0)
        {
            return Array.Empty<PublicKeySetData>();
        }

        Dictionary<string, PublicKeySetData> carriersByUuid = new Dictionary<string, PublicKeySetData>();
        if (SearchIndexer != null && SearchIndexer.HasIndex(typeof(PublicKeySetData)))
        {
            CollectByIndex(carriersByUuid, pems, candidateIdentities);
        }
        else
        {
            CollectByScan(carriersByUuid, candidateIdentities);
        }

        return carriersByUuid.Values.ToList();
    }

    /// <summary>
    /// Collects carriers from the search index only, querying a material column solely when it
    /// has an index directory (an absent directory means the column has no non-null values —
    /// there is nothing to find there, so the arm is skipped rather than issued as a query that
    /// would fall back to a full scan).  No full store pass is taken.
    /// </summary>
    private void CollectByIndex(Dictionary<string, PublicKeySetData> carriersByUuid, IReadOnlyList<string> pems, IReadOnlyList<string> candidateIdentities)
    {
        List<PublicKeySetData> hits = new List<PublicKeySetData>();
        foreach (string pem in pems)
        {
            string? fingerprint = KeyMaterialIdentity.CanonicalKeyFingerprint(pem);
            if (fingerprint != null)
            {
                QueryIndexedColumn(hits, nameof(PublicKeySetData.PublicRsaKeyFingerprint), fingerprint);
                QueryIndexedColumn(hits, nameof(PublicKeySetData.PublicEccKeyFingerprint), fingerprint);
            }
            QueryIndexedColumn(hits, nameof(PublicKeySetData.PublicRsaKey), pem);
            QueryIndexedColumn(hits, nameof(PublicKeySetData.PublicEccKey), pem);
        }

        AddSharedMaterial(carriersByUuid, hits, candidateIdentities);
    }

    /// <summary>
    /// Collects carriers by a single full <c>RetrieveAll</c> filtered in memory — the fallback
    /// for a store with no key-set search index (legacy, unmigrated, or convenience-constructed
    /// without an indexer).  Exactly one store pass.
    /// </summary>
    private void CollectByScan(Dictionary<string, PublicKeySetData> carriersByUuid, IReadOnlyList<string> candidateIdentities)
    {
        AddSharedMaterial(carriersByUuid, Repository.RetrieveAll<PublicKeySetData>(), candidateIdentities);
    }

    /// <summary>
    /// Issues an indexed equality query for a column, but only when that column has an index
    /// directory — otherwise skips it (no non-null values exist there) so the query can never
    /// degrade to a full scan.
    /// </summary>
    private void QueryIndexedColumn(List<PublicKeySetData> hits, string propertyName, string value)
    {
        if (SearchIndexer == null || !SearchIndexer.HasIndex(typeof(PublicKeySetData), propertyName))
        {
            return;
        }

        hits.AddRange(Repository.Query<PublicKeySetData>(new Dictionary<string, object>
        {
            { propertyName, value }
        }));
    }

    /// <summary>
    /// Adds each row that shares canonical material identity with the candidate to the carrier
    /// map, de-duplicated by row identity.
    /// </summary>
    private static void AddSharedMaterial(Dictionary<string, PublicKeySetData> carriersByUuid, IEnumerable<PublicKeySetData> rows, IReadOnlyList<string> candidateIdentities)
    {
        foreach (PublicKeySetData row in rows)
        {
            if (KeyMaterialIdentity.SharesMaterial(row, candidateIdentities))
            {
                carriersByUuid[row.Uuid] = row;
            }
        }
    }
}
