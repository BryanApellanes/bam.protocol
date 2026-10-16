namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Verifies the signature in a per-request proof of key possession, made with the actor's registered ECC
/// key and carried in <c>X-Bam-Body-Signature</c>. Despite the header's name, the signature covers the whole
/// canonical request, of which the body is one part (its digest). What is signed is the <see cref="CanonicalRequest"/>,
/// which <see cref="RequestProofEvaluator"/> builds; this contract checks only the signature.
/// </summary>
public interface IRequestProof
{
    /// <summary>
    /// Verifies a signature over the given text.
    /// </summary>
    /// <param name="signedText">The signed text: the canonical request.</param>
    /// <param name="signatureBase64">The signature from the <c>X-Bam-Body-Signature</c> header.</param>
    /// <param name="algorithm">The algorithm from <c>X-Bam-Body-Signature-Algorithm</c>, or null for the default.</param>
    /// <param name="eccPublicKeyPem">The PEM of the actor's registered ECC public key.</param>
    /// <returns>True when the signature verifies; false for any failure, including malformed input.</returns>
    bool Verify(string signedText, string signatureBase64, string? algorithm, string eccPublicKeyPem);
}
