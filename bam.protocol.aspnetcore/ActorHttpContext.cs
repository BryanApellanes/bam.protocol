using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Where <see cref="ActorAuthenticationMiddleware"/> leaves the authenticated actor for downstream
/// filters and handlers, and how they read it back. Stored in <see cref="HttpContext.Items"/> under
/// stable keys so the adapter does not depend on ASP.NET's claims model.
/// </summary>
public static class ActorHttpContext
{
    /// <summary>The <see cref="HttpContext.Items"/> key holding the resolved <see cref="IActor"/>.</summary>
    public const string ActorKey = "Bam.Protocol.AspNetCore.Actor";

    /// <summary>The <see cref="HttpContext.Items"/> key holding the actor's registered ECC public key PEM.</summary>
    public const string EccPublicKeyPemKey = "Bam.Protocol.AspNetCore.EccPublicKeyPem";

    /// <summary>Records the authenticated actor (and, when enrolled, its ECC public key PEM) on the context.</summary>
    /// <param name="context">The request context.</param>
    /// <param name="actor">The actor to record.</param>
    /// <param name="eccPublicKeyPem">The registered ECC public key PEM, or null for the anonymous sentinel.</param>
    public static void SetActor(this HttpContext context, IActor actor, string? eccPublicKeyPem)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(actor);
        context.Items[ActorKey] = actor;
        context.Items[EccPublicKeyPemKey] = eccPublicKeyPem;
    }

    /// <summary>Gets the actor recorded by authentication, or null when the request never passed the middleware.</summary>
    /// <param name="context">The request context.</param>
    /// <returns>The actor, or null.</returns>
    public static IActor? GetActor(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Items.TryGetValue(ActorKey, out object? value) ? value as IActor : null;
    }

    /// <summary>Gets the ECC public key PEM recorded by authentication, or null for anonymous or unauthenticated requests.</summary>
    /// <param name="context">The request context.</param>
    /// <returns>The PEM, or null.</returns>
    public static string? GetActorEccPublicKeyPem(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Items.TryGetValue(EccPublicKeyPemKey, out object? value) ? value as string : null;
    }
}
