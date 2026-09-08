using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.UserAccounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Self-service enrollment: register a key set (<see cref="IPublicKeySetRegistrar"/>), request a
/// confirmation challenge, and confirm it by signature (<see cref="IAccountConfirmation"/> — the
/// existing device-key confirmation flow). All three are anonymous by necessity. Failure responses are
/// deliberately generic (anti-probing, matching the confirmation flow's own stance); handler logic is
/// exposed for direct testing.
/// </summary>
public static class ActorEnrollmentEndpoints
{
    /// <summary>The default route prefix.</summary>
    public const string DefaultPrefix = "/actor";

    /// <summary>
    /// Maps <c>{prefix}/register</c>, <c>{prefix}/challenge</c>, and <c>{prefix}/confirm</c>.
    /// </summary>
    /// <param name="routes">The host's route builder.</param>
    /// <param name="prefix">The route prefix; defaults to <see cref="DefaultPrefix"/>.</param>
    /// <returns>The route group, for further configuration.</returns>
    public static RouteGroupBuilder MapActorEnrollment(this IEndpointRouteBuilder routes, string prefix = DefaultPrefix)
    {
        ArgumentNullException.ThrowIfNull(routes);
        RouteGroupBuilder group = routes.MapGroup(prefix).WithMetadata(new AnonymousAccessAttribute());
        group.MapPost("/register", (ActorRegistrationRequest request, IPublicKeySetRegistrar registrar) => Register(request, registrar));
        group.MapPost("/challenge", (ActorChallengeRequest request, IAccountConfirmation confirmation) => Challenge(request, confirmation));
        group.MapPost("/confirm", (ActorConfirmRequest request, IAccountConfirmation confirmation) => Confirm(request, confirmation));
        return group;
    }

    /// <summary>Registers a key set for a handle.</summary>
    /// <param name="request">The handle and PEM material.</param>
    /// <param name="registrar">Admits key sets under the store's uniqueness and blocklist policy.</param>
    /// <returns>200 with the registered handle, or 400 when admission is refused.</returns>
    public static IResult Register(ActorRegistrationRequest request, IPublicKeySetRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(registrar);
        try
        {
            PublicKeySetData registered = registrar.Register(new PublicKeySetData
            {
                KeySetHandle = request.KeySetHandle,
                PublicRsaKey = request.PublicRsaKeyPem,
                PublicEccKey = request.PublicEccKeyPem,
            });
            return Results.Ok(new ActorRegistrationResponse { KeySetHandle = registered.KeySetHandle });
        }
        catch (Exception exception) when (exception is InvalidOperationException || exception is ArgumentException)
        {
            return Results.Json(new ActorAuthFailure { Messages = ["Registration was not accepted."] }, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>Creates a confirmation challenge for a handle.</summary>
    /// <param name="request">The handle.</param>
    /// <param name="confirmation">The confirmation flow.</param>
    /// <returns>200 with the challenge bytes (base64) and expiry.</returns>
    public static IResult Challenge(ActorChallengeRequest request, IAccountConfirmation confirmation)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(confirmation);
        ConfirmationChallenge challenge = confirmation.CreateChallenge(request.PersonHandle);
        return Results.Ok(new ActorChallengeResponse
        {
            ChallengeBase64 = Convert.ToBase64String(challenge.ChallengeBytes),
            ExpiresUtc = challenge.ExpiresUtc,
        });
    }

    /// <summary>Confirms a handle by verifying the signed challenge.</summary>
    /// <param name="request">The handle and the signature over the challenge.</param>
    /// <param name="confirmation">The confirmation flow.</param>
    /// <returns>200 when confirmed, otherwise 401 with a generic reason.</returns>
    public static IResult Confirm(ActorConfirmRequest request, IAccountConfirmation confirmation)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(confirmation);
        byte[] signature;
        try
        {
            signature = Convert.FromBase64String(request.SignatureBase64);
        }
        catch (FormatException)
        {
            return Results.Json(new ActorAuthFailure { Messages = ["Confirmation was not accepted."] }, statusCode: StatusCodes.Status401Unauthorized);
        }

        ConfirmationResult result = confirmation.VerifySignedChallenge(request.PersonHandle, signature);
        if (!result.Confirmed)
        {
            return Results.Json(new ActorAuthFailure { Messages = ["Confirmation was not accepted."] }, statusCode: StatusCodes.Status401Unauthorized);
        }

        return Results.Ok(new ActorConfirmResponse { Confirmed = true });
    }
}

/// <summary>Registers a key set for a handle.</summary>
public sealed record ActorRegistrationRequest
{
    /// <summary>Gets the handle the key set is registered under.</summary>
    public required string KeySetHandle { get; init; }

    /// <summary>Gets the RSA public key PEM (used by the confirmation flow).</summary>
    public required string PublicRsaKeyPem { get; init; }

    /// <summary>Gets the ECC public key PEM (used for tokens and body signatures).</summary>
    public required string PublicEccKeyPem { get; init; }
}

/// <summary>The registration response.</summary>
public sealed record ActorRegistrationResponse
{
    /// <summary>Gets the registered handle.</summary>
    public required string KeySetHandle { get; init; }
}

/// <summary>Requests a confirmation challenge.</summary>
public sealed record ActorChallengeRequest
{
    /// <summary>Gets the handle to challenge.</summary>
    public required string PersonHandle { get; init; }
}

/// <summary>The challenge to sign.</summary>
public sealed record ActorChallengeResponse
{
    /// <summary>Gets the challenge bytes, base64 encoded; sign the base64 TEXT per the confirmation convention.</summary>
    public required string ChallengeBase64 { get; init; }

    /// <summary>Gets when the challenge expires.</summary>
    public required DateTime ExpiresUtc { get; init; }
}

/// <summary>Confirms a challenge.</summary>
public sealed record ActorConfirmRequest
{
    /// <summary>Gets the handle being confirmed.</summary>
    public required string PersonHandle { get; init; }

    /// <summary>Gets the signature over the challenge, base64 encoded.</summary>
    public required string SignatureBase64 { get; init; }
}

/// <summary>The confirmation response.</summary>
public sealed record ActorConfirmResponse
{
    /// <summary>Gets whether the handle was confirmed.</summary>
    public required bool Confirmed { get; init; }
}
