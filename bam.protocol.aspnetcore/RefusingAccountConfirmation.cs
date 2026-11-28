using System.Security.Cryptography;
using Bam.UserAccounts;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// An <see cref="IAccountConfirmation"/> that never confirms anyone (bam.protocol#119). It is for hosts that don't
/// use RSA confirmation at all, such as a one-visit demo on <see cref="InMemoryActorStoreRegistration"/>, but must
/// still register a confirmation flow because <c>UseActorAuthentication</c> requires one.
/// <para>
/// <see cref="CreateChallenge"/> answers with 32 random bytes that are already expired, so <c>/actor/challenge</c>
/// answers normally and reveals nothing about the handle. <see cref="VerifySignedChallenge"/> always answers
/// not confirmed, so <c>/actor/confirm</c> gives its usual generic 401. Neither throws.
/// </para>
/// </summary>
public sealed class RefusingAccountConfirmation : IAccountConfirmation
{
    private readonly TimeProvider _time;

    /// <summary>Creates the confirmation flow.</summary>
    /// <param name="timeProvider">The clock for the challenge's expiry; <see cref="TimeProvider.System"/> when null.</param>
    public RefusingAccountConfirmation(TimeProvider? timeProvider = null)
    {
        _time = timeProvider ?? TimeProvider.System;
    }

    /// <summary>Answers with a random challenge that has already expired and can never be confirmed.</summary>
    /// <param name="personHandle">The handle; echoed back, never looked up.</param>
    /// <returns>The challenge.</returns>
    public ConfirmationChallenge CreateChallenge(string personHandle)
    {
        return new ConfirmationChallenge
        {
            PersonHandle = personHandle ?? string.Empty,
            ChallengeBytes = RandomNumberGenerator.GetBytes(32),
            ExpiresUtc = _time.GetUtcNow().UtcDateTime,
        };
    }

    /// <summary>Never confirms.</summary>
    /// <param name="personHandle">The handle.</param>
    /// <param name="signedChallenge">The signature; ignored.</param>
    /// <returns>A result with <see cref="ConfirmationResult.Confirmed"/> false.</returns>
    public ConfirmationResult VerifySignedChallenge(string personHandle, byte[] signedChallenge)
    {
        return new ConfirmationResult { Confirmed = false };
    }
}
