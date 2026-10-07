using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Hybrid-mode verifier: checks the server signature and expiry of a token minted by
/// <see cref="ServerActorTokenIssuer"/>, then resolves the <c>sub</c> handle's ACTIVE key set and
/// requires its ECC fingerprint to equal the token's <c>kfp</c>. That match is the revocation and
/// rotation check: revoke or rotate the set and every outstanding token stops verifying, with no
/// denylist.
/// </summary>
public sealed class ServerIssuedTokenVerifier : IActorTokenVerifier
{
    private readonly IPublicKeySetResolver _keySets;
    private readonly INamedKeyStorage _keys;
    private readonly ActorAuthenticationOptions _options;

    /// <summary>Creates the verifier.</summary>
    /// <param name="keySets">Resolves registered key sets by handle (revoked tombstones never resolve).</param>
    /// <param name="keys">Custody of the server signing key pair (its public half verifies tokens).</param>
    /// <param name="options">Issuer name, key name, and skew tunables.</param>
    public ServerIssuedTokenVerifier(IPublicKeySetResolver keySets, INamedKeyStorage keys, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(keySets);
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(options);
        _keySets = keySets;
        _keys = keys;
        _options = options;
    }

    /// <inheritdoc />
    public ActorTokenVerification Verify(string bearerToken)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            return ActorTokenVerification.Failed("Bearer token missing.");
        }

        BamJwtToken token;
        try
        {
            token = BamJwtToken.Decode(bearerToken);
        }
        catch (Exception)
        {
            return ActorTokenVerification.Failed("Invalid token format.");
        }

        if (!string.Equals(token.Issuer, _options.Issuer, StringComparison.Ordinal))
        {
            return ActorTokenVerification.Failed($"Token issuer '{token.Issuer}' is not this host's issuer.");
        }

        string? timing = TokenTiming.Check(token, _options.ClockSkewAllowance);
        if (timing is not null)
        {
            return ActorTokenVerification.Failed(timing);
        }

        if (string.IsNullOrWhiteSpace(token.KeyFingerprint))
        {
            return ActorTokenVerification.Failed("Token carries no key binding (kfp claim); hybrid mode requires a server-issued token.");
        }

        byte[]? serverPem = _keys.GetNamedKey(_options.ServerKeyName);
        if (serverPem is null || serverPem.Length == 0)
        {
            return ActorTokenVerification.Failed($"No server signing key is stored under '{_options.ServerKeyName}'.");
        }

        bool signatureValid;
        try
        {
            AsymmetricCipherKeyPair serverKey = serverPem.PemToKeyPair();
            signatureValid = BamJwtToken.Verify(bearerToken, serverKey.Public);
        }
        catch (Exception)
        {
            signatureValid = false;
        }

        if (!signatureValid)
        {
            return ActorTokenVerification.Failed("Token signature does not verify against the server key.");
        }

        PublicKeySetData? keySet = _keySets.ResolveByHandle(token.ActorHandle);
        if (keySet is null)
        {
            return ActorTokenVerification.Failed($"No active key set is registered for handle '{token.ActorHandle}'.");
        }

        string? activeFingerprint = PublicKeyFingerprint.Of(keySet.PublicEccKey);
        if (activeFingerprint is null || !string.Equals(activeFingerprint, token.KeyFingerprint, StringComparison.Ordinal))
        {
            return ActorTokenVerification.Failed("Token key binding does not match the handle's active key set (rotated or revoked); obtain a new token.");
        }

        return new ActorTokenVerification
        {
            Success = true,
            Actor = new ActorData { Handle = keySet.KeySetHandle, Name = keySet.KeySetHandle },
            KeySetHandle = keySet.KeySetHandle,
            EccPublicKeyPem = keySet.PublicEccKey,
            Messages = ["Server-issued token verified and key binding matches the active key set."],
        };
    }
}
