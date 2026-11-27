# Actor-auth vectors

Known-answer vectors for [the actor-auth wire](../actor-auth.md). Every implementation reads the
same files: the C# one in `bam.protocol.aspnetcore.tests/WireVectorsShould.cs`, and each client
library in its own tests.

| File | What it pins |
|---|---|
| `keys.json` | Three test keys, and one public key made by openssl, each in three encodings with its one fingerprint. |
| `canonical-request.json` | The text a request proof signs, for a set of methods, targets and bodies. Targets and nonces that are refused. |
| `signatures.json` | Signatures that verify and signatures that must not. |
| `der-raw.json` | The same signatures as DER and as raw `r‖s`. |
| `tokens.json` | Tokens for fixed claims, tokens whose signature must be refused, and signed tokens the host cannot read. |
| `request-proof.json` | Whole requests, with the verdict a host with default settings gives. Replays. |
| `messages.json` | JSON request and response bodies, including the time format a real host writes. |
| `csharp-produced.json` | Signatures, tokens and proofs written by the C# implementation. |
| `sealed-session.json` | Sealed frames (draft, [`../sealed-frames.md`](../sealed-frames.md)): session transcripts, their signatures, the shared secret and the two frame keys. Sessions a client must refuse. |
| `sealed-frames.json` | Sealed frames (draft): frames in both directions with their nonce, associated data and ciphertext. Frames a receiver must refuse. |

## Rules for the files

- Bytes are hex or base64. There is no PEM text; wrap a `SubjectPublicKeyInfo` in PEM yourself where
  an API wants it.
- The keys are test keys. Each private scalar is `SHA-256("bam-wire test key: <name>")` reduced into
  `[1, n-1]`. Never use them for anything else.
- ECDSA signatures differ every time most libraries sign, so a vector gives a signature to verify,
  never one to reproduce.

## Where they come from

`tools/generate.py` writes every file except `csharp-produced.json`. It uses only the Python
standard library, and its signatures use RFC 6979 nonces, so running it again writes the same
bytes:

    python spec/vectors/tools/generate.py
    git diff --exit-code spec/vectors

That makes the generator a second implementation, independent of the C# code: the C# tests pass
only when the two agree. The key encodings, fingerprints and signatures were also checked once
with openssl.

`csharp-produced.json` runs the other way. It was written once by the C# implementation
(`EccSignatureProvider.Sign`, `BamJwtToken.Encode`, `RequestProofSigner.Sign`) with the test keys, by a
throwaway test that is not kept,
so a client that verifies it has read real C# output. C# signatures are not deterministic, so the
file is kept as it is and not regenerated.

What a running host answers (status, content type and body, including the answers ASP.NET writes
before a handler runs) is pinned by `bam.protocol.aspnetcore.tests/ActorEndpointAnswersShould.cs`,
not by a vector file.

## Changing the wire

Change the specification and the vectors first, in one pull request with the C# change. A vector
the C# implementation answers differently fails `WireVectorsShould`.
