using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

/// <summary>The minimum interval between rotations of one handle (bam.protocol#89).</summary>
[UnitTestMenu("KeySetRotationInterval Should", Selector = "ksri")]
public class KeySetRotationIntervalShould : UnitTestMenuContainer
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);
    private static readonly DateTimeOffset Start = new DateTimeOffset(2026, 11, 13, 12, 0, 0, TimeSpan.Zero);

    [UnitTest]
    public void RefuseARotationInsideTheIntervalAndRestartItAfterOne()
    {
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair third = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair fourth = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(RefuseARotationInsideTheIntervalAndRestartItAfterOne));
        ManualClock clock = new ManualClock(Start);

        When.A<PublicKeySetRegistrar>("rotates straight after registering, a minute later, at the interval, and a minute after that",
            () => Registrar(repository, new KeySetRotationPolicy(Interval, clock)),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "rotator", PublicRsaKey = first.PublicKeyPem });
                DateTime? stampedAtRegistration = registrar.Resolve("rotator")!.LastRotatedUtc;
                RotationAttempt firstRotation = TryRotate(registrar, "rotator", first, second);
                DateTime? stampedAtFirst = registrar.Resolve("rotator")!.LastRotatedUtc;

                clock.Advance(TimeSpan.FromMinutes(1));
                int rowsBeforeEarly = RowsFor(repository, "rotator");
                RotationAttempt early = TryRotate(registrar, "rotator", second, third);
                string keyAfterEarly = registrar.Resolve("rotator")!.PublicRsaKey;
                int rowsAfterEarly = RowsFor(repository, "rotator");

                clock.Advance(TimeSpan.FromMinutes(9));
                RotationAttempt atInterval = TryRotate(registrar, "rotator", second, third);
                PublicKeySetData afterRotation = registrar.Resolve("rotator")!;

                clock.Advance(TimeSpan.FromMinutes(1));
                RotationAttempt soonAfter = TryRotate(registrar, "rotator", third, fourth);
                string keyAfterSoon = registrar.Resolve("rotator")!.PublicRsaKey;
                return new IntervalOutcome(stampedAtRegistration, firstRotation, stampedAtFirst, early, keyAfterEarly, rowsAfterEarly - rowsBeforeEarly,
                    atInterval, afterRotation.PublicRsaKey, afterRotation.LastRotatedUtc, soonAfter, keyAfterSoon);
            })
        .TheTest
        .ShouldPass<IntervalOutcome>((because, outcome) =>
        {
            because.ItsTrue("a fresh registration has never rotated", outcome.StampedAtRegistration is null);
            because.ItsTrue("so its first rotation is not delayed", outcome.FirstRotation.Succeeded);
            because.ItsTrue("and stamps when it happened, from the policy's clock", SameInstant(outcome.StampedAtFirst, Start));
            because.ItsTrue("a second rotation one minute later is refused as too soon", outcome.Early.TooSoon);
            because.ItsTrue("the refusal is still an InvalidKeySetRotationException, so existing handlers fail closed", outcome.Early.Invalid);
            because.ItsTrue($"and says when to retry: nine minutes ({outcome.Early.RetryAfter})", outcome.Early.RetryAfter == TimeSpan.FromMinutes(9));
            because.ItsTrue("the refused rotation changed nothing", outcome.KeyAfterEarly == second.PublicKeyPem);
            because.ItsTrue($"and wrote nothing: no tombstone for the key it would have retired ({outcome.RowsWrittenByEarly} rows)", outcome.RowsWrittenByEarly == 0);
            because.ItsTrue("a rotation at the interval succeeds", outcome.AtInterval.Succeeded);
            because.ItsTrue("and moves the handle to the new key", outcome.KeyAfterRotation == third.PublicKeyPem);
            because.ItsTrue("and restamps the rotation time", SameInstant(outcome.StampedAtRotation, Start + Interval));
            because.ItsTrue($"so the next rotation a minute later waits nine minutes again ({outcome.SoonAfter.RetryAfter})", outcome.SoonAfter.TooSoon && outcome.SoonAfter.RetryAfter == TimeSpan.FromMinutes(9));
            because.ItsTrue("and leaves the key alone", outcome.KeyAfterSoon == third.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void WaitTheWholeIntervalWhenTheClockGoesBack()
    {
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair third = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(WaitTheWholeIntervalWhenTheClockGoesBack));
        ManualClock clock = new ManualClock(Start);

        When.A<PublicKeySetRegistrar>("rotates, then rotates again after the clock is set back five minutes, and again at the interval of the set-back clock",
            () => Registrar(repository, new KeySetRotationPolicy(Interval, clock)),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "rewound", PublicRsaKey = first.PublicKeyPem });
                RotationAttempt firstRotation = TryRotate(registrar, "rewound", first, second);
                clock.Advance(TimeSpan.FromMinutes(-5));
                RotationAttempt afterRewind = TryRotate(registrar, "rewound", second, third);
                clock.Advance(Interval);
                RotationAttempt atRewoundInterval = TryRotate(registrar, "rewound", second, third);
                string keyAfter = registrar.Resolve("rewound")!.PublicRsaKey;
                return new RewindOutcome(firstRotation, afterRewind, atRewoundInterval, keyAfter);
            })
        .TheTest
        .ShouldPass<RewindOutcome>((because, outcome) =>
        {
            because.ItsTrue("the first rotation succeeds", outcome.First.Succeeded);
            because.ItsTrue("a rotation with the clock set back before the stamp is refused, not let through", outcome.AfterRewind.TooSoon);
            because.ItsTrue($"and waits the whole interval ({outcome.AfterRewind.RetryAfter})", outcome.AfterRewind.RetryAfter == Interval);
            because.ItsTrue($"an interval later on the set-back clock, which is five minutes short of the stamp plus the interval, it is refused again ({outcome.AtRewoundInterval.RetryAfter})",
                outcome.AtRewoundInterval.TooSoon && outcome.AtRewoundInterval.RetryAfter == TimeSpan.FromMinutes(5));
            because.ItsTrue("and the key never moved", outcome.KeyAfter == second.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record RewindOutcome(RotationAttempt First, RotationAttempt AfterRewind, RotationAttempt AtRewoundInterval, string KeyAfter);

    [UnitTest]
    public void LetARowFromBeforeTheFieldRotateAtOnceAndThenSpaceItsRotations()
    {
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair third = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(LetARowFromBeforeTheFieldRotateAtOnceAndThenSpaceItsRotations));

        When.A<ObjectDataRepository>("rotates a row written before LastRotatedUtc existed, then again a minute later",
            () => repository,
            store =>
            {
                // Written straight to the store, as a row registered before the field existed. (Clearing the field
                // through Update would not do: the store keeps a property's old value when it is set to null,
                // BryanApellanes/bam.data.objects#9.)
                store.Create(new PublicKeySetData
                {
                    KeySetHandle = "legacy",
                    PublicRsaKey = first.PublicKeyPem,
                    // Only so the row is a valid one: nothing reads Created for the interval.
                    Created = DateTime.UtcNow,
                    Uuid = Guid.NewGuid().ToString(),
                    Cuid = Bam.Cuid.Generate(),
                });
                ManualClock clock = new ManualClock(Start);
                PublicKeySetRegistrar limited = Registrar(store, new KeySetRotationPolicy(Interval, clock));
                bool legacyHadNoTime = limited.Resolve("legacy")!.LastRotatedUtc is null;
                RotationAttempt once = TryRotate(limited, "legacy", first, second);
                clock.Advance(TimeSpan.FromMinutes(1));
                RotationAttempt again = TryRotate(limited, "legacy", second, third);
                return new LegacyOutcome(legacyHadNoTime, once, again);
            })
        .TheTest
        .ShouldPass<LegacyOutcome>((because, outcome) =>
        {
            because.ItsTrue("the row has no rotation time", outcome.LegacyHadNoTime);
            because.ItsTrue("so it may rotate at once", outcome.Once.Succeeded);
            because.ItsTrue("and that rotation starts the interval: another a minute later is refused", outcome.Again.TooSoon);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ClearACallerSuppliedRotationTime()
    {
        RsaPublicPrivateKeyPair keys = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair next = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(ClearACallerSuppliedRotationTime));

        When.A<PublicKeySetRegistrar>("registers a key set that claims it rotated just now, then rotates it",
            () => Registrar(repository, new KeySetRotationPolicy(Interval, new ManualClock(Start))),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "planter", PublicRsaKey = keys.PublicKeyPem, LastRotatedUtc = Start.UtcDateTime });
                DateTime? stored = registrar.Resolve("planter")!.LastRotatedUtc;
                return new PlantedOutcome(stored, TryRotate(registrar, "planter", keys, next));
            })
        .TheTest
        .ShouldPass<PlantedOutcome>((because, outcome) =>
        {
            because.ItsTrue($"the registrar drops the caller's value ({outcome.Stored})", outcome.Stored is null);
            because.ItsTrue("so the planted time does not delay the handle's first rotation", outcome.FirstRotation.Succeeded);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ChangeNothingByDefault()
    {
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair third = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(ChangeNothingByDefault));

        When.A<PublicKeySetRegistrar>("rotates twice in a row with the default policy",
            () => new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider())),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "quick", PublicRsaKey = first.PublicKeyPem });
                RotationAttempt once = TryRotate(registrar, "quick", first, second);
                RotationAttempt twice = TryRotate(registrar, "quick", second, third);
                return new DefaultOutcome(once, twice, registrar.Resolve("quick")!.PublicRsaKey);
            })
        .TheTest
        .ShouldPass<DefaultOutcome>((because, outcome) =>
        {
            because.ItsTrue("the first rotation succeeds", outcome.Once.Succeeded);
            because.ItsTrue("and so does one straight after it", outcome.Twice.Succeeded);
            because.ItsTrue("leaving the third key", outcome.FinalKey == third.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseAnIntervalOutOfRange()
    {
        When.A<string>("builds policies with a negative interval, one over two days, two days, and zero",
            () => "policies",
            _ => new RangeOutcome(
                Refused(TimeSpan.FromSeconds(-1)),
                Refused(TimeSpan.FromDays(2) + TimeSpan.FromSeconds(1)),
                Refused(TimeSpan.FromDays(2)),
                Refused(TimeSpan.Zero),
                KeySetRotationPolicy.None.MinimumInterval))
        .TheTest
        .ShouldPass<RangeOutcome>((because, outcome) =>
        {
            because.ItsTrue("a negative interval is refused", outcome.NegativeRefused);
            because.ItsTrue("one over two days is refused", outcome.OverTwoDaysRefused);
            because.ItsTrue("two days, twice the longest proof age, is accepted", !outcome.TwoDaysRefused);
            because.ItsTrue("zero is accepted", !outcome.ZeroRefused);
            because.ItsTrue("the default policy has no interval", outcome.DefaultInterval == TimeSpan.Zero);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void UseAPolicyTheHostBindsAfterTheProfileRepository()
    {
        string rootPath = $"./.bam/tests/{nameof(UseAPolicyTheHostBindsAfterTheProfileRepository)}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair third = new RsaPublicPrivateKeyPair();
        ManualClock clock = new ManualClock(Start);

        When.A<ServiceRegistry>("binds a ten-minute policy after AddEncryptedProfileRepository and rotates twice a minute apart",
            () =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.AddEncryptedProfileRepository(rootPath);
                registry.For<KeySetRotationPolicy>().Use(new KeySetRotationPolicy(Interval, clock));
                return registry;
            },
            registry => RotateTwiceAMinuteApart(registry.Get<IPublicKeySetRegistrar>(), "hosted", first, second, third, clock))
        .TheTest
        .ShouldPass<RotationAttempt>((because, _, attempt) =>
        {
            because.ItsTrue("the registry's registrar applies the host's policy", attempt.TooSoon);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void KeepAPolicyTheHostBindsBeforeTheProfileRepository()
    {
        string root = $"./.bam/tests/{nameof(KeepAPolicyTheHostBindsBeforeTheProfileRepository)}";
        foreach (string path in new[] { root + "-before", root + "-none" })
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair third = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair other = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair otherNext = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair otherLast = new RsaPublicPrivateKeyPair();
        ManualClock clock = new ManualClock(Start);

        When.A<string>("binds a ten-minute policy before AddEncryptedProfileRepository, and binds none in a second registry",
            () => root,
            _ =>
            {
                ServiceRegistry before = new ServiceRegistry();
                before.For<KeySetRotationPolicy>().Use(new KeySetRotationPolicy(Interval, clock));
                before.AddEncryptedProfileRepository(root + "-before");
                RotationAttempt boundBefore = RotateTwiceAMinuteApart(before.Get<IPublicKeySetRegistrar>(), "early-bound", first, second, third, clock);

                ServiceRegistry none = new ServiceRegistry();
                none.AddEncryptedProfileRepository(root + "-none");
                RotationAttempt nothingBound = RotateTwiceAMinuteApart(none.Get<IPublicKeySetRegistrar>(), "unbound", other, otherNext, otherLast, clock);
                return new BindOrderOutcome(boundBefore, nothingBound);
            })
        .TheTest
        .ShouldPass<BindOrderOutcome>((because, outcome) =>
        {
            because.ItsTrue("a policy bound before the profile repository is kept, not replaced by None", outcome.BoundBefore.TooSoon);
            because.ItsTrue("with none bound, the registry's default sets no limit", outcome.NothingBound.Succeeded);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void TellOnlyTheKeyHolderThatAHandleRotatedRecently()
    {
        RsaPublicPrivateKeyPair current = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair rotated = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair stranger = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair next = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(TellOnlyTheKeyHolderThatAHandleRotatedRecently));
        ManualClock clock = new ManualClock(Start);

        When.A<PublicKeySetRegistrar>("rotates a handle inside its interval with a signature from a key that is not the handle's",
            () => Registrar(repository, new KeySetRotationPolicy(Interval, clock)),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "watched", PublicRsaKey = current.PublicKeyPem });
                RotationAttempt first = TryRotate(registrar, "watched", current, rotated);
                clock.Advance(TimeSpan.FromMinutes(1));
                PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "watched", PublicRsaKey = next.PublicKeyPem };
                RotationAttempt forged = Attempt(() => registrar.Rotate(proposed, PublicKeySetRegistrarShould.SignRotation(stranger, rotated.PublicKeyPem, proposed)));
                return new ForgedOutcome(first, forged);
            })
        .TheTest
        .ShouldPass<ForgedOutcome>((because, outcome) =>
        {
            because.ItsTrue("the handle rotated a minute ago", outcome.First.Succeeded);
            because.ItsTrue("the forged rotation is refused", !outcome.Forged.Succeeded && outcome.Forged.Invalid);
            because.ItsTrue("as an unproven rotation, not as one too soon: nothing says when the handle last rotated", !outcome.Forged.TooSoon && outcome.Forged.RetryAfter is null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static RotationAttempt RotateTwiceAMinuteApart(IPublicKeySetRegistrar registrar, string handle, RsaPublicPrivateKeyPair first, RsaPublicPrivateKeyPair second, RsaPublicPrivateKeyPair third, ManualClock clock)
    {
        registrar.Register(new PublicKeySetData { KeySetHandle = handle, PublicRsaKey = first.PublicKeyPem });
        RotationAttempt once = TryRotate(registrar, handle, first, second);
        if (!once.Succeeded)
        {
            throw new InvalidOperationException("The first rotation of a fresh handle was refused.");
        }
        clock.Advance(TimeSpan.FromMinutes(1));
        return TryRotate(registrar, handle, second, third);
    }

    private static int RowsFor(ObjectDataRepository repository, string handle)
    {
        return repository.RetrieveAll<PublicKeySetData>().Count(keySet => keySet.KeySetHandle == handle);
    }

    private static PublicKeySetRegistrar Registrar(ObjectDataRepository repository, KeySetRotationPolicy policy)
    {
        return new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider()), new PublicKeySetResolver(repository), policy);
    }

    private static RotationAttempt TryRotate(IPublicKeySetRegistrar registrar, string handle, RsaPublicPrivateKeyPair current, RsaPublicPrivateKeyPair next)
    {
        PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = handle, PublicRsaKey = next.PublicKeyPem };
        return Attempt(() => registrar.Rotate(proposed, PublicKeySetRegistrarShould.SignRotation(current, current.PublicKeyPem, proposed)));
    }

    private static RotationAttempt Attempt(Action rotate)
    {
        try
        {
            rotate();
            return new RotationAttempt(true, false, false, null);
        }
        catch (KeySetRotationTooSoonException tooSoon)
        {
            return new RotationAttempt(false, true, tooSoon is InvalidKeySetRotationException, tooSoon.RetryAfter);
        }
        catch (InvalidKeySetRotationException)
        {
            return new RotationAttempt(false, false, true, null);
        }
    }

    private static bool Refused(TimeSpan interval)
    {
        try
        {
            _ = new KeySetRotationPolicy(interval, TimeProvider.System);
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return true;
        }
    }

    private static bool SameInstant(DateTime? stamped, DateTimeOffset expected)
    {
        if (stamped is null)
        {
            return false;
        }
        DateTime utc = stamped.Value.Kind == DateTimeKind.Local ? stamped.Value.ToUniversalTime() : DateTime.SpecifyKind(stamped.Value, DateTimeKind.Utc);
        return Math.Abs((utc - expected.UtcDateTime).TotalMilliseconds) < 1;
    }

    /// <summary>A clock the test moves by hand.</summary>
    private sealed class ManualClock : TimeProvider
    {
        private DateTimeOffset _now;

        public ManualClock(DateTimeOffset start)
        {
            _now = start;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _now;
        }

        public void Advance(TimeSpan by)
        {
            _now += by;
        }
    }

    private sealed record RotationAttempt(bool Succeeded, bool TooSoon, bool Invalid, TimeSpan? RetryAfter);

    private sealed record IntervalOutcome(DateTime? StampedAtRegistration, RotationAttempt FirstRotation, DateTime? StampedAtFirst, RotationAttempt Early, string KeyAfterEarly, int RowsWrittenByEarly,
        RotationAttempt AtInterval, string KeyAfterRotation, DateTime? StampedAtRotation, RotationAttempt SoonAfter, string KeyAfterSoon);

    private sealed record LegacyOutcome(bool LegacyHadNoTime, RotationAttempt Once, RotationAttempt Again);

    private sealed record PlantedOutcome(DateTime? Stored, RotationAttempt FirstRotation);

    private sealed record DefaultOutcome(RotationAttempt Once, RotationAttempt Twice, string FinalKey);

    private sealed record RangeOutcome(bool NegativeRefused, bool OverTwoDaysRefused, bool TwoDaysRefused, bool ZeroRefused, TimeSpan DefaultInterval);

    private sealed record BindOrderOutcome(RotationAttempt BoundBefore, RotationAttempt NothingBound);

    private sealed record ForgedOutcome(RotationAttempt First, RotationAttempt Forged);
}
