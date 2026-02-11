using System.Text;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("AuthPipeline should")]
public class AuthPipelineShould : UnitTestMenuContainer
{
    [UnitTest]
    public void AuthenticateWithValidJwt()
    {
        EccPublicPrivateKeyPair keyPair = new EccPublicPrivateKeyPair();
        PrivateKeyProvider privateKey = new PrivateKeyProvider(keyPair);
        string actorHandle = 8.RandomLetters();
        string sessionId = 16.RandomLetters();

        BamJwtToken jwtToken = new BamJwtToken(sessionId, actorHandle, "bam-test");
        string encodedToken = jwtToken.Encode(privateKey.GetPrivateKey());

        When.A<BamAuthenticator>("authenticates a valid JWT",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                IProfile profile = Substitute.For<IProfile>();
                profile.ProfileHandle.Returns(actorHandle);
                profile.Name.Returns("Test User");
                profileManager.FindProfileByHandle(actorHandle).Returns(profile);
                return new BamAuthenticator(profileManager);
            },
            (authenticator) =>
            {
                IBamServerContext context = CreateMockContext(actorHandle, sessionId, encodedToken, keyPair.PublicKeyPem);
                return authenticator.Authenticate(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            BamAuthentication auth = because.TheResult.As<BamAuthentication>();
            because.ItsTrue("authentication succeeded", auth.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectInvalidJwt()
    {
        EccPublicPrivateKeyPair keyPair = new EccPublicPrivateKeyPair();
        PrivateKeyProvider privateKey = new PrivateKeyProvider(keyPair);
        string actorHandle = 8.RandomLetters();
        string sessionId = 16.RandomLetters();

        BamJwtToken jwtToken = new BamJwtToken(sessionId, actorHandle, "bam-test");
        string encodedToken = jwtToken.Encode(privateKey.GetPrivateKey());

        // Tamper with the token
        string[] parts = encodedToken.Split('.');
        parts[1] = parts[1] + "x";
        string tamperedToken = string.Join(".", parts);

        When.A<BamAuthenticator>("rejects an invalid JWT",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                IProfile profile = Substitute.For<IProfile>();
                profile.ProfileHandle.Returns(actorHandle);
                profileManager.FindProfileByHandle(actorHandle).Returns(profile);
                return new BamAuthenticator(profileManager);
            },
            (authenticator) =>
            {
                IBamServerContext context = CreateMockContext(actorHandle, sessionId, tamperedToken, keyPair.PublicKeyPem);
                return authenticator.Authenticate(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            BamAuthentication auth = because.TheResult.As<BamAuthentication>();
            because.ItsTrue("authentication failed", !auth.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ValidateBodySignature()
    {
        EccPublicPrivateKeyPair keyPair = new EccPublicPrivateKeyPair();
        string body = "test body content";

        When.A<RequestSecurityValidator>("validates body signature",
            () => new RequestSecurityValidator(),
            (validator) =>
            {
                // Sign the body using ECC key source
                EccSignatureProvider provider = new EccSignatureProvider();
                ISignature signature = provider.Sign(keyPair, body, "SHA256WITHECDSA");

                IBamServerContext context = Substitute.For<IBamServerContext>();
                IBamRequest request = Substitute.For<IBamRequest>();
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { Headers.BodySignature, Convert.ToBase64String(signature.SignatureBytes) },
                    { Headers.BodySignatureAlgorithm, "SHA256WITHECDSA" }
                };
                request.Headers.Returns(headers);
                request.Content.Returns(body);
                context.BamRequest.Returns(request);

                IServerSessionState sessionState = Substitute.For<IServerSessionState>();
                sessionState.Get<string>("ClientPublicKey").Returns(keyPair.PublicKeyPem);
                context.ServerSessionState.Returns(sessionState);

                return validator.ValidateBodySignature(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.Is<bool>("body signature is valid", b => b);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ValidateNonceHash()
    {
        string body = "test body for nonce";
        string nonce = 32.RandomLetters();

        When.A<RequestSecurityValidator>("validates nonce hash",
            () => new RequestSecurityValidator(),
            (validator) =>
            {
                byte[] nonceBytes = Encoding.UTF8.GetBytes(nonce);
                byte[] hash = Hmac.Sha256(body, nonceBytes);
                string hashBase64 = Convert.ToBase64String(hash);

                IBamServerContext context = Substitute.For<IBamServerContext>();
                IBamRequest request = Substitute.For<IBamRequest>();
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { Headers.Nonce, nonce },
                    { Headers.NonceHash, hashBase64 }
                };
                request.Headers.Returns(headers);
                request.Content.Returns(body);
                context.BamRequest.Returns(request);

                return validator.ValidateNonceHash(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.Is<bool>("nonce hash is valid", b => b);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DecryptBody()
    {
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        string originalBody = "secret message content";

        When.A<RequestSecurityValidator>("decrypts body",
            () => new RequestSecurityValidator(),
            (validator) =>
            {
                // Client encrypts with session key
                AesKey sessionKey = clientKeyPair.GetSharedAesKey(serverKeyPair.PublicKeyPem);
                string encryptedBody = Aes.Encrypt(originalBody, sessionKey);

                IBamServerContext context = Substitute.For<IBamServerContext>();
                IBamRequest request = Substitute.For<IBamRequest>();
                request.Content.Returns(encryptedBody);
                context.BamRequest.Returns(request);

                // Server derives same key from its private key + client's public key
                IServerSessionState sessionState = Substitute.For<IServerSessionState>();
                string serverPrivateKeyPem = new PrivateKeyProvider(serverKeyPair).GetPrivateKey().ToPem();
                sessionState.Get<string>("ServerPrivateKey").Returns(serverPrivateKeyPem);
                sessionState.Get<string>("ClientPublicKey").Returns(clientKeyPair.PublicKeyPem);
                context.ServerSessionState.Returns(sessionState);

                return validator.DecryptBody(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            because.ItsTrue("decrypted body matches original", because.Result.Equals(originalBody));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RunAuthenticationInitializationHandler()
    {
        EccPublicPrivateKeyPair keyPair = new EccPublicPrivateKeyPair();
        PrivateKeyProvider privateKey = new PrivateKeyProvider(keyPair);
        string actorHandle = 8.RandomLetters();
        string sessionId = 16.RandomLetters();
        bool canContinue = false;

        BamJwtToken jwtToken = new BamJwtToken(sessionId, actorHandle, "bam-test");
        string encodedToken = jwtToken.Encode(privateKey.GetPrivateKey());

        When.A<AuthenticationInitializationHandler>("handles authentication initialization",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                IProfile profile = Substitute.For<IProfile>();
                profile.ProfileHandle.Returns(actorHandle);
                profileManager.FindProfileByHandle(actorHandle).Returns(profile);
                IAuthenticator authenticator = new BamAuthenticator(profileManager);
                return new AuthenticationInitializationHandler(authenticator);
            },
            (handler) =>
            {
                IBamServerContext context = CreateMockContext(actorHandle, sessionId, encodedToken, keyPair.PublicKeyPem);

                BamServerInitializationContext initialization = new HttpBamServerInitializationContext();
                initialization.ServerContext = context;
                initialization.CanContinue = true;

                BamServerInitializationContext result = handler.HandleInitialization(initialization);
                canContinue = result.CanContinue;
                return canContinue;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("can continue after authentication", canContinue);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static IBamServerContext CreateMockContext(string actorHandle, string sessionId, string encodedToken, string clientPublicKeyPem)
    {
        IBamServerContext context = Substitute.For<IBamServerContext>();
        IBamRequest request = Substitute.For<IBamRequest>();
        Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { Headers.Authorization, $"Bearer {encodedToken}" }
        };
        request.Headers.Returns(headers);
        context.BamRequest.Returns(request);

        IActor actor = Substitute.For<IActor>();
        actor.Handle.Returns(actorHandle);
        context.Actor.Returns(actor);

        IServerSessionState sessionState = Substitute.For<IServerSessionState>();
        sessionState.SessionId.Returns(sessionId);
        sessionState.Get<string>("ClientPublicKey").Returns(clientPublicKeyPem);
        context.ServerSessionState.Returns(sessionState);

        return context;
    }
}
