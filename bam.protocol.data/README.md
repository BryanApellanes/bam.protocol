# bam.protocol.data

Data models, interfaces, and generated DAOs for the Bam Framework protocol layer -- actors, devices, profiles, keys, sessions, and server accounts.

## Overview

`bam.protocol.data` defines the domain data model for the Bam protocol ecosystem. It provides POCO types and their corresponding generated DAO (Data Access Object) classes across several functional areas: common infrastructure data (actors, agents, devices, machines, host addresses, NICs, process descriptors), client session data, server session data, profile data (persons, organizations, groups, certificates, key sets), private key storage, and registration data.

The project uses the Bam DAO generation pattern where POCO classes in domain folders (e.g., `Common/`, `Client/`, `Server/`, `Profile/`, `Private/`) have corresponding `Generated_Dao/` subfolders containing auto-generated DAO classes with typed queries, collections, columns, paged queries, wrappers, schema contexts, and schema repositories. These generated classes provide strongly-typed database access without manual SQL.

In addition to data types, the project defines key service interfaces: `IProfileRepository` (CRUD for profiles, persons, public key sets, certificates), `IAccountManager` (account registration), `IKeyManager` (RSA/ECC/AES key generation and actor key retrieval), `ICertificateManager` (X.509 certificate lifecycle), and `IProfileManager` (profile registration, lookup by handle or public key). Entity interfaces like `IActor`, `IPerson`, `IAgent`, `IDevice`, `IOrganization`, `IGroup`, and `IMachine` define the protocol's identity model.

## Key Classes

| Class / Interface | Description |
|---|---|
| `IActor` | Base identity: `Handle` (unique ID) and `Name` (display name), with `[CompositeKey]` attributes. |
| `IPerson` | Extends `IActor` with contact info: `Phone`, `Email`, `FirstName`, `LastName`, `MiddleName`. |
| `IAgent` | Extends `IActor` with associated `ActorData` and `DeviceData`. |
| `IDevice` | Extends `IActor` with `DeviceType`. |
| `IOrganization` | Organization with `Handle`, `Name`, and associated `People`. |
| `IGroup` | Group with `Name`, `Description`, and associated `PersonDatas`. |
| `IMachine` | Machine with `Name`, `DnsName`, `HostAddresses`, and `NetworkInterfaces`. |
| `IProfile` | Profile: `ProfileHandle`, `Name`, privacy toggles, `DeviceHandle`, `PersonHandle`. |
| `IProfileRepository` | Repository interface for CRUD on profiles, persons, devices, organizations, agents, public key sets, certificates, and agent certificates. |
| `IProfileManager` | High-level profile management: registration of persons, devices, organizations, and agents; lookup by handle or public key. |
| `IAccountManager` | Account registration from person registration data; combined person+device registration via `RegisterAccountWithDevice`. |
| `IKeyManager` | Key generation (RSA, ECC, AES) and actor key retrieval (signing + encryption). |
| `ICertificateManager` | X.509 certificate creation (root CA, signed) and loading. |
| `IApplicationKeySet` | Marker for application-scoped key sets with `ApplicationName`. |
| `AgentData` | POCO for software agents with actor, device, and process descriptor references. |
| `DeviceData` | POCO for devices extending `MachineData` with process descriptor and device type. |
| `MachineData` | POCO for machines with NIC and host address collections, auto-initialized from the local system. |
| `ProcessDescriptorData` | POCO capturing current process info (PID, machine name); provides `ProcessDescriptorData.Current`. |
| `HostAddressData` | POCO for IP/hostname address data. |
| `NicData` | POCO for network interface card data. |
| `ClientKeySetData` | Client-side key set data: client/server RSA keys, client/server ECC keys, machine/host info. |
| `ClientSessionData` | Client session with `SessionId` and key-value pairs. |
| `ServerSession` | Server-side session with `SessionId` and key-value pairs. |
| `ServerSessionKeyValuePair` | Key-value pair stored within a server session. |
| `ServerAccountData` | Server account data with issuer and profile handle. |
| `AccountData` | Account data with `PersonHandle`. |
| `InboxData` | Inbox associated with an account (stub). |
| `OutboxData` | Outbox associated with an account (stub). |
| `ProfileData` | Profile POCO with person reference, privacy settings, and auto-generated handle. |
| `PublicKeySetData` | Public RSA and ECC keys with a `KeySetHandle`. |
| `PersonData` | Person POCO implementing `IPerson`. |
| `PersonRegistrationData` | Registration DTO for new person profiles. |
| `DeviceRegistrationData` | Registration DTO for device registration with handle, name, and device type. |
| `OrganizationRegistrationData` | Registration DTO for organizations with handle and name. |
| `AgentRegistrationData` | Registration DTO for agents binding a person to a device. |
| `KeySetFile` | File-based key set persistence in `~/.bam/data/`. |
| `MachineInfo` | Simple `IMachine` implementation. |

## Dependencies

### Project References
- `bam.base` -- Core framework utilities (extension methods, `Args`, `Cuid`, etc.)
- `bam.data.repositories` -- Repository base classes (`RepoData`, `KeyedAuditRepoData`, `CompositeKeyAuditRepoData`, `[CompositeKey]`)
- `bam.encryption` -- Cryptographic types (`EccPublicKey`, `RsaPublicKey`, `AesKey`, `IPublicKey`, `IPrivateKey`)
- `bam.protocol` -- Protocol interfaces (`IActor`, `IKeySet`, `DeviceTypes`, etc.)

### Package References
None.

## Usage Examples

### Defining and persisting a server session
```csharp
using Bam.Protocol.Data.Server;

ServerSession session = new ServerSession
{
    SessionId = Cuid.Generate()
};
session.KeyValues.Add(new ServerSessionKeyValuePair { Key = "ClientPublicKey", Value = clientPublicKeyPem });

// Save via generated schema repository
session = serverSessionRepository.Save(session);
```

### Working with profile data
```csharp
using Bam.Protocol.Data.Profile;

ProfileData profile = new ProfileData
{
    PersonHandle = "abc123",
    Name = "Personal Profile",
    ShowEmail = false,
    ShowPhone = false
};

profile = profileRepository.SaveProfile(profile);
```

### Using the IProfileRepository interface
```csharp
using Bam.Protocol.Data;

IProfileRepository repo = serviceRegistry.Get<IProfileRepository>();

// Save and look up a public key set
PublicKeySetData keySet = new PublicKeySetData
{
    KeySetHandle = actorHandle,
    PublicRsaKey = rsaPublicKeyPem,
    PublicEccKey = eccPublicKeyPem
};
repo.SavePublicKeySet(keySet);

// Find by handle
PublicKeySetData found = repo.FindPublicKeySetByHandle(actorHandle);
```

### Device initialization
```csharp
using Bam.Protocol.Data.Common;

// DeviceData auto-initializes with local machine info
DeviceData device = new DeviceData(initialize: true);
// device.Name, device.DnsName, device.HostAddresses, device.NetworkInterfaces are populated
// device.Handle is auto-generated
// device.ProcessDescriptorData is set to ProcessDescriptorData.Current
```

## Registration Chain

The protocol defines a natural identity hierarchy for registration:

```
1. Person       → the human identity (name, email, phone)
2. Device       → the machine a person uses (auto-detected OS type)
3. Agent        → the software process (Person + Device binding)
4. Organization → a group of people
```

Registration flows:
- **Person**: `IProfileManager.RegisterPersonProfile()` creates a `PersonData` and associated `ProfileData`.
- **Device**: `IProfileManager.RegisterDeviceProfile()` creates a `DeviceData` (auto-detecting `DeviceType` from the OS if not specified) and links it to an existing profile via `DeviceHandle`.
- **Agent**: `IProfileManager.RegisterAgent()` creates an `AgentData` binding a person and device together.
- **Organization**: `IProfileManager.RegisterOrganization()` creates an `OrganizationData`.
- **Combined**: `IAccountManager.RegisterAccountWithDevice()` performs person + device + agent registration in a single call.

## Known Gaps / Not Yet Implemented

- **`InboxData` and `OutboxData` are stubs** -- These classes contain only an `AccountDataId` foreign key and an `AccountData` navigation property with no additional fields or behavior, suggesting a messaging feature that is not yet implemented.
