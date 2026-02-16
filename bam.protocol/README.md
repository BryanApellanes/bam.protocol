# bam.protocol

Core protocol abstractions and primitives for the Bam Framework communication layer.

## Overview

`bam.protocol` defines the foundational types, interfaces, and data structures that underpin the Bam Framework's custom communication protocol. It provides the contract layer for requests, responses, encryption, method invocation, and session management that both clients and servers implement against.

The library includes a complete HTTP request/response abstraction (`IHttpRequest`, `IBamRequest`, `IBamResponse`) alongside an encrypted variant (`EncryptedHttpRequest`) that transparently handles content and header encryption using pluggable `IEncryptor`/`IDecryptor` implementations. A key feature is the `MethodInvocationRequest` system, which allows clients to serialize method calls -- including instance context -- and transmit them to a server for remote execution, supporting both JSON and YAML serialization formats.

The project also defines cryptographic primitives such as `IKeySet` (RSA + ECC public keys), `IClientKeySet` (client-side asymmetric/symmetric encryption operations), `ISecretExchange` (legacy AES-based secret sharing, now deprecated in favor of ECC), and session management interfaces (`IClientSessionManager`, `IClientSessionState`) that enable secure session establishment with ECDH-derived AES keys.

## Key Classes

| Class / Interface | Description |
|---|---|
| `IBamRequest` | Server-side request abstraction with HTTP semantics (headers, cookies, URL, content). |
| `IBamResponse` | Server-side response abstraction with status code, headers, output stream, and send semantics. |
| `IHttpRequest` | Client-side HTTP request abstraction with URI, verb, content, and conversion to `HttpRequestMessage`. |
| `HttpRequest` | Default implementation of `IHttpRequest` with bidirectional conversion to/from `HttpRequestMessage`. |
| `BamRequest` | Concrete `IBamRequest` implementation parsed from a `BamRequestLine`. |
| `BamResponse` | Abstract `IBamResponse` base class providing header/cookie management and redirect support. |
| `BamRequestLine` | Parses raw request lines (e.g., `GET /path BAM/2.0`) into method, URI, and protocol version. |
| `EncryptedHttpRequest` | HTTP request whose content is stored as an encrypted `Cipher`; prevents direct content assignment. |
| `HttpRequestEncryptor` | Encrypts an `IHttpRequest` into an `EncryptedHttpRequest` (content + headers). |
| `HttpRequestDecryptor` | Decrypts an `IEncryptedHttpRequest` back into a plain `IHttpRequest`. |
| `MethodInvocationRequest` | Serializable RPC request that captures a method, its arguments, and optional instance context for remote invocation. |
| `OperationIdentifier` | Encodes a `MethodInfo` as a portable string (`Type+Method, Assembly`) and resolves it back. |
| `InvocationContextSerializer` | Pluggable serializer (JSON/YAML) for method invocation instance context. |
| `IKeySet` | Public key set: `KeySetHandle`, `PublicRsaKey`, `PublicEccKey`. |
| `IClientKeySet` | Client key set with asymmetric/symmetric encrypt/decrypt operations plus server and client key references. |
| `IClientSessionManager` | Starts sessions asynchronously; returns `StartSessionResponse` or `IClientSessionState`. |
| `IClientSessionState` | Disposable session state holding session ID, nonce, server public key, and ECDH-derived AES key access. |
| `StartSessionRequest` | Session initiation request carrying the client's ECC public key. |
| `StartSessionResponse` | Session initiation response carrying session ID, nonce, and server's ECC public key. |
| `ICommand` | Command abstraction: `TypeName`, `MethodName`, `Arguments`. |
| `Command` | Simple POCO implementation of `ICommand`. |
| `BamAccess` | Enum for access levels: `Denied`, `Read`, `Write`. |
| `RequiredAccessAttribute` | Attribute for decorating classes/methods with minimum required `BamAccess` level. |
| `HostBinding` | Represents a host:port binding with SSL toggle and configuration loading. |
| `DefaultPorts` | Constants: HTTP 8080, TCP 8413, UDP 8414. |
| `BamClientProtocols` | Enum: `Http`, `Tcp`, `Udp`. |
| `SecretExchange` | Legacy secret exchange via AES-encrypted cipher (deprecated). |
| `ISecretExchange` | Deprecated interface for AES-based secret sharing between server and client. |

## Dependencies

### Project References
- `bam.base` -- Core framework utilities, extension methods, logging
- `bam.configuration` -- Configuration management (`IConfigurable`, `Config`)
- `bam.data.objects` -- Object data encoding/decoding (`IObjectEncoderDecoder`)
- `bam.data` -- Data framework primitives
- `bam.encryption` -- Cryptographic primitives (`IEncryptor`, `IDecryptor`, `AesKey`, `EccPublicPrivateKeyPair`, etc.)
- `bam.storage.encryption` -- Encrypted storage abstractions

### Package References
None (relies entirely on project references and .NET 10 SDK).

## Usage Examples

### Creating and encrypting an HTTP request
```csharp
using Bam.Encryption;
using Bam.Protocol;

// Create a plain request
HttpRequest request = new HttpRequest
{
    Uri = new Uri("https://example.com/api/data"),
    Verb = HttpVerbs.Post,
    Content = "{\"key\": \"value\"}"
};

// Encrypt using an AES encryptor
IEncryptor encryptor = new AesEncryptor(aesKey);
HttpRequestEncryptor requestEncryptor = new HttpRequestEncryptor(encryptor);
EncryptedHttpRequest encrypted = requestEncryptor.EncryptRequest(request);
```

### Building a method invocation request (client-side RPC)
```csharp
using Bam.Protocol;

// Create invocation for a remote method
MethodInvocationRequest invocation = MethodInvocationRequest.For<MyService>("ProcessOrder", orderId, quantity);

// Initialize for client transmission (captures serialized context)
invocation.ClientInitialize(serviceRegistry);

// Serialize to JSON for transport
string json = invocation.ToJson();
```

### Using operation identifiers
```csharp
using Bam.Protocol;

// Encode a method as a portable identifier
string opId = OperationIdentifier.For<MyService>("ProcessOrder");
// Result: "MyNamespace.MyService+ProcessOrder, MyAssembly, Version=..."

// Resolve back to MethodInfo on the server
MethodInfo method = OperationIdentifier.ToMethod(opId);
```

### Checking access requirements
```csharp
using Bam.Protocol;

[RequiredAccess(BamAccess.Write)]
public class OrderService
{
    [RequiredAccess(BamAccess.Read)]
    public Order GetOrder(string id) { ... }

    public void PlaceOrder(Order order) { ... } // Inherits class-level Write access
}
```

## Known Gaps / Not Yet Implemented

- **`ISecretExchange` is deprecated** -- Marked `[Obsolete("Use Ecc keys to exchange shared secrets")]`. The `SecretExchange` class still exists but should not be used in new code; ECC key exchange via `IClientSessionState.DeriveSessionAesKey()` is the replacement.
- No `NotImplementedException` stubs or empty method bodies were found in this project.
