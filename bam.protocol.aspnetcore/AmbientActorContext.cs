namespace Bam.Protocol.AspNetCore;

/// <summary>
/// An <see cref="IActorContext"/> carried on the current async flow for the duration of one invocation.
/// </summary>
/// <remarks>
/// Backed by an <see cref="AsyncLocal{T}"/>, so the bridge calls <see cref="Enter"/> around the method call and
/// every service resolved during that call, on that flow, sees the same caller; concurrent invocations never see
/// each other's. A single instance is registered in the dispatch registry as <see cref="IActorContext"/>; it holds
/// no state of its own, so one instance serves every request.
/// </remarks>
public sealed class AmbientActorContext : IActorContext
{
    private static readonly AsyncLocal<ActorInvocationContext?> Current = new AsyncLocal<ActorInvocationContext?>();

    /// <inheritdoc />
    public IActor Actor => Require().Actor;

    /// <inheritdoc />
    public string? KeyFingerprint => Require().KeyFingerprint;

    /// <inheritdoc />
    public string? ClientIp => Require().ClientIp;

    /// <inheritdoc />
    public bool IsActive => Current.Value is not null;

    /// <summary>
    /// Makes <paramref name="context"/> the current invocation's caller until the returned scope is disposed, which
    /// restores whatever was current before.
    /// </summary>
    /// <param name="context">The caller facts for the invocation about to run.</param>
    /// <returns>A scope to dispose when the invocation completes.</returns>
    public static IDisposable Enter(ActorInvocationContext context)
    {
        Args.ThrowIfNull(context, nameof(context));
        ActorInvocationContext? previous = Current.Value;
        Current.Value = context;
        return new Scope(previous);
    }

    private static ActorInvocationContext Require()
    {
        return Current.Value ?? throw new InvalidOperationException("No actor invocation is in progress on the current async flow.");
    }

    private sealed class Scope : IDisposable
    {
        private readonly ActorInvocationContext? _previous;
        private bool _disposed;

        public Scope(ActorInvocationContext? previous)
        {
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Current.Value = _previous;
        }
    }
}
