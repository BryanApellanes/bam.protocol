namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Decides whether a bearer-token string proves an enrolled actor. Implementations differ by token
/// model (<see cref="ServerIssuedTokenVerifier"/> for hybrid mode, <see cref="SignedActorTokenVerifier"/>
/// for client-signed tokens); <see cref="ActorAuthenticationMiddleware"/> depends only on this seam.
/// </summary>
public interface IActorTokenVerifier
{
    /// <summary>
    /// Verifies a bearer token (the value after the <c>Bearer</c> scheme). Never throws for a
    /// malformed token; failures are reported in the result.
    /// </summary>
    /// <param name="bearerToken">The compact JWT string.</param>
    /// <returns>The verification outcome.</returns>
    ActorTokenVerification Verify(string bearerToken);
}
