using Bam.Protocol.Data;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Decides whether an enrolled actor is admitted, that is, whether it holds any access at all.
/// Enrollment (<c>/actor/register</c>, <c>/actor/confirm</c>) is anonymous and self-service, so it only
/// proves that a caller holds the keys it registered; admission is what grants access.
/// <see cref="ConfiguredActorAdmission"/> is closed by default. Hosts substitute their own (an approval
/// store, a directory lookup) by registering this contract before <c>AddActorAuthentication</c>.
/// </summary>
public interface IActorAdmission
{
    /// <summary>Gets whether the actor is admitted.</summary>
    /// <param name="actor">An authenticated actor (never the anonymous sentinel).</param>
    /// <returns>True when the actor may hold <see cref="ActorAuthenticationOptions.EnrolledActorAccess"/>.</returns>
    bool IsAdmitted(IActor actor);
}
