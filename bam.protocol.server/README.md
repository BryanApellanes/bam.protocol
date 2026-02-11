# bam.protocol.server — Request Processing Pipeline

## Overview

When `BamServer` receives an HTTP, TCP, or UDP request, it flows through a five-stage initialization pipeline before the method is invoked. Each stage can short-circuit the pipeline by setting `CanContinue = false`, which causes the server to return an error response immediately.

```
Request
  │
  ▼
┌─────────────────────────┐
│  1. Session Resolution  │──▶ 419 Session Init Failed / 420 Session Required
├─────────────────────────┤
│  2. Actor Resolution    │──▶ 460 Actor Resolution Failed
├─────────────────────────┤
│  3. Authentication      │──▶ 401 Authentication Failed
│    a. AES body decrypt  │
│    b. ECDSA signature   │
│    c. HMAC nonce hash   │
│    d. JWT verification  │
├─────────────────────────┤
│  4. Command Resolution  │──▶ 461 Command Resolution Failed
├─────────────────────────┤
│  5. Authorization       │──▶ 462 Authorization Failed / 403 Denied
└─────────────┬───────────┘
              │
              ▼
┌─────────────────────────┐
│  Method Invocation      │──▶ 200 + JSON result
└─────────────────────────┘
```

## Entry Point

`BamServer.HandleHttpRequests()` (`BamServer.cs:223`) is the HTTP entry point. It:

1. Generates a unique request ID (`BamServer.cs:227`)
2. Creates an `IBamServerContext` via `ServerContextProvider.CreateServerContext()` (`BamServer.cs:232`)
3. Passes it to `InitializeServerContext()` (`BamServer.cs:239`), which creates a `BamServerInitializationContext` and delegates to `BamServerContextInitializer.InitializeServerContext()` (`BamServer.cs:353-362`)
4. Gets a response — either one set during initialization (e.g., session creation) or one created by `ResponseProvider.CreateResponse()` (`BamServer.cs:242-243`)
5. Sends the response (`BamServer.cs:244`)

TCP and UDP follow the same pipeline via `HandleTcpRequest()` (`BamServer.cs:311`) and `HandleUdpRequests()` (`BamServer.cs:264`).

## Pipeline Orchestration

`BamServerContextInitializer.InitializeServerContext()` (`BamServerContextInitializer.cs:44-93`) runs the five stages in order. Between each stage it checks `initialization.CanContinue` and returns early if any stage fails. Events are fired before and after each stage.

```
OnBeforeInitialization()      ← custom handlers (line 51)
InitializeSession()           ← line 53
InitializeActor()             ← line 59
InitializeAuthentication()    ← line 65
InitializeCommand()           ← line 71
InitializeAuthorization()     ← line 77
OnAfterInitialization()       ← custom handlers (line 85)
```

## Stage 1: Session Resolution

**Handler:** `ServerSessionInitializationHandler` (`ServerSessionInitializationHandler.cs:14-45`)

Checks whether the request carries a `SessionId` header (`line 18`):

- **Has session ID** — retrieves existing session via `SessionManager.GetSession()` (`line 20`). If the session is not found, sets `InitializationStatus.SessionInitializationFailed` (`line 24`) → HTTP 419.
- **No session ID, path is `/bam/session/create`** — creates a new session via `HandleSessionCreation()` (`line 34`). This calls `SessionManager.StartSession()` (`line 59`), which:
  - Generates a `Cuid` session ID (`ServerSessionManager.cs:33`)
  - Persists the session to the database (`ServerSessionManager.cs:36`)
  - Stores the client's public key in session state (`ServerSessionManager.cs:39`)
  - Generates a server ECC key pair and stores the private key (`ServerSessionManager.cs:41-42`)
  - Returns a `StartSessionResponse` with session ID, nonce, and server public key (`ServerSessionManager.cs:46-47`)

  The response is set directly on the context (`ServerSessionInitializationHandler.cs:65`) and `CanContinue` is set to `false` (`line 66`) — the pipeline ends here for session creation requests.
- **No session ID, other path** — sets `InitializationStatus.SessionRequired` (`line 39`) → HTTP 420.

## Stage 2: Actor Resolution

**Handler:** `ActorResolverInitializationHandler` (`ActorResolverInitializationHandler.cs:13-25`)

Delegates to `ActorResolver.ResolveActor()` (`line 16`), which:

1. Reads `ClientPublicKey` from server session state (`ActorResolver.cs:18`)
2. Computes its SHA256 hash and looks up the profile via `ProfileManager.FindProfileByPublicKey()` (`ActorResolver.cs:24`)
3. Returns an `ActorData` with `Handle` and `Name` from the profile (`ActorResolver.cs:30`)

If the actor is `null`, sets `InitializationStatus.ActorResolutionFailed` (`ActorResolverInitializationHandler.cs:20`) → HTTP 460.

## Stage 3: Authentication

**Handler:** `AuthenticationInitializationHandler` (`AuthenticationInitializationHandler.cs:12-24`)

Delegates to `BamAuthenticator.Authenticate()` (`line 15`), which performs four checks in order (`BamAuthenticator.cs:19-109`):

### 3a. AES Body Decryption (`BamAuthenticator.cs:26-39`)

If the session has both `ServerPrivateKey` and `ClientPublicKey`, calls `RequestSecurityValidator.DecryptBody()` (`line 32`). This derives a shared AES key via ECDH (`RequestSecurityValidator.cs:92-104`) — combining the server's private key with the client's public key — and decrypts the request body (`RequestSecurityValidator.cs:84`). The decrypted plaintext **replaces** the request content (`BamAuthenticator.cs:35`), so all subsequent checks operate on the plaintext.

### 3b. ECDSA Body Signature (`BamAuthenticator.cs:42-49`)

If the `X-Bam-Body-Signature` header is present, calls `RequestSecurityValidator.ValidateBodySignature()` (`line 44`). This retrieves the client's public key from session state (`RequestSecurityValidator.cs:22`), uses BouncyCastle to verify the ECDSA signature against the (now-decrypted) body (`RequestSecurityValidator.cs:35-38`). Failure returns immediately with `Success = false`.

### 3c. HMAC Nonce Hash (`BamAuthenticator.cs:52-59`)

If the `X-Bam-Nonce-Hash` header is present, calls `RequestSecurityValidator.ValidateNonceHash()` (`line 54`). This computes HMAC-SHA256 of the body using the nonce as the key (`RequestSecurityValidator.cs:63`) and compares against the header value (`RequestSecurityValidator.cs:66`). Failure returns immediately.

### 3d. JWT Verification (`BamAuthenticator.cs:62-108`)

Requires an `Authorization: Bearer <token>` header (`line 62`). Decodes the JWT (`line 77`), checks expiry (`line 84`), looks up the actor's profile by handle (`line 90`), retrieves the client's public key from session state (`line 97`), and verifies the JWT's ECDSA signature (`line 103`). Any failure returns `Success = false` with a specific message.

If authentication fails, the handler sets `InitializationStatus.AuthenticationFailed` (`AuthenticationInitializationHandler.cs:19`).

## Stage 4: Command Resolution

**Handler:** `CommandInitializationHandler` (`CommandInitializationHandler.cs:10-22`)

Delegates to `CommandResolver.ResolveCommand()` (`line 13`), which parses the request to determine the target type, method, and arguments. If the command is `null`, sets `InitializationStatus.CommandResolutionFailed` (`line 17`) → HTTP 461.

## Stage 5: Authorization

**Handler:** `AuthorizationCalculatorInitializationHandler` (`AuthorizationCalculatorInitializationHandler.cs:10-22`)

Delegates to `AuthorizationCalculator.CalculateAuthorization()` (`line 13`), which:

1. Gets the required access level from the command's target type/method via `[RequiredAccess]` attribute — checks the method first, then the class (`AuthorizationCalculator.cs:36-61`). Uses a `ConcurrentDictionary` type cache for performance (`line 8`).
2. Gets the actor's access level from `AccessLevelProvider.GetAccessLevel()` (`AuthorizationCalculator.cs:26`)
3. Compares: if `actorAccess >= requiredAccess`, authorization succeeds (`line 28-30`). Access levels are ordered: `Denied < Read < Write`.

If authorization is `null`, the handler sets `InitializationStatus.AuthorizationCalculationFailed` (`AuthorizationCalculatorInitializationHandler.cs:17`) → HTTP 462.

## Response Generation

After the pipeline completes, `BamResponseProvider` routes the response (`BamResponseProvider.cs:22-30`, `33-48`):

- **Pipeline failed** (`Status != Success`) — `DefaultBamResponseProvider.CreateFailureResponse()` (`DefaultBamResponseProvider.cs:26-43`) maps the `InitializationStatus` to an HTTP status code (`line 70-91`) and returns an error response.
- **Pipeline succeeded** — routes based on `AuthorizationCalculation.Access` (`BamResponseProvider.cs:35-47`):
  - `BamAccess.Read` → `CreateReadResponse()` (`DefaultBamResponseProvider.cs:51-55`)
  - `BamAccess.Write` → `CreateWriteResponse()` (`DefaultBamResponseProvider.cs:45-49`)
  - `BamAccess.Denied` → `CreateDeniedResponse()` → HTTP 403 (`DefaultBamResponseProvider.cs:57-63`)

Both `CreateReadResponse` and `CreateWriteResponse` invoke `BamRequestProcessor.ProcessRequestContext()` (`DefaultBamResponseProvider.cs:47,53`), which:

1. Deserializes the request body as `MethodInvocationRequest` (`BamRequestProcessor.cs:19`)
2. Calls `ServerInitialize()` to resolve the target method and instance via `ServiceRegistry` (`line 20`)
3. Invokes the method reflectively and returns the result (`line 21`)

The result is wrapped in `BamResponse<object>` with status 200 and serialized as JSON.

## Status Code Summary

| Status Code | Meaning | Pipeline Stage |
|---|---|---|
| 200 | Success | Method invoked, result returned |
| 403 | Access Denied | Authorization — actor lacks required access |
| 419 | Session Init Failed | Session — ID present but session not found |
| 420 | Session Required | Session — no session ID on non-creation request |
| 460 | Actor Resolution Failed | Actor — public key not found or no matching profile |
| 461 | Command Resolution Failed | Command — could not parse target type/method |
| 462 | Authorization Calculation Failed | Authorization — calculator returned null |
| 500 | Internal Server Error | Unhandled exception or unknown status |

## Extensibility

- **Before/after handlers** — `BamServerContextInitializer.AddBeforeInitializationHandler()` (`BamServerContextInitializer.cs:140`) and `AddAfterInitializationHandler()` (`line 146`) allow inserting custom `IBamServerContextInitializationHandler` steps.
- **Events** — each pipeline stage fires Started/Complete event pairs (e.g., `ResolveSessionStateStarted`/`ResolveSessionStateComplete`), plus `InitializationException` on errors.
- **Dependency injection** — all handlers, resolvers, calculators, and providers are injected via `BamServerOptions.ComponentRegistry`, allowing any component to be replaced.
