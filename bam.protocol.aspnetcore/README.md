# bam.protocol.aspnetcore

Actor authentication for ASP.NET Core hosts on top of bam.protocol's key-set machinery — the adapter
designed in threeheadz-tracker#30 and delivered by bam.protocol#41. Nothing cryptographic or
identity-defining lives here: tokens are `BamJwtToken`, identity is `PublicKeyFingerprint`, key sets
resolve through `IPublicKeySetResolver` (revoked tombstones never resolve), enrollment is
`bam.useraccounts`' device-key confirmation. This project binds those to middleware, endpoint
filters, and minimal-API endpoints.

## Two factors

**Hybrid mode (default).** A caller enrolls once (register a key set, confirm by signed challenge),
then obtains a server-issued token by POSTing a short-lived token it signed itself to
`/actor/token`. The server token (ES256, signed with the host's ECC key from `INamedKeyStorage`)
carries `kfp` — the canonical fingerprint of the caller's registered ECC key. Every request presents
that token as `Authorization: Bearer ...`; protected endpoints additionally require a request proof
made with the same key (see "Request proof" below). Verification
resolves the caller's ACTIVE key set on every request and requires its fingerprint to equal `kfp`, so
rotating or revoking the set invalidates outstanding tokens with no denylist.

**ClientSignedOnly mode.** For hosts without a server key: the bearer token is the caller's own
signed token (max 5 minutes), verified against the registered key set. No `/actor/token` needed.

**Client token audience.** A client-signed token (the bearer token in `ClientSignedOnly` mode, and the
proof posted to `/actor/token`) carries `aud`, the audience of the host it is meant for. An enabled host must
set `ActorAuth:ClientTokenAudience`, for example to its public origin, and rejects a client-signed token whose
`aud` is missing or different, so a token one host has seen cannot be replayed at another.
`UseActorAuthentication` refuses to start without it; `ActorAuth:AllowMissingClientTokenAudience=true` is the
explicit opt-out, for a single-host test setup only. Only the single-string form of `aud` is accepted.

## Request proof

The proof binds a signature to one request, once, for a few minutes (bam.protocol#46). The client signs this
text, one field per line joined with `\n`, UTF-8 encoded, with its ECC key:

```
BAM-REQUEST-PROOF-1
<METHOD, upper case>
<path, exactly as on the request line>
<query, exactly as on the request line, with its leading ?, or an empty line>
<X-Bam-Timestamp: Unix seconds>
<X-Bam-Proof-Nonce: 16-128 characters of A-Z a-z 0-9 - _>
<lowercase hex SHA-256 of the raw body bytes>
```

The path and query are the request target's bytes as sent, split at the first `?`, with no decoding or
re-escaping: a client in any language signs what it puts on the request line, which must be an origin-form
target (see below). It sends `X-Bam-Body-Signature` (base64; despite the name it signs the whole text above),
`X-Bam-Body-Signature-Algorithm`, `X-Bam-Timestamp` and `X-Bam-Proof-Nonce` (not `X-Bam-Nonce`, which the native
transport uses for its session nonce). For .NET, `Bam.Protocol.RequestProofSigner.Sign(...).ApplyTo(request)` does
all of it; `CanonicalRequest` and the signer live in the core `bam.protocol` assembly, so a client needs no ASP.NET
Core reference. The host rebuilds the text from the request it received, so a proof made for one method, path,
query or body fails on any other; the body is hashed as raw bytes, never decoded. The timestamp must be within `ActorAuth:RequestProofMaxAge` (default five
minutes) of the host's clock, and a nonce is accepted once per key while its proof could still be accepted.
The nonce is recorded only after the signature verifies. The algorithm is pinned: an
`X-Bam-Body-Signature-Algorithm` outside `ActorAuth:AllowedBodySignatureAlgorithms` (default
`SHA256WITHECDSA` only) fails verification. Only an origin-form request target (one that starts with `/` and has no fragment) carries a proof: an absolute-form target is refused, because a server reduces it by its own rules and the signed path could then differ from the routed one. The canonical text has no host line, so `ClientTokenAudience` is what keeps a client-signed token, and the proofs made with it, bound to one host. A proxy that rewrites the path or query before the evaluator runs makes every proof fail closed ("the signature does not verify"); evaluate proofs on the request as the client sent it.

Used nonces live in `IRequestProofNonceStore`. The default, `MemoryRequestProofNonceStore`, holds at most
`ActorAuth:RequestProofNoncesPerKey` unexpired nonces per signing key and `ActorAuth:RequestProofNonceCapacity` in
all, in one process. At either limit it refuses new proofs rather than forget an unexpired nonce, which could
then be replayed; the per-key limit means one busy or hostile key locks out only itself. A capacity refusal is
reported as such ("cannot record another proof for this key right now") and logged at Warning, distinct from a
replay ("the nonce was already used"). A host running more than one instance registers a shared store before
`AddActorAuthentication`, or a proof accepted by one instance can be replayed once at each of the others.

## Host wiring

```csharp
builder.Services.AddActorAuthentication(builder.Configuration);   // binds the ActorAuth section
// prerequisites the host provides: IPublicKeySetResolver, IPublicKeySetRegistrar,
// IAccountConfirmation (AddDeviceKeyAccountConfirmation), INamedKeyStorage (hybrid)

app.UseActorAuthentication();            // no-op unless ActorAuth:Enabled = true
app.MapActorEnrollment();                // /actor/register, /actor/challenge, /actor/confirm (anonymous)
app.MapActorToken();                     // /actor/token (anonymous; hybrid mode)
app.MapPost("/cognition", handler).RequireActorAccess(BamAccess.Execute);   // token + request proof
app.MapGet("/healthz", handler).WithMetadata(new AnonymousAccessAttribute());
```

`RequireActorAccess` records `RequiredAccessAttribute` metadata and attaches the request-proof and
access filters. Endpoints at or above `ActorAuth:ProofRequiredAtOrAbove` (default `Execute`) require
the request proof; `[RequireRequestProof(false)]` waives it, `[RequireRequestProof]` forces it on a
`Read` endpoint. Endpoints marked `AnonymousAccessAttribute` bypass authentication and carry the
well-known anonymous actor. `RequireActorAccess` also marks its endpoint non-anonymous, so mapping it
inside an anonymous route group doesn't make it anonymous.

**Endpoints mapped without `RequireActorAccess` get authentication and admission only.** The middleware
verifies the bearer token and checks admission on every non-anonymous endpoint, but the access-level check
and the request proof run only where `RequireActorAccess` attached the filters. That includes MVC controllers, where endpoint filters don't run.
A host that proxies or uses its own pipeline applies the same rule with `RequestProofRule.RequiresProof`
and the registered `RequestProofEvaluator`, and the access level with `IActorAccessPolicy`.

## Invocation (`/invoke`)

`BamClient<T>.InvokeAsync` posts a `MethodInvocationRequest` (`OperationIdentifier` + named `Arguments`) to
`/invoke` and expects the method's result back as bare JSON. The native `bam.server` pipeline serves that
endpoint with its own session security; this adapter serves it behind actor authentication instead, so one
host can expose typed RPC and plain HTTP routes under the same tokens and proofs.

```csharp
ServiceRegistry dispatch = new ServiceRegistry();              // or a WebServiceRegistry to enforce [WebService]
dispatch.For<LogEventService>().Use<LogEventService>();

builder.Services.AddActorAuthentication(builder.Configuration);
builder.Services.AddBamInvocation(dispatch, new BamInvocationOptions
{
    InvocableTypes = new[] { typeof(LogEventService) }         // the complete allow-list; required
});                                                            // Pattern and MaxBodyBytes have defaults

app.UseActorAuthentication();
app.MapBamInvocation();                                        // POST /invoke, never anonymous
```

The operation identifier is caller-controlled text, so `RegisteredTypeInvocationTargetResolver` resolves it
only against the allow-list the host writes as `BamInvocationOptions.InvocableTypes` (required, each type
validated as registered, frozen at `AddBamInvocation`; same full name, same assembly simple name; version,
culture and token are ignored) and only to a public, non-overloaded, non-generic method that the named type
declares itself. The bridge never derives the list from the registry: a `ServiceRegistry`'s type list is a
construction cache that also holds the implementation behind an interface key and every constructor
dependency it resolved, before or after `AddBamInvocation` (`UseSingleton<T>()` constructs at registration),
so none of those is invocable unless the host lists it. Inherited methods are not reachable through a derived type's name, so `object` members,
property accessors and anything inherited from a base (a logging helper, say) are unreachable; a base type's
own public methods are invocable only under the base type's name, with the base type's own rules, and only
when the base is on the allow-list. `OperationIdentifier.For` names the declaring type, so `BamClient<T>`
already sends the base name for an inherited method. The resolver never loads or scans assemblies. Closed
generic types cannot be named (their full names contain commas) and are not invocable. An identifier that
does not resolve answers `461`, the native `CommandResolutionFailed` code.

For an interface-keyed registration (`For<ILogEventService>().Use<LogEventService>()`) the identifier names
the interface, and the interface's methods and attributes are what apply: `[RequiredAccess]` on the
implementation is not consulted. Put the access attributes on the interface methods, or register the class.
If both the interface and the class are registered, each key carries its own rules and the caller chooses the
key, so keep their attributes consistent: the weaker key is the one that counts. Members an interface inherits
from a base interface are not reachable through the derived interface's name.

Each method's own rules apply, read from `[RequiredAccess]`, `[RequireRequestProof]` and `[AnonymousAccess]`
on the method or the type that declares it (the method wins). Attributes are not inherited, unlike the native
pipeline: an override without its own attribute takes its declaring type's level, and a derived type without
`[RequiredAccess]` is `Denied` whatever its base declares, so attribute the override or the derived type when it
should differ. The required access defaults to `Denied` when neither carries `[RequiredAccess]`, so a dispatchable
service must declare its level. Static methods take no instance, so a `WebServiceRegistry` host's `[WebService]`
check does not run for them (bam.protocol#70); their own attributes are the only gate. The caller's
access comes from `IActorAccessPolicy`; the request proof is demanded by the same threshold rule as
`RequireActorAccess`. Methods marked `[AnonymousAccess]` are refused with `403`: `/invoke` is not an
anonymous surface. `MapBamInvocation` refuses to map when `ActorAuth:Enabled` is false, because without the
middleware no request to it can succeed, and the handler answers `401` before reading any of the body when no
verified actor is present.

| Status | Meaning |
| --- | --- |
| `200` | The result, as bare JSON (`null` for `void`/`Task`). `Task<T>` is awaited and unwrapped. |
| `400` | The body is not an invocation envelope, or an argument cannot be converted to its parameter type (`InvocationArgumentException`, naming the parameter). |
| `401` | No verified actor (`Authentication failed.`), or the request proof is missing or invalid. |
| `403` | The actor's access is below the method's requirement, or the method allows anonymous access. |
| `413` | The body exceeds `BamInvocationOptions.MaxBodyBytes` (default 1 MiB). |
| `461` | The operation does not name a registered public method, or the registry would not supply the instance. |
| `500` | The method threw. The caller sees only `Invocation failed.`; the exception is logged on the host. |

A dispatched service learns who is calling through `IActorContext`, which `AddBamInvocation` registers in the
dispatch registry (unless the host already did). It carries the verified `Actor`, the `KeyFingerprint` of the
actor's ECC key and the `ClientIp`, scoped to the current invocation on the async flow, so a service that
takes it in its constructor reads the identity the middleware verified and never anything the payload claimed.

## Admission: enrolling grants nothing by default

The enrollment endpoints are anonymous and self-service, so enrolling proves only that a caller holds the
keys it registered. `/actor/confirm` proves possession of the RSA key; it does not by itself gate access.
Access comes from admission (`IActorAdmission`), which is checked wherever an actor gains standing:

- the middleware answers the generic 401 to an unadmitted actor on every non-anonymous endpoint, whether or
  not it is mapped with `RequireActorAccess` (MVC controllers included);
- `/actor/token` issues no server token to an unadmitted actor (the same generic 401);
- `ConfiguredActorAccessPolicy` gives `EnrolledActorAccess` only to admitted actors and `Denied` to everyone
  else, for hosts that use the policy without the middleware.

The default `ConfiguredActorAdmission` admits nobody. A host opts in with `ActorAuth:OpenEnrollment = true`
(anyone who can reach `/actor/register` gets in), or lists entries in `ActorAuth:AdmittedHandles`, or
registers its own `IActorAdmission` (an approval store, a directory) before `AddActorAuthentication`.

Each `AdmittedHandles` entry is either `handle` or `handle@fingerprint`. The fingerprint is
`PublicKeyFingerprint.Of` the actor's ECC public key (64 hex characters, compared without regard to case), the same value a server token carries as `kfp` and
`/actor/token` returns as `KeyFingerprint`. Text after the last `@` counts as a pin only when it has that shape, so a handle that itself contains `@` (for example `svc.bot@acme`) is an unpinned entry. An unpinned `handle` entry admits that handle with any key, so
it is satisfied by whoever registers the handle first; pin the key to rule that out. A pinned entry admits
the handle only with that key, so rotating or revoking the key needs the entry updated to the successor
key's fingerprint.

The fingerprint does not depend on how the key is encoded. An EC key written with explicit curve
parameters (as Bam's `EccKeyPair` writes it) and the same key written with its named-curve OID (as openssl
and .NET write it) give one fingerprint, so an operator can compute a pin from the actor's public key PEM
with:

```
openssl ec -pubin -in ecc-public.pem -param_enc named_curve -conv_form uncompressed -outform DER | sha256sum
```

**Upgrading past bam.protocol#47.** Before it, an EC key registered with explicit curve parameters
fingerprinted differently. For such a key, which includes every key Bam's `EccKeyPair` generated, the
fingerprint changes on upgrade. A handle pinned to the old value is refused (401 from `/actor/token` and on
every request) until its `AdmittedHandles` entry is replaced, so compute the new pin from the actor's
registered public key with the openssl command above; `/actor/token`'s `KeyFingerprint` shows the new value
only for a handle admitted without a pin. Expect tokens issued before the upgrade to fail verification until
the actor requests a new one. Keys registered in named-curve form
keep their fingerprint.

Every authentication failure, from the middleware and from `/actor/token`, answers 401 with the same body,
`Authentication failed.`; the verifier's reason is logged. Every refused registration answers the same 400.

## Configuration (`ActorAuth`)

| Key | Default | Meaning |
|---|---|---|
| `Enabled` | `false` | Insert the middleware at all |
| `Mode` | `Hybrid` | `Hybrid` or `ClientSignedOnly` |
| `ServerKeyName` | `actor-auth-server-key` | Named key (ECC key pair PEM bytes) in `INamedKeyStorage` |
| `Issuer` | `bam` | `iss` on server tokens |
| `ServerTokenLifetime` | `00:45:00` | 30–60 min recommended |
| `ClientTokenMaxLifetime` | `00:05:00` | Cap on client-signed token lifetime |
| `ClockSkewAllowance` | `00:01:00` | Tolerance on iat/exp |
| `ProofRequiredAtOrAbove` | `Execute` | Access ladder threshold for the request proof |
| `EnrolledActorAccess` | `Execute` | Access an admitted enrolled actor holds (phase-1 policy) |
| `OpenEnrollment` | `false` | Admit every enrolled actor |
| `AdmittedHandles` | empty | `handle` or `handle@fingerprint` entries admitted when `OpenEnrollment` is off (exact match) |
| `AllowedBodySignatureAlgorithms` | `SHA256WITHECDSA` | Algorithms the request-proof signature may name |
| `RequestProofMaxAge` | `00:05:00` | How far a proof's timestamp may be from the host's clock |
| `RequestProofNonceCapacity` | `200000` | Unexpired nonces `MemoryRequestProofNonceStore` holds in all before refusing new proofs |
| `RequestProofNoncesPerKey` | `10000` | Unexpired nonces it holds for one signing key before refusing that key's new proofs |
| `ClientTokenAudience` | none (required when `Enabled`) | `aud` required in client-signed tokens |
| `AllowMissingClientTokenAudience` | `false` | Explicit opt-out that lets an enabled host start without `ClientTokenAudience` |

`UseActorAuthentication` fails fast, naming what is missing, when a prerequisite contract is not
registered or (hybrid) the server key is absent. Provisioning the server key is the host's job.
`MapActorToken` refuses to map in `ClientSignedOnly` mode, which has no server key.

## Client recipe

1. Generate an RSA pair and an ECC pair; `POST /actor/register` with both public PEMs and your handle.
2. Optionally `POST /actor/challenge`, sign the base64 text of the challenge with the RSA key
   (SHA512WITHRSA), and `POST /actor/confirm` to prove possession of the RSA key. Confirmation does not
   grant access; the host's admission does (see above).
3. Mint a `BamJwtToken` (sub = handle, `aud` = the host's `ClientTokenAudience`, 2-minute expiry) signed
   with the ECC key; `POST /actor/token` → server token (45 min).
4. Call protected endpoints with `Authorization: Bearer <server token>` and a fresh request proof for each
   request (`Bam.Protocol.RequestProofSigner.Sign(...).ApplyTo(request)`, or the four headers described under "Request proof").
5. On 401 "key binding does not match", your key set was rotated or revoked — re-issue.

Deferred by design: encrypted bodies. See the design doc.
