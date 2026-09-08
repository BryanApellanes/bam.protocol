using Bam.Encryption;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Mints server-signed <see cref="BamJwtToken"/>s bound to the caller's registered ECC key: the
/// <c>kfp</c> claim is <see cref="PublicKeyFingerprint.Of"/> the key's PEM, the single canonical
/// identity definition the whole key-set store uses. The signing key pair is read from
/// <see cref="INamedKeyStorage"/> under <see cref="ActorAuthenticationOptions.ServerKeyName"/> as PEM bytes.
/// </summary>
public sealed class ServerActorTokenIssuer
{
    private readonly INamedKeyStorage _keys;
    private readonly ActorAuthenticationOptions _options;

    /// <summary>Creates the issuer.</summary>
    /// <param name="keys">Custody of the server signing key pair.</param>
    /// <param name="options">Issuer name, key name, and token lifetime.</param>
    public ServerActorTokenIssuer(INamedKeyStorage keys, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(options);
        _keys = keys;
        _options = options;
    }

    /// <summary>
    /// Issues a token for an actor whose key possession was just proven.
    /// </summary>
    /// <param name="actor">The verified actor (its handle becomes <c>sub</c>).</param>
    /// <param name="eccPublicKeyPem">The actor's registered ECC public key PEM, whose fingerprint becomes <c>kfp</c>.</param>
    /// <returns>The issued token and its expiry.</returns>
    /// <exception cref="InvalidOperationException">The server key is not in storage, or the PEM does not fingerprint.</exception>
    public IssuedActorToken Issue(IActor actor, string eccPublicKeyPem)
    {
        ArgumentNullException.ThrowIfNull(actor);
        string fingerprint = PublicKeyFingerprint.Of(eccPublicKeyPem)
            ?? throw new InvalidOperationException("The actor's ECC public key PEM does not parse; no key binding can be issued.");

        BamJwtToken token = new BamJwtToken(Guid.NewGuid().ToString("N"), actor.Handle, _options.Issuer, _options.ServerTokenLifetime)
        {
            KeyFingerprint = fingerprint,
        };
        string encoded = token.Encode(ServerKey().Private);
        return new IssuedActorToken { Token = encoded, ExpiresUtc = token.Expiry, KeyFingerprint = fingerprint };
    }

    /// <summary>
    /// Loads the server signing key pair, failing descriptively when the named key is absent.
    /// </summary>
    /// <returns>The key pair.</returns>
    /// <exception cref="InvalidOperationException">No key is stored under the configured name.</exception>
    public AsymmetricCipherKeyPair ServerKey()
    {
        byte[]? pem = _keys.GetNamedKey(_options.ServerKeyName);
        if (pem is null || pem.Length == 0)
        {
            throw new InvalidOperationException(
                $"No server signing key is stored under '{_options.ServerKeyName}' (ActorAuthenticationOptions.ServerKeyName); provision an ECC key pair PEM in INamedKeyStorage before enabling hybrid mode.");
        }

        return pem.PemToKeyPair();
    }
}

/// <summary>An issued server token with the facts a client needs to use it.</summary>
public sealed record IssuedActorToken
{
    /// <summary>Gets the compact JWT.</summary>
    public required string Token { get; init; }

    /// <summary>Gets when the token expires.</summary>
    public required DateTimeOffset ExpiresUtc { get; init; }

    /// <summary>Gets the canonical fingerprint of the key the token is bound to.</summary>
    public required string KeyFingerprint { get; init; }
}
