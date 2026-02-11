using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Bam.Protocol.Server;

public class BamJwtToken
{
    public BamJwtToken(string sessionId, string actorHandle, string issuer, TimeSpan? expiry = null)
    {
        SessionId = sessionId;
        ActorHandle = actorHandle;
        Issuer = issuer;
        IssuedAt = DateTimeOffset.UtcNow;
        Expiry = IssuedAt + (expiry ?? TimeSpan.FromHours(1));
    }

    public string SessionId { get; set; }
    public string ActorHandle { get; set; }
    public string Issuer { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset Expiry { get; set; }

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
