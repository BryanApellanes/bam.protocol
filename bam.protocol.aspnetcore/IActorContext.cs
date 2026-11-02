namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Gives a dispatched service the verified caller of the invocation currently running.
/// </summary>
/// <remarks>
/// Services invoked over <c>/invoke</c> are resolved from the Bam <c>ServiceRegistry</c>, not from the ASP.NET
/// container, so they cannot ask for an <c>HttpContext</c>. The bridge registers an implementation in the dispatch
/// registry and scopes each call, so a service that takes <see cref="IActorContext"/> in its constructor reads the
/// actor the middleware verified, never anything the payload claimed. Members throw
/// <see cref="InvalidOperationException"/> when no invocation is in progress on the current async flow.
/// </remarks>
public interface IActorContext
{
    /// <summary>
    /// Gets the verified actor making the current invocation.
    /// </summary>
    IActor Actor { get; }

    /// <summary>
    /// Gets the canonical fingerprint of the actor's registered ECC key, or null when it has none.
    /// </summary>
    string? KeyFingerprint { get; }

    /// <summary>
    /// Gets the remote IP address the request arrived from, as the host saw it, or null when unknown.
    /// </summary>
    string? ClientIp { get; }

    /// <summary>
    /// Gets a value indicating whether an invocation is in progress on the current async flow.
    /// </summary>
    bool IsActive { get; }
}
