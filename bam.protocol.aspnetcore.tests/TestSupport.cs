using System.Text;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Server;
using Bam.UserAccounts;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.AspNetCore.Tests;

/// <summary>Deterministic key material and tokens for the adapter tests.</summary>
internal static class TestKeys
{
    internal static EccKeyPair NewEcc()
    {
        return new EccKeyPair();
    }

    internal static byte[] PairPem(EccKeyPair pair)
    {
        AsymmetricCipherKeyPair keyPair = new AsymmetricCipherKeyPair(pair.PublicKey.Value, pair.PrivateKey.Value);
        return keyPair.ToPem(Encoding.UTF8);
    }

    internal static string ClientToken(EccKeyPair pair, string handle, TimeSpan? lifetime = null, string issuer = "client")
    {
        BamJwtToken token = new BamJwtToken(Guid.NewGuid().ToString("N"), handle, issuer, lifetime ?? TimeSpan.FromMinutes(2));
        return token.Encode(pair.PrivateKey.Value);
    }

    internal static string BodySignature(EccKeyPair pair, string body)
    {
        EccSignatureProvider signatures = new EccSignatureProvider();
        ISignature signature = signatures.Sign(new PrivateKeyProvider(pair.PrivateKey.Value), body, BodySignatureProofVerifier.DefaultAlgorithm);
        return signature.SignatureBase64;
    }

    internal static PublicKeySetData KeySet(string handle, EccKeyPair pair)
    {
        return new PublicKeySetData
        {
            KeySetHandle = handle,
            PublicEccKey = pair.PublicPem,
        };
    }
}

/// <summary>In-memory key-set resolver: resolves by handle, honoring the revoked-skip contract.</summary>
internal sealed class FakeKeySetResolver : IPublicKeySetResolver
{
    private readonly Dictionary<string, PublicKeySetData> _sets = new Dictionary<string, PublicKeySetData>(StringComparer.Ordinal);

    internal FakeKeySetResolver Add(PublicKeySetData keySet)
    {
        _sets[keySet.KeySetHandle] = keySet;
        return this;
    }

    internal void Revoke(string handle)
    {
        _sets[handle].RevokedUtc = DateTime.UtcNow;
    }

    internal void Rotate(string handle, EccKeyPair newPair)
    {
        _sets[handle].PublicEccKey = newPair.PublicPem;
    }

    public PublicKeySetData? ResolveByHandle(string keySetHandle)
    {
        return _sets.TryGetValue(keySetHandle, out PublicKeySetData? keySet) && keySet.RevokedUtc is null ? keySet : null;
    }

    public PublicKeySetData? ResolveByKeyMaterial(string publicKeyPem)
    {
        return _sets.Values.FirstOrDefault(keySet => keySet.RevokedUtc is null && keySet.PublicEccKey == publicKeyPem);
    }

    public IEnumerable<PublicKeySetData> FindKeyMaterialClaims(PublicKeySetData candidate)
    {
        return Array.Empty<PublicKeySetData>();
    }
}

/// <summary>In-memory named key storage.</summary>
internal sealed class InMemoryNamedKeyStorage : INamedKeyStorage
{
    private readonly Dictionary<string, byte[]> _keys = new Dictionary<string, byte[]>(StringComparer.Ordinal);

    public bool SaveNamedKey(string keyName, byte[] keyBytes)
    {
        _keys[keyName] = keyBytes;
        return true;
    }

    public byte[]? GetNamedKey(string name)
    {
        return _keys.TryGetValue(name, out byte[]? bytes) ? bytes : null;
    }
}

/// <summary>Scripted confirmation flow: one challenge per handle, confirmed when the signature matches the script.</summary>
internal sealed class FakeAccountConfirmation : IAccountConfirmation
{
    private readonly byte[] _acceptedSignature;

    internal FakeAccountConfirmation(byte[] acceptedSignature)
    {
        _acceptedSignature = acceptedSignature;
    }

    internal string? LastChallengedHandle { get; private set; }

    public ConfirmationChallenge CreateChallenge(string personHandle)
    {
        LastChallengedHandle = personHandle;
        return new ConfirmationChallenge
        {
            PersonHandle = personHandle,
            ChallengeBytes = Encoding.UTF8.GetBytes("challenge-for-" + personHandle),
            ExpiresUtc = DateTime.UtcNow.AddMinutes(10),
        };
    }

    public ConfirmationResult VerifySignedChallenge(string personHandle, byte[] signedChallenge)
    {
        return new ConfirmationResult { Confirmed = signedChallenge.AsSpan().SequenceEqual(_acceptedSignature) };
    }
}

/// <summary>Records what was registered and echoes it back, or refuses when scripted to.</summary>
internal sealed class FakeKeySetRegistrar : IPublicKeySetRegistrar
{
    internal PublicKeySetData? Registered { get; private set; }

    internal bool Refuse { get; set; }

    internal Exception? RefuseWith { get; set; }

    public PublicKeySetData Register(PublicKeySetData publicKeySetData)
    {
        if (RefuseWith is not null)
        {
            throw RefuseWith;
        }

        if (Refuse)
        {
            throw new InvalidOperationException("scripted refusal");
        }

        Registered = publicKeySetData;
        return publicKeySetData;
    }

    public PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature)
    {
        return newKeySet;
    }

    public PublicKeySetData? Resolve(string keySetHandle)
    {
        return Registered;
    }
}

/// <summary>Builds request contexts carrying endpoint metadata, headers, and bodies.</summary>
internal static class Requests
{
    internal static DefaultHttpContext WithEndpoint(params object[] metadata)
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.SetEndpoint(new Endpoint(null, new EndpointMetadataCollection(metadata), "test-endpoint"));
        return context;
    }

    internal static DefaultHttpContext WithBody(this DefaultHttpContext context, string body)
    {
        return context.WithBodyBytes(Encoding.UTF8.GetBytes(body));
    }

    internal static DefaultHttpContext WithBodyBytes(this DefaultHttpContext context, byte[] bytes)
    {
        context.Request.Body = new MemoryStream(bytes);
        context.Request.ContentLength = bytes.Length;
        return context;
    }

    internal static DefaultHttpContext At(this DefaultHttpContext context, string method, string path, string query = "")
    {
        context.Request.Method = method;
        context.Request.Path = new PathString(path);
        context.Request.QueryString = new QueryString(query);
        return context;
    }

    /// <summary>Signs the request as a client would, with <see cref="RequestProofSigner"/>, and sets the proof headers.</summary>
    internal static DefaultHttpContext Proved(this DefaultHttpContext context, EccKeyPair pair, DateTimeOffset now)
    {
        RequestProofHeaders headers = Proof(context, pair, now);
        return context.WithProof(headers);
    }

    internal static RequestProofHeaders Proof(DefaultHttpContext context, EccKeyPair pair, DateTimeOffset now)
    {
        if (string.IsNullOrEmpty(context.Request.Method))
        {
            context.At("POST", "/actor-api/hello");
        }

        byte[] body = Array.Empty<byte>();
        if (context.Request.Body.CanSeek)
        {
            context.Request.Body.Position = 0;
            using MemoryStream copy = new MemoryStream();
            context.Request.Body.CopyTo(copy);
            context.Request.Body.Position = 0;
            body = copy.ToArray();
        }

        Uri uri = new Uri(context.Request.Path.ToUriComponent() + context.Request.QueryString.Value, UriKind.Relative);
        return RequestProofSigner.Sign(new PrivateKeyProvider(pair.PrivateKey.Value), context.Request.Method, uri, body, now);
    }

    internal static DefaultHttpContext WithProof(this DefaultHttpContext context, RequestProofHeaders headers)
    {
        foreach (KeyValuePair<string, string> header in headers.ToHeaders())
        {
            context.Request.Headers[header.Key] = header.Value;
        }

        return context;
    }
}

/// <summary>A clock tests set by hand.</summary>
internal sealed class FixedClock : TimeProvider
{
    internal FixedClock(DateTimeOffset now)
    {
        Now = now;
    }

    internal DateTimeOffset Now { get; set; }

    public override DateTimeOffset GetUtcNow()
    {
        return Now;
    }
}

/// <summary>Builds request-proof evaluators for tests.</summary>
internal static class Evaluators
{
    internal static RequestProofEvaluator Create(TimeProvider? clock = null, ActorAuthenticationOptions? options = null, IRequestProofNonceStore? nonces = null)
    {
        ActorAuthenticationOptions effective = options ?? new ActorAuthenticationOptions();
        return new RequestProofEvaluator(new BodySignatureProofVerifier(effective), nonces ?? new MemoryRequestProofNonceStore(effective), effective, clock);
    }
}
