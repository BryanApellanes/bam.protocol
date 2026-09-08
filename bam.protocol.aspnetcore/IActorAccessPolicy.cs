namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Maps an authenticated actor to its <see cref="BamAccess"/> level. A narrow counterpart of the
/// Bam-native <c>IAccessLevelProvider</c> (which binds to the whole server context): this seam takes
/// only the actor, so group-based policies can be substituted without touching the filters.
/// </summary>
public interface IActorAccessPolicy
{
    /// <summary>Gets the access level the actor holds.</summary>
    /// <param name="actor">The actor resolved by authentication (possibly the anonymous sentinel).</param>
    /// <returns>The actor's access level.</returns>
    BamAccess GetAccess(IActor actor);
}
