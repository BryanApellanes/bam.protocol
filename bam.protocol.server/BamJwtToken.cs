using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents a BAM JWT token for session-based authentication using ECDSA signatures.
/// </summary>
public class BamJwtToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamJwtToken"/> class.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="actorHandle">The actor handle (subject).</param>
    /// <param name="issuer">The token issuer.</param>
    /// <param name="expiry">The token lifetime. Defaults to one hour.</param>
    public BamJwtToken(string sessionId, string actorHandle, string issuer, TimeSpan? expiry = null)
    {
        SessionId = sessionId;
        ActorHandle = actorHandle;
        Issuer = issuer;
        IssuedAt = DateTimeOffset.UtcNow;
        Expiry = IssuedAt + (expiry ?? TimeSpan.FromHours(1));
    }

    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the actor handle (subject claim).
    /// </summary>
    public string ActorHandle { get; set; }

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the time at which the token was issued.
    /// </summary>
    public DateTimeOffset IssuedAt { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTimeOffset Expiry { get; set; }

    /// <summary>
    /// Encodes this token as a signed JWT string using the specified private key.
    /// </summary>
    /// <param name="privateKey">The ECDSA private key to sign the token with.</param>
    /// <returns>The encoded JWT string.</returns>
    public string Encode(AsymmetricKeyParameter privateKey)
    {
        string header = Base64UrlEncode(JsonSerializer.Serialize(new { alg = "ES256", typ = "JWT" }));
        string payload = Base64UrlEncode(JsonSerializer.Serialize(new
        {
            sub = ActorHandle,
            sid = SessionId,
            iss = Issuer,
            iat = IssuedAt.ToUnixTimeSeconds(),
            exp = Expiry.ToUnixTimeSeconds()
        }));

        string signingInput = $"{header}.{payload}";
        byte[] inputBytes = Encoding.UTF8.GetBytes(signingInput);

        ISigner signer = SignerUtilities.GetSigner("SHA256WITHECDSA");
        signer.Init(true, privateKey);
        signer.BlockUpdate(inputBytes, 0, inputBytes.Length);
        byte[] signatureBytes = signer.GenerateSignature();

        string signature = Base64UrlEncode(signatureBytes);
        return $"{header}.{payload}.{signature}";
    }

    /// <summary>
    /// Decodes a JWT string into a <see cref="BamJwtToken"/> without verifying the signature.
    /// </summary>
    /// <param name="token">The JWT string to decode.</param>
    /// <returns>The decoded token.</returns>
    public static BamJwtToken Decode(string token)
    {
        string[] parts = token.Split('.');
        if (parts.Length != 3)
        {
            throw new ArgumentException("Invalid JWT token format.");
        }

        string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
        using JsonDocument doc = JsonDocument.Parse(payloadJson);
        JsonElement root = doc.RootElement;

        string sub = root.GetProperty("sub").GetString() ?? string.Empty;
        string sid = root.GetProperty("sid").GetString() ?? string.Empty;
        string iss = root.GetProperty("iss").GetString() ?? string.Empty;
        long iat = root.GetProperty("iat").GetInt64();
        long exp = root.GetProperty("exp").GetInt64();

        return new BamJwtToken(sid, sub, iss)
        {
            IssuedAt = DateTimeOffset.FromUnixTimeSeconds(iat),
            Expiry = DateTimeOffset.FromUnixTimeSeconds(exp)
        };
    }

    /// <summary>
    /// Verifies the signature of a JWT string using the specified public key.
    /// </summary>
    /// <param name="token">The JWT string to verify.</param>
    /// <param name="publicKey">The ECDSA public key to verify the signature against.</param>
    /// <returns>True if the signature is valid; otherwise false.</returns>
    public static bool Verify(string token, AsymmetricKeyParameter publicKey)
    {
        string[] parts = token.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        try
        {
            string signingInput = $"{parts[0]}.{parts[1]}";
            byte[] inputBytes = Encoding.UTF8.GetBytes(signingInput);
            byte[] signatureBytes = Base64UrlDecode(parts[2]);

            ISigner signer = SignerUtilities.GetSigner("SHA256WITHECDSA");
            signer.Init(false, publicKey);
            signer.BlockUpdate(inputBytes, 0, inputBytes.Length);
            return signer.VerifySignature(signatureBytes);
        }
        catch
        {
            return false;
        }
    }

    private static string Base64UrlEncode(string input)
    {
        return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        string base64 = input.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
