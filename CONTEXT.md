# bam.protocol — Context

## Intent

**bam.protocol** is a custom application protocol framework providing a full client-server communication stack over three transports: **HTTP**, **TCP**, and **UDP**. It defines its own wire format (`BAM/2.0` protocol version for TCP/UDP, standard HTTP for web), with a request pipeline modeled after enterprise middleware:

1. **Request reading** — parse raw bytes into `BamRequest` (request line, headers, content body)
2. **Server context creation** — wrap the request with server-side state
3. **Context initialization pipeline** — session resolution → actor (user) resolution → command resolution → authorization calculation
4. **Response generation** — based on initialization outcome (custom status codes like 419=session init failed, 420=session required, 460=actor resolution failed, etc.)
5. **Remote method invocation** — `MethodInvocationRequest` serializes a method call (class + method + args via `OperationIdentifier`) for the client to transmit and the server to invoke reflectively

## Subprojects

| Project | Role | State |
|---|---|---|
| **bam.protocol** | Core types: `BamRequest`, `BamResponse`, `MethodInvocationRequest`, `HostBinding`, HTTP encryption wrappers, interfaces | Fairly complete — types are populated, serialization works |
| **bam.protocol.server** | `BamServer` (3-transport listener), `BamServerBuilder`, context initializer pipeline, session/actor/auth handlers | Implemented — server lifecycle, request processing, session management, response generation all functional |
| **bam.protocol.client** | `BamClient` with HTTP/TCP/UDP request builders | Mostly wired up; HTTP and TCP response handlers exist; `BamClient<T>.Invoke<TR>()` is still a stub |
| **bam.protocol.data** | DAO layer — generated schema repos for sessions, profiles, keys, accounts, common entities (Machine, NIC, Device, Actor, Agent) | Generated code is in place; hand-written models exist; `IAccountManager` has no implementing class |
| **bam.protocol.profile** | Profile/identity management — `ProfileManager`, `CertificateManager`, `CertificateAuthority`, `KeyManager`, `PrivateKeyManager`, X.509 name providers | Implemented — profile registration, key management, certificate creation/loading all functional |
| **bam.protocol.tests** | Tests for server lifecycle, builder, client request creation, request reader, sessions, invocation, profiles, keys | Tests exist and build; integration tests folder is empty |

## Current State — What Works

- **Builds cleanly** (0 errors)
- `BamServer` starts/stops across all 3 transports, fires lifecycle events
- `BamServerBuilder` fluent API constructs servers with custom options and event handlers
- `BamClient` creates properly typed HTTP/TCP/UDP requests with correct defaults
- `BamRequestReader` parses request lines, headers, and content from streams
- `ServerSessionManager` creates, retrieves, and ends sessions with key pair generation
- `ServerSessionState` loads/saves key-value pairs to the session repo
- `BamServerContextInitializer` runs the full pipeline (session → actor → command → auth) with before/after hooks
- `BamRequestProcessor` deserializes `MethodInvocationRequest` JSON and invokes methods reflectively via `ServiceRegistry`
- `DefaultBamResponseProvider` generates denied (403), read (200), and write (200) responses with status code mapping
- `ActorResolver` resolves actor identity from session ID
- `AuthorizationCalculator` returns authorization (currently hardcoded to `BamAccess.Write`)
- `ProfileManager` registers person profiles, creates/gets/finds profiles by handle or public key
- `KeyManager` retrieves signing (RSA) and encryption (ECC) keys, derives shared AES keys via ECDH
- `PrivateKeyManager` generates and stores RSA/ECC private keys in opaque storage, retrieves by public key
- `CertificateManager` creates root CA and signed certificates via `CertificateAuthority`, persists to `CertificateData`/`AgentCertificateData`, loads by actor handle
- `CertificateAuthority` issues X.509 certificates using BouncyCastle, supports both RSA and ECC signing
- Client-server HTTP roundtrip works (test expects 400 "session required" — which validates the pipeline runs and returns the failure response)

## What's Stubbed / Remaining Work

### NotImplementedException stubs

- **`BamClient<T>.Invoke<TR>()`** — generic typed RPC invocation on the client side

### Minimal/placeholder implementations

- **`ActorResolver.ResolveActor()`** — maps session ID to actor Handle/Name; no real identity lookup against profiles or accounts
- **`AuthorizationCalculator.CalculateAuthorization()`** — hardcodes `BamAccess.Write` for all requests; no real permission model
- **`KeyManager.GenerateRsaKeyPair()`/`GenerateEccKeyPair()`/`GenerateAesKey()`** — return `new` instances (key generation happens in constructors); no persistence of the generated keys

### Missing implementations

- **`IAccountManager`** — interface defined (`RegisterAccount(PersonRegistrationData)`) but no implementing class exists
- **`DeviceRegistrationData.InitializeAsync()`** — abstract with no concrete derived classes; TODO says to use `OSInfo.Current` for platform detection

### TODOs in code

- **`BamRequestReader.ReadRequest(Stream)` line 51** — TODO: "ensure other request properties are set" (only sets RequestLine, Headers, Content)
- **`DeviceRegistrationData.DeviceType`** — TODO: "set this using OSInfo.Current for Windows, Mac and Linux"

### Test gaps

- Integration tests folder exists but is empty — no end-to-end test exercises a full successful request roundtrip (session start → authenticated request → method invocation → response)

## What the Tests Reveal

- The "happy path" for a full request currently stops at initialization failure (session required → 400/420) — no test exercises successful request *processing*
- Session state persistence tests pass (load/save key-values)
- Session manager tests pass (start/end/get sessions)
- Client request builder tests pass (type assertions, defaults)
- Server builder tests pass (port/IP/name configuration)
- `BamRequestReader` stream parsing tests pass
- Certificate authority tests pass (generate certificates from options)
- Key manager tests pass (key generation, signing key retrieval, shared AES derivation)
- Profile manager tests pass (registration, lookup)

## Bottom Line

The framework is **substantially implemented** — the server listens across 3 transports, the full initialization pipeline runs, sessions are managed end-to-end, requests are processed via reflective method invocation, profiles and keys are persisted, and certificates can be created and loaded. The remaining gaps are: a real authorization model (currently everything gets Write access), real actor resolution (currently uses session ID as identity), the `IAccountManager` implementation, the generic `BamClient<T>.Invoke<TR>()` RPC method, and integration tests proving a full authenticated roundtrip.
