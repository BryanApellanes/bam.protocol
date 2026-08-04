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

// Register a public key set (first registration wins — the registered key set is the
// trust anchor for device-key account confirmation). A second registration for the same
// handle throws PublicKeySetConflictException; key material already registered under a
// different handle throws PublicKeySetKeyMaterialConflictException; unparseable key
// material throws InvalidPublicKeySetException. The registrar stamps the creation time
// server-side, so a caller-supplied Created cannot influence resolution.
PublicKeySetData keySet = new PublicKeySetData
{
    KeySetHandle = actorHandle,   // handle equality is ORDINAL and case-sensitive
    PublicRsaKey = rsaPublicKeyPem,
    PublicEccKey = eccPublicKeyPem
};
repo.SavePublicKeySet(keySet);

// Find by handle (deterministic — if duplicate rows exist, the earliest-created wins)
PublicKeySetData found = repo.FindPublicKeySetByHandle(actorHandle);

// Replace a registered key set: rotation requires proof of possession of the currently
// registered key — a SHA512WITHRSA signature over
// KeySetRotationPayload.Compose(currentPublicRsaKeySha256, newKeySet), made with the
// private key matching the currently registered public RSA key. The payload binds the
// SHA-256 of the current key and is length-prefixed so it cannot be re-split into a
// different key set. The row is updated in place; an invalid proof or unparseable key
// throws InvalidKeySetRotationException.
PublicKeySetData newKeySet = new PublicKeySetData
{
    KeySetHandle = actorHandle,
    PublicRsaKey = newRsaPublicKeyPem,
    PublicEccKey = newEccPublicKeyPem
};
string currentPublicRsaKeySha256 = found.PublicRsaKey.Sha256();
byte[] rotationSignature = SignWithCurrentPrivateKey(
    KeySetRotationPayload.Compose(currentPublicRsaKeySha256, newKeySet));
repo.RotatePublicKeySet(newKeySet, rotationSignature);

// Break-glass revocation: an authorized administrator revokes the active key set, freeing the
// handle for re-registration while keeping the revoked (compromised) key material blocklisted.
// The admin signs the target-bound RevocationPayload offline (e.g. on a YubiKey / PIV applet);
// only the admin PUBLIC key is held by the framework (IAdminPublicKeySource), and revocation
// fails closed when no admin key is configured. A revoked key set is no longer authoritative,
// so FindPublicKeySetByHandle and device-key confirmation stop honoring it.
//
// The third argument is the OPTIONAL authorized-successor binding (bam.protocol#21). Pass null to
// leave the freed handle openly re-registrable (the pre-#21 behavior); pass a successor fingerprint
// to restrict re-registration to exactly that key, closing the revoke->re-register hijack window.
// The successor is a signed field of RevocationPayload, so ONE admin proof both revokes and names
// the successor.
PublicKeySetData target = repo.FindPublicKeySetByHandle(actorHandle);

// (a) Unbound revocation — handle is openly re-registrable afterward:
byte[] openProof = SignWithBreakGlassAdminPrivateKey(RevocationPayload.Compose(target, null));
repo.RevokePublicKeySet(actorHandle, openProof, null);

// (b) Successor-bound revocation — only successorPublicRsaKeyPem may re-register the handle:
string successorFingerprint = PublicKeyFingerprint.Of(successorPublicRsaKeyPem);
byte[] boundProof = SignWithBreakGlassAdminPrivateKey(
    RevocationPayload.Compose(target, successorFingerprint));
repo.RevokePublicKeySet(actorHandle, boundProof, successorFingerprint);
// A re-registration whose RSA identity key is not the bound successor now throws
// UnauthorizedSuccessorException; the bound successor's own Register(...) succeeds.
```

> Consumers exposing key-set registration or rotation over a network surface remain
> responsible for authenticating and authorizing the caller; the policy above bounds what
> any caller can do to an already-registered handle (see `IPublicKeySetRegistrar`).
>
> **Recovery via revocation:** registration is first-registration-wins and rotation requires the
> current private key, so a handle whose registered key becomes unusable (a lost private key, or a
> valid-but-uncontrolled first registration) cannot recover by itself. The recovery path is
> **break-glass revocation** (`RevokePublicKeySet`, BryanApellanes/bam.protocol#11): an authorized
> administrator revokes the key set, which frees the handle for re-registration while keeping the
> revoked key material blocklisted. This can recover an **ECC-only** handle (below).
>
> **Successor binding (BryanApellanes/bam.protocol#21):** revocation optionally binds an
> **admin-authorized successor** — the canonical fingerprint (`PublicKeyFingerprint.Of`) of the one
> key permitted to re-register the freed handle, carried as a signed field of `RevocationPayload` so a
> single admin proof both revokes and names the successor. When a successor is bound, a re-registration
> whose RSA identity key does not match is rejected with `UnauthorizedSuccessorException`. This
> **narrows** the revoke→re-register window from "any first caller" to "any holder of the bound public
> key"; when the governing (latest) revocation binds no successor, the handle stays openly
> re-registrable, unchanged from #11 (hijack resistance is then the consumer's own authz).
>
> **The binding authenticates a public key, not proof of possession — mind its limits** (bam.protocol#25
> review SF1 / auditor Condition 4):
> - **The successor public key must stay secret until the handle is claimed.** The gate admits anyone
>   presenting a key set whose RSA material fingerprints to the binding; `Register` demands no signature.
>   A public key is not designed to be secret, so anyone who has seen the successor's PEM (published,
>   reused, or logged) can front-run the claim. Do not bind a successor whose public key is already
>   observable.
> - **Only the RSA identity key is bound; the candidate's ECC field is unconstrained.** A claimant
>   presenting the bound RSA key may attach *their own* `PublicEccKey`, and session/actor resolution
>   keys off the ECC material — so traffic encrypted to the handle can reach them until the real
>   successor notices. Recovery is **rotation** (which proves possession of the registered RSA key).
> - A durable fix — requiring a signed proof-of-possession claim at re-registration when a binding
>   exists — is tracked as a follow-on (see bam.protocol#21's spin-offs).
>
> **Interim caveat:** the binding names a *single* successor and there is no admin path to re-point it
> if that successor key is itself lost before it claims the handle — re-binding a lost successor is
> tracked as BryanApellanes/bam.protocol#23; until it lands, do not bind an irreplaceable handle to a
> successor key you cannot guarantee will be available to claim it.
>
> **Relational/DAO path carries no revocation state** (bam.protocol#25 auditor Condition 5): the
> key-set policy (registration, rotation, revocation, successor binding) runs entirely over the
> object-data (JSON) store — `RevokedUtc`/`RevokedBy`/`AuthorizedSuccessorFingerprint` and the stamped
> fingerprints live there. The generated SQL/DAO projection of `PublicKeySetData` does **not** carry
> these columns, so a consumer reading key sets through the relational path would see revoked rows as
> active and no bindings. Treat the object-data repository as authoritative for key-set trust decisions;
> do not route them through the DAO path until the DAO is regenerated to carry revocation state.
>
> Break-glass admin-key rotation is tracked as BryanApellanes/bam.protocol#17, and revocation-proof
> freshness beyond target-binding as BryanApellanes/bam.protocol#15.
>
> **Revocation does not, by itself, terminate access on the default server pipeline.** Until the
> revoked-key filter in `ProfileManager.FindProfileByPublicKey` lands (BryanApellanes/bam.protocol#13),
> a holder of a revoked private key still **authenticates** (the JWT is verified against the
> client-supplied public key from session state, with no active-key-set check) and still **retains
> the victim's access level** (`ActorResolver` → `FindProfileByPublicKey` unfiltered scan →
> `GroupAccessLevelProvider`). Revocation stops certificate minting and handle resolution via
> `FindPublicKeySetByHandle`/device-key confirmation, but not authentication or the actor's
> authorization level — an operator revoking to end an intrusion must also land #13 (and rely on
> consumer-side session invalidation) to fully cut access.
>
> **Pre-registration squatting:** because a public key maps to exactly one handle, an attacker
> who learns a victim's public key *before* the victim registers it (the client public key
> travels in `StartSessionRequest` and is stored into session state, so it is observable
> pre-registration) can register it under a squatted handle first and permanently block the
> victim's own registration of that key. It is recoverable — the victim generates a fresh
> keypair — and it needs the same unauthenticated registration surface that consumer authz
> (BryanApellanes/bamsvc#6, BryanApellanes/socialkeyinfrastructure.io#9) is responsible for
> closing. Enforcing uniqueness is still strictly safer than not; this is the accepted trade.
>
> **ECC-only registrations are role-limited and unrotatable:** registering a key set with only
> a `PublicEccKey` (no RSA) is allowed for encryption-only actors (ECDH shared-key derivation),
> but such a handle can never rotate (rotation proves possession of the current *RSA* key) and
> can never pass device-key confirmation (which requires a non-empty RSA key). There is no
> in-place upgrade to add an RSA key later — rotation needs a current RSA key and re-registration
> conflicts on the handle — so an ECC-only handle is permanently encryption-only until its key set
> is **revoked** (BryanApellanes/bam.protocol#11), after which the freed handle can be
> re-registered with an RSA-bearing key set, or a different handle with fresh key material is used.

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
