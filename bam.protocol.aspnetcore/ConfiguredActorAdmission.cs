using Bam.Protocol.Data;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The default <see cref="IActorAdmission"/>: admits every actor when
/// <see cref="ActorAuthenticationOptions.OpenEnrollment"/> is on, otherwise only the handles in
/// <see cref="ActorAuthenticationOptions.AdmittedHandles"/> (ordinal match). With neither set it admits
/// nobody, so a freshly enrolled actor holds no access until the host opts in.
/// </summary>
public sealed class ConfiguredActorAdmission : IActorAdmission
{
    private readonly ActorAuthenticationOptions _options;
    private readonly HashSet<string> _admitted;

    /// <summary>Creates the admission check.</summary>
    /// <param name="options">The options carrying the enrollment switch and the admitted handles.</param>
    public ConfiguredActorAdmission(ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
        _admitted = new HashSet<string>(options.AdmittedHandles ?? Array.Empty<string>(), StringComparer.Ordinal);
    }

    /// <inheritdoc />
    public bool IsAdmitted(IActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        return _options.OpenEnrollment || _admitted.Contains(actor.Handle);
    }
}
