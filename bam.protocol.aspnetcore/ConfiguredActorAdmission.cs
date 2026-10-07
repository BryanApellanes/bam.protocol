using Bam.Protocol;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The default <see cref="IActorAdmission"/>: admits every actor when
/// <see cref="ActorAuthenticationOptions.OpenEnrollment"/> is on, otherwise only the entries in
/// <see cref="ActorAuthenticationOptions.AdmittedHandles"/>. An entry is either <c>handle</c>, which admits that
/// handle with any registered key, or <c>handle@fingerprint</c>, which admits it only with the key whose
/// <see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/> matches. Both parts match exactly (ordinal). With
/// neither switch set it admits nobody, so a freshly enrolled actor holds no access until the host opts in.
/// An unpinned entry is satisfied by whoever registers that handle first; pin the key to rule that out. A pinned
/// key that is rotated or revoked needs its entry updated to the successor key's fingerprint.
/// </summary>
public sealed class ConfiguredActorAdmission : IActorAdmission
{
    /// <summary>Separates a handle from a pinned key fingerprint in an <see cref="ActorAuthenticationOptions.AdmittedHandles"/> entry.</summary>
    public const char PinSeparator = '@';

    private readonly ActorAuthenticationOptions _options;
    private readonly HashSet<string> _anyKey;
    private readonly Dictionary<string, HashSet<string>> _pinned;

    /// <summary>Creates the admission check.</summary>
    /// <param name="options">The options carrying the enrollment switch and the admitted entries.</param>
    public ConfiguredActorAdmission(ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
        _anyKey = new HashSet<string>(StringComparer.Ordinal);
        _pinned = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach (string entry in options.AdmittedHandles ?? Array.Empty<string>())
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                continue;
            }

            // Handles may themselves contain '@' (e.g. svc.bot@acme), so the text after the last '@' is a pin only
            // when it has the shape of a key fingerprint: 64 hex characters (SHA-256 of the canonical public key).
            int separator = entry.LastIndexOf(PinSeparator);
            if (separator <= 0 || !IsFingerprint(entry.Substring(separator + 1)))
            {
                _anyKey.Add(entry);
                continue;
            }

            string handle = entry.Substring(0, separator);
            string fingerprint = entry.Substring(separator + 1).ToLowerInvariant();
            if (!_pinned.TryGetValue(handle, out HashSet<string>? fingerprints))
            {
                fingerprints = new HashSet<string>(StringComparer.Ordinal);
                _pinned[handle] = fingerprints;
            }

            fingerprints.Add(fingerprint);
        }
    }

    /// <inheritdoc />
    public bool IsAdmitted(IActor actor, string? keyFingerprint)
    {
        ArgumentNullException.ThrowIfNull(actor);
        if (_options.OpenEnrollment || _anyKey.Contains(actor.Handle))
        {
            return true;
        }

        return keyFingerprint is not null
            && _pinned.TryGetValue(actor.Handle, out HashSet<string>? fingerprints)
            && fingerprints.Contains(keyFingerprint.ToLowerInvariant());
    }

    /// <summary>Whether text has the shape of a key fingerprint: 64 hexadecimal characters.</summary>
    /// <param name="text">The candidate pin.</param>
    /// <returns>True for a SHA-256 hex fingerprint.</returns>
    public static bool IsFingerprint(string text)
    {
        if (text is null || text.Length != 64)
        {
            return false;
        }

        foreach (char character in text)
        {
            if (!Uri.IsHexDigit(character))
            {
                return false;
            }
        }

        return true;
    }
}
