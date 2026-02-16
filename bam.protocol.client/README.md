# bam.protocol.client

Client-side implementation of the Bam Framework protocol for HTTP, TCP, and UDP communication with Bam servers.

## Overview

`bam.protocol.client` provides the concrete client implementations that communicate with Bam protocol servers. The central class, `BamClient`, supports three transport protocols -- HTTP, TCP, and UDP -- each with its own request builder and request type. Clients use a fluent builder pattern (`BamClientRequestBuilder`) to construct requests and then send them via `ReceiveResponseAsync`, which dispatches to the appropriate transport handler based on request type.

The project includes a typed variant `BamClient<T>` that adds RPC-style method invocation over HTTP, serializing `MethodInvocationRequest` payloads and deserializing typed responses. `BamServiceClient` extends `BamClient` with session establishment capabilities, creating ECC key pairs and exchanging public keys with the server via `ClientSessionManager`.

Security is handled by `ClientSessionState`, which manages the ECDH-derived AES session key with memory protection via `ProtectedAesKeyUsageContext`, and `ClientRequestSecurityProvider`, which encrypts request bodies, signs them with ECC, computes HMAC nonce hashes, and attaches JWT authorization tokens to outgoing requests.

## Key Classes

| Class / Interface | Description |
|---|---|
| `BamClient` | Core client supporting HTTP, TCP, and UDP transports with pluggable request builders and response handlers. |
| `BamClient<T>` | Generic typed client that adds `Invoke<TR>` / `InvokeAsync<TR>` for RPC-style method invocation over HTTP. |
| `BamServiceClient` | Extends `BamClient` with `StartSession` to establish authenticated sessions with a server. |
| `IBamServiceClient` | Interface for service clients that can start sessions. |
| `BamClientRequestBuilder` | Abstract fluent builder for constructing `IBamClientRequest` instances (base address, path, query string, method, content). |
| `HttpBamClientRequestBuilder` | Builds `HttpClientRequest` instances targeting the HTTP port (default 8080). |
| `TcpBamClientRequestBuilder` | Builds `TcpClientRequest` instances targeting the TCP port (default 8413) with POST method. |
| `UdpBamClientRequestBuilder` | Builds `UdpClientRequest` instances targeting the UDP port (default 8414). |
| `HttpClientRequest` | HTTP-specific client request implementation. |
| `TcpClientRequest` | TCP-specific client request using `BAM/2.0` protocol format. |
| `UdpClientRequest` | UDP-specific client request using `BAM/2.0` protocol format. |
| `BamClientResponse` | Response wrapper that handles both `HttpResponseMessage` responses and raw `BAM/2.0` text responses. |
| `BamClientResponse<T>` | Typed response variant for deserialized content. |
| `ClientKeySet` | Client-side key set with RSA/ECC key pairs, asymmetric/symmetric encryption, and AES key derivation from ECDH. |
| `ClientSessionManager` | Manages session lifecycle: sends `StartSessionRequest` to server, parses `StartSessionResponse`, creates `ClientSessionState`. |
| `ClientSessionState` | Disposable session state holding session ID, nonce, server public key, and client key pair with protected AES key access. |
| `ClientRequestSecurityProvider` | Encrypts request bodies (AES), signs them (ECC), computes HMAC nonce hashes, and attaches Bearer JWT tokens. |
| `BamInvocationException` | Exception thrown when a remote method invocation returns a non-200 status code. |

## Dependencies

### Project References
- `bam.protocol` -- Core protocol interfaces and types (`IBamClient`, `IBamClientRequest`, `HostBinding`, etc.)
- `bam.protocol.data` -- Data types for client sessions and key sets (`ClientKeySetData`, `ProcessDescriptorData`)
- `bam.protocol.server` -- Server types needed for `BamHostBinding`, `BamServer` port constants

### Package References
None.

## Usage Examples

### Creating a client and making an HTTP request
```csharp
using Bam.Protocol.Client;
using Bam.Data.Objects;

// Create a client with default addresses (HTTP:8080, TCP:8413, UDP:8414)
BamClient client = new BamClient(new JsonObjectDataEncoder());

// Build and send an HTTP GET request
IBamClientRequest request = client.CreateHttpRequest("/api/status");
IBamClientResponse response = await client.ReceiveResponseAsync(request);
Console.WriteLine($"Status: {response.StatusCode}, Body: {response.Content}");
```

### Using the typed client for RPC invocation
```csharp
using Bam.Protocol.Client;
using Bam.Server;

// Create a typed client targeting a specific service
BamClient<OrderService> client = new BamClient<OrderService>(new HostBinding("myserver.local", 8080));

// Invoke a remote method and get a typed result
OrderResult result = await client.InvokeAsync<OrderResult>("ProcessOrder", orderId, quantity);
```

### Establishing a secure session
```csharp
using Bam.Protocol.Client;

BamServiceClient client = new BamServiceClient(new JsonObjectDataEncoder(), new HostBinding("myserver.local", 8080));

// Start a session (exchanges ECC public keys, receives nonce + session ID)
StartSessionResponse session = client.StartSession("myserver.local", 8080);

// Or use the session manager directly for full state management
ClientSessionManager sessionManager = new ClientSessionManager(httpClient, hostBinding);
IClientSessionState state = await sessionManager.StartSessionAsync();

// All subsequent requests will be encrypted with the derived session key
client.SessionState = state;
IBamClientRequest request = client.CreateHttpRequest("/api/secure-endpoint");
IBamClientResponse response = await client.ReceiveResponseAsync(request);
```

### Building requests with the fluent API
```csharp
using Bam.Protocol.Client;

BamClient client = new BamClient(new JsonObjectDataEncoder());

IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Http)
    .BaseAddress(new HostBinding("api.example.com", 443))
    .Path("/v2/orders")
    .HttpMethod(HttpMethods.POST)
    .Content(new { OrderId = "12345", Quantity = 5 })
    .Build();
```

## Known Gaps / Not Yet Implemented

- **TODO in `BamClient.ReceiveResponseAsync`** -- Contains a comment referencing legacy streaming client/server implementations from `Bam.Net` for improved request type dispatch. The current implementation dispatches by concrete request type, which works but could be more extensible.
- **`ClientKeySetDataManager.cs` is excluded from compilation** -- The `.csproj` explicitly removes this file via `<Compile Remove="ClientKeySetDataManager.cs">`, suggesting it is a planned but incomplete feature for managing persisted client key sets.
- **UDP responses are fire-and-forget** -- `SendUdpAsync` returns a hardcoded `"UDP_SENT"` response string with no actual response reception, by design for broadcast/event scenarios but limiting for request/response patterns.
