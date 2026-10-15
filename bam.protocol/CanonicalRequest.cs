using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Bam.Protocol;

/// <summary>
/// The text a request proof signs. It binds the signature to one request: the method, the path and query exactly
/// as they appear on the request line, a timestamp, a single-use nonce, and the SHA-256 of the raw body bytes, one
/// per line after a version line. Clients build it through <see cref="RequestProofSigner"/> and hosts rebuild it
/// from the request they received, so both sides use this one definition (bam.protocol#46).
/// <code>
/// BAM-REQUEST-PROOF-1
/// POST
/// /actor-api/hello
/// ?lang=en
/// 1760572800
/// 3f1c9d0a6b2e4f7a8c1d5e9b0a2f4c6d
/// 9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08
/// </code>
/// The path and query are the request target's bytes as sent, split at the first <c>?</c>, with no decoding or
/// re-escaping, so a client in any language signs exactly what it puts on the request line.
/// </summary>
public static class CanonicalRequest
{
    /// <summary>The first line of every canonical request; a later format changes it.</summary>
    public const string Version = "BAM-REQUEST-PROOF-1";

    /// <summary>The request header carrying the proof's timestamp, in Unix seconds.</summary>
    public const string TimestampHeader = "X-Bam-Timestamp";

    /// <summary>
    /// The request header carrying the proof's single-use nonce. It is not <c>X-Bam-Nonce</c>, which the native
    /// transport uses for its session nonce.
    /// </summary>
    public const string NonceHeader = "X-Bam-Proof-Nonce";

    /// <summary>The signature algorithm a client uses unless the host allows another.</summary>
    public const string DefaultAlgorithm = "SHA256WITHECDSA";

    /// <summary>The shortest nonce accepted.</summary>
    public const int MinimumNonceLength = 16;

    /// <summary>The longest nonce accepted.</summary>
    public const int MaximumNonceLength = 128;

    /// <summary>Builds the canonical request text.</summary>
    /// <param name="method">The HTTP method; compared upper-case.</param>
    /// <param name="path">The path as on the request line.</param>
    /// <param name="query">The query as on the request line, including its leading <c>?</c>, or empty.</param>
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
        canonical.Append(timestamp.ToString(CultureInfo.InvariantCulture)).Append('\n');
        canonical.Append(nonce).Append('\n');
        canonical.Append(bodySha256Hex);
        return canonical.ToString();
    }

    /// <summary>
    /// Splits a request target into the path and query a proof signs. An absolute-form target
    /// (<c>http://host/path?q</c>) is reduced to its path and query first.
    /// </summary>
    /// <param name="requestTarget">The request target as on the request line.</param>
    /// <returns>The path (at least <c>/</c>) and the query with its leading <c>?</c>, or empty.</returns>
    public static KeyValuePair<string, string> SplitTarget(string requestTarget)
    {
        ArgumentNullException.ThrowIfNull(requestTarget);
        string target = requestTarget;
        int scheme = target.IndexOf("://", StringComparison.Ordinal);
        if (scheme > 0 && target.IndexOf('/', StringComparison.Ordinal) > scheme)
        {
            int pathStart = target.IndexOf('/', scheme + 3);
            target = pathStart < 0 ? "/" : target.Substring(pathStart);
        }

        int question = target.IndexOf('?', StringComparison.Ordinal);
        string path = question < 0 ? target : target.Substring(0, question);
        string query = question < 0 ? string.Empty : target.Substring(question);
        return new KeyValuePair<string, string>(path.Length == 0 ? "/" : path, query);
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
