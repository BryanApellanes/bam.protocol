# bam.protocol.tests

Unit and integration tests for the Bam Framework protocol layer, covering client, server, profile, transport, and cryptographic security components.

## Overview

`bam.protocol.tests` is an executable test project that uses the Bam custom test runner (`BamConsoleContext.StaticMain`), not xUnit or NUnit. Tests are organized into unit and integration test classes decorated with `[UnitTestMenu]` attributes, using the fluent `When.A<T>()` and `After.Setup()` API to define test scenarios with structured assertions via `ShouldPass` / `because` patterns.

The test suite covers the full protocol stack: client request building and dispatching (`BamClientShould`), server initialization pipeline and builder (`BamServerBuilderShould`, `BamServerShould`), request line parsing (`RequestLineShould`), request reading (`RequestReaderShould`), method invocation serialization and execution (`InvocationRequestShould`), session management on both client and server sides (`ClientSessionStateShould`, `BamServerSessionStateShould`, `BamServerSessionProviderShould`, `SessionDataRepositoryShould`), JWT token handling (`BamJwtTokenShould`), actor resolution (`ActorResolverShould`), authorization (`AuthorizationCalculatorShould`, `AuthPipelineShould`), client request security (`ClientRequestSecurityProviderShould`), profile management (`ProfileManagerShould`, `AccountManagerShould`), key management (`KeyManagerShould`), certificate management (`CertificateManagerShould`, `CertificateAuthorityShould`), device data initialization (`DeviceDataShould`), protected AES key usage (`ProtectedAesKeyUsageContextShould`), and transport protocols (`TcpUdpTransportShould`). Integration tests include authenticated roundtrip (`AuthenticatedRoundtripShould`) and secure transport (`SecureTransportRoundtripShould`).

Test support classes like `TestClass`, `TestEchoService`, `TestMethodInvocationRequest`, `TestRequestReader`, and `TestSetup` provide fixtures for method invocation, mock services, and database setup using in-memory SQLite.

## Key Classes

| Class / Interface | Description |
|---|---|
| `Program` | Entry point calling `BamConsoleContext.StaticMain(args)` for the menu-driven test runner. |
| `TestSetup` | Helper that creates an in-memory SQLite `ServerSessionSchemaRepository` pre-loaded with test session data. |
| `TestClass` | Simple test target with a `TestMethod(string, string)` that returns a formatted string including instance state. |
| `TestEchoService` | Simple echo service for testing remote method invocation roundtrips. |
| `TestMethodInvocationRequest` | Subclass of `MethodInvocationRequest` exposing `GetInstance()` and `GetMethodInfo()` for test assertions. |
| `TestRequestReader` | Custom request reader for test scenarios. |
| **Unit Tests** | |
| `InvocationRequestShould` | Tests `MethodInvocationRequest` client/server initialization, serialization, execution, and round-trip. |
| `BamClientShould` | Tests client request builder creation (HTTP/TCP/UDP), request construction, HTTP response handling, and session start. |
| `BamClientRequestBuilderShould` | Tests the fluent request builder pattern for all protocols. |
| `ClientSessionStateShould` | Tests ECC key pair session state, AES key derivation, protected key usage. |
| `ClientRequestSecurityProviderShould` | Tests body encryption, ECC signing, nonce hashing, and request preparation. |
| `BamServerBuilderShould` | Tests the fluent server builder producing correctly configured `BamServer` instances. |
| `BamServerShould` | Tests server lifecycle (start, handle requests, stop). |
| `RequestLineShould` | Tests parsing of `BamRequestLine` from raw request strings. |
| `RequestReaderShould` | Tests `BamRequestReader` parsing of HTTP and stream-based requests. |
| `BamServerSessionStateShould` | Tests server-side `ServerSessionState` get/set with database persistence. |
| `BamServerSessionProviderShould` | Tests `ServerSessionManager` session creation and retrieval. |
| `SessionDataRepositoryShould` | Tests `ServerSessionSchemaRepository` CRUD operations. |
| `BamJwtTokenShould` | Tests JWT encode, decode, and ECC signature verification. |
| `ActorResolverShould` | Tests actor resolution from session public keys and profile lookup. |
| `AuthorizationCalculatorShould` | Tests `[RequiredAccess]` attribute evaluation and access level comparison. |
| `AuthPipelineShould` | Tests the full authentication pipeline integration. |
| `ProfileManagerShould` | Tests profile registration, creation, and lookup. |
| `AccountManagerShould` | Tests account registration flow. |
| `KeyManagerShould` | Tests RSA/ECC/AES key generation and actor key retrieval. |
| `CertificateManagerShould` | Tests X.509 certificate creation and loading. |
| `CertificateAuthorityShould` | Tests certificate authority certificate generation. |
| `DeviceDataShould` | Tests `DeviceData` initialization with machine/process info. |
| `ProtectedAesKeyUsageContextShould` | Tests protected AES key memory management and usage patterns. |
| `TcpUdpTransportShould` | Tests TCP and UDP transport request/response. |
| **Integration Tests** | |
| `AuthenticatedRoundtripShould` | End-to-end test: session creation, authentication, and method invocation. |
| `SecureTransportRoundtripShould` | End-to-end test: encrypted transport with body encryption, signing, and nonce verification. |

## Dependencies

### Project References
- `bam.base` -- Core framework utilities
- `bam.encryption` -- Cryptographic types for key generation and verification in tests
- `bam.generators` -- Code generation utilities (for DAO-related test scenarios)
- `bam.test` -- Bam test framework (`UnitTestMenu`, `UnitTest`, `When.A<T>()`, `After.Setup()`, etc.)
- `bam.protocol` -- Core protocol types under test
- `bam.protocol.client` -- Client implementations under test
- `bam.protocol.server` -- Server implementations under test

### Package References
- `NSubstitute` 5.3.0 -- Mocking framework for interface substitution in unit tests

## Usage Examples

### Running all unit tests
```bash
dotnet run --project bam.protocol.tests -- --ut
```

Note: Use `--ut` (double dash), not `/ut`. Git Bash rewrites `/ut` to a filesystem path.

### Running a specific test menu
```bash
dotnet run --project bam.protocol.tests -- --ut --menu "BamClient should"
```

### Test structure pattern
```csharp
using Bam.Test;

[UnitTestMenu("BamClient should", "bcls")]
public class BamClientShould : UnitTestMenuContainer
{
    [UnitTest]
    public void CreateHttpRequest()
    {
        When.A<BamClient>("creates an HTTP request",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateHttpRequest("/api/test"))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .IsOfType<HttpClientRequest>()
                .As<IBamClientRequest>("Path equals expected", r => "/api/test".Equals(r.Path));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
```

### Using TestSetup for database-backed tests
```csharp
using Bam.Protocol.Tests;

string testSessionId = Cuid.Generate();
var keyValues = new Dictionary<string, string>
{
    { "ClientPublicKey", clientPublicKeyPem },
    { "ServerPrivateKey", serverPrivateKeyPem }
};

ServerSessionSchemaRepository repo = TestSetup.CreateTestData(testSessionId, keyValues, "my_test_db");
```

## Known Gaps / Not Yet Implemented

- **`Tests\Integration\` folder is declared but sparse** -- The `.csproj` includes `<Folder Include="Tests\Integration\" />` as a placeholder. The `AuthenticatedRoundtripShould` and `SecureTransportRoundtripShould` integration tests exist but the integration test area is still growing.
- **No test coverage for `BamProxyServer`** -- The proxy server has no dedicated test class.
- **No test coverage for UDP response patterns** -- Tests exist for UDP request sending but not for any response-receiving pattern (consistent with the fire-and-forget design in the client).
