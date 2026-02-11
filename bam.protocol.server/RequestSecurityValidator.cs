using System.Text;
using Bam.Encryption;
using Bam.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Bam.Protocol.Server;

public class RequestSecurityValidator
{
    public bool ValidateBodySignature(IBamServerContext context)
    {
        IBamRequest request = context.BamRequest;
        if (!request.Headers.TryGetValue(Headers.BodySignature, out string signatureBase64))
        {
            return false;
        }

        request.Headers.TryGetValue(Headers.BodySignatureAlgorithm, out string algorithm);
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

    public bool ValidateNonceHash(IBamServerContext context)
    {
        IBamRequest request = context.BamRequest;
        if (!request.Headers.TryGetValue(Headers.Nonce, out string nonce))
        {
            return false;
        }

        if (!request.Headers.TryGetValue(Headers.NonceHash, out string nonceHashBase64))
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

    public string DecryptBody(IBamServerContext context)
    {
        AesKey aesKey = DeriveSessionAesKey(context.ServerSessionState);
        if (aesKey == null)
        {
            return null;
        }

        try
        {
            return Aes.Decrypt(context.BamRequest.Content, aesKey);
        }
        catch
        {
            return null;
        }
    }

    public AesKey DeriveSessionAesKey(IServerSessionState state)
    {
        string serverPrivateKeyPem = state.Get<string>("ServerPrivateKey");
        string clientPublicKeyPem = state.Get<string>("ClientPublicKey");

        if (string.IsNullOrEmpty(serverPrivateKeyPem) || string.IsNullOrEmpty(clientPublicKeyPem))
        {
            return null;
        }

        byte[] serverPemBytes = Encoding.UTF8.GetBytes(serverPrivateKeyPem);
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair(serverPemBytes);
        return serverKeyPair.GetSharedAesKey(clientPublicKeyPem);
    }
}
