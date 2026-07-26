namespace Bam.Protocol.Data;

/// <summary>
/// Base exception for failures of the key-set revocation policy — the break-glass recovery path
/// for the registration trust anchor (bam.protocol#11).  Thrown when a revocation cannot be
/// performed (e.g. no active key set is registered for the handle); see <see cref="IKeySetRevocation"/>.
/// </summary>
public class KeySetRevocationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetRevocationException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose revocation was rejected.</param>
    /// <param name="message">A description of why the revocation was rejected.</param>
    public KeySetRevocationException(string keySetHandle, string message) : base(message)
    {
        this.KeySetHandle = keySetHandle;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetRevocationException"/> class,
    /// preserving the underlying cause.
    /// </summary>
    /// <param name="keySetHandle">The handle whose revocation was rejected.</param>
    /// <param name="message">A description of why the revocation was rejected.</param>
    /// <param name="innerException">The underlying exception that caused this rejection.</param>
    public KeySetRevocationException(string keySetHandle, string message, Exception? innerException)
        : base(message, innerException)
    {
        this.KeySetHandle = keySetHandle;
    }

    /// <summary>
    /// Gets the handle whose revocation was rejected.
    /// </summary>
    public string KeySetHandle { get; }
}
