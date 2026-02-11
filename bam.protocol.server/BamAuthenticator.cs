using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Web;

namespace Bam.Protocol.Server;

public class BamAuthenticator : IAuthenticator
{
    public BamAuthenticator(IProfileManager profileManager)
    {
        ProfileManager = profileManager;
        SecurityValidator = new RequestSecurityValidator();
    }

    protected IProfileManager ProfileManager { get; set; }
    protected RequestSecurityValidator SecurityValidator { get; set; }

    public BamAuthentication Authenticate(IBamServerContext serverContext)
    {
        IBamRequest request = serverContext.BamRequest;
        IActor actor = serverContext.Actor;
        List<string> messages = new List<string>();

        // 1. Decrypt body if session keys are available
        if (serverContext.ServerSessionState != null)
        {
            string serverPrivateKey = serverContext.ServerSessionState.Get<string>("ServerPrivateKey");
            string clientPublicKey = serverContext.ServerSessionState.Get<string>("ClientPublicKey");
            if (!string.IsNullOrEmpty(serverPrivateKey) && !string.IsNullOrEmpty(clientPublicKey))
            {
                string decrypted = SecurityValidator.DecryptBody(serverContext);
                if (decrypted != null)
                {
                    request.Content = decrypted;
                    messages.Add("Body decrypted.");
                }
            }
        }

        // 2. Verify body signature if present
        if (request.Headers.ContainsKey(Headers.BodySignature))
        {
            if (!SecurityValidator.ValidateBodySignature(serverContext))
            {
                return new BamAuthentication(false, actor, request, new[] { "Body signature verification failed." });
            }
            messages.Add("Body signature verified.");
        }

        // 3. Verify nonce hash if present
        if (request.Headers.ContainsKey(Headers.NonceHash))
        {
            if (!SecurityValidator.ValidateNonceHash(serverContext))
            {
                return new BamAuthentication(false, actor, request, new[] { "Nonce hash verification failed." });
            }
            messages.Add("Nonce hash verified.");
        }

        // 4. Verify JWT from Authorization header
        if (!request.Headers.TryGetValue(Headers.Authorization, out string authHeader))
        {
            return new BamAuthentication(false, actor, request, new[] { "Authorization header missing." });
        }

        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return new BamAuthentication(false, actor, request, new[] { "Authorization header must use Bearer scheme." });
        }

        string token = authHeader.Substring("Bearer ".Length).Trim();

        BamJwtToken jwtToken;
        try
        {
            jwtToken = BamJwtToken.Decode(token);
        }
        catch
        {
            return new BamAuthentication(false, actor, request, new[] { "Invalid JWT token format." });
        }

        if (jwtToken.Expiry < DateTimeOffset.UtcNow)
        {
            return new BamAuthentication(false, actor, request, new[] { "JWT token has expired." });
        }

        // Look up the actor's profile to get the public key for JWT verification
        IProfile profile = ProfileManager.FindProfileByHandle(jwtToken.ActorHandle);
        if (profile == null)
        {
            return new BamAuthentication(false, actor, request, new[] { $"Profile not found for handle: {jwtToken.ActorHandle}" });
        }

        // Get the client's public key from session state for JWT verification
        string clientPublicKeyPem = serverContext.ServerSessionState?.Get<string>("ClientPublicKey");
        if (string.IsNullOrEmpty(clientPublicKeyPem))
        {
            return new BamAuthentication(false, actor, request, new[] { "Client public key not found in session." });
        }

        if (!BamJwtToken.Verify(token, clientPublicKeyPem.PemToKey()))
        {
            return new BamAuthentication(false, actor, request, new[] { "JWT signature verification failed." });
        }

        messages.Add("JWT verified.");
        return new BamAuthentication(true, actor, request, messages.ToArray());
    }
}
