using System.Text;
using Bam;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// Composes the single canonical serialization a break-glass revocation proof is made over.
/// The admin signs this payload offline (e.g. on a YubiKey); <see cref="IRevocationAuthority"/>
/// verifies the signature against the admin public key.  Both sides MUST use this composition.
/// <para>
/// Like <see cref="KeySetRotationPayload"/>, each field is length-prefixed by its UTF-8 byte
/// count so the concatenation is injective over bytes (bam.protocol#8 review, condition C1).
/// </para>
/// <para>
/// The payload is <b>target-bound</b>: it binds the key set's handle, the SHA-256 of its
/// currently registered public RSA key, and the row's <see cref="Bam.Data.Repositories.RepoData.Uuid"/>.
/// A captured revocation proof therefore names the exact registration it authorizes revoking and
/// cannot be replayed against a differently-keyed re-registration of the same handle — the
/// revocation analogue of the rotation proof's current-key binding (condition C2a).  A
/// consume-once freshness nonce beyond target-binding is tracked in bam.protocol#15.
/// </para>
/// <para>
/// The payload additionally binds the <b>authorized successor</b> (bam.protocol#21): the canonical
/// fingerprint of the one key the admin permits to re-register the handle after revocation, or the
/// empty string when the revocation binds no successor.  Because the successor is a signed field,
/// the same single admin proof both authorizes the revocation and names the successor — a captured
/// proof cannot be re-purposed to authorize a <i>different</i> successor, and an unbound (empty)
/// revocation cannot be upgraded to a bound one without re-signing.
/// </para>
/// </summary>
public static class RevocationPayload
{
    /// <summary>
    /// Composes the canonical revocation payload for a target key set: its handle, the SHA-256
    /// of its currently registered public RSA key, its <c>Uuid</c>, and the authorized successor
    /// fingerprint (empty when none) — each length-prefixed so the concatenation is injective.  The
    /// revocation proof is a SHA512WITHRSA signature over the UTF-8 encoding of this string, produced
    /// with the break-glass admin private key.
    /// </summary>
    /// <param name="target">The registered key set being revoked.</param>
    /// <param name="successorFingerprint">
    /// The canonical fingerprint (<see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/>) of the key
    /// authorized to re-register the freed handle, or null/empty to bind no successor (leaving the
    /// handle openly re-registrable).
    /// </param>
    /// <returns>The canonical payload string to sign or verify.</returns>
    public static string Compose(PublicKeySetData target, string? successorFingerprint)
    {
        StringBuilder payload = new StringBuilder();
        AppendField(payload, target.KeySetHandle);
        AppendField(payload, (target.PublicRsaKey ?? string.Empty).Sha256());
        AppendField(payload, target.Uuid);
        AppendField(payload, successorFingerprint);
        return payload.ToString();
    }

    private static void AppendField(StringBuilder payload, string? value)
    {
        string fieldValue = value ?? string.Empty;
        payload.Append(Encoding.UTF8.GetByteCount(fieldValue));
        payload.Append(':');
        payload.Append(fieldValue);
        payload.Append(',');
    }
}
