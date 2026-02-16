# bam.protocol.server

Server-side implementation of the Bam Framework protocol, providing HTTP, TCP, and UDP listeners with a full request processing pipeline including session management, authentication, authorization, and command resolution.

## Overview

`bam.protocol.server` implements the Bam protocol server, handling concurrent HTTP, TCP, and UDP requests. The central class `BamServer` listens on all three transports simultaneously (default ports: HTTP 8080, TCP 8413, UDP 8414) and processes each request through a configurable initialization pipeline. The `BamServerBuilder` provides a fluent API for constructing servers with custom ports, IP addresses, event handlers, and service registrations.

The request processing pipeline (`BamRequestPipeline`) is orchestrated by `BamServerContextInitializer`, which runs five ordered initialization stages: session resolution, actor resolution, authentication, command resolution, and authorization. Each stage is handled by a dedicated `IBamServerContextInitializationHandler` and can short-circuit the pipeline if it fails. The pipeline emits events at each stage boundary, enabling fine-grained observability. Successful requests are dispatched to `DefaultBamResponseProvider`, which delegates to `BamRequestProcessor` for method invocation via deserialized `MethodInvocationRequest` payloads.

Security is built-in: `ServerSessionManager` manages server-side session state with ECC key exchange and nonce generation; `BamAuthenticator` validates JWT tokens (ECC-signed, custom `BamJwtToken` format), body signatures, and nonce hashes; `AuthorizationCalculator` checks `[RequiredAccess]` attributes against actor access levels configured through `GroupAccessConfiguration`. The `RequestSecurityValidator` handles body decryption (AES from ECDH-derived keys), signature verification (ECDSA), and nonce hash validation (HMAC-SHA256).

## Request Processing Pipeline

When `BamServer` receives an HTTP, TCP, or UDP request, it flows through a five-stage initialization pipeline before the method is invoked. Each stage can short-circuit the pipeline by setting `CanContinue = false`, which causes the server to return an error response immediately.

```
Request
  |
  v
+-------------------------+
|  1. Session Resolution  |-->  419 Session Init Failed / 420 Session Required
+-------------------------+
|  2. Actor Resolution    |-->  460 Actor Resolution Failed
+-------------------------+
|  3. Authentication      |-->  401 Authentication Failed
|    a. AES body decrypt  |
|    b. ECDSA signature   |
|    c. HMAC nonce hash   |
|    d. JWT verification  |
+-------------------------+
|  4. Command Resolution  |-->  461 Command Resolution Failed
+-------------------------+
|  5. Authorization       |-->  462 Authorization Failed / 403 Denied
+----------+--------------+
           |
           v
+-------------------------+
|  Method Invocation      |-->  200 + JSON result
+-------------------------+
```

### Stage 1: Session Resolution

Checks whether the request carries a `SessionId` header:

- **Has session ID** -- retrieves existing session via `SessionManager.GetSession()`. If the session is not found, sets `InitializationStatus.SessionInitializationFailed` (HTTP 419).
- **No session ID, path is `/bam/session/create`** -- creates a new session via `SessionManager.StartSession()`, which generates a CUID session ID, persists to database, stores the client's ECC public key, generates a server ECC key pair, and returns a `StartSessionResponse` with session ID, nonce, and server public key. The pipeline short-circuits here.
- **No session ID, other path** -- sets `InitializationStatus.SessionRequired` (HTTP 420).

### Stage 2: Actor Resolution

Reads `ClientPublicKey` from server session state, computes its SHA256 hash, looks up the profile via `ProfileManager.FindProfileByPublicKey()`, and returns an `ActorData` with handle and name. If null, HTTP 460.

### Stage 3: Authentication

Performs four checks in order:
1. **AES Body Decryption** -- Derives a shared AES key via ECDH (server private key + client public key) and decrypts the request body in-place.
2. **ECDSA Body Signature** -- Verifies the `X-Bam-Body-Signature` header against the (decrypted) body using the client's public key.
3. **HMAC Nonce Hash** -- Computes HMAC-SHA256 of the body using the nonce as key and compares against the `X-Bam-Nonce-Hash` header.
4. **JWT Verification** -- Decodes the `Authorization: Bearer <token>` JWT, checks expiry, looks up the actor profile, and verifies the ECDSA signature.

### Stage 4: Command Resolution

Parses the request body as `MethodInvocationRequest` to extract the target type, method, and arguments. If parsing fails, HTTP 461.

### Stage 5: Authorization

Checks `[RequiredAccess]` attributes (method-level first, then class-level) against the actor's access level from `AccessLevelProvider`. Access levels are ordered: `Denied < Read < Write`. If access is insufficient, HTTP 462 or 403.

### Response Generation

After the pipeline completes:
- **Pipeline failed** -- maps `InitializationStatus` to an HTTP status code and returns an error response.
- **Pipeline succeeded** -- routes based on `AuthorizationCalculation.Access`: `Read` or `Write` invokes `BamRequestProcessor.ProcessRequestContext()` (deserializes `MethodInvocationRequest`, resolves instance via `ServiceRegistry`, invokes method, returns JSON result with status 200); `Denied` returns HTTP 403.

### Status Code Summary

| Status Code | Meaning | Pipeline Stage |
|---|---|---|
| 200 | Success | Method invoked, result returned |
| 403 | Access Denied | Authorization -- actor lacks required access |
| 419 | Session Init Failed | Session -- ID present but session not found |
| 420 | Session Required | Session -- no session ID on non-creation request |
| 460 | Actor Resolution Failed | Actor -- public key not found or no matching profile |
| 461 | Command Resolution Failed | Command -- could not parse target type/method |
| 462 | Authorization Calculation Failed | Authorization -- calculator returned null |
| 500 | Internal Server Error | Unhandled exception or unknown status |

## Key Classes

| Class / Interface | Description |
|---|---|
| `BamServer` | Main server class: starts HTTP/TCP/UDP listeners, dispatches requests through the pipeline, fires lifecycle events. |
| `BamServerBuilder` | Fluent builder for configuring ports, IP addresses, server name, event handlers, and service registrations. |
| `BamServerOptions` | Configuration object holding all server options including a `ServiceRegistry` pre-wired with default implementations. |
| `HttpServer` | Low-level `HttpListener` wrapper that manages host bindings, thread-based request handling, and listener deconfliction. |
| `BamRequestReader` | Parses raw HTTP/TCP/stream requests into `BamRequest` instances (request line, headers, content). |
| `BamRequestPipeline` | Runs the server initialization pipeline for a request, creating protocol-appropriate initialization contexts. |
| `BamServerContextInitializer` | Orchestrates five pipeline stages with before/after extension points and event emission. |
| `BamServerContext` | Mutable server context carrying request, response, actor, session state, authentication, command, and authorization results. |
| `BamServerContextProvider` | Creates `BamServerContext` instances from HTTP, TCP, or stream inputs. |
| `BamRequestProcessor` | Deserializes `MethodInvocationRequest`, server-initializes it, and invokes the method. |
| `DefaultBamResponseProvider` | Creates responses based on authorization and initialization status with status code mapping. |
| `BamResponseProvider` | Abstract base for response creation with access-level dispatching. |
| `ServerSessionManager` | Manages server-side sessions: creates sessions with ECC key exchange, stores session state, validates session IDs. |
| `ServerSessionState` | Dictionary-backed session state persisted via `ServerSessionSchemaRepository`. |
| `ServerSessionInitializationHandler` | Pipeline handler that resolves or creates session state from request headers. |
| `BamAuthenticator` | Full authentication: body decryption, signature verification, nonce hash validation, JWT verification. |
| `AuthenticationInitializationHandler` | Pipeline handler that runs `BamAuthenticator.Authenticate`. |
| `BamAuthentication` | Authentication result with success flag, actor, request, and messages. |
| `BamJwtToken` | Custom JWT implementation using ES256 (ECDSA with SHA-256) for encode, decode, and verify. |
| `ActorResolver` | Resolves an `IActor` from session state by looking up the client's public key in the profile manager. |
| `CommandResolver` | Extracts an `ICommand` from a `MethodInvocationRequest` in the request body. |
| `AuthorizationCalculator` | Checks `[RequiredAccess]` attributes against actor access levels with type caching. |
| `GroupAccessConfiguration` | Configures group-to-access-level mappings for authorization. |
| `GroupAccessLevelProvider` | Provides access levels based on group membership configuration. |
| `RequestSecurityValidator` | Validates body signatures (ECDSA), nonce hashes (HMAC-SHA256), and decrypts bodies (AES from ECDH). |
| `NonceProvider` | Generates cryptographically secure random nonces (default 32 characters). |
| `CommunicationHandler` | Aggregates all server-side service providers (request reader, session manager, actor resolver, etc.). |
| `BamHostBinding` | Server-specific host binding. |
| `BamProxyServer` | Proxy server for forwarding requests. |
| `InitializationStatus` | Enum tracking pipeline stage outcomes. |

## Dependencies

### Project References
- `bam.protocol` -- Core protocol types (`IBamRequest`, `IBamResponse`, `MethodInvocationRequest`, `ICommand`, etc.)
- `bam.protocol.data` -- Data types (`ServerSession`, `ServerAccountData`, `IActor`, `ProcessDescriptorData`, etc.)
- `bam.protocol.profile` -- Profile management (`IProfileManager`, `IKeyManager`, `EncryptedProfileRepository`, etc.)

### Package References
None (BouncyCastle for JWT signing is provided transitively).

## Usage Examples

### Building and starting a server with defaults
```csharp
using Bam.Protocol.Server;

BamServer server = new BamServer();
await server.StartAsync();

// Server is now listening on HTTP:8080, TCP:8413, UDP:8414
// Press any key to stop...
server.Stop();
```

### Using the fluent builder with custom configuration
```csharp
using Bam.Protocol.Server;

BamServer server = new BamServerBuilder()
    .ServerName("MyAppServer")
    .TcpPort(9000)
    .UdpPort(9001)
    .TcpIPAddress("0.0.0.0")
    .UdpIPAddress("0.0.0.0")
    .OnStarted((sender, args) => Console.WriteLine("Server started!"))
    .OnTcpClientConnected((sender, args) => Console.WriteLine("TCP client connected"))
    .OnCreateContextStarted((sender, args) => Console.WriteLine("Processing request..."))
    .Build();

await server.StartAsync();
```

### Configuring group-based authorization
```csharp
using Bam.Protocol.Server;

BamServerOptions options = new BamServerOptions();
options.ConfigureGroupAccess(config =>
{
    config.SetAccessLevel("admin", BamAccess.Write);
    config.SetAccessLevel("viewer", BamAccess.Read);
});

BamServer server = new BamServer(options);
```

### Server-side session management
```csharp
using Bam.Protocol.Server;

IServerSessionManager sessionManager = serviceRegistry.Get<IServerSessionManager>();

// Start a session (typically triggered by a StartSessionRequest)
StartSessionResponse response = sessionManager.StartSession(startSessionRequest, outputStream);

// Look up a session from a request
IServerSessionState state = sessionManager.GetSession(bamRequest);
string clientPublicKey = state.Get<string>("ClientPublicKey");
```

### Processing a remote method invocation on the server
```csharp
using Bam.Protocol.Server;

IBamRequestProcessor processor = serviceRegistry.Get<IBamRequestProcessor>();

// Deserializes MethodInvocationRequest, resolves instance, invokes method
object result = processor.ProcessRequestContext(serverContext);
```

## Extensibility

- **Before/after handlers** -- `BamServerContextInitializer.AddBeforeInitializationHandler()` and `AddAfterInitializationHandler()` allow inserting custom `IBamServerContextInitializationHandler` steps.
- **Events** -- Each pipeline stage fires Started/Complete event pairs (e.g., `ResolveSessionStateStarted`/`ResolveSessionStateComplete`), plus `InitializationException` on errors.
- **Dependency injection** -- All handlers, resolvers, calculators, and providers are injected via `BamServerOptions.ComponentRegistry`, allowing any component to be replaced.

## Known Gaps / Not Yet Implemented

- **TODO in `BamRequestReader.ReadRequest(Stream)`** -- Contains comment "TODO: ensure other request properties are set". The stream-based request reader sets headers and content but does not populate `QueryString`, `Cookies`, `UserHostAddress`, `UserHostName`, `UserLanguages`, or `RawUrl`.
- **`DefaultBamResponseProvider.LogAccessDenied` is a no-op** -- The method body is a comment with no default logging implementation.
- **UDP response handling is unidirectional** -- UDP requests are received and processed but no response is sent back to the sender; the pipeline fires events and initializes context but does not write a response.
- **`BamProxyServer` is early-stage** -- The proxy server class exists but its integration with the main pipeline is minimal.
