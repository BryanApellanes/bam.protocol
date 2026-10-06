using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The token-issuance surface for hybrid mode: POST the caller's short-lived client-signed token and
/// receive a server-issued token bound to the caller's registered key. Anonymous by necessity (the
/// client-signed token IS the proof); the handler logic is exposed for direct testing.
/// </summary>
public static class ActorTokenEndpoints
{
    /// <summary>The default route for token issuance.</summary>
    public const string DefaultPattern = "/actor/token";

    /// <summary>
    /// Maps the issuance endpoint.
    /// </summary>
    /// <param name="routes">The host's route builder.</param>
    /// <param name="pattern">The route pattern; defaults to <see cref="DefaultPattern"/>.</param>
    /// <returns>The route handler builder, for further configuration.</returns>
    /// <exception cref="InvalidOperationException">The registered options select <see cref="ActorAuthenticationMode.ClientSignedOnly"/>, which has no server key to issue with.</exception>
    public static RouteHandlerBuilder MapActorToken(this IEndpointRouteBuilder routes, string pattern = DefaultPattern)
    {
        ArgumentNullException.ThrowIfNull(routes);
        ActorAuthenticationOptions? options = routes.ServiceProvider.GetService<ActorAuthenticationOptions>();
        if (options is not null && options.Mode == ActorAuthenticationMode.ClientSignedOnly)
        {
            throw new InvalidOperationException("MapActorToken issues server tokens, which ActorAuthenticationMode.ClientSignedOnly does not use. Select Hybrid mode or don't map the token endpoint.");
        }

        return routes
            .MapPost(pattern, (ActorTokenRequest request, SignedActorTokenVerifier proof, ServerActorTokenIssuer issuer, ILoggerFactory loggers) =>
                Issue(request, proof, issuer, loggers.CreateLogger(typeof(ActorTokenEndpoints))))
            .WithMetadata(new AnonymousAccessAttribute());
    }

    /// <summary>
    /// Issues a server token when the presented client-signed token proves an enrolled actor.
    /// </summary>
    /// <param name="request">The client-signed token.</param>
    /// <param name="proof">Verifies the client-signed token against the registered key set.</param>
    /// <param name="issuer">Mints the server token.</param>
    /// <param name="logger">Receives the verifier's reason on failure; optional.</param>
    /// <returns>200 with the token, or 401 with the fixed <see cref="ActorAuthenticationMiddleware.FailureMessage"/>.</returns>
    public static IResult Issue(ActorTokenRequest request, SignedActorTokenVerifier proof, ServerActorTokenIssuer issuer, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(proof);
        ArgumentNullException.ThrowIfNull(issuer);

        ActorTokenVerification verification = proof.Verify(request.ClientToken);
        if (!verification.Success || verification.Actor is null || verification.EccPublicKeyPem is null)
        {
            // The reason (unknown handle, bad signature, expiry) is logged, never returned: telling them
            // apart would let a caller enumerate registered handles.
            logger?.LogInformation("Actor token issuance refused: {Reasons}", string.Join(" ", verification.Messages));
            return Results.Json(new ActorAuthFailure { Messages = [ActorAuthenticationMiddleware.FailureMessage] }, statusCode: StatusCodes.Status401Unauthorized);
        }

        IssuedActorToken issued = issuer.Issue(verification.Actor, verification.EccPublicKeyPem);
        return Results.Ok(new ActorTokenResponse
        {
            Token = issued.Token,
            ExpiresUtc = issued.ExpiresUtc,
            KeyFingerprint = issued.KeyFingerprint,
        });
    }
}

/// <summary>The issuance request: a token the caller signed with its own ECC key.</summary>
public sealed record ActorTokenRequest
{
    /// <summary>Gets the client-signed compact JWT (sub = key-set handle, short expiry).</summary>
    public required string ClientToken { get; init; }
}

/// <summary>The issuance response.</summary>
public sealed record ActorTokenResponse
{
    /// <summary>Gets the server-issued compact JWT to send as the Bearer token.</summary>
    public required string Token { get; init; }

    /// <summary>Gets when the token expires.</summary>
    public required DateTimeOffset ExpiresUtc { get; init; }

    /// <summary>Gets the canonical fingerprint of the key the token is bound to.</summary>
    public required string KeyFingerprint { get; init; }
}

/// <summary>A failed authentication or enrollment step, with reasons.</summary>
public sealed record ActorAuthFailure
{
    /// <summary>Gets the failure reasons.</summary>
    public required IReadOnlyList<string> Messages { get; init; }
}
