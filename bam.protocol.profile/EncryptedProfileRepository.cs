using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

public class EncryptedProfileRepository : IProfileRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedProfileRepository"/> class with the
    /// default key-set policies — a <see cref="PublicKeySetRegistrar"/> verifying rotation proofs
    /// via <see cref="RsaKeySetRotationVerifier"/>, a <see cref="KeySetRevocation"/> whose
    /// admin key is unconfigured (revocation fails closed until a break-glass key is supplied),
    /// and a <see cref="PublicKeySetResolver"/> over the same repository as the resolution
    /// authority.
    /// </summary>
    /// <param name="repository">The object-data repository profile data is persisted in.</param>
    public EncryptedProfileRepository(ObjectDataRepository repository)
        : this(repository,
            new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider())),
            new KeySetRevocation(repository, new RsaRevocationAuthority(new RsaSignatureProvider(), new StaticAdminPublicKeySource(null))),
            new PublicKeySetResolver(repository))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedProfileRepository"/> class with the
    /// default resolution authority, a <see cref="PublicKeySetResolver"/> over the same
    /// repository.
    /// </summary>
    /// <param name="repository">The object-data repository profile data is persisted in.</param>
    /// <param name="publicKeySetRegistrar">The registration policy guarding the key-set trust anchor; see <see cref="IPublicKeySetRegistrar"/>.</param>
    /// <param name="keySetRevocation">The break-glass revocation policy; see <see cref="IKeySetRevocation"/>.</param>
    public EncryptedProfileRepository(ObjectDataRepository repository, IPublicKeySetRegistrar publicKeySetRegistrar, IKeySetRevocation keySetRevocation)
        : this(repository, publicKeySetRegistrar, keySetRevocation, new PublicKeySetResolver(repository))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedProfileRepository"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository profile data is persisted in.</param>
    /// <param name="publicKeySetRegistrar">The registration policy guarding the key-set trust anchor; see <see cref="IPublicKeySetRegistrar"/>.</param>
    /// <param name="keySetRevocation">The break-glass revocation policy; see <see cref="IKeySetRevocation"/>.</param>
    /// <param name="publicKeySetResolver">The deterministic key-set resolution authority the read path delegates to; see <see cref="IPublicKeySetResolver"/>.</param>
    public EncryptedProfileRepository(ObjectDataRepository repository, IPublicKeySetRegistrar publicKeySetRegistrar, IKeySetRevocation keySetRevocation, IPublicKeySetResolver publicKeySetResolver)
    {
        this.Repository = repository;
        this.PublicKeySetRegistrar = publicKeySetRegistrar;
        this.KeySetRevocation = keySetRevocation;
        this.PublicKeySetResolver = publicKeySetResolver;
    }

    protected ObjectDataRepository Repository { get; }

    /// <summary>
    /// Gets the registration policy guarding the key-set trust anchor.  The key-set members
    /// of this repository delegate to it so the policy stays in one independently testable
    /// place.
    /// </summary>
    protected IPublicKeySetRegistrar PublicKeySetRegistrar { get; }

    /// <summary>
    /// Gets the break-glass revocation policy.  <see cref="RevokePublicKeySet"/> delegates to it.
    /// </summary>
    protected IKeySetRevocation KeySetRevocation { get; }

    /// <summary>
    /// Gets the deterministic key-set resolution authority the read path delegates to, shared
    /// with the registrar so resolution and registration-time uniqueness can never disagree.
    /// </summary>
    protected IPublicKeySetResolver PublicKeySetResolver { get; }

    public ProfileData SaveProfile(ProfileData profileData)
    {
        return Repository.Create(profileData);
    }

    public ProfileData FindProfileByHandle(string handle)
    {
        return Repository.Query<ProfileData>(p => p.ProfileHandle == handle).FirstOrDefault()!;
    }

    public ProfileData FindProfileByPersonHandle(string personHandle)
    {
        return Repository.Query<ProfileData>(p => p.PersonHandle == personHandle).FirstOrDefault()!;
    }

    public PersonData SavePerson(PersonData personData)
    {
        return Repository.Create(personData);
    }

    public PersonData FindPersonByHandle(string handle)
    {
        return Repository.Query<PersonData>(p => p.Handle == handle).FirstOrDefault()!;
    }

    public DeviceData SaveDevice(DeviceData deviceData)
    {
        return Repository.Create(deviceData);
    }

    public DeviceData FindDeviceByHandle(string handle)
    {
        return Repository.Query<DeviceData>(d => d.Handle == handle).FirstOrDefault()!;
    }

    public OrganizationData SaveOrganization(OrganizationData organizationData)
    {
        return Repository.Create(organizationData);
    }

    public OrganizationData FindOrganizationByHandle(string handle)
    {
        return Repository.Query<OrganizationData>(o => o.Handle == handle).FirstOrDefault()!;
    }

    public AgentData SaveAgent(AgentData agentData)
    {
        return Repository.Create(agentData);
    }

    public AgentData FindAgentByHandle(string handle)
    {
        return Repository.Query<AgentData>(a => a.Handle == handle).FirstOrDefault()!;
    }

    /// <inheritdoc />
    public PublicKeySetData SavePublicKeySet(PublicKeySetData publicKeySetData)
    {
        return PublicKeySetRegistrar.Register(publicKeySetData);
    }

    /// <inheritdoc />
    public PublicKeySetData RotatePublicKeySet(PublicKeySetData newKeySet, byte[] rotationSignature)
    {
        return PublicKeySetRegistrar.Rotate(newKeySet, rotationSignature);
    }

    /// <inheritdoc />
    public PublicKeySetData RevokePublicKeySet(string keySetHandle, byte[] adminProof)
    {
        return KeySetRevocation.Revoke(keySetHandle, adminProof);
    }

    /// <inheritdoc />
    public PublicKeySetData FindPublicKeySetByHandle(string keySetHandle)
    {
        return PublicKeySetRegistrar.Resolve(keySetHandle)!;
    }

    /// <inheritdoc />
    public PublicKeySetData FindPublicKeySetByPublicKey(string publicKeyPem)
    {
        return PublicKeySetResolver.ResolveByKeyMaterial(publicKeyPem)!;
    }

    public IEnumerable<PublicKeySetData> GetAllPublicKeySets()
    {
        return Repository.RetrieveAll<PublicKeySetData>();
    }

    public CertificateData SaveCertificate(CertificateData certificateData)
    {
        return Repository.Create(certificateData);
    }

    public CertificateData FindCertificateByHash(string hash)
    {
        return Repository.Query<CertificateData>(c => c.Hash == hash).FirstOrDefault()!;
    }

    public AgentCertificateData SaveAgentCertificate(AgentCertificateData agentCertificateData)
    {
        return Repository.Create(agentCertificateData);
    }

    public AgentCertificateData FindAgentCertificateByHandle(string agentHandle)
    {
        return Repository.Query<AgentCertificateData>(c => c.AgentHandle == agentHandle).FirstOrDefault()!;
    }

    public GroupData SaveGroup(GroupData groupData)
    {
        return Repository.Create(groupData);
    }

    public GroupData FindGroupByName(string name)
    {
        return Repository.Query<GroupData>(g => g.Name == name).FirstOrDefault()!;
    }

    public IEnumerable<GroupData> GetGroupsForPerson(string personHandle)
    {
        PersonData person = FindPersonByHandle(personHandle);
        if (person?.GroupDatas != null)
        {
            return person.GroupDatas;
        }
        return Enumerable.Empty<GroupData>();
    }

    public void AddPersonToGroup(string personHandle, string groupName)
    {
        PersonData person = FindPersonByHandle(personHandle);
        GroupData group = FindGroupByName(groupName);
        if (group == null)
        {
            group = SaveGroup(new GroupData { Name = groupName, Description = groupName });
        }

        person.GroupDatas ??= new List<GroupData>();
        if (!person.GroupDatas.Any(g => g.Name == groupName))
        {
            person.GroupDatas.Add(group);
            SavePerson(person);
        }
    }

    public void RemovePersonFromGroup(string personHandle, string groupName)
    {
        PersonData person = FindPersonByHandle(personHandle);
        if (person?.GroupDatas == null) return;

        var group = person.GroupDatas.FirstOrDefault(g => g.Name == groupName);
        if (group != null)
        {
            person.GroupDatas.Remove(group);
            SavePerson(person);
        }
    }
}
