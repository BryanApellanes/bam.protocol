using Bam.Protocol.Server;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The one definition of token time validity shared by both verifiers: a token is rejected when it
/// has expired (beyond the skew allowance) or claims to have been issued in the future (beyond the
/// skew allowance).
/// </summary>
internal static class TokenTiming
{
    /// <summary>
    /// Checks the token's iat and exp against the current time with the given tolerance.
    /// </summary>
    /// <param name="token">The decoded token.</param>
    /// <param name="skewAllowance">Tolerance for clock drift.</param>
    /// <returns>A failure message, or null when the timing is valid.</returns>
    internal static string? Check(BamJwtToken token, TimeSpan skewAllowance)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (token.Expiry + skewAllowance < now)
        {
            return "Token has expired.";
        }

        if (token.IssuedAt - skewAllowance > now)
        {
            return "Token is issued in the future.";
        }

        return null;
    }
}
