using Bam.Protocol.Server;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Phase-1 access policy: every enrolled actor holds <see cref="ActorAuthenticationOptions.EnrolledActorAccess"/>,
/// the anonymous sentinel (<see cref="AnonymousActorProvider.AnonymousHandle"/>) holds
/// <see cref="BamAccess.Denied"/>. Group-based policies implement <see cref="IActorAccessPolicy"/> later
/// without touching the filters.
/// </summary>
public sealed class ConfiguredActorAccessPolicy : IActorAccessPolicy
{
    private readonly ActorAuthenticationOptions _options;

    /// <summary>Creates the policy.</summary>
    /// <param name="options">The options carrying the enrolled-actor access level.</param>
    public ConfiguredActorAccessPolicy(ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    /// <inheritdoc />
    public BamAccess GetAccess(IActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        if (string.Equals(actor.Handle, AnonymousActorProvider.AnonymousHandle, StringComparison.Ordinal))
        {
            return BamAccess.Denied;
        }

        return _options.EnrolledActorAccess;
    }
}
