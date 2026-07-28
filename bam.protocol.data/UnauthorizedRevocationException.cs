namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a revocation is rejected because the presented break-glass admin proof does not
/// authorize it — an invalid or missing signature, a signature by a non-admin key, or a proof
/// bound to a different target than the one being revoked.  The key set is left unchanged.
/// </summary>
public class UnauthorizedRevocationException : KeySetRevocationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedRevocationException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose revocation was rejected.</param>
    public UnauthorizedRevocationException(string keySetHandle)
        : base(keySetHandle, $"The break-glass admin proof does not authorize revoking the key set for handle '{keySetHandle}'.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedRevocationException"/> class,
    /// preserving the underlying cause (e.g. malformed admin key material).
    /// </summary>
    /// <param name="keySetHandle">The handle whose revocation was rejected.</param>
    /// <param name="innerException">The underlying exception that caused the rejection.</param>
    public UnauthorizedRevocationException(string keySetHandle, Exception? innerException)
        : base(keySetHandle, $"The break-glass admin proof does not authorize revoking the key set for handle '{keySetHandle}'.", innerException)
    {
    }
}
