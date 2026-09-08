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
`X-Bam-Body-Signature` (SHA256WITHECDSA over the raw body, base64) made with the same key. Verification
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
well-known anonymous actor.

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
| `EnrolledActorAccess` | `Execute` | Access every enrolled actor holds (phase-1 policy) |

`UseActorAuthentication` fails fast, naming what is missing, when a prerequisite contract is not
registered or (hybrid) the server key is absent. Provisioning the server key is the host's job.

## Client recipe

1. Generate an RSA pair and an ECC pair; `POST /actor/register` with both public PEMs and your handle.
2. `POST /actor/challenge`, sign the base64 text of the challenge with the RSA key (SHA512WITHRSA),
   `POST /actor/confirm`.
3. Mint a `BamJwtToken` (sub = handle, 2-minute expiry) signed with the ECC key; `POST /actor/token`
   → server token (45 min).
4. Call protected endpoints with `Authorization: Bearer <server token>` and
   `X-Bam-Body-Signature: <base64 SHA256WITHECDSA over the exact body>`.
5. On 401 "key binding does not match", your key set was rotated or revoked — re-issue.

Deferred by design: nonce single-use custody (`X-Bam-Nonce`), encrypted bodies. See the design doc.
