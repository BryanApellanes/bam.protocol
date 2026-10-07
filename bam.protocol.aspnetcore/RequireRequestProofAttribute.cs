namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Endpoint metadata that forces (<c>required</c> true) or waives (<c>required</c> false) the
/// body-signature requirement independent of the access ladder derived from
/// <see cref="RequiredAccessAttribute"/> and <see cref="ActorAuthenticationOptions.ProofRequiredAtOrAbove"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Delegate, AllowMultiple = false)]
public sealed class RequireRequestProofAttribute : Attribute
{
    /// <summary>Creates the override.</summary>
    /// <param name="required">Whether a body signature is required for the endpoint.</param>
    public RequireRequestProofAttribute(bool required = true)
    {
        Required = required;
    }

    /// <summary>Gets whether a body signature is required.</summary>
    public bool Required { get; }
}
