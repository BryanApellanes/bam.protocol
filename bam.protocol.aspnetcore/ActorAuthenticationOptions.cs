namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Tunables for actor authentication. Hosts bind this from the <c>ActorAuth</c> configuration
/// section (see <see cref="ActorAuthenticationRegistration"/>); tests construct it directly. Every
/// value has a safe default so a host overrides only what it needs.
/// </summary>
public sealed record ActorAuthenticationOptions
{
    /// <summary>The default configuration section name hosts bind this record from.</summary>
    public const string DefaultSectionName = "ActorAuth";

    /// <summary>
    /// Master switch. When false the host inserts no middleware and behaves exactly as it did
    /// before actor authentication existed. Off by default so a host without enrollment
    /// infrastructure keeps running.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>Which token model the host verifies. Hybrid requires <see cref="ServerKeyName"/> to resolve.</summary>
    public ActorAuthenticationMode Mode { get; init; } = ActorAuthenticationMode.Hybrid;

    /// <summary>
    /// The name under which the server's ECC signing key pair (PEM bytes) is kept in
    /// <see cref="Bam.Encryption.INamedKeyStorage"/>. Used both to issue tokens and to verify their signatures.
    /// </summary>
    public string ServerKeyName { get; init; } = "actor-auth-server-key";

    /// <summary>The issuer written into server-issued tokens and required on verification.</summary>
    public string Issuer { get; init; } = "bam";

    /// <summary>Lifetime of server-issued tokens. Design guidance: 30 to 60 minutes.</summary>
    public TimeSpan ServerTokenLifetime { get; init; } = TimeSpan.FromMinutes(45);

    /// <summary>
    /// Longest accepted lifetime of a client-signed token (the issuance proof, and every token in
    /// <see cref="ActorAuthenticationMode.ClientSignedOnly"/>). A token whose exp minus iat exceeds
    /// this is rejected.
    /// </summary>
    public TimeSpan ClientTokenMaxLifetime { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>Tolerance applied to iat and exp comparisons so modest clock drift does not cause spurious 401s.</summary>
    public TimeSpan ClockSkewAllowance { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Endpoints whose <see cref="RequiredAccessAttribute"/> is at or above this level require a
    /// body signature (the per-request proof). <see cref="RequireRequestProofAttribute"/> overrides per endpoint.
    /// </summary>
    public BamAccess ProofRequiredAtOrAbove { get; init; } = BamAccess.Execute;

    /// <summary>The access level <see cref="ConfiguredActorAccessPolicy"/> grants every enrolled (non-anonymous) actor.</summary>
    public BamAccess EnrolledActorAccess { get; init; } = BamAccess.Execute;
}
