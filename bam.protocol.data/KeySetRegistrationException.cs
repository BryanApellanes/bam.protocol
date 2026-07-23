namespace Bam.Protocol.Data;

/// <summary>
/// Base exception for violations of the public key-set registration policy — the trust anchor
/// for device-key account confirmation.  Registration is first-registration-wins and a
/// registered key set may only be replaced through a rotation that proves possession of the
/// currently registered key; see <see cref="IPublicKeySetRegistrar"/>.
/// </summary>
public class KeySetRegistrationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetRegistrationException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose registration operation was rejected.</param>
    /// <param name="message">A description of the policy violation.</param>
    public KeySetRegistrationException(string keySetHandle, string message) : base(message)
    {
        this.KeySetHandle = keySetHandle;
    }

    /// <summary>
    /// Gets the handle whose registration operation was rejected.
    /// </summary>
    public string KeySetHandle { get; }
}
