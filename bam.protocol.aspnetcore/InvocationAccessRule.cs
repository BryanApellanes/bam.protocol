namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The access and proof rules for an invocation target, with the same semantics the endpoint filters apply to a
/// mapped route.
/// </summary>
/// <remarks>
/// <see cref="ActorAccessEndpointFilter"/> and <see cref="RequestProofRule"/> read a route's endpoint metadata. An
/// invocation names a method instead, so the same two decisions are made here from the
/// <see cref="InvocationTarget"/>: access is granted when the held level is at least the required level (the
/// <c>AuthorizationCalculator</c> comparison), and a body proof is required when the target says so explicitly, else
/// when its required access reaches <see cref="ActorAuthenticationOptions.ProofRequiredAtOrAbove"/>.
/// </remarks>
public static class InvocationAccessRule
{
    /// <summary>
    /// Decides whether <paramref name="held"/> satisfies <paramref name="required"/>.
    /// </summary>
    /// <param name="held">The access the actor holds.</param>
    /// <param name="required">The access the target requires.</param>
    /// <returns>True when the actor may invoke the target.</returns>
    public static bool IsAllowed(BamAccess held, BamAccess required)
    {
        return required != BamAccess.Denied && held >= required;
    }

    /// <summary>
    /// Decides whether invoking <paramref name="target"/> requires the per-request body proof.
    /// </summary>
    /// <param name="target">The resolved target.</param>
    /// <param name="options">The actor-auth options carrying the proof threshold.</param>
    /// <returns>True when the request must carry a valid body signature.</returns>
    public static bool RequiresProof(InvocationTarget target, ActorAuthenticationOptions options)
    {
        Args.ThrowIfNull(target, nameof(target));
        Args.ThrowIfNull(options, nameof(options));

        if (target.ExplicitProof.HasValue)
        {
            return target.ExplicitProof.Value;
        }

        return target.RequiredAccess >= options.ProofRequiredAtOrAbove;
    }
}
