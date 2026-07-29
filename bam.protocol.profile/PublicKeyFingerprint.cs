using Bam;
using Bam.Encryption;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

/// <summary>
/// Computes a re-encoding-independent fingerprint of a PEM-encoded public key: the SHA-256 of
/// the parsed key's canonical DER <c>SubjectPublicKeyInfo</c> encoding.  This is the single shared
/// contract that both sides of successor-binding (bam.protocol#21) agree on — the admin computes a
/// successor's fingerprint when authorizing a revocation, and <see cref="PublicKeySetRegistrar"/>
/// recomputes a candidate's fingerprint when gating re-registration of a freed handle — so the two
/// must never drift.  It is also the equality basis for the one-handle-to-one-key-material invariant.
/// <para>
/// Ordinal PEM comparison is deliberately avoided: a trivial re-encoding (an appended newline, CRLF
/// line endings, trailing spaces) parses to the identical key while being string- and SHA-unequal,
/// which would let a revoked/compromised key slip past the blocklist, the uniqueness check, and the
/// successor gate (bam.protocol#18 review, condition C1 / T1).
/// </para>
/// </summary>
public static class PublicKeyFingerprint
{
    /// <summary>
    /// Returns the SHA-256 of the parsed key's canonical DER <c>SubjectPublicKeyInfo</c> encoding,
    /// or null when <paramref name="pem"/> is null, empty, or does not parse to a public key.  Two
    /// PEM strings that parse to the same key yield the same non-null fingerprint regardless of
    /// their textual encoding.
    /// </summary>
    /// <param name="pem">The PEM-encoded public key, or null/empty.</param>
    /// <returns>The canonical fingerprint, or null when the input is empty or unparseable.</returns>
    public static string? Of(string? pem)
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
}
