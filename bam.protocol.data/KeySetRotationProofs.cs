using Bam.Logging;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// The ordered list of rotation proofs a host trusts.  The profile registration seeds it with
/// the framework proofs; a host adds its own (bam.useraccounts adds a passkey proof) at
/// registration time.  Adding is thread-safe; the list is read on every rotation.
/// </summary>
public class KeySetRotationProofs
{
    private readonly object _sync = new object();
    private List<IKeySetRotationProof> _proofs = new List<IKeySetRotationProof>();

    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetRotationProofs"/> class holding
    /// <paramref name="proofs"/> in order.
    /// </summary>
    /// <param name="proofs">The proofs to start with.</param>
    public KeySetRotationProofs(params IKeySetRotationProof[] proofs)
    {
        foreach (IKeySetRotationProof proof in proofs)
        {
            Add(proof);
        }
    }

    /// <summary>
    /// Gets a snapshot of the proofs, in the order they are tried.
    /// </summary>
    public IReadOnlyList<IKeySetRotationProof> All => _proofs.AsReadOnly();

    /// <summary>
    /// Appends <paramref name="proof"/> unless a proof with the same <see cref="IKeySetRotationProof.Name"/>
    /// is already present; a duplicate is logged and dropped, so a host proof can never replace a
    /// framework one.  There is no remove: a host that trusts fewer proofs than the defaults
    /// registers its own <see cref="KeySetRotationProofs"/> instead.
    /// </summary>
    /// <param name="proof">The proof to trust.</param>
    /// <returns>This instance, for chaining.</returns>
    public KeySetRotationProofs Add(IKeySetRotationProof proof)
    {
        ArgumentNullException.ThrowIfNull(proof);
        lock (_sync)
        {
            if (_proofs.Any(existing => string.Equals(existing.Name, proof.Name, StringComparison.Ordinal)))
            {
                Log.Warn("Ignored a key-set rotation proof named '{0}': a proof with that name is already trusted.", proof.Name);
                return this;
            }
            // copy-on-write so a rotation reading All never sees a list being changed
            _proofs = new List<IKeySetRotationProof>(_proofs) { proof };
        }
        return this;
    }

    /// <summary>
    /// Returns the proofs that anchor <paramref name="keySet"/>, in order.
    /// </summary>
    /// <param name="keySet">The key set a rotation would move away from.</param>
    /// <returns>The anchoring proofs.</returns>
    public IReadOnlyList<IKeySetRotationProof> Anchoring(PublicKeySetData keySet)
    {
        return All.Where(proof => proof.Anchors(keySet)).ToList();
    }

    /// <summary>
    /// Returns whether any trusted proof anchors <paramref name="keySet"/>, i.e. whether a key
    /// set could ever be rotated away from once registered.
    /// </summary>
    /// <param name="keySet">The key set to check.</param>
    /// <returns>True when some proof anchors it.</returns>
    public bool AnyAnchors(PublicKeySetData keySet)
    {
        return All.Any(proof => proof.Anchors(keySet));
    }
}
