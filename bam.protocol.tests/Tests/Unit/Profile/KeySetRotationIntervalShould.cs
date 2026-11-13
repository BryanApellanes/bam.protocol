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
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(RefuseARotationInsideTheIntervalAndRestartItAfterOne));
        ManualClock clock = new ManualClock(Start);

        When.A<PublicKeySetRegistrar>("rotates one minute after registering, at the interval, and one minute after that",
            () => Registrar(repository, new KeySetRotationPolicy(Interval, clock)),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "rotator", PublicRsaKey = first.PublicKeyPem });
                DateTime? stampedAtRegistration = registrar.Resolve("rotator")!.KeyMaterialSinceUtc;

                clock.Advance(TimeSpan.FromMinutes(1));
                RotationAttempt early = TryRotate(registrar, "rotator", first, second);
                string keyAfterEarly = registrar.Resolve("rotator")!.PublicRsaKey;

                clock.Advance(TimeSpan.FromMinutes(9));
                RotationAttempt atInterval = TryRotate(registrar, "rotator", first, second);
                PublicKeySetData afterRotation = registrar.Resolve("rotator")!;

                clock.Advance(TimeSpan.FromMinutes(1));
                RotationAttempt soonAfter = TryRotate(registrar, "rotator", second, third);
                string keyAfterSoon = registrar.Resolve("rotator")!.PublicRsaKey;
                return new IntervalOutcome(stampedAtRegistration, early, keyAfterEarly, atInterval, afterRotation.PublicRsaKey, afterRotation.KeyMaterialSinceUtc, soonAfter, keyAfterSoon);
            })
        .TheTest
        .ShouldPass<IntervalOutcome>((because, outcome) =>
        {
            because.ItsTrue("registration stamps when the keys were set, from the policy's clock", SameInstant(outcome.StampedAtRegistration, Start));
            because.ItsTrue("a rotation one minute after registering is refused as too soon", outcome.Early.TooSoon);
            because.ItsTrue("the refusal is still an InvalidKeySetRotationException, so existing handlers fail closed", outcome.Early.Invalid);
            because.ItsTrue($"and says when to retry: nine minutes ({outcome.Early.RetryAfter})", outcome.Early.RetryAfter == TimeSpan.FromMinutes(9));
            because.ItsTrue("the refused rotation changed nothing", outcome.KeyAfterEarly == first.PublicKeyPem);
            because.ItsTrue("a rotation at the interval succeeds", outcome.AtInterval.Succeeded);
            because.ItsTrue("and moves the handle to the new key", outcome.KeyAfterRotation == second.PublicKeyPem);
            because.ItsTrue("and restamps when the keys were set", SameInstant(outcome.StampedAtRotation, Start + Interval));
            because.ItsTrue($"so the next rotation a minute later waits nine minutes again ({outcome.SoonAfter.RetryAfter})", outcome.SoonAfter.TooSoon && outcome.SoonAfter.RetryAfter == TimeSpan.FromMinutes(9));
            because.ItsTrue("and leaves the key alone", outcome.KeyAfterSoon == second.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void JudgeARowWithoutAKeyMaterialTimeFromItsCreation()
    {
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(JudgeARowWithoutAKeyMaterialTimeFromItsCreation));

        When.A<ObjectDataRepository>("rotates a row written before KeyMaterialSinceUtc existed, a minute after and an interval after its creation",
            () => repository,
            store =>
            {
                // Written straight to the store, as a row registered before the field existed: no KeyMaterialSinceUtc.
                // (Clearing the field through Update would not do: the store keeps a property's old value when it is
                // set to null, BryanApellanes/bam.data.objects#9.)
                store.Create(new PublicKeySetData
                {
                    KeySetHandle = "legacy",
                    PublicRsaKey = first.PublicKeyPem,
                    Created = DateTime.UtcNow,
                    Uuid = Guid.NewGuid().ToString(),
                    Cuid = Bam.Cuid.Generate(),
                });
                PublicKeySetData row = new PublicKeySetResolver(store).ResolveByHandle("legacy")!;
                DateTimeOffset created = new DateTimeOffset(DateTime.SpecifyKind(row.Created!.Value, DateTimeKind.Utc).ToUniversalTime());

                ManualClock clock = new ManualClock(created + TimeSpan.FromMinutes(1));
                PublicKeySetRegistrar limited = Registrar(store, new KeySetRotationPolicy(Interval, clock));
                bool legacyHadNoTime = limited.Resolve("legacy")!.KeyMaterialSinceUtc is null;
                RotationAttempt early = TryRotate(limited, "legacy", first, second);
                clock.Advance(Interval);
                RotationAttempt later = TryRotate(limited, "legacy", first, second);
                return new LegacyOutcome(legacyHadNoTime, early, later);
            })
        .TheTest
        .ShouldPass<LegacyOutcome>((because, outcome) =>
        {
            because.ItsTrue("the row has no key-material time", outcome.LegacyHadNoTime);
            because.ItsTrue("a rotation a minute after the row was created is refused", outcome.Early.TooSoon);
            because.ItsTrue("one an interval later succeeds", outcome.Later.Succeeded);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void OverwriteACallerSuppliedKeyMaterialTime()
    {
        RsaPublicPrivateKeyPair keys = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = PublicKeySetRegistrarShould.CreateObjectDataRepository(nameof(OverwriteACallerSuppliedKeyMaterialTime));

        When.A<PublicKeySetRegistrar>("registers a key set that claims its keys were set in 2000",
            () => Registrar(repository, new KeySetRotationPolicy(Interval, new ManualClock(Start))),
            registrar =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "planter", PublicRsaKey = keys.PublicKeyPem, KeyMaterialSinceUtc = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
                return registrar.Resolve("planter")!.KeyMaterialSinceUtc;
            })
        .TheTest
        .ShouldPass<DateTime?>((because, _, stamped) =>
        {
            because.ItsTrue($"the registrar's own time replaces it ({stamped})", SameInstant(stamped, Start));
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
        When.A<string>("builds policies with a negative interval, one over a day, a day, and zero",
            () => "policies",
            _ => new RangeOutcome(
                Refused(TimeSpan.FromSeconds(-1)),
                Refused(KeySetRotationPolicy.MaxInterval + TimeSpan.FromSeconds(1)),
                Refused(KeySetRotationPolicy.MaxInterval),
                Refused(TimeSpan.Zero),
                KeySetRotationPolicy.None.MinimumInterval))
        .TheTest
        .ShouldPass<RangeOutcome>((because, outcome) =>
        {
            because.ItsTrue("a negative interval is refused", outcome.NegativeRefused);
            because.ItsTrue("one over a day is refused", outcome.OverADayRefused);
            because.ItsTrue("a day is accepted", !outcome.ADayRefused);
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
        ManualClock clock = new ManualClock(Start);

        When.A<ServiceRegistry>("binds a ten-minute policy after AddEncryptedProfileRepository and rotates a minute after registering",
            () =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.AddEncryptedProfileRepository(rootPath);
                registry.For<KeySetRotationPolicy>().Use(new KeySetRotationPolicy(Interval, clock));
                return registry;
            },
            registry =>
            {
                IPublicKeySetRegistrar registrar = registry.Get<IPublicKeySetRegistrar>();
                registrar.Register(new PublicKeySetData { KeySetHandle = "hosted", PublicRsaKey = first.PublicKeyPem });
                clock.Advance(TimeSpan.FromMinutes(1));
                return TryRotate(registrar, "hosted", first, second);
            })
        .TheTest
        .ShouldPass<RotationAttempt>((because, _, attempt) =>
        {
            because.ItsTrue("the registry's registrar applies the host's policy", attempt.TooSoon);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static PublicKeySetRegistrar Registrar(ObjectDataRepository repository, KeySetRotationPolicy policy)
    {
        return new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider()), new PublicKeySetResolver(repository), policy);
    }

    private static RotationAttempt TryRotate(IPublicKeySetRegistrar registrar, string handle, RsaPublicPrivateKeyPair current, RsaPublicPrivateKeyPair next)
    {
        PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = handle, PublicRsaKey = next.PublicKeyPem };
        try
        {
            registrar.Rotate(proposed, PublicKeySetRegistrarShould.SignRotation(current, current.PublicKeyPem, proposed));
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

    private sealed record IntervalOutcome(DateTime? StampedAtRegistration, RotationAttempt Early, string KeyAfterEarly, RotationAttempt AtInterval, string KeyAfterRotation, DateTime? StampedAtRotation, RotationAttempt SoonAfter, string KeyAfterSoon);

    private sealed record LegacyOutcome(bool LegacyHadNoTime, RotationAttempt Early, RotationAttempt Later);

    private sealed record DefaultOutcome(RotationAttempt Once, RotationAttempt Twice, string FinalKey);

    private sealed record RangeOutcome(bool NegativeRefused, bool OverADayRefused, bool ADayRefused, bool ZeroRefused, TimeSpan DefaultInterval);
}
