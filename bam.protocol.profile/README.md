# bam.protocol.profile

Profile management, key management, certificate authority, and encrypted profile storage for the Bam Framework.

## Overview

`bam.protocol.profile` provides the concrete implementations for identity and cryptographic profile management in the Bam protocol. It includes `ProfileManager` for creating, registering, and looking up user profiles; `KeyManager` for generating RSA, ECC, and shared AES keys and retrieving actor signing/encryption keys; `CertificateManager` and `CertificateAuthority` for X.509 certificate lifecycle management using BouncyCastle; and `PrivateKeyManager` for secure private key storage via opaque encrypted filesystem storage.

The project also provides `EncryptedProfileRepository`, an `IProfileRepository` implementation that stores all profile data (persons, profiles, public key sets, certificates) in an AES-encrypted object data repository backed by the filesystem. The `ProfileRepositoryServiceRegistration` extension method registers all necessary services into a `ServiceRegistry` to wire up the full encrypted profile storage pipeline.

X.509 certificate generation is handled by `CertificateAuthority`, which extends `CertificateIssuer` and uses configurable `IX509NameProvider` implementations for X.500 distinguished name construction. Two OID provider implementations (`OIDProvider` and `ThreeHeadzPenOidProvider`) support custom OID fields in certificates via IANA Private Enterprise Numbers.

## Key Classes

| Class / Interface | Description |
|---|---|
| `ProfileManager` | Implements `IProfileManager`: registers persons, devices, organizations, and agents; creates profiles; looks up by handle or public key SHA. |
| `Profile` | Simple POCO implementing `IProfile` with handle, name, privacy toggles, device/person handles. |
| `AccountManager` | Implements `IAccountManager`: registers accounts by creating a profile then saving a `ServerAccountData` record; supports combined person+device+agent registration via `RegisterAccountWithDevice`. |
| `KeyManager` | Implements `IKeyManager`: generates RSA/ECC/AES keys, retrieves actor signing and encryption private keys, derives shared AES keys via ECDH. |
| `CertificateManager` | Implements `ICertificateManager`: creates root CA and signed X.509 certificates, loads certificates from repository, persists cert + agent-cert associations. |
| `CertificateAuthority` | Extends `CertificateIssuer`: creates X.509 certificates with configurable options, issuer name, subject name, and key material. |
| `PrivateKeyManager` | Implements `IPrivateKeyManager`: generates RSA/ECC private keys and stores them in opaque encrypted filesystem storage keyed by public key SHA. |
| `EncryptedProfileRepository` | Implements `IProfileRepository`: encrypted CRUD for profiles, persons, devices, organizations, agents, public key sets, certificates, and agent certificates using `ObjectDataRepository`. |
| `ProfileRepositoryServiceRegistration` | Static extension `AddEncryptedProfileRepository` that registers all encrypted profile storage dependencies into a `ServiceRegistry`. |
| `GenerateCertificateOptions` | Configuration object for certificate generation (issuer name, subject name, keys). |
| `IX509NameProvider` | Interface for constructing `X509Name` from actor or string subject. |
| `X509NameProvider` | Default `IX509NameProvider` implementation. |
| `BamX509NameProvider` | Bam-specific X.500 name provider. |
| `IOidProvider` | Interface for OID field providers. |
| `OIDProvider` | Base OID provider for custom certificate fields. |
| `PenOidProvider` | OID provider using IANA Private Enterprise Numbers. |
| `ThreeHeadzPenOidProvider` | Three Headz specific PEN OID provider (PEN 63498). |
| `ThreeHeadzPen` | Constants for Three Headz IANA PEN. |
| `ICustomFieldProvider` | Interface for providing custom fields to certificates. |
| `CustomFieldProvider` | Default custom field provider implementation. |
| `IHandleProvider` | Interface for generating unique handles. |
| `ProfileType` | Enum or type distinguishing profile types. |
| `Country` | Country data for certificate generation. |

## Dependencies

### Project References
- `bam.data.objects` -- Object data repository, encoding/decoding (`ObjectDataRepository`, `ICompositeKeyCalculator`)
- `bam.data.repositories` -- Repository base classes and composite key attributes
- `bam.encryption` -- All cryptographic types (`AesKey`, `EccPublicPrivateKeyPair`, `RsaPublicPrivateKeyPair`, `CertificateIssuer`, etc.)
- `bam.protocol` -- Core protocol interfaces (`IActor`, `IKeySet`, `IPrivateKeyManager`)
- `bam.protocol.data` -- Data models (`ProfileData`, `PersonData`, `PublicKeySetData`, `IProfileRepository`, etc.)

### Package References
None (BouncyCastle is provided transitively via `bam.encryption`).

## Usage Examples

### Registering a profile and account
```csharp
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;
using Bam.Protocol.Data;

// Create and register a person profile
IProfileManager profileManager = serviceRegistry.Get<IProfileManager>();
IProfile profile = profileManager.RegisterPersonProfile(new PersonRegistrationData
{
    FirstName = "Alice",
    LastName = "Smith",
    Email = "alice@example.com",
    Phone = "555-0123"
});

// Register a device and link it to the person's profile
IProfile updatedProfile = profileManager.RegisterDeviceProfile(new DeviceRegistrationData
{
    Name = "Alice's Laptop",
    DeviceType = DeviceTypes.DesktopWindows
}, profile.PersonHandle);

// Register an agent (binds person + device)
AgentData agent = profileManager.RegisterAgent(new AgentRegistrationData
{
    Name = "Alice@Laptop",
    PersonHandle = profile.PersonHandle,
    DeviceHandle = updatedProfile.DeviceHandle
});

// Register an organization
OrganizationData org = profileManager.RegisterOrganization(new OrganizationRegistrationData
{
    Handle = "acme",
    Name = "Acme Corp"
});

// Or do person + device + agent registration in one call
IAccountManager accountManager = serviceRegistry.Get<IAccountManager>();
AccountData account = accountManager.RegisterAccountWithDevice(
    new PersonRegistrationData
    {
        FirstName = "Alice",
        LastName = "Smith",
        Email = "alice@example.com"
    },
    new DeviceRegistrationData
    {
        Name = "Alice's Laptop",
        DeviceType = DeviceTypes.DesktopWindows
    }
);
```

### Managing cryptographic keys
```csharp
using Bam.Protocol.Profile;
using Bam.Encryption;

IKeyManager keyManager = serviceRegistry.Get<IKeyManager>();

// Generate key pairs
RsaPublicPrivateKeyPair rsaKeyPair = keyManager.GenerateRsaKeyPair();
EccPublicPrivateKeyPair eccKeyPair = keyManager.GenerateEccKeyPair();
AesKey aesKey = keyManager.GenerateAesKey();

// Derive a shared AES key between two actors (ECDH)
AesKey sharedKey = keyManager.GenerateSharedAesKey(actorAlice, actorBob);

// Retrieve signing and encryption keys
IPrivateKey signingKey = keyManager.GetSigningKey(actor);
IPrivateKey encryptionKey = keyManager.GetEncryptionKey(actor);
```

### Creating X.509 certificates
```csharp
using Bam.Protocol;
using Org.BouncyCastle.X509;

ICertificateManager certManager = serviceRegistry.Get<ICertificateManager>();

// Create a root CA certificate for an actor
X509Certificate rootCert = certManager.CreateRootCACertificate(actor);

// Create a signed certificate
X509Certificate signedCert = certManager.CreateSignedCertificate(actor);

// Load an existing root CA certificate
X509Certificate loaded = certManager.LoadRootCACertificate(actor);
```

### Wiring up the encrypted profile repository
```csharp
using Bam.DependencyInjection;
using Bam.Protocol.Profile;

ServiceRegistry registry = new ServiceRegistry();
registry.AddEncryptedProfileRepository("./.bam/profile");

// Now resolve any profile service
IProfileRepository repo = registry.Get<IProfileRepository>();
IProfileManager manager = registry.Get<IProfileManager>();
```

## Known Gaps / Not Yet Implemented

- **`CertificateManager.CreateSignedCertificate` is identical to `CreateRootCACertificate`** -- Both methods call `CertificateAuthority.CreateCertificate(actor.Name, ...)` with the same logic. The signed certificate variant should ideally use the root CA's private key for signing rather than the issuer's key, implementing proper certificate chain semantics.
- **No certificate revocation support** -- There is no CRL (Certificate Revocation List) or OCSP implementation for revoking certificates.
- **`EncryptedProfileRepository` generates a new AES key on each initialization** -- The `ProfileRepositoryServiceRegistration.AddEncryptedProfileRepository` creates a fresh `AesKey()` each time, meaning data is not recoverable across process restarts unless the key is externally persisted.
