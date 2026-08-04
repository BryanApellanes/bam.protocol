namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a handle whose governing revocation tombstone bound an <i>authorized successor</i>
/// is re-registered with a key other than that successor.  Break-glass revocation frees a handle
/// for re-registration, but when the admin proof also named a successor key
/// (<see cref="PublicKeySetData.AuthorizedSuccessorFingerprint"/>), only that key may re-register
/// the freed handle — this closes the revoke→re-register hijack window where any first caller
/// could otherwise seize the handle (bam.protocol#21, tightening bam.protocol#11's acceptance
/// criterion).  Distinct from <see cref="PublicKeySetConflictException"/> (handle still actively
/// registered) and <see cref="RevokedKeyMaterialException"/> (the presented material is itself
/// blocklisted): here the handle is free, but bound to a specific successor the candidate is not.
/// </summary>
public class UnauthorizedSuccessorException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedSuccessorException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle bound to a successor other than the presented key.</param>
    public UnauthorizedSuccessorException(string keySetHandle)
        : base(keySetHandle, $"The public key set presented for handle '{keySetHandle}' is not the successor authorized by the revocation that freed it. Only the admin-authorized successor key may re-register this handle.")
    {
    }
}
