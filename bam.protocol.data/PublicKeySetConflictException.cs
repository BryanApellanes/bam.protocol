namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a public key set is registered for a handle that already has one.  Registration
/// is first-registration-wins: the first key set registered under a handle is authoritative and
/// can only be replaced through <see cref="IPublicKeySetRegistrar.Rotate"/> with proof of
/// possession of the currently registered key.  This rejection is what prevents an attacker
/// from becoming a handle by registering their own key under it (see bam.protocol#8).
/// </summary>
public class PublicKeySetConflictException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetConflictException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle that already has a registered key set.</param>
    public PublicKeySetConflictException(string keySetHandle)
        : base(keySetHandle, $"A public key set is already registered for handle '{keySetHandle}'. Use rotation with proof of possession of the currently registered key to replace it.")
    {
    }
}
