namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a registration presents public key material that belongs to a <i>revoked</i> key
/// set.  Revocation frees the handle for re-registration but keeps the revoked key material
/// blocklisted, because revocation usually means the key is compromised — so it must not be
/// re-registered under any handle (bam.protocol#11).  Distinct from
/// <see cref="PublicKeySetKeyMaterialConflictException"/>, which signals a clash with an
/// <i>active</i> registration.
/// </summary>
public class RevokedKeyMaterialException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RevokedKeyMaterialException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose registration was rejected.</param>
    public RevokedKeyMaterialException(string keySetHandle)
        : base(keySetHandle, $"The public key material presented for handle '{keySetHandle}' belongs to a revoked key set and is blocklisted from re-registration.")
    {
    }
}
