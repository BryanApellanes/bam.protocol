namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The caller facts the bridge records for one invocation.
/// </summary>
/// <param name="Actor">The actor the middleware verified.</param>
/// <param name="KeyFingerprint">The canonical fingerprint of the actor's registered ECC key, or null.</param>
/// <param name="ClientIp">The remote IP address of the request, or null.</param>
public sealed record ActorInvocationContext(IActor Actor, string? KeyFingerprint, string? ClientIp);
