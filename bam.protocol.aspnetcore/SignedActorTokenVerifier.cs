using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Server;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Verifies a token the caller signed with its own ECC private key against the ECC public key in
/// the caller's registered ACTIVE key set (resolved canonically through
/// <see cref="IPublicKeySetResolver.ResolveByHandle"/>, so a revoked set never resolves). This is the
/// <see cref="ActorAuthenticationMode.ClientSignedOnly"/> verifier and the proof-of-possession step
/// behind server-token issuance. The token's <c>sub</c> is the key-set handle; the resolved actor
/// carries that handle as both handle and name (a profile-aware decorator can enrich it later).
/// </summary>
public sealed class SignedActorTokenVerifier : IActorTokenVerifier
{
    private readonly IPublicKeySetResolver _keySets;
    private readonly ActorAuthenticationOptions _options;

    /// <summary>Creates the verifier.</summary>
    /// <param name="keySets">Resolves registered key sets by handle (revoked tombstones never resolve).</param>
    /// <param name="options">Lifetime and skew tunables.</param>
    public SignedActorTokenVerifier(IPublicKeySetResolver keySets, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(keySets);
        ArgumentNullException.ThrowIfNull(options);
        _keySets = keySets;
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

        if (token.Expiry - token.IssuedAt > _options.ClientTokenMaxLifetime)
        {
            return ActorTokenVerification.Failed($"Client-signed token lifetime exceeds the {_options.ClientTokenMaxLifetime} maximum.");
        }

        string? timing = TokenTiming.Check(token, _options.ClockSkewAllowance);
        if (timing is not null)
        {
            return ActorTokenVerification.Failed(timing);
        }

        PublicKeySetData? keySet = _keySets.ResolveByHandle(token.ActorHandle);
        if (keySet is null)
        {
            return ActorTokenVerification.Failed($"No active key set is registered for handle '{token.ActorHandle}'.");
        }

        if (string.IsNullOrWhiteSpace(keySet.PublicEccKey))
        {
            return ActorTokenVerification.Failed($"Key set '{keySet.KeySetHandle}' carries no ECC public key.");
        }

        bool signatureValid;
        try
        {
            signatureValid = BamJwtToken.Verify(bearerToken, keySet.PublicEccKey.PemToKey());
        }
        catch (Exception)
        {
            signatureValid = false;
        }

        if (!signatureValid)
        {
            return ActorTokenVerification.Failed("Token signature does not verify against the registered ECC key.");
        }

        return new ActorTokenVerification
        {
            Success = true,
            Actor = new ActorData { Handle = keySet.KeySetHandle, Name = keySet.KeySetHandle },
            KeySetHandle = keySet.KeySetHandle,
            EccPublicKeyPem = keySet.PublicEccKey,
            Messages = ["Client-signed token verified against the registered ECC key."],
        };
    }
}
