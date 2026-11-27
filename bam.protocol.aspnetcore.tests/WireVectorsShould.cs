using System.Text;
using System.Text.Json;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Bam.Protocol.AspNetCore.Tests;

/// <summary>
/// Holds the C# implementation to the known-answer vectors in <c>spec/vectors</c>, the same files every other
/// implementation of the actor-auth wire reads (bam.protocol#110). A vector that this implementation answers
/// differently fails here, so the files cannot drift from the code. Nothing here opens a port or starts a host.
/// </summary>
[UnitTestMenu("Wire Vectors Should", Selector = "wv")]
public class WireVectorsShould : UnitTestMenuContainer
{
    [UnitTest]
    public void GiveEveryEncodingOfAKeyItsOneFingerprint()
    {
        When.A<WireVectorsShould>("fingerprints every key in keys.json in each of its encodings",
            (test) =>
            {
                KeysFile file = Vectors.Read<KeysFile>("keys.json");
                List<string> failures = new List<string>();
                int count = 0;
                foreach (KeyVector key in file.Keys.Concat(file.PublicOnly))
                {
                    foreach (KeyValuePair<string, string> form in key.Encodings())
                    {
                        count++;
                        string? fingerprint = PublicKeyFingerprint.Of(Vectors.PublicPem(form.Value));
                        if (fingerprint != key.Fingerprint)
                        {
                            failures.Add($"{key.Name} ({form.Key}) gave {fingerprint}");
                        }
                    }

                    if (key.PrivateScalarHex is not null)
                    {
                        count++;
                        ECPrivateKeyParameters privateKey = Vectors.PrivateKey(key.PrivateScalarHex);
                        string point = Convert.ToHexStringLower(privateKey.Parameters.G.Multiply(privateKey.D).GetEncoded(false));
                        if (point != key.PublicPointHex)
                        {
                            failures.Add($"{key.Name}: the private scalar gives point {point}");
                        }
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 15))
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ComposeTheCanonicalRequestText()
    {
        When.A<WireVectorsShould>("composes the text of every case in canonical-request.json",
            (test) =>
            {
                CanonicalFile file = Vectors.Read<CanonicalFile>("canonical-request.json");
                List<string> failures = new List<string>();
                int count = 0;
                foreach (CanonicalVector vector in file.Cases)
                {
                    count++;
                    byte[] body = Convert.FromHexString(vector.BodyHex);
                    KeyValuePair<string, string> target = CanonicalRequest.SplitTarget(vector.Target);
                    string digest = CanonicalRequest.BodyDigest(body);
                    string text = CanonicalRequest.Compose(vector.Method, target.Key, target.Value, vector.Timestamp, vector.Nonce, digest);
                    if (target.Key != vector.Path || target.Value != vector.Query)
                    {
                        failures.Add($"{vector.Name}: split into '{target.Key}' and '{target.Value}'");
                    }

                    if (digest != vector.BodySha256Hex)
                    {
                        failures.Add($"{vector.Name}: body digest {digest}");
                    }

                    if (text != vector.Canonical || Convert.ToHexStringLower(Encoding.UTF8.GetBytes(text)) != vector.CanonicalUtf8Hex)
                    {
                        failures.Add($"{vector.Name}: composed a different text");
                    }
                }

                foreach (NamedTarget refused in file.RefusedTargets)
                {
                    count++;
                    if (CanonicalRequest.IsOriginForm(refused.Target))
                    {
                        failures.Add($"target '{refused.Name}' was accepted");
                    }
                }

                foreach (string nonce in file.WellFormedNonces)
                {
                    count++;
                    if (!CanonicalRequest.IsWellFormedNonce(nonce))
                    {
                        failures.Add($"nonce '{nonce}' was refused");
                    }
                }

                foreach (NamedNonce malformed in file.MalformedNonces)
                {
                    count++;
                    if (CanonicalRequest.IsWellFormedNonce(malformed.Nonce))
                    {
                        failures.Add($"nonce '{malformed.Name}' was accepted");
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 27))
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void VerifyTheSignaturesAsRecorded()
    {
        When.A<WireVectorsShould>("verifies every signature in signatures.json and csharp-produced.json",
            (test) =>
            {
                KeysFile keys = Vectors.Read<KeysFile>("keys.json");
                BodySignatureProofVerifier verifier = new BodySignatureProofVerifier();
                List<string> failures = new List<string>();
                int count = 0;
                foreach (SignatureVector vector in Vectors.Read<SignaturesFile>("signatures.json").Cases.Concat(Vectors.Read<ProducedFile>("csharp-produced.json").Signatures))
                {
                    count++;
                    bool verified = verifier.Verify(vector.Text, vector.SignatureBase64, vector.Algorithm, keys.PublicPemOf(vector.Key));
                    if (verified != vector.Verifies)
                    {
                        failures.Add($"{vector.Name}: verified = {verified}");
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 13))
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ReadADerSignatureAsItsRawForm()
    {
        When.A<WireVectorsShould>("converts every signature in der-raw.json both ways",
            (test) =>
            {
                List<string> failures = new List<string>();
                int count = 0;
                foreach (DerRawVector vector in Vectors.Read<DerRawFile>("der-raw.json").Cases)
                {
                    count++;
                    Asn1Sequence sequence = Asn1Sequence.GetInstance(Convert.FromHexString(vector.DerHex));
                    BigInteger r = DerInteger.GetInstance(sequence[0]).PositiveValue;
                    BigInteger s = DerInteger.GetInstance(sequence[1]).PositiveValue;
                    string raw = Convert.ToHexStringLower(Vectors.Fixed32(r)) + Convert.ToHexStringLower(Vectors.Fixed32(s));
                    string der = Convert.ToHexStringLower(new DerSequence(new DerInteger(new BigInteger(1, Convert.FromHexString(vector.RHex))), new DerInteger(new BigInteger(1, Convert.FromHexString(vector.SHex)))).GetEncoded());
                    if (raw != vector.RawHex || vector.RawHex != vector.RHex + vector.SHex)
                    {
                        failures.Add($"{vector.Name}: DER reads as {raw}");
                    }

                    if (der != vector.DerHex)
                    {
                        failures.Add($"{vector.Name}: r and s encode as {der}");
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 6))
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void WriteAndReadTheTokensAsRecorded()
    {
        When.A<WireVectorsShould>("encodes, verifies and decodes every token in tokens.json and csharp-produced.json",
            (test) =>
            {
                KeysFile keys = Vectors.Read<KeysFile>("keys.json");
                TokensFile file = Vectors.Read<TokensFile>("tokens.json");
                List<string> failures = new List<string>();
                int count = 0;
                foreach (TokenVector vector in file.Cases.Concat(Vectors.Read<ProducedFile>("csharp-produced.json").Tokens))
                {
                    count++;
                    TokenClaims claims = vector.Claims;
                    BamJwtToken token = new BamJwtToken(claims.Sid, claims.Sub, claims.Iss)
                    {
                        IssuedAt = DateTimeOffset.FromUnixTimeSeconds(claims.Iat),
                        Expiry = DateTimeOffset.FromUnixTimeSeconds(claims.Exp),
                        KeyFingerprint = claims.Kfp,
                        Audience = claims.Aud,
                    };
                    string encoded = token.Encode(Vectors.PrivateKey(keys.Find(vector.Key).PrivateScalarHex!));
                    if (encoded.Substring(0, encoded.LastIndexOf('.')) != vector.SigningInput)
                    {
                        failures.Add($"{vector.Name}: encoded as {encoded.Substring(0, encoded.LastIndexOf('.'))}");
                    }

                    ECPublicKeyParameters publicKey = Vectors.PublicKey(keys.Find(vector.Key).PublicPointHex);
                    if (!BamJwtToken.Verify(encoded, publicKey) || !BamJwtToken.Verify(vector.Token, publicKey))
                    {
                        failures.Add($"{vector.Name}: does not verify");
                    }

                    string[] segments = vector.Token.Split('.');
                    if (vector.HeaderJson is not null || vector.PayloadJson is not null || vector.SignatureDerHex is not null)
                    {
                        bool layout = vector.HeaderJson == Encoding.UTF8.GetString(Vectors.FromBase64Url(segments[0]))
                            && vector.PayloadJson == Encoding.UTF8.GetString(Vectors.FromBase64Url(segments[1]))
                            && vector.SignatureDerHex == Convert.ToHexStringLower(Vectors.FromBase64Url(segments[2]));
                        if (!layout)
                        {
                            failures.Add($"{vector.Name}: headerJson, payloadJson or signatureDerHex is not what the token holds");
                        }
                    }

                    BamJwtToken decoded = BamJwtToken.Decode(vector.Token);
                    bool same = decoded.ActorHandle == claims.Sub && decoded.SessionId == claims.Sid && decoded.Issuer == claims.Iss
                        && decoded.IssuedAt.ToUnixTimeSeconds() == claims.Iat && decoded.Expiry.ToUnixTimeSeconds() == claims.Exp
                        && decoded.KeyFingerprint == claims.Kfp && decoded.Audience == claims.Aud;
                    if (!same)
                    {
                        failures.Add($"{vector.Name}: decoded to other claims");
                    }
                }

                if (file.Cases.Any(vector => vector.HeaderJson is null || vector.PayloadJson is null || vector.SignatureDerHex is null))
                {
                    failures.Add("a case in tokens.json lacks headerJson, payloadJson or signatureDerHex");
                }

                foreach (RefusedToken refused in file.Refused)
                {
                    count++;
                    if (BamJwtToken.Verify(refused.Token, Vectors.PublicKey(keys.Find(refused.Key).PublicPointHex)))
                    {
                        failures.Add($"'{refused.Name}' verified");
                    }
                }

                foreach (UnreadableToken unreadable in file.Unreadable)
                {
                    count++;
                    bool read;
                    try
                    {
                        BamJwtToken.Decode(unreadable.Token);
                        read = true;
                    }
                    catch (Exception)
                    {
                        read = false;
                    }

                    if (read || !BamJwtToken.Verify(unreadable.Token, Vectors.PublicKey(keys.Find("actor").PublicPointHex)))
                    {
                        failures.Add($"'{unreadable.Name}': read = {read}; it must be signed by the actor and unreadable");
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 16))
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void JudgeEveryRequestProofAsRecorded()
    {
        When.A<WireVectorsShould>("judges every request in request-proof.json and csharp-produced.json",
            (test) =>
            {
                KeysFile keys = Vectors.Read<KeysFile>("keys.json");
                List<string> failures = new List<string>();
                int count = 0;
                ProofFile recorded = Vectors.Read<ProofFile>("request-proof.json");
                if (recorded.MaxAgeSeconds is null)
                {
                    failures.Add("request-proof.json lacks maxAgeSeconds");
                }

                foreach (ProofFile file in new[] { recorded, Vectors.Read<ProducedFile>("csharp-produced.json").RequestProofs })
                {
                    string callerPem = keys.PublicPemOf(file.SigningKey);
                    if (file.MaxAgeSeconds is not null && file.MaxAgeSeconds != (long)new ActorAuthenticationOptions().RequestProofMaxAge.TotalSeconds)
                    {
                        failures.Add($"maxAgeSeconds is {file.MaxAgeSeconds}, and the host's default is {new ActorAuthenticationOptions().RequestProofMaxAge.TotalSeconds}");
                    }

                    DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(file.NowUnixSeconds);
                    foreach (ProofVector vector in file.Cases)
                    {
                        count++;
                        bool verified = Judge(Evaluators.Create(new FixedClock(now)), vector, callerPem);
                        if (verified != vector.Verified)
                        {
                            failures.Add($"{vector.Name}: verified = {verified}");
                        }
                    }

                    foreach (ProofVector vector in file.Replays)
                    {
                        count++;
                        RequestProofEvaluator evaluator = Evaluators.Create(new FixedClock(now));
                        bool first = Judge(evaluator, vector, callerPem);
                        bool second = Judge(evaluator, vector, callerPem);
                        if (!first || second)
                        {
                            failures.Add($"{vector.Name}: first = {first}, second = {second}");
                        }
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 22))
            .SoBeHappy()
            .UnlessItFailed();
    }

    /// <summary>
    /// Pins the property names of the request and response types under ASP.NET's default web JSON settings. It
    /// does not show that a given host uses those settings; ActorEndpointAnswersShould shows what a running host
    /// with defaults answers.
    /// </summary>
    [UnitTest]
    public void ReadAndWriteTheJsonBodiesAsRecorded()
    {
        When.A<WireVectorsShould>("reads every request and writes every response in messages.json",
            (test) =>
            {
                MessagesFile file = Vectors.Read<MessagesFile>("messages.json");
                JsonSerializerOptions web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                List<string> failures = new List<string>();
                int count = 0;
                foreach (RequestMessage request in file.Requests)
                {
                    count++;
                    bool accepted;
                    try
                    {
                        accepted = request.Kind == "register"
                            ? JsonSerializer.Deserialize<ActorRegistrationRequest>(request.Json, web)?.KeySetHandle == "ada"
                            : JsonSerializer.Deserialize<ActorTokenRequest>(request.Json, web)?.ClientToken == "a.b.c";
                    }
                    catch (JsonException)
                    {
                        accepted = false;
                    }

                    if (accepted != request.Accepted)
                    {
                        failures.Add($"{request.Name}: accepted = {accepted}");
                    }
                }

                foreach (ResponseMessage response in file.Responses)
                {
                    count++;
                    string json = response.Kind switch
                    {
                        "register" => JsonSerializer.Serialize(new ActorRegistrationResponse { KeySetHandle = response.KeySetHandle! }, web),
                        "token" => JsonSerializer.Serialize(new ActorTokenResponse { Token = response.Token!, ExpiresUtc = DateTimeOffset.Parse(response.ExpiresUtc!, System.Globalization.CultureInfo.InvariantCulture), KeyFingerprint = response.KeyFingerprint! }, web),
                        _ => JsonSerializer.Serialize(new ActorAuthFailure { Messages = new[] { response.Message! } }, web),
                    };
                    if (json != response.Json)
                    {
                        failures.Add($"{response.Name}: wrote {json}");
                    }

                    if (response.ExpiresUtc is not null && !response.Json.Contains("\"expiresUtc\":\"" + response.ExpiresUtc + "\"", StringComparison.Ordinal))
                    {
                        failures.Add($"{response.Name}: expiresUtc is not the text in json");
                    }
                }

                return new VectorOutcome(count, string.Join("; ", failures));
            })
            .TheTest
            .ShouldPass<VectorOutcome>((because, outcome) => Agree(because, outcome, 9))
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static bool Judge(RequestProofEvaluator evaluator, ProofVector vector, string callerPem)
    {
        DefaultHttpContext context = Requests.WithEndpoint().WithBodyBytes(Convert.FromHexString(vector.BodyHex));
        context.Request.Method = vector.Method;
        context.Features.Get<IHttpRequestFeature>()!.RawTarget = vector.Target;
        foreach (KeyValuePair<string, string> header in vector.Headers)
        {
            context.Request.Headers[header.Key] = header.Value;
        }

        return evaluator.EvaluateAsync(context.Request, callerPem).GetAwaiter().GetResult().Success;
    }

    private static void Agree(Because because, VectorOutcome outcome, int atLeast)
    {
        because.ItsTrue($"the file held its vectors: {outcome.Checked} checked", outcome.Checked >= atLeast);
        because.ItsTrue("every vector was answered as recorded: " + outcome.Failures, outcome.Failures.Length == 0);
    }

    private sealed record VectorOutcome(int Checked, string Failures);
}

/// <summary>Reads the vector files copied beside the test assembly and builds keys from their bytes.</summary>
internal static class Vectors
{
    private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    internal static T Read<T>(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "vectors", fileName);
        return JsonSerializer.Deserialize<T>(File.ReadAllText(path, Encoding.UTF8), Json)
            ?? throw new InvalidOperationException($"{fileName} is empty.");
    }

    /// <summary>Wraps a DER SubjectPublicKeyInfo as the PEM text the C# API takes. The vectors hold no PEM.</summary>
    internal static string PublicPem(string spkiBase64)
    {
        StringBuilder pem = new StringBuilder("-----BEGIN PUBLIC KEY-----\n");
        for (int offset = 0; offset < spkiBase64.Length; offset += 64)
        {
            pem.Append(spkiBase64, offset, Math.Min(64, spkiBase64.Length - offset)).Append('\n');
        }

        return pem.Append("-----END PUBLIC KEY-----\n").ToString();
    }

    internal static ECPrivateKeyParameters PrivateKey(string scalarHex)
    {
        return new ECPrivateKeyParameters("ECDSA", new BigInteger(1, Convert.FromHexString(scalarHex)), Curve());
    }

    internal static ECPublicKeyParameters PublicKey(string pointHex)
    {
        ECDomainParameters curve = Curve();
        return new ECPublicKeyParameters("ECDSA", curve.Curve.DecodePoint(Convert.FromHexString(pointHex)), curve);
    }

    internal static byte[] FromBase64Url(string text)
    {
        string padded = text.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(padded + new string('=', (4 - padded.Length % 4) % 4));
    }

    internal static byte[] Fixed32(BigInteger value)
    {
        byte[] unsigned = value.ToByteArrayUnsigned();
        byte[] padded = new byte[32];
        Array.Copy(unsigned, 0, padded, 32 - unsigned.Length, unsigned.Length);
        return padded;
    }

    private static ECDomainParameters Curve()
    {
        X9ECParameters parameters = ECNamedCurveTable.GetByName("prime256v1");
        return new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H, parameters.GetSeed());
    }
}

internal sealed record KeysFile(IReadOnlyList<KeyVector> Keys, IReadOnlyList<KeyVector> PublicOnly)
{
    internal KeyVector Find(string name)
    {
        return Keys.Single(key => key.Name == name);
    }

    internal string PublicPemOf(string name)
    {
        return Vectors.PublicPem(Find(name).SpkiNamedCurveBase64);
    }
}

internal sealed record KeyVector(string Name, string? PrivateScalarHex, string PublicPointHex, string SpkiNamedCurveBase64, string SpkiExplicitParametersBase64, string SpkiCompressedPointBase64, string Fingerprint)
{
    internal IReadOnlyList<KeyValuePair<string, string>> Encodings()
    {
        return new[]
        {
            new KeyValuePair<string, string>("named curve", SpkiNamedCurveBase64),
            new KeyValuePair<string, string>("explicit parameters", SpkiExplicitParametersBase64),
            new KeyValuePair<string, string>("compressed point", SpkiCompressedPointBase64),
        };
    }
}

internal sealed record CanonicalFile(IReadOnlyList<CanonicalVector> Cases, IReadOnlyList<NamedTarget> RefusedTargets, IReadOnlyList<string> WellFormedNonces, IReadOnlyList<NamedNonce> MalformedNonces);

internal sealed record CanonicalVector(string Name, string Method, string Target, long Timestamp, string Nonce, string BodyHex, string Path, string Query, string BodySha256Hex, string Canonical, string CanonicalUtf8Hex);

internal sealed record NamedTarget(string Name, string Target);

internal sealed record NamedNonce(string Name, string Nonce);

internal sealed record SignaturesFile(IReadOnlyList<SignatureVector> Cases);

internal sealed record SignatureVector(string Name, string Key, string Text, string? Algorithm, string SignatureBase64, bool Verifies);

internal sealed record DerRawFile(IReadOnlyList<DerRawVector> Cases);

internal sealed record DerRawVector(string Name, string RHex, string SHex, string DerHex, string RawHex);

internal sealed record TokensFile(IReadOnlyList<TokenVector> Cases, IReadOnlyList<RefusedToken> Refused, IReadOnlyList<UnreadableToken> Unreadable);

internal sealed record TokenVector(string Name, string Key, TokenClaims Claims, string SigningInput, string Token, string? HeaderJson, string? PayloadJson, string? SignatureDerHex);

internal sealed record UnreadableToken(string Name, string Token);

internal sealed record TokenClaims(string Sub, string Sid, string Iss, long Iat, long Exp, string? Kfp, string? Aud);

internal sealed record RefusedToken(string Name, string Key, string Token);

internal sealed record ProofFile(long NowUnixSeconds, long? MaxAgeSeconds, string SigningKey, IReadOnlyList<ProofVector> Cases, IReadOnlyList<ProofVector> Replays);

internal sealed record ProofVector(string Name, string Method, string Target, string BodyHex, IReadOnlyDictionary<string, string> Headers, bool Verified);

internal sealed record ProducedFile(IReadOnlyList<SignatureVector> Signatures, IReadOnlyList<TokenVector> Tokens, ProofFile RequestProofs);

internal sealed record MessagesFile(IReadOnlyList<RequestMessage> Requests, IReadOnlyList<ResponseMessage> Responses);

internal sealed record RequestMessage(string Name, string Kind, bool Accepted, string Json);

internal sealed record ResponseMessage(string Name, string Kind, string Json, string? KeySetHandle, string? Token, string? ExpiresUtc, string? KeyFingerprint, string? Message);
