namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a key set presented for registration carries key material that is not a
/// parseable public key.  Registering unparseable material would permanently brick a handle:
/// first-registration-wins blocks re-registration, rotation requires a valid signature by the
/// (unusable) registered key, and no revocation path exists yet (bam.protocol#8 review,
/// condition C5; revocation tracked as bam.protocol#11).
/// </summary>
public class InvalidPublicKeySetException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPublicKeySetException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose registration was rejected.</param>
    /// <param name="message">A description of which key material failed to parse.</param>
    /// <param name="innerException">The underlying key-parsing exception.</param>
    public InvalidPublicKeySetException(string keySetHandle, string message, Exception? innerException)
        : base(keySetHandle, message, innerException)
    {
    }
}
