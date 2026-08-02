using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol.Profile;

public interface IProfileManager
{
    IProfile RegisterPersonProfile(PersonRegistrationData personRegistrationData);
    IProfile RegisterDeviceProfile(DeviceRegistrationData deviceRegistrationData, string personHandle);
    DeviceData FindDeviceByHandle(string handle);

    OrganizationData RegisterOrganization(OrganizationRegistrationData organizationRegistrationData);
    OrganizationData FindOrganizationByHandle(string handle);

    AgentData RegisterAgent(AgentRegistrationData agentRegistrationData);
    AgentData FindAgentByHandle(string handle);

    IProfile GetProfile(string handle, bool createIfNotExists = false);
    IProfile CreateProfile();
    IProfile FindProfileByHandle(string handle);

    /// <summary>
    /// Finds the profile whose registered key set carries a public key with the specified
    /// SHA-256 digest of its PEM encoding.  Digest-based matching cannot be served by an
    /// indexed store lookup — callers holding the full PEM should prefer
    /// <see cref="FindProfileByPublicKeyPem"/>.  Implementations MUST resolve
    /// deterministically: when duplicate key material exists, the earliest-created key set wins.
    /// </summary>
    /// <param name="publicKeyPemSha">The SHA-256 digest of the public key's PEM encoding.</param>
    /// <returns>The resolved profile, or null when the digest matches no registered key set.</returns>
    IProfile FindProfileByPublicKey(string publicKeyPemSha);

    IProfile FindProfileByPublicKey(IPublicKey publicKey);

    /// <summary>
    /// Finds the profile whose registered key set carries the specified PEM-encoded public key
    /// (matched against both the RSA and ECC key fields).  The preferred key-to-profile lookup:
    /// it resolves through the store's indexed, deterministic key-set resolution (see
    /// <see cref="IPublicKeySetResolver"/>).
    /// </summary>
    /// <param name="publicKeyPem">The PEM-encoded public key material to resolve.</param>
    /// <returns>The resolved profile, or null when the material matches no registered key set.</returns>
    IProfile FindProfileByPublicKeyPem(string publicKeyPem);
}
