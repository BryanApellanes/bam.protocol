namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a key-set rotation is rejected: either no key set is registered for the handle
/// (rotation is not registration — use <see cref="IPublicKeySetRegistrar.Register"/>), the
/// presented rotation signature does not prove possession of the currently registered key, or
/// the proposed key material is not a parseable key.  The registered key set is left unchanged
/// in every rejection case.
/// </summary>
public class InvalidKeySetRotationException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidKeySetRotationException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose rotation was rejected.</param>
    /// <param name="message">A description of why the rotation was rejected.</param>
    public InvalidKeySetRotationException(string keySetHandle, string message)
        : base(keySetHandle, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidKeySetRotationException"/> class,
    /// preserving the underlying cause (e.g. a key-parsing failure).
    /// </summary>
    /// <param name="keySetHandle">The handle whose rotation was rejected.</param>
    /// <param name="message">A description of why the rotation was rejected.</param>
    /// <param name="innerException">The underlying exception that caused this rejection.</param>
    public InvalidKeySetRotationException(string keySetHandle, string message, Exception? innerException)
        : base(keySetHandle, message, innerException)
    {
    }
}
