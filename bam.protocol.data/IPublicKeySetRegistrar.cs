using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// Owns the public key-set registration policy that anchors device-key account confirmation:
/// registration is first-registration-wins, a registered key set is replaced only through
/// rotation with proof of possession of the currently registered key, and handle resolution is
/// deterministic (earliest registration wins over any duplicate rows).  Consumers exposing
/// these operations over a network surface remain responsible for authenticating and
/// authorizing the caller; this policy bounds what any caller — authorized or not — can do to
/// an already-registered handle.
/// </summary>
public interface IPublicKeySetRegistrar
{
    /// <summary>
    /// Registers the first key set for a handle.  First registration wins: if a key set is
    /// already registered for <see cref="Bam.Protocol.IKeySet.KeySetHandle"/>, the attempt is
    /// rejected and the registered key set is unchanged.
    /// <para>
    /// Handle equality is <b>ordinal and case-sensitive</b>; a consumer that treats handles
    /// case-insensitively must canonicalize before calling, or <c>alice</c> and <c>Alice</c>
    /// become distinct trust anchors.  The registrar stamps the persisted row's creation time
    /// server-side (a caller-supplied <c>Created</c> is ignored) and enforces that public key
    /// material maps to exactly one handle.
    /// </para>
    /// </summary>
    /// <param name="publicKeySetData">The key set to register.</param>
    /// <returns>The persisted key set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="publicKeySetData"/> is null.</exception>
    /// <exception cref="ArgumentException">The key set's handle is null, empty, or whitespace.</exception>
    /// <exception cref="PublicKeySetConflictException">A key set is already registered for the handle.</exception>
    /// <exception cref="PublicKeySetKeyMaterialConflictException">The key material is already registered under a different handle.</exception>
    /// <exception cref="InvalidPublicKeySetException">The key material is not a parseable public key.</exception>
    PublicKeySetData Register(PublicKeySetData publicKeySetData);

    /// <summary>
    /// Replaces the registered key set for a handle, gated on proof of possession of the
    /// currently registered key: <paramref name="rotationSignature"/> must be a valid
    /// SHA512WITHRSA signature over <see cref="KeySetRotationPayload.Compose"/> of
    /// <paramref name="newKeySet"/>, made with the private key matching the currently
    /// registered public RSA key.  The existing row is updated in place — rotation never
    /// creates a second row for the handle.
    /// </summary>
    /// <param name="newKeySet">The key set to rotate to, carrying the handle being rotated.</param>
    /// <param name="rotationSignature">The raw signature bytes proving possession of the current key.</param>
    /// <returns>The updated key set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="newKeySet"/> is null.</exception>
    /// <exception cref="ArgumentException">The key set's handle is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidKeySetRotationException">No key set is registered for the handle, the proof is invalid, or the proposed key material is not parseable.</exception>
    PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature);

    /// <summary>
    /// Resolves the authoritative key set for a handle deterministically: if duplicate rows
    /// exist, the earliest-created row wins, so a later-added row can never displace the first
    /// registration.  Neither <see cref="Register"/> nor <see cref="Rotate"/> creates
    /// cross-handle key-material duplicates (both enforce one handle-to-one key set), so
    /// duplicates arise only from legacy data or direct store tampering below the registrar.
    /// </summary>
    /// <param name="keySetHandle">The handle to resolve.</param>
    /// <returns>The authoritative key set, or null when none is registered.</returns>
    PublicKeySetData? Resolve(string keySetHandle);
}
