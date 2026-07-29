using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

public interface IProfileRepository
{
    ProfileData SaveProfile(ProfileData profileData);
    ProfileData FindProfileByHandle(string handle);
    ProfileData FindProfileByPersonHandle(string personHandle);

    PersonData SavePerson(PersonData personData);
    PersonData FindPersonByHandle(string handle);

    DeviceData SaveDevice(DeviceData deviceData);
    DeviceData FindDeviceByHandle(string handle);

    OrganizationData SaveOrganization(OrganizationData organizationData);
    OrganizationData FindOrganizationByHandle(string handle);

    AgentData SaveAgent(AgentData agentData);
    AgentData FindAgentByHandle(string handle);

    /// <summary>
    /// Registers the first key set for a handle.  First registration wins: implementations
    /// MUST reject the save when a key set is already registered for the handle, because the
    /// registered key set is the trust anchor for device-key account confirmation (see
    /// <see cref="IPublicKeySetRegistrar"/>).  Consumers exposing this operation remain
    /// responsible for authenticating the caller.
    /// </summary>
    /// <param name="publicKeySetData">The key set to register.</param>
    /// <returns>The persisted key set.</returns>
    /// <exception cref="PublicKeySetConflictException">A key set is already registered for the handle.</exception>
    PublicKeySetData SavePublicKeySet(PublicKeySetData publicKeySetData);

    /// <summary>
    /// Replaces the registered key set for a handle, gated on proof of possession of the
    /// currently registered key: <paramref name="rotationSignature"/> must be a valid
    /// SHA512WITHRSA signature over <see cref="KeySetRotationPayload.Compose"/> of
    /// <paramref name="newKeySet"/>, made with the private key matching the currently
    /// registered public RSA key.  Implementations MUST update the existing row in place —
    /// rotation never creates a second row for the handle.
    /// </summary>
    /// <param name="newKeySet">The key set to rotate to, carrying the handle being rotated.</param>
    /// <param name="rotationSignature">The raw signature bytes proving possession of the current key.</param>
    /// <returns>The updated key set.</returns>
    /// <exception cref="InvalidKeySetRotationException">No key set is registered for the handle, or the proof is invalid.</exception>
    PublicKeySetData RotatePublicKeySet(PublicKeySetData newKeySet, byte[] rotationSignature);

    /// <summary>
    /// Revokes the active key set registered under a handle, gated on a break-glass admin proof
    /// (<paramref name="adminProof"/> — a signature over the target-bound
    /// <see cref="RevocationPayload"/> bound to <paramref name="authorizedSuccessorFingerprint"/>,
    /// verified against the configured admin public key).  Implementations MUST tombstone the row in
    /// place (<see cref="PublicKeySetData.RevokedUtc"/>): the handle becomes free to re-register, but
    /// the revoked key material stays blocklisted.  When a successor is bound, only that key may
    /// re-register the freed handle (bam.protocol#21).  See <see cref="IKeySetRevocation"/> and
    /// bam.protocol#11.
    /// </summary>
    /// <param name="keySetHandle">The handle whose active key set is being revoked.</param>
    /// <param name="adminProof">The raw admin signature bytes authorizing the revocation.</param>
    /// <param name="authorizedSuccessorFingerprint">
    /// The canonical fingerprint of the key authorized to re-register the freed handle, or null to
    /// bind no successor (leaving the handle openly re-registrable).
    /// </param>
    /// <returns>The tombstoned key set.</returns>
    /// <exception cref="KeySetRevocationException">No active key set is registered for the handle.</exception>
    /// <exception cref="UnauthorizedRevocationException">The admin proof does not authorize the revocation.</exception>
    PublicKeySetData RevokePublicKeySet(string keySetHandle, byte[] adminProof, string? authorizedSuccessorFingerprint);

    /// <summary>
    /// Finds the authoritative key set registered for a handle.  Implementations MUST resolve
    /// deterministically: when duplicate rows exist (legacy data or direct store tampering),
    /// the earliest-created row wins, so a later-added row can never displace the first
    /// registration.  A revoked key set is not authoritative and MUST NOT be returned.
    /// </summary>
    /// <param name="keySetHandle">The handle to resolve.</param>
    /// <returns>The authoritative key set, or null when none is registered.</returns>
    PublicKeySetData FindPublicKeySetByHandle(string keySetHandle);

    IEnumerable<PublicKeySetData> GetAllPublicKeySets();

    CertificateData SaveCertificate(CertificateData certificateData);
    CertificateData FindCertificateByHash(string hash);

    AgentCertificateData SaveAgentCertificate(AgentCertificateData agentCertificateData);
    AgentCertificateData FindAgentCertificateByHandle(string agentHandle);

    GroupData SaveGroup(GroupData groupData);
    GroupData FindGroupByName(string name);
    IEnumerable<GroupData> GetGroupsForPerson(string personHandle);
    void AddPersonToGroup(string personHandle, string groupName);
    void RemovePersonFromGroup(string personHandle, string groupName);
}
