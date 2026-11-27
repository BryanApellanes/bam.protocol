# Sealed frames

**Status: draft. No host implements this yet.** It is written first so the C# host, bam.js and bam.py
are built to one text. bam.protocol#116 tracks the C# side. It depends on bam.encryption#38 (the
cipher, key derivation and key agreement) and on bam.protocol#112 (a request proof that names its
host). Where this text leans on either, it says so.

[Actor auth](actor-auth.md) proves who sent a request and that it was not changed. It does not hide
the request or the answer; TLS does that, between the client and whatever ends the TLS connection.
Sealed frames hide and protect both, between the client and the host itself, whatever sits between
them.

A client that speaks sealed frames first speaks actor auth: it is registered, admitted, and holds a
server token.

The known-answer vectors are in [`vectors/`](https://github.com/BryanApellanes/bam.protocol/tree/bam_protocol/spec/vectors):
`sealed-session.json` and `sealed-frames.json`.

## The pieces

| Piece | What it is |
|---|---|
| Host key | The host's token-signing key. It also signs each session's transcript. A client pins it. |
| Session | Two fresh P-256 keys, one from each side, and what is derived from them. It belongs to one actor. |
| Transcript | The text both sides build that says what the session is: who, where, which keys, until when. |
| Frame keys | Two AES-256 keys derived from the session, one for each direction. |
| Frame | One sealed message: a sequence number and a ciphertext. |

## Encodings

As in [actor auth](actor-auth.md#encodings): base64url has no padding, hex is lower case, and an
ECDSA signature is ASN.1 DER.

A public key on this wire is base64url of its **named-curve, uncompressed SubjectPublicKeyInfo**: 91
bytes, the prefix `3059301306072a8648ce3d020106082a8648ce3d030107034200` and then the 65-byte point.
A receiver refuses any other encoding here. (The host's own key is published the same way.)

## 1. Learn and pin the host key

```
GET /actor/server-key
```

No authentication.

```json
{
  "current": { "publicKey": "<base64url SPKI>", "fingerprint": "<64 hex>" },
  "next":    { "publicKey": "<base64url SPKI>", "fingerprint": "<64 hex>" }
}
```

- `fingerprint` is the [key fingerprint](actor-auth.md#key-fingerprint) of `publicKey`. A client
  computes it and does not trust the field.
- `next` is present only while the host is about to change keys. The host signs with `current`
  until `next` has been published for a while, then `next` becomes `current`.
- **Pinning.** This answer is only as trustworthy as the connection it came over. A client that
  wants more than TLS gives it is configured with the fingerprint by its operator, out of band, and
  refuses a session signed by any other key. A client may add a `next` fingerprint to its pins only
  when it learned it over a sealed session signed by a key it already pins, or from its operator.
- A client with no pin accepts `current` and `next` as published. It is then protected against
  someone who can read the traffic, and not against someone who can stand in for the host.

## 2. Open a session

```
POST /actor/session
Authorization: Bearer <server token>
Content-Type: application/json
<request proof headers>

{ "clientPublicKey": "<base64url SPKI>" }
```

- `clientPublicKey` is a P-256 key pair the client made for this session and uses for nothing
  else.
- The request carries a bearer token and a request proof like any protected request. The proof is
  the form that names its host (bam.protocol#112). A host does not accept the older form here.

```json
{
  "sessionId": "<32 hex>",
  "serverPublicKey": "<base64url SPKI>",
  "expires": 1790003600,
  "signature": "<base64url DER>"
}
```

- `sessionId` is 32 lower-case hex characters the host chose at random.
- `serverPublicKey` is a P-256 key pair the host made for this session and uses for nothing else.
- `expires` is Unix seconds. The host refuses frames after it. It is at most one hour ahead.
- `signature` is the host key's signature over the transcript.
- Refusals are those of any protected endpoint (`401`, `403`, `429`, `503`), and `400` when
  `clientPublicKey` is missing or is not a P-256 key in the encoding above.

## 3. The transcript

Both sides build the same text. It is UTF-8, seven lines joined by `\n`, with no trailing newline:

```
BAM-SESSION-TRANSCRIPT-1
<audience>
<actor key fingerprint>
<sessionId>
<clientPublicKey>
<serverPublicKey>
<expires>
```

- `audience` is the host's configured public origin, the same value the request proof names
  (bam.protocol#112). The client uses the value it was configured with, not anything the host
  sent. The host uses its own configuration, never the `Host` header.
- `actor key fingerprint` is the fingerprint of the actor's registered key, the `kfp` claim of the
  server token.
- `clientPublicKey` and `serverPublicKey` are the base64url texts exactly as they went over the
  wire.
- `expires` is the number in plain decimal.

The client verifies `signature` over these bytes with the host key (ECDSA P-256 with SHA-256, as
for tokens) before it derives anything. If it does not verify with a pinned key (or, with no pins,
with `current` or `next`), the client stops and does not use the session.

The first line keeps this signature apart from every other thing the host key signs. A token's
signed text is base64url with a dot in it and can never begin with this line.

## 4. Frame keys

```
shared  = the X coordinate of ECDH(own private key, peer public key), as exactly 32 bytes, big-endian
salt    = SHA-256(transcript)
key c2s = HKDF-SHA256(ikm = shared, salt = salt, info = "BAM-SEALED-FRAME-1 client-to-server", length = 32)
key s2c = HKDF-SHA256(ikm = shared, salt = salt, info = "BAM-SEALED-FRAME-1 server-to-client", length = 32)
```

- `shared` keeps its leading zero bytes. A library that returns the secret as a number, or trims
  it, gives the wrong keys about once in 256 sessions. One of the vectors has a leading zero.
- The `info` texts are ASCII, with one space, and no terminator.
- The client seals with `key c2s` and opens with `key s2c`. The host does the reverse.
- Both sides discard `shared` and their session private key once the two keys exist.

## 5. A frame

A frame is sealed with AES-256-GCM.

```
nonce = direction (4 bytes) ‖ sequence (8 bytes, big-endian)
        direction is 00 00 00 01 from the client, 00 00 00 02 from the host
aad   = UTF-8 of five lines joined by "\n", no trailing newline:
          BAM-SEALED-FRAME-1
          <sessionId>
          <c2s or s2c>
          <sequence in decimal>
          <content type>
sealed = AES-256-GCM(key, nonce, plaintext, aad)   ciphertext with its 16-byte tag appended
```

- `sequence` starts at 0 for each direction and goes up by one for each frame. The two directions
  count separately.
- `content type` is the media type of the plaintext, as it would have been in `Content-Type`, in
  lower case with no parameters (`application/json`). It may be empty.
- A sender sends at most 2^20 frames in one direction in one session. To send more, open a new
  session.

On the wire a frame is JSON:

```json
{ "sessionId": "<32 hex>", "sequence": 0, "contentType": "application/json", "last": true, "ciphertext": "<base64url>" }
```

- `last` is `true` on the final frame of a message. A single request or a single answer is one
  frame with `last: true`. A streamed answer is several frames, and only the final one says `true`.
- `last` is protected too: when it is `true`, the text `;last` is appended to the content type
  line of the associated data (`application/json;last`, or `;last` for an empty content type). So a
  stream cut short cannot be passed off as complete.

## 6. Sending a sealed request

```
POST /invoke
Authorization: Bearer <server token>
Content-Type: application/bam-sealed+json
<request proof headers>

{ "sessionId": "…", "sequence": 3, "contentType": "application/json", "last": true, "ciphertext": "…" }
```

- The plaintext is the body the request would have carried unsealed.
- The request proof covers the sealed body, the bytes actually sent.
- The session must belong to the actor the token names. A frame for another actor's session is
  refused.
- An endpoint that accepts sealed requests answers with a sealed body: one frame,
  `Content-Type: application/bam-sealed+json`, or, for a stream, `text/event-stream` with one frame
  per event in its `data:` line.
- The status line and headers are not sealed. A host that seals answers puts nothing in a header
  that the sealed body should hide.

## 7. What a receiver refuses

A receiver opens a frame only if all of these hold, and checks them in this order:

1. The session exists, belongs to this actor, and has not expired.
2. `sequence` is exactly the next one expected for that direction, and below 2^20. Not lower, not
   higher.
3. The tag verifies, with the associated data built from the frame's own fields.

- Nothing is decrypted, parsed or acted on before the tag verifies.
- When 2 or 3 fails, the receiver ends the session: it forgets the keys and refuses every later
  frame for that session id. The client opens a new session.
- The host answers a refused frame with `401` and a problem document, and does not say which
  check failed.
- Every frame arrives inside a request that already passed actor auth, so only the actor, or
  someone who can alter the actor's traffic, can send a frame that fails. Ending the session then
  costs the client one round trip to open another.

## What a client must get right

- Verify the transcript signature before deriving keys, with a pinned key when it has one.
- Build the transcript from its own configured audience, not from anything the host sent.
- Keep `shared` at 32 bytes.
- Never reuse a sequence number in a direction, including after a failed send. A request that
  failed in transit has used its number; if the client cannot know whether the host saw it, it
  opens a new session.
- Never use a session key pair twice.
- Treat a failed tag as the end of the session, not as a retry.
- Refuse an answer whose final frame does not say `last`.

## What this does not do

- It does not hide the path, the query, the status or the headers.
- It does not hide who is talking: the bearer token names the actor.
- It does not protect a client with no pinned key from someone who can stand in for the host.
- Forward secrecy holds once both sides have discarded their session private keys. A host that
  keeps session keys on disk gives it up.
