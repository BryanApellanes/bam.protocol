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

    /// <summary>
    /// The access level <see cref="ConfiguredActorAccessPolicy"/> grants an enrolled actor that
    /// <see cref="IActorAdmission"/> admits. Enrollment alone grants nothing: see <see cref="OpenEnrollment"/>
    /// and <see cref="AdmittedHandles"/>.
    /// </summary>
    public BamAccess EnrolledActorAccess { get; init; } = BamAccess.Execute;

    /// <summary>
    /// When true, every enrolled actor is admitted, so anyone who can reach the anonymous enrollment
    /// endpoints holds <see cref="EnrolledActorAccess"/>. Off by default: enrollment is closed unless the
    /// host opts in here or lists handles in <see cref="AdmittedHandles"/>.
    /// </summary>
    public bool OpenEnrollment { get; init; }

    /// <summary>
    /// The actors <see cref="ConfiguredActorAdmission"/> admits when <see cref="OpenEnrollment"/> is off. Each
    /// entry is <c>handle</c> (any registered key) or <c>handle@fingerprint</c> (only the key whose
    /// <see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/> matches, as returned in
    /// <c>ActorTokenResponse.KeyFingerprint</c>), matched exactly (ordinal). Empty by default, which admits
    /// nobody. An unpinned entry is satisfied by whoever registers that handle first; a rotated or revoked
    /// pinned key needs its entry updated to the successor's fingerprint.
    /// </summary>
    public IReadOnlyList<string> AdmittedHandles { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The <c>X-Bam-Body-Signature-Algorithm</c> values <see cref="BodySignatureProofVerifier"/> accepts,
    /// compared without regard to case. A request naming any other algorithm fails verification before any
    /// cryptography runs, so the caller cannot choose a scheme that doesn't hash the body. Defaults to
    /// <see cref="BodySignatureProofVerifier.DefaultAlgorithm"/> only.
    /// </summary>
    public IReadOnlyList<string> AllowedBodySignatureAlgorithms { get; init; } = new[] { BodySignatureProofVerifier.DefaultAlgorithm };

    /// <summary>
    /// How far a request proof's <c>X-Bam-Timestamp</c> may be from the host's clock, in either direction.
    /// A captured proof stops working once its timestamp leaves this window, and its nonce is remembered
    /// only that long. Defaults to five minutes; at most one day. It is separate from
    /// <see cref="ClockSkewAllowance"/>, which only pads token <c>iat</c>/<c>exp</c> checks: this one is the whole
    /// lifetime of a proof.
    /// </summary>
    public TimeSpan RequestProofMaxAge { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The most unexpired request-proof nonces <see cref="MemoryRequestProofNonceStore"/> holds in all. When it is
    /// full, new proofs are refused (401) rather than an unexpired nonce evicted. Defaults to 200,000.
    /// </summary>
    public int RequestProofNonceCapacity { get; init; } = 200_000;

    /// <summary>
    /// The most unexpired request-proof nonces <see cref="MemoryRequestProofNonceStore"/> holds for one signing key.
    /// A key at this limit has its new proofs refused until its nonces expire; other keys are unaffected, so one
    /// actor cannot lock everyone else out. Defaults to 10,000, about 33 signed requests a second sustained over the
    /// default window; at most <see cref="RequestProofNonceCapacity"/>.
    /// </summary>
    public int RequestProofNoncesPerKey { get; init; } = 10_000;

    /// <summary>
    /// The audience this host requires in client-signed tokens (the <c>aud</c> claim), for example its public
    /// origin. A client-signed token whose <c>aud</c> is missing or different is rejected, so a token made for, or
    /// seen by, one host cannot be used at another. Required when <see cref="Enabled"/> is true:
    /// <see cref="ActorAuthenticationRegistration.VerifyPrerequisites"/> refuses to start without it unless
    /// <see cref="AllowMissingClientTokenAudience"/> is set. A blank value counts as unset.
    /// </summary>
    public string? ClientTokenAudience { get; init; }

    /// <summary>
    /// An explicit opt-out that lets an enabled host start without <see cref="ClientTokenAudience"/>, accepting
    /// client-signed tokens made for any host. Off by default; meant only for a single-host test setup.
    /// </summary>
    public bool AllowMissingClientTokenAudience { get; init; }
}
