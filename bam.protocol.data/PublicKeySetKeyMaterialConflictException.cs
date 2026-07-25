namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a registration presents public key material (RSA or ECC) that is already
/// registered under a <i>different</i> handle.  A public key set must map to exactly one
/// handle: without this guard an attacker could register a victim's public key under a handle
/// they control, causing <c>FindProfileByPublicKey</c> to misattribute the victim's
/// authenticated session (bam.protocol#8 review, condition C4).
/// </summary>
public class PublicKeySetKeyMaterialConflictException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetKeyMaterialConflictException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose registration was rejected.</param>
    /// <param name="existingHandle">The handle the presented key material is already registered under.</param>
    public PublicKeySetKeyMaterialConflictException(string keySetHandle, string existingHandle)
        : base(keySetHandle, $"The public key material presented for handle '{keySetHandle}' is already registered under handle '{existingHandle}'. A public key set maps to exactly one handle.")
    {
        this.ExistingHandle = existingHandle;
    }

    /// <summary>
    /// Gets the handle the presented key material is already registered under.
    /// </summary>
    public string ExistingHandle { get; }
}
