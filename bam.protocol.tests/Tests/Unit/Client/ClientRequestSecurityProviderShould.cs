using System.Text;
using Bam.Encryption;
using Bam.Protocol.Client;
using Bam.Protocol.Data;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("ClientRequestSecurityProvider should", "crsps")]
public class ClientRequestSecurityProviderShould : UnitTestMenuContainer
{
    [UnitTest]
    public void EncryptBodyRoundtrip()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();
        string originalBody = "secret request body content";

        When.A<ClientRequestSecurityProvider>("encrypts body that server can decrypt",
            () => new ClientRequestSecurityProvider(),
            (provider) =>
            {
                // Client encrypts with ECDH-derived key
                AesKey clientKey = clientKeyPair.GetSharedAesKey(serverKeyPair.PublicKeyPem);
                string encrypted = provider.EncryptBody(originalBody, clientKey);

                // Server decrypts with its own ECDH-derived key
                AesKey serverKey = serverKeyPair.GetSharedAesKey(clientKeyPair.PublicKeyPem);
                string decrypted = Aes.Decrypt(encrypted, serverKey);

                return decrypted;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("decrypted body matches original", because.Result.Equals(originalBody));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SignBodyRoundtrip()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        string body = "body to sign";

        When.A<ClientRequestSecurityProvider>("signs body that server can verify",
            () => new ClientRequestSecurityProvider(),
            (provider) =>
            {
                string signatureBase64 = provider.SignBody(body, clientKeyPair);

                // Server verifies using RequestSecurityValidator pattern
                IBamServerContext context = Substitute.For<IBamServerContext>();
                IBamRequest request = Substitute.For<IBamRequest>();
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { Headers.BodySignature, signatureBase64 },
                    { Headers.BodySignatureAlgorithm, "SHA256WITHECDSA" }
                };
                request.Headers.Returns(headers);
                request.Content.Returns(body);
                context.BamRequest.Returns(request);

                IServerSessionState sessionState = Substitute.For<IServerSessionState>();
                sessionState.Get<string>("ClientPublicKey").Returns(clientKeyPair.PublicKeyPem);
                context.ServerSessionState.Returns(sessionState);

                RequestSecurityValidator validator = new RequestSecurityValidator();
                return validator.ValidateBodySignature(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.Is<bool>("server validated the signature", b => b);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ComputeNonceHashRoundtrip()
    {
        string body = "body for nonce hash";
        string nonce = 32.RandomLetters();

        When.A<ClientRequestSecurityProvider>("computes nonce hash that server can verify",
            () => new ClientRequestSecurityProvider(),
            (provider) =>
            {
                string nonceHash = provider.ComputeNonceHash(body, nonce);

                // Server verifies using RequestSecurityValidator pattern
                IBamServerContext context = Substitute.For<IBamServerContext>();
                IBamRequest request = Substitute.For<IBamRequest>();
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { Headers.Nonce, nonce },
                    { Headers.NonceHash, nonceHash }
                };
                request.Headers.Returns(headers);
                request.Content.Returns(body);
                context.BamRequest.Returns(request);

                RequestSecurityValidator validator = new RequestSecurityValidator();
                return validator.ValidateNonceHash(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.Is<bool>("server validated the nonce hash", b => b);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void PrepareHttpRequestSetsHeaders()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();
        string body = "request body to secure";

        When.A<ClientRequestSecurityProvider>("prepares HTTP request with security headers",
            () => new ClientRequestSecurityProvider(),
            (provider) =>
            {
                ClientSessionState sessionState = new ClientSessionState(
                    "session-123",
                    "nonce-456",
                    serverKeyPair.GetEccPublicKey(),
                    clientKeyPair);

                HttpRequestMessage httpRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "http://localhost/test");
                provider.PrepareHttpRequest(httpRequest, body, sessionState);

                bool hasSessionId = httpRequest.Headers.Contains(Headers.SessionId);
                bool hasBodySignature = httpRequest.Headers.Contains(Headers.BodySignature);
                bool hasNonce = httpRequest.Headers.Contains(Headers.Nonce);
                bool hasNonceHash = httpRequest.Headers.Contains(Headers.NonceHash);
                bool hasContent = httpRequest.Content != null;

                return new object[] { hasSessionId, hasBodySignature, hasNonce, hasNonceHash, hasContent };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("has session ID header", r => (bool)r[0])
                .As<object[]>("has body signature header", r => (bool)r[1])
                .As<object[]>("has nonce header", r => (bool)r[2])
                .As<object[]>("has nonce hash header", r => (bool)r[3])
                .As<object[]>("has encrypted content", r => (bool)r[4]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void PrepareTcpRequestSetsHeaders()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();
        string body = "request body to secure";

        When.A<ClientRequestSecurityProvider>("prepares TCP request with security headers",
            () => new ClientRequestSecurityProvider(),
            (provider) =>
            {
                ClientSessionState sessionState = new ClientSessionState(
                    "session-123",
                    "nonce-456",
                    serverKeyPair.GetEccPublicKey(),
                    clientKeyPair);

                TcpClientRequest tcpRequest = new TcpClientRequest();
                string encryptedBody = provider.PrepareTcpRequest(tcpRequest, body, sessionState);

                bool hasSessionId = tcpRequest.Headers.ContainsKey(Headers.SessionId);
                bool hasBodySignature = tcpRequest.Headers.ContainsKey(Headers.BodySignature);
                bool hasNonce = tcpRequest.Headers.ContainsKey(Headers.Nonce);
                bool hasNonceHash = tcpRequest.Headers.ContainsKey(Headers.NonceHash);
                bool bodyIsEncrypted = !body.Equals(encryptedBody);

                return new object[] { hasSessionId, hasBodySignature, hasNonce, hasNonceHash, bodyIsEncrypted };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("has session ID header", r => (bool)r[0])
                .As<object[]>("has body signature header", r => (bool)r[1])
                .As<object[]>("has nonce header", r => (bool)r[2])
                .As<object[]>("has nonce hash header", r => (bool)r[3])
                .As<object[]>("body is encrypted (different from original)", r => (bool)r[4]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
