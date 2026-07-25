using System.Text;

namespace Bam.Protocol.Data;

/// <summary>
/// Composes the single canonical serialization of a key-set rotation that rotation signatures
/// are made over.  Both sides of a rotation — the client signing with the private key matching
/// the currently registered RSA key, and the server-side verifier — MUST use this composition;
/// any drift between signer and verifier makes legitimate rotations fail.
/// <para>
/// The serialization is <b>unambiguous</b>: each field is length-prefixed (netstring form
/// <c>{charLength}:{value},</c>) so no re-splitting of the fields across the multi-line PEM
/// boundaries can produce identical bytes for a different field tuple.  A newline-joined form
/// is <i>malleable</i> — because PEM values themselves contain newlines, one signature could be
/// re-interpreted as authorizing a different key set (bam.protocol#8 review, condition C1).
/// </para>
/// <para>
/// The payload also binds the SHA-256 of the <i>currently registered</i> public RSA key (the
/// key being rotated away from), so a captured proof names the exact key it rotates from and
/// cannot be replayed against a different current key (condition C2a).  Freshness against
/// straight replay after a rollback (a nonce) is tracked separately (bam.protocol#15).
/// </para>
/// </summary>
public static class KeySetRotationPayload
{
    /// <summary>
    /// Composes the canonical rotation payload: the SHA-256 of the currently registered public
    /// RSA key, followed by the proposed key set's handle, public RSA key, and public ECC key —
    /// each length-prefixed so the concatenation is injective.  The rotation signature is a
    /// SHA512WITHRSA signature over the UTF-8 encoding of this string, produced with the private
    /// key matching the currently registered public RSA key.
    /// </summary>
    /// <param name="currentPublicRsaKeySha256">SHA-256 of the currently registered public RSA key (the key being rotated away from).</param>
    /// <param name="proposed">The proposed key set being rotated to.</param>
    /// <returns>The canonical payload string to sign or verify.</returns>
    public static string Compose(string currentPublicRsaKeySha256, IKeySet proposed)
    {
        StringBuilder payload = new StringBuilder();
        AppendField(payload, currentPublicRsaKeySha256);
        AppendField(payload, proposed.KeySetHandle);
        AppendField(payload, proposed.PublicRsaKey);
        AppendField(payload, proposed.PublicEccKey);
        return payload.ToString();
    }

    private static void AppendField(StringBuilder payload, string? value)
    {
        string fieldValue = value ?? string.Empty;
        // Prefix the UTF-8 BYTE count, not the UTF-16 char count: the signature is computed over
        // the UTF-8 bytes of this payload, so a char-count prefix would let a lone surrogate and
        // the replacement character (both Length==1, same UTF-8 bytes) collide, and would mislead
        // a non-.NET signer. Byte-length prefixing keeps the concatenation injective over bytes.
        payload.Append(Encoding.UTF8.GetByteCount(fieldValue));
        payload.Append(':');
        payload.Append(fieldValue);
        payload.Append(',');
    }
}
