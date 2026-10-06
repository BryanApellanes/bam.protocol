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
that token as `Authorization: Bearer ...`; protected endpoints additionally require
`X-Bam-Body-Signature` (SHA256WITHECDSA over the raw body, base64) made with the same key. The algorithm
is pinned: an `X-Bam-Body-Signature-Algorithm` outside `ActorAuth:AllowedBodySignatureAlgorithms` (default
`SHA256WITHECDSA` only) fails verification. Verification
resolves the caller's ACTIVE key set on every request and requires its fingerprint to equal `kfp`, so
rotating or revoking the set invalidates outstanding tokens with no denylist.

**ClientSignedOnly mode.** For hosts without a server key: the bearer token is the caller's own
signed token (max 5 minutes), verified against the registered key set. No `/actor/token` needed.

## Host wiring

```csharp
builder.Services.AddActorAuthentication(builder.Configuration);   // binds the ActorAuth section
// prerequisites the host provides: IPublicKeySetResolver, IPublicKeySetRegistrar,
// IAccountConfirmation (AddDeviceKeyAccountConfirmation), INamedKeyStorage (hybrid)

app.UseActorAuthentication();            // no-op unless ActorAuth:Enabled = true
app.MapActorEnrollment();                // /actor/register, /actor/challenge, /actor/confirm (anonymous)
app.MapActorToken();                     // /actor/token (anonymous; hybrid mode)
app.MapPost("/cognition", handler).RequireActorAccess(BamAccess.Execute);   // token + body signature
app.MapGet("/healthz", handler).WithMetadata(new AnonymousAccessAttribute());
```

`RequireActorAccess` records `RequiredAccessAttribute` metadata and attaches the request-proof and
access filters. Endpoints at or above `ActorAuth:ProofRequiredAtOrAbove` (default `Execute`) require
the body signature; `[RequireRequestProof(false)]` waives it, `[RequireRequestProof]` forces it on a
`Read` endpoint. Endpoints marked `AnonymousAccessAttribute` bypass authentication and carry the
well-known anonymous actor. `RequireActorAccess` also marks its endpoint non-anonymous, so mapping it
inside an anonymous route group doesn't make it anonymous.

**Endpoints mapped without `RequireActorAccess` get authentication only.** The middleware verifies the
bearer token on every non-anonymous endpoint, but the access check and the body signature run only where
`RequireActorAccess` attached the filters. That includes MVC controllers, where endpoint filters don't run.
A host that proxies or uses its own pipeline applies the same rule with `RequestProofRule.RequiresProof`
and `RequestProofRule.EvaluateAsync`, and the access level with `IActorAccessPolicy`.

## Admission: enrolling grants nothing by default

The enrollment endpoints are anonymous and self-service, so enrolling proves only that a caller holds the
keys it registered. `/actor/confirm` proves possession of the RSA key; it does not by itself gate access.
Access comes from admission (`IActorAdmission`): `ConfiguredActorAccessPolicy` gives `EnrolledActorAccess`
only to actors that are admitted, and `Denied` to everyone else. The default `ConfiguredActorAdmission`
admits nobody. A host opts in with `ActorAuth:OpenEnrollment = true` (anyone who can reach
`/actor/register` gets `EnrolledActorAccess`), or lists handles in `ActorAuth:AdmittedHandles`, or
registers its own `IActorAdmission` (an approval store, a directory) before `AddActorAuthentication`.

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
| `ProofRequiredAtOrAbove` | `Execute` | Access ladder threshold for body signatures |
| `EnrolledActorAccess` | `Execute` | Access an admitted enrolled actor holds (phase-1 policy) |
| `OpenEnrollment` | `false` | Admit every enrolled actor |
| `AdmittedHandles` | empty | Handles admitted when `OpenEnrollment` is off (exact match) |
| `AllowedBodySignatureAlgorithms` | `SHA256WITHECDSA` | Algorithms the body signature may name |

`UseActorAuthentication` fails fast, naming what is missing, when a prerequisite contract is not
registered or (hybrid) the server key is absent. Provisioning the server key is the host's job.
`MapActorToken` refuses to map in `ClientSignedOnly` mode, which has no server key.

## Client recipe

1. Generate an RSA pair and an ECC pair; `POST /actor/register` with both public PEMs and your handle.
2. Optionally `POST /actor/challenge`, sign the base64 text of the challenge with the RSA key
   (SHA512WITHRSA), and `POST /actor/confirm` to prove possession of the RSA key. Confirmation does not
   grant access; the host's admission does (see above).
3. Mint a `BamJwtToken` (sub = handle, 2-minute expiry) signed with the ECC key; `POST /actor/token`
   → server token (45 min).
4. Call protected endpoints with `Authorization: Bearer <server token>` and
   `X-Bam-Body-Signature: <base64 SHA256WITHECDSA over the exact body>`.
5. On 401 "key binding does not match", your key set was rotated or revoked — re-issue.

Deferred by design: nonce single-use custody (`X-Bam-Nonce`), encrypted bodies. See the design doc.
