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
/// </summary>
public static class RevocationPayload
{
    /// <summary>
    /// Composes the canonical revocation payload for a target key set: its handle, the SHA-256
    /// of its currently registered public RSA key, and its <c>Uuid</c> — each length-prefixed so
    /// the concatenation is injective.  The revocation proof is a SHA512WITHRSA signature over the
    /// UTF-8 encoding of this string, produced with the break-glass admin private key.
    /// </summary>
    /// <param name="target">The registered key set being revoked.</param>
    /// <returns>The canonical payload string to sign or verify.</returns>
    public static string Compose(PublicKeySetData target)
    {
        StringBuilder payload = new StringBuilder();
        AppendField(payload, target.KeySetHandle);
        AppendField(payload, (target.PublicRsaKey ?? string.Empty).Sha256());
        AppendField(payload, target.Uuid);
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
