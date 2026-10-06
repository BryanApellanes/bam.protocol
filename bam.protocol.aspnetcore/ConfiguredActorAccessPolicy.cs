using Bam.Protocol.Server;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Phase-1 access policy: an enrolled actor that <see cref="IActorAdmission"/> admits holds
/// <see cref="ActorAuthenticationOptions.EnrolledActorAccess"/>; an actor that is not admitted, and the
/// anonymous sentinel (<see cref="AnonymousActorProvider.AnonymousHandle"/>), hold <see cref="BamAccess.Denied"/>.
/// Admission is closed by default (<see cref="ConfiguredActorAdmission"/>). Group-based policies implement
/// <see cref="IActorAccessPolicy"/> later without touching the filters.
/// </summary>
public sealed class ConfiguredActorAccessPolicy : IActorAccessPolicy
{
    private readonly ActorAuthenticationOptions _options;
    private readonly IActorAdmission _admission;

    /// <summary>Creates the policy.</summary>
    /// <param name="options">The options carrying the enrolled-actor access level.</param>
    /// <param name="admission">Decides which enrolled actors hold that level.</param>
    public ConfiguredActorAccessPolicy(ActorAuthenticationOptions options, IActorAdmission admission)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(admission);
        _options = options;
        _admission = admission;
    }

    /// <inheritdoc />
    public BamAccess GetAccess(IActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        if (string.Equals(actor.Handle, AnonymousActorProvider.AnonymousHandle, StringComparison.Ordinal))
        {
            return BamAccess.Denied;
        }

        return _admission.IsAdmitted(actor) ? _options.EnrolledActorAccess : BamAccess.Denied;
    }
}
