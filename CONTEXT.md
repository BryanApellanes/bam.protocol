# bam.protocol — Context

## Intent

**bam.protocol** is a custom application protocol framework providing a full client-server communication stack over three transports: **HTTP**, **TCP**, and **UDP**. It defines its own wire format (`BAM/2.0` protocol version for TCP/UDP, standard HTTP for web), with a request pipeline modeled after enterprise middleware:

1. **Request reading** — parse raw bytes into `BamRequest` (request line, headers, content body)
2. **Server context creation** — wrap the request with server-side state
3. **Context initialization pipeline** — session resolution → actor (user) resolution → authentication (JWT + body signature + nonce hash + body decryption) → command resolution → authorization calculation
4. **Response generation** — based on initialization outcome (custom status codes like 419=session init failed, 420=session required, 460=actor resolution failed, etc.)
5. **Remote method invocation** — `MethodInvocationRequest` serializes a method call (class + method + args via `OperationIdentifier`) for the client to transmit and the server to invoke reflectively

## Subprojects

| Project | Role | State |
|---|---|---|
| **bam.protocol** | Core types: `BamRequest`, `BamResponse`, `MethodInvocationRequest`, `HostBinding`, HTTP encryption wrappers, interfaces | Fairly complete — types are populated, serialization works |
| **bam.protocol.server** | `BamServer` (3-transport listener), `BamServerBuilder`, context initializer pipeline, session/actor/auth handlers | Implemented — server lifecycle, request processing, session management, response generation all functional |
| **bam.protocol.client** | `BamClient` with HTTP/TCP/UDP request builders, `BamClient<T>` typed RPC client, `ClientSessionManager`, `ClientSessionState`, `ClientRequestSecurityProvider`, `BamInvocationException` | HTTP/TCP response handlers work; session creation returns `IClientSessionState`; outgoing requests get encrypted body + signature + nonce + JWT Authorization headers when session is active; `BamClient<T>.InvokeAsync<TR>()`/`Invoke<TR>()` provides typed RPC invocation |
| **bam.protocol.data** | DAO layer — generated schema repos for sessions, profiles, keys, accounts, common entities (Machine, NIC, Device, Actor, Agent) | Generated code is in place; hand-written models exist |
| **bam.protocol.profile** | Profile/identity management — `ProfileManager`, `CertificateManager`, `CertificateAuthority`, `KeyManager`, `PrivateKeyManager`, `AccountManager`, X.509 name providers | Implemented — profile registration, key management, certificate creation/loading, account registration all functional |
| **bam.protocol.tests** | Tests for server lifecycle, builder, client request creation, request reader, sessions, invocation, profiles, keys, client session state, request security roundtrips, AES key protection, full authenticated HTTP roundtrip, typed RPC invocation via `BamClient<T>` | 94 tests pass; integration tests prove the complete pipeline end-to-end over real HTTP |

## Current State — What Works

- **Builds cleanly** (0 errors)
- `BamServer` starts/stops across all 3 transports, fires lifecycle events
- `BamServerBuilder` fluent API constructs servers with custom options and event handlers
- `BamClient` creates properly typed HTTP/TCP/UDP requests with correct defaults
- `BamRequestReader` parses request lines, headers, and content from streams
- `ServerSessionManager` creates, retrieves, and ends sessions with key pair generation
- `ServerSessionState` loads/saves key-value pairs to the session repo
- `BamServerContextInitializer` runs the full pipeline (session → actor → authentication → command → authorization) with before/after hooks
- `BamRequestProcessor` deserializes `MethodInvocationRequest` JSON and invokes methods reflectively via `ServiceRegistry`
- `DefaultBamResponseProvider` generates denied (403), read (200), and write (200) responses with status code mapping
- `ActorResolver` resolves actor identity from client public key via `IProfileManager` — looks up `ClientPublicKey` from session state, finds matching profile by SHA256 hash, returns `ActorData` with `PersonHandle`/`Name`
- `AuthorizationCalculator` resolves required access from `[RequiredAccess]` attributes on service classes/methods via reflection, compares against actor's access level from `GroupAccessLevelProvider` (which looks up the actor's groups and returns the highest configured access level), and returns an `AuthorizationCalculation` with grant/deny decision
- `ProfileManager` registers person profiles, creates/gets/finds profiles by handle or public key
- `KeyManager` retrieves signing (RSA) and encryption (ECC) keys, derives shared AES keys via ECDH
- `PrivateKeyManager` generates and stores RSA/ECC private keys in opaque storage, retrieves by public key
- `CertificateManager` creates root CA and signed certificates via `CertificateAuthority`, persists to `CertificateData`/`AgentCertificateData`, loads by actor handle
- `CertificateAuthority` issues X.509 certificates using BouncyCastle, supports both RSA and ECC signing
- `AccountManager` registers accounts: delegates to ProfileManager for person/profile creation, persists ServerAccountData with server issuer
- Client-server HTTP roundtrip works (test expects 400 "session required" — which validates the pipeline runs and returns the failure response)
- **Auth pipeline** — `BamAuthenticator` validates JWT tokens (custom `BamJwtToken` using BouncyCastle ECDSA), verifies ECC body signatures (`X-Bam-Body-Signature`), validates nonce hashes (`X-Bam-Nonce-Hash` via HMAC-SHA256), and decrypts AES-encrypted request bodies using ECDH-derived session keys
- `AuthenticationInitializationHandler` integrates authentication into the server context initializer pipeline
- `RequestSecurityValidator` provides body signature validation, nonce hash validation, and body decryption; disposes ECC key pairs and AES keys after use via `using` blocks
- `EccSignatureProvider` implements ECC signing via BouncyCastle
- **Client-side session handling** — `ClientSessionManager.StartSessionAsync()` generates a client ECC key pair, sends the public key to the server, receives server public key + session ID + nonce, and returns `IClientSessionState` holding all session data
- **Client-side request security** — `ClientRequestSecurityProvider` encrypts request bodies (AES), signs them (ECDSA), computes nonce hashes (HMAC-SHA256), and attaches JWT `Authorization: Bearer` header when `AuthorizationToken` is set on the session state; `BamClient.CreateHttpRequestMessage()` applies all session headers (`X-Bam-Session-Id`, `X-Bam-Body-Signature`, `X-Bam-Nonce`, `X-Bam-Nonce-Hash`, `Authorization`) and encrypted body when `SessionState` is set
- **ECDH key agreement** — both client and server derive the same shared AES key via ECDH (client private + server public == server private + client public); verified by roundtrip tests
- **AES key protection in memory** — `ProtectedAesKeyUsageContext` encrypts AES key/IV with an ephemeral key at construction, zeros the originals, and only decrypts within a `UseKey` callback (following the `RsaPrivateKeyUsageContext` pattern); `ClientSessionState.UseSessionKey()` lazily initializes this context for repeated use
- **ECC key disposal** — `EccPublicPrivateKeyPair` implements `IDisposable`, zeroing the PEM `byte[]` and nulling the `AsymmetricCipherKeyPair`
- **AesKey property shadowing fix** — `DisposableAesKey.Key`/`IV` changed from `internal` to `public virtual`, `AesKey.Key`/`IV` changed to `public override`, eliminating separate backing fields so `Dispose()` zeros the correct data
- **Client JWT support** — `IClientSessionState.AuthorizationToken` property allows callers to set a pre-encoded JWT; `ClientRequestSecurityProvider.PrepareHttpRequest()` attaches it as `Authorization: Bearer <token>` header
- **Full authenticated HTTP roundtrip** — integration test (`AuthenticatedRoundtripShould.CompleteAuthenticatedHttpRoundtrip`) proves the entire pipeline works end-to-end: session creation → ECDH key exchange → AES body encryption → ECDSA body signature → HMAC-SHA256 nonce hash → JWT authentication → actor resolution → command resolution → authorization → reflective method invocation → JSON response over real HTTP
- **Typed RPC invocation** — `BamClient<T>.InvokeAsync<TR>(methodName, args)` builds a `MethodInvocationRequest`, sets the `OperationIdentifier`, serializes to JSON, sends to `/invoke` via POST with full session security, and deserializes the typed response; sync `Invoke<TR>()` delegates to the async version; non-200 responses throw `BamInvocationException` with service type, method name, status code, and response content; integration test (`AuthenticatedRoundtripShould.InvokeTypedMethodViaGenericClient`) proves `BamClient<TestEchoService>.InvokeAsync<string>("Echo", "Hello")` returns `"Echo: Hello"` through the full authenticated pipeline
- **94 tests pass** (0 failures)

## What's Stubbed / Remaining Work

### Minimal/placeholder implementations

- **`KeyManager.GenerateRsaKeyPair()`/`GenerateEccKeyPair()`/`GenerateAesKey()`** — return `new` instances (key generation happens in constructors); no persistence of the generated keys

### Interfaces with no implementing class

| Interface | Project | Methods | Notes |
|---|---|---|---|
| `IClientKeySetDataManager` | bam.protocol | `ApplicationNameProvider`, key set CRUD | Client-side key persistence manager |
| `IClientKeySource` | bam.protocol | Extends `IAesKeySource`, `IRsaPublicKeySource` | Client-side key source |
| `IHandleProvider` | bam.protocol.profile | `CreateHandle(IPerson\|IActor\|IDevice\|IKeySet)` | Handle generation is currently done inline |

### Other missing implementations

- **`DeviceRegistrationData.InitializeAsync()`** — abstract with no concrete derived classes; TODO says to use `OSInfo.Current` for platform detection

### TODOs in code

- **`BamRequestReader.ReadRequest(Stream)` line 51** — TODO: "ensure other request properties are set" (only sets RequestLine, Headers, Content)
- **`DeviceRegistrationData.DeviceType`** — TODO: "set this using OSInfo.Current for Windows, Mac and Linux"

### Test gaps

- Client-side session/security tests are unit-level (mock ECDH roundtrips, not real HTTP) — the integration test covers the full roundtrip but additional failure-mode integration tests (expired JWT, wrong key, access denied) could be valuable

## What the Tests Reveal

- The "happy path" for a full request is tested end-to-end via two integration tests: `CompleteAuthenticatedHttpRoundtrip` (manual request building) and `InvokeTypedMethodViaGenericClient` (typed `BamClient<T>` RPC) — both share a common `StartServerWithSession` setup helper
- Session state persistence tests pass (load/save key-values)
- Session manager tests pass (start/end/get sessions)
- Client request builder tests pass (type assertions, defaults)
- Server builder tests pass (port/IP/name configuration)
- `BamRequestReader` stream parsing tests pass
- Certificate authority tests pass (generate certificates from options)
- Key manager tests pass (key generation, signing key retrieval, shared AES derivation)
- Profile manager tests pass (registration, lookup)

## Suggested Next Steps (priority order)

1. **`IClientKeySetDataManager` / `IClientKeySource`** — client-side key persistence so keys survive across process restarts (currently keys live only in memory for the session lifetime).
2. **Failure-mode integration tests** — expired JWT, wrong signing key, access denied, invalid session — to verify the pipeline returns appropriate error responses.
3. **`DeviceRegistrationData` subclasses** — platform-specific device registration using `OSInfo.Current` for Windows/Mac/Linux detection.

## Bottom Line

The framework is **fully implemented and proven end-to-end** — both client and server sides are functional and tested together over real HTTP. The server listens across 3 transports, runs the full initialization pipeline (session → actor → JWT authentication with ECC body signatures, nonce hashing, and AES body decryption → command resolution → authorization), and invokes methods reflectively. The client creates sessions via ECDH key exchange, encrypts and signs outgoing request bodies, attaches all required security headers including JWT Authorization, and receives responses. `BamClient<T>` provides typed RPC invocation — callers write `client.InvokeAsync<string>("Echo", "Hello")` instead of manually building `MethodInvocationRequest` objects. Key material is protected in memory via `ProtectedAesKeyUsageContext` (encrypt-at-rest, decrypt-on-use pattern) and disposed deterministically (`EccPublicPrivateKeyPair`, `AesKey` both implement `IDisposable` with proper zeroing). 94 tests pass, including two integration tests proving the full authenticated HTTP roundtrip. The primary remaining work is **client-side key persistence** and **failure-mode integration tests**.
