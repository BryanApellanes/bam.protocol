namespace Bam.Protocol.Data;

/// <summary>
/// Composes the single canonical serialization of a key set that rotation signatures are made
/// over.  Both sides of a rotation — the client signing with the private key matching the
/// currently registered RSA key, and the server-side verifier — MUST use this composition;
/// any drift between signer and verifier makes legitimate rotations fail.
/// </summary>
public static class KeySetRotationPayload
{
    /// <summary>
    /// Composes the canonical rotation payload for the specified key set: the key-set handle,
    /// the PEM-encoded public RSA key, and the PEM-encoded public ECC key, joined by single
    /// newline characters.  The rotation signature is a SHA512WITHRSA signature over the UTF-8
    /// encoding of this string, produced with the private key matching the currently
    /// registered public RSA key.
    /// </summary>
    /// <param name="keySet">The proposed key set being rotated to.</param>
    /// <returns>The canonical payload string to sign or verify.</returns>
    public static string Compose(IKeySet keySet)
    {
        return $"{keySet.KeySetHandle}\n{keySet.PublicRsaKey}\n{keySet.PublicEccKey}";
    }
}
