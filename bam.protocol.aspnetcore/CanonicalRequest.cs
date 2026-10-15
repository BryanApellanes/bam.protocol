using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The text a request proof signs. It binds the signature to one request: the method, the path and query
/// as sent, a timestamp, a single-use nonce, and the SHA-256 of the raw body bytes, one per line after a
/// version line. Both sides build it here, the client through <see cref="RequestProofSigner"/> and the host
/// through <see cref="RequestProofEvaluator"/>, so they cannot drift (bam.protocol#46).
/// <code>
/// BAM-REQUEST-PROOF-1
/// POST
/// /actor-api/hello
/// ?lang=en
/// 1760572800
/// 3f1c9d0a6b2e4f7a8c1d5e9b0a2f4c6d
/// 9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08
/// </code>
/// </summary>
public static class CanonicalRequest
{
    /// <summary>The first line of every canonical request; a later format changes it.</summary>
    public const string Version = "BAM-REQUEST-PROOF-1";

    /// <summary>The request header carrying the proof's timestamp, in Unix seconds.</summary>
    public const string TimestampHeader = "X-Bam-Timestamp";

    /// <summary>The shortest nonce accepted.</summary>
    public const int MinimumNonceLength = 16;

    /// <summary>The longest nonce accepted.</summary>
    public const int MaximumNonceLength = 128;

    /// <summary>Builds the canonical request text.</summary>
    /// <param name="method">The HTTP method; compared upper-case.</param>
    /// <param name="path">The escaped path, as <see cref="CanonicalPath"/> returns it.</param>
    /// <param name="query">The query string as sent, including its leading <c>?</c>, or empty.</param>
    /// <param name="timestamp">The proof's timestamp, in Unix seconds.</param>
    /// <param name="nonce">The proof's nonce; must satisfy <see cref="IsWellFormedNonce"/>.</param>
    /// <param name="bodySha256Hex">The lowercase hex SHA-256 of the raw body bytes.</param>
    /// <returns>The text to sign or verify.</returns>
    /// <exception cref="ArgumentException">An input could break the line structure.</exception>
    public static string Compose(string method, string path, string query, long timestamp, string nonce, string bodySha256Hex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(method);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(bodySha256Hex);
        if (!IsWellFormedNonce(nonce))
        {
            throw new ArgumentException("The nonce must be 16 to 128 characters of A-Z, a-z, 0-9, '-' or '_'.", nameof(nonce));
        }

        if (ContainsLineBreak(method) || ContainsLineBreak(path) || ContainsLineBreak(query))
        {
            throw new ArgumentException("The method, path and query must not contain line breaks.");
        }

        StringBuilder canonical = new StringBuilder();
        canonical.Append(Version).Append('\n');
        canonical.Append(method.ToUpperInvariant()).Append('\n');
        canonical.Append(path).Append('\n');
        canonical.Append(query).Append('\n');
        canonical.Append(timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        canonical.Append(nonce).Append('\n');
        canonical.Append(bodySha256Hex);
        return canonical.ToString();
    }

    /// <summary>The escaped form of a path, the same on both sides whatever escaping the client used.</summary>
    /// <param name="pathBase">The host's path base, or empty.</param>
    /// <param name="path">The request path.</param>
    /// <returns>The escaped path.</returns>
    public static string CanonicalPath(PathString pathBase, PathString path)
    {
        return pathBase.Add(path).ToUriComponent();
    }

    /// <summary>The lowercase hex SHA-256 of a byte sequence.</summary>
    /// <param name="body">The raw body bytes.</param>
    /// <returns>The digest.</returns>
    public static string BodyDigest(byte[] body)
    {
        ArgumentNullException.ThrowIfNull(body);
        return Convert.ToHexStringLower(SHA256.HashData(body));
    }

    /// <summary>The lowercase hex SHA-256 of a stream, read from its current position to the end.</summary>
    /// <param name="body">The raw body stream.</param>
    /// <returns>The digest.</returns>
    public static async Task<string> BodyDigestAsync(Stream body)
    {
        ArgumentNullException.ThrowIfNull(body);
        byte[] digest = await SHA256.HashDataAsync(body).ConfigureAwait(false);
        return Convert.ToHexStringLower(digest);
    }

    /// <summary>Whether a nonce has the accepted shape: 16 to 128 characters of A-Z, a-z, 0-9, '-' or '_'.</summary>
    /// <param name="nonce">The candidate nonce.</param>
    /// <returns>True when it is well formed.</returns>
    public static bool IsWellFormedNonce(string? nonce)
    {
        if (nonce is null || nonce.Length < MinimumNonceLength || nonce.Length > MaximumNonceLength)
        {
            return false;
        }

        foreach (char c in nonce)
        {
            bool allowed = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-' || c == '_';
            if (!allowed)
            {
                return false;
            }
        }

        return true;
    }

    private static bool ContainsLineBreak(string value)
    {
        return value.Contains('\n', StringComparison.Ordinal) || value.Contains('\r', StringComparison.Ordinal);
    }
}
