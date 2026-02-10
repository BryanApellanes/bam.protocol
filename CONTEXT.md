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
| **bam.protocol.server** | `BamServer` (3-transport listener), `BamServerBuilder`, context initializer pipeline, session/actor/auth handlers | Structurally complete but several key implementations are stubs |
| **bam.protocol.client** | `BamClient` with HTTP/TCP/UDP request builders | Mostly wired up; HTTP and TCP response handlers exist, UDP send not fully implemented |
| **bam.protocol.data** | DAO layer — generated schema repos for sessions, profiles, keys, accounts, common entities (Machine, NIC, Device, Actor, Agent) | Generated code is in place; hand-written models exist |
| **bam.protocol.profile** | Profile/identity management — `ProfileManager`, `CertificateManager`, `CertificateAuthority`, `KeyManager`, X.509 name providers | Almost entirely `NotImplementedException` stubs |
| **bam.protocol.tests** | Tests for server lifecycle, builder, client request creation, request reader, sessions, invocation | Tests exist and build, but integration tests (StartSession, Receive400) exercise limited paths |

## Current State — What Works

- **Builds cleanly** (0 errors, 0 warnings)
- `BamServer` starts/stops across all 3 transports, fires lifecycle events
- `BamServerBuilder` fluent API constructs servers with custom options and event handlers
- `BamClient` creates properly typed HTTP/TCP/UDP requests with correct defaults
- `BamRequestReader` parses request lines, headers, and content from streams
- `ServerSessionState` loads/saves key-value pairs to the session repo
- `BamServerContextInitializer` runs the full pipeline (session → actor → command → auth) with before/after hooks
- Client-server HTTP roundtrip works (test expects 400 "session required" — which validates the pipeline runs and returns the failure response)

## What's Stubbed / NotImplemented

These are `throw new NotImplementedException()`:

- **`BamRequestProcessor.ProcessRequestContext()`** — the actual request processing after initialization succeeds
- **`ActorResolver.ResolveActor()`** — no user/identity resolution
- **`ServerSessionManager.StartSession()`** — can't create new sessions (commented-out key storage)
- **`ServerSessionManager.EndSession()`** — can't end sessions
- **`DefaultBamResponseProvider.CreateDeniedResponse()`** — no denial response
- **`DefaultBamResponseProvider.CreateReadResponse()`** — no read response
- **`DefaultBamResponseProvider.CreateWriteResponse()`** — no write response
- **`DefaultBamResponseProvider.LogAccessDenied()`** — no denied logging
- **`ProfileManager`** — all 6 methods are stubs (register, get, create, find by handle, find by public key)
- **`BamRequestReader.ReadRequest(Stream)` line 52** — TODO: "ensure other request properties are set"

## What the Tests Reveal

- The "happy path" for a full request currently stops at initialization failure (session required → 400/420) — no test exercises successful request *processing*
- Session state persistence tests pass (load/save key-values)
- Client request builder tests pass (type assertions, defaults)
- Server builder tests pass (port/IP/name configuration)
- The `BamRequestReader` stream parsing tests pass

## Bottom Line

The framework's **skeleton is complete** — the server listens, the pipeline runs, the client connects, sessions persist. But the **actual business logic** (processing requests, resolving actors, managing sessions end-to-end, generating success responses, profile/identity management) is all stubbed out. The next work would be implementing these stubs to get a full request roundtrip through the pipeline.
