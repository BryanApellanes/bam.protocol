# Actor auth on the wire

This describes what a client sends and reads to call a Bam host as an authenticated actor, exactly
enough to write a client in any language. The C# code in `bam.protocol.aspnetcore` is the
implementation it describes. It does not describe the older native session
(`/bam/session/create`), and it does not cover encrypted bodies: under actor auth, bodies are
plaintext and TLS is the only confidentiality.

The known-answer vectors in [`vectors/`](vectors/README.md) go with this text. A client is correct
when it gives the recorded answer for every vector. Where the text and a vector disagree, the vector
is right and the text has a defect.

Defaults below are the host's defaults. A host can change the ones marked *(setting)*.

## The pieces

| Piece | What it is |
|---|---|
| Handle | The actor's name. A string the actor chooses at registration. |
| Actor key | One ECDSA key pair on P-256 (also called prime256v1 and secp256r1). The private half never leaves the client. |
| Key fingerprint | 64 lowercase hex characters naming a public key. See [Key fingerprint](#key-fingerprint). |
| Client token | A token the client signs with the actor key, good for at most 5 minutes. |
| Server token | A token the host signs, good for 45 minutes *(setting)*, bound to the actor key's fingerprint. |
| Request proof | Four headers that sign one request with the actor key. |

A call goes: register once, wait to be admitted, trade a client token for a server token, then send
each request with the server token and a fresh request proof.

## Encodings

- **Signatures** are ECDSA on P-256 over SHA-256 of the signed bytes. The signature is the ASN.1 DER
  `SEQUENCE { INTEGER r, INTEGER s }`, not the 64-byte `r‖s` form. WebCrypto and JOSE libraries
  produce and expect `r‖s`, so a client converts. `vectors/der-raw.json` has both forms of the same
  signatures. The host refuses `r‖s`.
- **Base64** is standard base64 with padding in headers and JSON bodies. Tokens alone use base64url
  without padding.
- **Text** that is signed is signed as its UTF-8 bytes.
- **Public keys** travel as PEM `SubjectPublicKeyInfo` (`-----BEGIN PUBLIC KEY-----`). Send the
  named-curve form with an uncompressed point, which is what openssl, WebCrypto and most libraries
  write. The host also accepts explicit curve parameters and compressed points.

## Key fingerprint

The fingerprint of an EC public key is the lowercase hex SHA-256 of its DER `SubjectPublicKeyInfo`
written with the named-curve OID and an uncompressed point. Every encoding of one key has that one
fingerprint. For a P-256 key the hashed bytes are the 26-byte prefix
`3059301306072a8648ce3d020106082a8648ce3d030107034200` followed by the 65-byte point
(`04`, X, Y).

    openssl ec -pubin -in key.pem -param_enc named_curve -conv_form uncompressed -outform DER | sha256sum

Vectors: `keys.json`.

## Registration

    POST /actor/register
    Content-Type: application/json

    { "KeySetHandle": "ada", "PublicRsaKeyPem": "", "PublicEccKeyPem": "-----BEGIN PUBLIC KEY-----\n…" }

- No authentication. All three properties must be present. `PublicRsaKeyPem` may be the empty
  string; an actor that only signs tokens and proofs needs no RSA key.
- `200` with `{ "keySetHandle": "ada" }` when accepted.
- `400` with `{ "messages": ["Registration was not accepted."] }` otherwise. The reason is not
  given: a taken handle, a key already registered under another handle, a revoked key and a
  malformed key all look the same.

Registering does not grant access. The host's operator admits a handle, optionally pinned to a key
fingerprint. Until then every later step answers `401`.

`/actor/challenge` and `/actor/confirm` prove possession of the RSA key. They are optional, grant
nothing by themselves, and are not described here.

## Tokens

A token is three base64url segments joined by `.`: header, payload, signature.

- The header is `{"alg":"ES256","typ":"JWT"}`. The host does not read it.
- The payload is a JSON object with these claims:

  | Claim | Type | Meaning |
  |---|---|---|
  | `sub` | string | The handle. |
  | `sid` | string | Any unique string. The C# client uses 32 hex characters. Required, not otherwise checked. |
  | `iss` | string | In a client token, any string. In a server token, the host's issuer (`bam` by default). |
  | `iat` | integer | Issued at, Unix seconds. |
  | `exp` | integer | Expires at, Unix seconds. |
  | `aud` | string | Client tokens: the audience the host requires. A single string, not an array. |
  | `kfp` | string | Server tokens: the fingerprint of the actor key the token is bound to. |

- The signature is over the ASCII bytes of `header.payload` as sent, in DER, base64url without
  padding.

**The header says ES256 but the signature is DER.** A standard JOSE ES256 signature is `r‖s` and is
refused, so stock JWT libraries can neither make nor check these tokens.

The host verifies the signature over the bytes it received, so the layout of the JSON (spacing,
escaping, order) is the signer's choice.

Vectors: `tokens.json`, `csharp-produced.json`.

### Getting a server token

    POST /actor/token
    Content-Type: application/json

    { "ClientToken": "<client token>" }

The host accepts a client token when all of these hold:

1. `exp - iat` is at most 5 minutes *(setting)*.
2. Now is not after `exp`, and not before `iat`, each with 60 seconds of allowance *(setting)*.
3. `aud` equals the host's audience exactly. A host has an audience unless its operator opted out.
   The audience is something the operator tells you, usually the host's public origin.
4. `sub` names a registered handle that has not been revoked.
5. The signature verifies with that handle's registered EC key.
6. The handle, with that key, is admitted.

Answers:

- `200` with `{ "token": "<server token>", "expiresUtc": "2026-09-21T14:58:20+00:00", "keyFingerprint": "<64 hex>" }`.
- `401` with `{ "messages": ["Authentication failed."] }` for every refusal. The reasons are not
  told apart.

Renew before `expiresUtc`. A client token is single-purpose and short; make a new one for each
request to `/actor/token`.

A host in client-signed-only mode has no `/actor/token`. There the client token itself is the
bearer token on every request.

## Calling a protected endpoint

    Authorization: Bearer <server token>

The host accepts a server token when its `iss` is the host's issuer, it is within its times (the
same 60 seconds of allowance), its signature is the host's, and its `kfp` still equals the
fingerprint of the handle's registered key. After a key rotation or a revocation the token stops
working at once; get a new one.

A refused token answers `401` with the plain-text body `Authentication failed.`

## Request proof

An endpoint that changes things requires a request proof as well as the token. By default that is
every operation needing `Execute` access or more *(setting)*. A proof is four headers:

| Header | Value |
|---|---|
| `X-Bam-Body-Signature` | Standard base64 of the DER signature over the canonical text. |
| `X-Bam-Body-Signature-Algorithm` | `SHA256WITHECDSA`. May be omitted. Compared without regard to case. |
| `X-Bam-Timestamp` | Unix seconds, digits only. |
| `X-Bam-Proof-Nonce` | 16 to 128 characters of `A-Z a-z 0-9 - _`, never used before with this key. |

The canonical text is seven fields joined by a single line feed (`\n`), with no line feed at the
end:

    BAM-REQUEST-PROOF-1
    <METHOD, upper case>
    <path>
    <query>
    <timestamp>
    <nonce>
    <lowercase hex SHA-256 of the body bytes>

- **Path and query** come from the request target exactly as it is sent on the request line, split
  at the first `?`. The query keeps its `?`; with no `?` the query is empty. Nothing is decoded,
  re-encoded or normalised: `/a%20b`, `/a/../b` and `/a?` are signed as written.
- The target must start with `/` and hold no `#`.
- **The body digest** is over the exact bytes sent. With no body it is the SHA-256 of nothing,
  `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855`.
- **The timestamp** must be within 5 minutes of the host's clock, either way *(setting)*.
- **The nonce** is single-use for each actor key until its timestamp leaves that window. 16 random
  bytes as 32 hex characters is what the C# client sends.

**Sign what you send.** An HTTP library that rewrites the URL after you sign it (resolving dot
segments, re-encoding characters, dropping an empty query) breaks the proof. Build the target
string yourself, sign it, and send that string.

A new proof is needed for every request, including a retry.

Answers when a proof is refused:

| Status | Meaning | What to do |
|---|---|---|
| `401` | The proof is not acceptable: missing or malformed header, timestamp outside the window, signature does not verify, nonce already used. | Fix it. Check the clock first. |
| `429` with `Retry-After` | This key has too many unexpired nonces recorded. | Wait, then send again with a new proof. |
| `503` with `Retry-After` | The host cannot check or record proofs right now. | Wait, then send again with a new proof. |

The body is a problem document (`application/problem+json`) whose `detail` says why.

Vectors: `canonical-request.json`, `signatures.json`, `request-proof.json`, `csharp-produced.json`.

## Invoking an operation

    POST /invoke
    Authorization: Bearer <server token>
    Content-Type: application/json
    (the four proof headers, when the operation requires a proof)

    { "OperationIdentifier": "Sample.Greeter+Greet, Sample", "Arguments": { "name": "Ada" } }

- `OperationIdentifier` is `<type full name>+<method name>, <assembly name>`. Text after a second
  comma (a version, a culture) is ignored. The host serves only the types its operator registered.
- `Arguments` maps parameter names to JSON values. Names are matched without regard to case.
- The path `/invoke` is the default *(setting)*. The body may be up to 1 MiB *(setting)*.

Answers:

| Status | Meaning |
|---|---|
| `200` | The return value as JSON. Property names are as the C# type declares them, usually PascalCase. |
| `400` | The body is not an invocation, or an argument is missing or cannot be converted. |
| `401` | The token was refused, or the proof was refused. |
| `403` | The actor's access is too low for the operation, or the operation is anonymous, which `/invoke` does not serve. |
| `413` | The body is too large. |
| `429`, `503` | See [Request proof](#request-proof). |
| `461` | The operation is not available: not registered, or not a public method. |
| `500` | The operation threw. The body does not say why. |

Errors other than the `401` for a refused token are problem documents.

## JSON bodies

- The host matches request property names without regard to case, and every listed property must be
  present.
- The host writes response property names in camelCase by default. A host can change its JSON
  settings, so a client reads response names without regard to case.

Vectors: `messages.json`.

## What a client must get right

1. Convert signatures between `r‖s` and DER, in both directions.
2. Sign the request target it actually sends.
3. Use a new nonce and a current timestamp for every request.
4. Keep the clock within the window; a `401` on every proof is usually the clock.
5. Renew the server token before it expires, and after any `401`.
6. Keep the private key where other code cannot read it.
