using System.Text;
using Bam.Encryption;
using Bam.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Bam.Protocol.Server;

/// <summary>
/// Validates request security including body signatures, nonce hashes, and body decryption.
/// </summary>
public class RequestSecurityValidator
{
    /// <summary>
    /// Validates the body signature of the request using the client's public key from session state.
    /// </summary>
    /// <param name="context">The server context containing the request to validate.</param>
    /// <returns>True if the body signature is valid; otherwise false.</returns>
    public bool ValidateBodySignature(IBamServerContext context)
    {
        IBamRequest request = context.BamRequest;
        if (!request.Headers.TryGetValue(Headers.BodySignature, out string? signatureBase64))
        {
            return false;
        }

        request.Headers.TryGetValue(Headers.BodySignatureAlgorithm, out string? algorithm);
        algorithm = algorithm ?? "SHA256WITHECDSA";

        string clientPublicKeyPem = context.ServerSessionState.Get<string>("ClientPublicKey");
        if (string.IsNullOrEmpty(clientPublicKeyPem))
        {
            return false;
        }

        try
        {
            AsymmetricKeyParameter publicKey = clientPublicKeyPem.PemToKey();
            byte[] signatureBytes = Convert.FromBase64String(signatureBase64);
            string body = request.Content ?? string.Empty;
            byte[] dataBytes = Encoding.UTF8.GetBytes(body);

            ISigner signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(false, publicKey);
            signer.BlockUpdate(dataBytes, 0, dataBytes.Length);
            return signer.VerifySignature(signatureBytes);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the nonce hash of the request by computing HMAC-SHA256 of the body with the nonce.
    /// </summary>
    /// <param name="context">The server context containing the request to validate.</param>
    /// <returns>True if the nonce hash is valid; otherwise false.</returns>
    public bool ValidateNonceHash(IBamServerContext context)
    {
        IBamRequest request = context.BamRequest;
        if (!request.Headers.TryGetValue(Headers.Nonce, out string? nonce))
        {
            return false;
        }

        if (!request.Headers.TryGetValue(Headers.NonceHash, out string? nonceHashBase64))
        {
            return false;
        }

        try
        {
            string body = request.Content ?? string.Empty;
            byte[] nonceBytes = Encoding.UTF8.GetBytes(nonce);
            byte[] computedHash = Hmac.Sha256(body, nonceBytes);
            string computedHashBase64 = Convert.ToBase64String(computedHash);

            return string.Equals(nonceHashBase64, computedHashBase64, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Decrypts the request body using the session's derived AES key.
    /// </summary>
    /// <param name="context">The server context containing the encrypted request body.</param>
    /// <returns>The decrypted body string, or null if decryption fails.</returns>
    public string DecryptBody(IBamServerContext context)
    {
        using AesKey aesKey = DeriveSessionAesKey(context.ServerSessionState)!;
        if (aesKey == null)
        {
            return null!;
        }

        try
        {
            return Aes.Decrypt(context.BamRequest.Content, aesKey);
        }
        catch
        {
            return null!;
        }
    }

    /// <summary>
    /// Derives an AES key from the server's private key and the client's public key stored in session state.
    /// </summary>
    /// <param name="state">The session state containing the key material.</param>
    /// <returns>The derived AES key, or null if keys are not available.</returns>
    public AesKey DeriveSessionAesKey(IServerSessionState state)
    {
        string serverPrivateKeyPem = state.Get<string>("ServerPrivateKey");
        string clientPublicKeyPem = state.Get<string>("ClientPublicKey");

        if (string.IsNullOrEmpty(serverPrivateKeyPem) || string.IsNullOrEmpty(clientPublicKeyPem))
        {
            return null!;
        }

        byte[] serverPemBytes = Encoding.UTF8.GetBytes(serverPrivateKeyPem);
        using EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair(serverPemBytes);
        return serverKeyPair.GetSharedAesKey(clientPublicKeyPem);
    }
}
