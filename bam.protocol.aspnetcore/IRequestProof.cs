namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Verifies the per-request proof of key possession: a signature over the raw request body made with
/// the actor's registered ECC key (the existing <c>X-Bam-Body-Signature</c> convention).
/// </summary>
public interface IRequestProof
{
    /// <summary>
    /// Verifies a body signature.
    /// </summary>
    /// <param name="body">The raw request body exactly as received.</param>
    /// <param name="signatureBase64">The signature from the <c>X-Bam-Body-Signature</c> header.</param>
    /// <param name="algorithm">The algorithm from <c>X-Bam-Body-Signature-Algorithm</c>, or null for the default.</param>
    /// <param name="eccPublicKeyPem">The PEM of the actor's registered ECC public key.</param>
    /// <returns>True when the signature verifies; false for any failure, including malformed input.</returns>
    bool Verify(string body, string signatureBase64, string? algorithm, string eccPublicKeyPem);
}
