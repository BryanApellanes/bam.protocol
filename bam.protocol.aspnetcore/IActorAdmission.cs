using Bam.Protocol;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Decides whether an enrolled actor is admitted, that is, whether it holds any access at all.
/// Enrollment (<c>/actor/register</c>, <c>/actor/confirm</c>) is anonymous and self-service, so it only
/// proves that a caller holds the keys it registered; admission is what grants access. It is checked at
/// every point an actor gains standing: <see cref="ActorAuthenticationMiddleware"/> refuses an unadmitted actor
/// on every non-anonymous endpoint, <see cref="ActorTokenEndpoints"/> issues no server token to one, and
/// <see cref="ConfiguredActorAccessPolicy"/> grants it <see cref="BamAccess.Denied"/>.
/// <see cref="ConfiguredActorAdmission"/> is closed by default. Hosts substitute their own (an approval
/// store, a directory lookup) by registering this contract before <c>AddActorAuthentication</c>.
/// </summary>
public interface IActorAdmission
{
    /// <summary>Gets whether the actor, presenting the given key, is admitted.</summary>
    /// <param name="actor">An authenticated actor (never the anonymous sentinel).</param>
    /// <param name="keyFingerprint">
    /// The <see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/> of the ECC public key the actor authenticated
    /// with, or null when it is unknown. An admission bound to a key must refuse a null fingerprint.
    /// </param>
    /// <returns>True when the actor may hold <see cref="ActorAuthenticationOptions.EnrolledActorAccess"/>.</returns>
    bool IsAdmitted(IActor actor, string? keyFingerprint);
}
