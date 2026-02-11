using System.Text;
using Bam.Encryption;
using Bam.Web;

namespace Bam.Protocol.Client;

public class ClientRequestSecurityProvider
{
    public string EncryptBody(string body, AesKey sessionKey)
    {
        return Aes.Encrypt(body, sessionKey);
    }

    public string SignBody(string body, EccPublicPrivateKeyPair clientKeyPair, string algorithm = "SHA256WITHECDSA")
    {
        EccSignatureProvider provider = new EccSignatureProvider();
        ISignature signature = provider.Sign(clientKeyPair, body, algorithm);
        return signature.SignatureBase64;
    }

    public string ComputeNonceHash(string body, string nonce)
    {
        byte[] nonceBytes = Encoding.UTF8.GetBytes(nonce);
        byte[] hash = Hmac.Sha256(body, nonceBytes);
        return Convert.ToBase64String(hash);
    }

    public void PrepareHttpRequest(HttpRequestMessage httpRequest, string body, IClientSessionState sessionState)
    {
        httpRequest.Headers.Add(Headers.SessionId, sessionState.SessionId);

        sessionState.UseSessionKey(sessionKey =>
        {
            string encryptedBody = EncryptBody(body, sessionKey);
            httpRequest.Content = new StringContent(encryptedBody, Encoding.UTF8, "application/json");
        });

        if (sessionState is ClientSessionState clientState)
        {
            string signature = SignBody(body, clientState.ClientKeyPair);
            httpRequest.Headers.Add(Headers.BodySignature, signature);
        }

        string nonceHash = ComputeNonceHash(body, sessionState.Nonce);
        httpRequest.Headers.Add(Headers.Nonce, sessionState.Nonce);
        httpRequest.Headers.Add(Headers.NonceHash, nonceHash);

        if (!string.IsNullOrEmpty(sessionState.AuthorizationToken))
        {
            httpRequest.Headers.Add(Headers.Authorization, $"Bearer {sessionState.AuthorizationToken}");
        }
    }
}
