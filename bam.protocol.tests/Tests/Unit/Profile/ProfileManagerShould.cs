using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("ProfileManager Should", Selector = "pms")]
public class ProfileManagerShould : UnitTestMenuContainer
{
    private static IProfileRepository CreateRepository(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
        AesKey aesKey = new AesKey();
        ICompositeKeyCalculator compositeKeyCalculator = new CompositeKeyCalculator();
        IObjectDataIdentityCalculator identityCalculator = new ObjectDataIdentityCalculator();
        IObjectDataLocatorFactory locatorFactory = new ObjectDataLocatorFactory(identityCalculator);
        IObjectEncoderDecoder encoderDecoder = new JsonObjectDataEncoder();
        IObjectDataFactory factory = new ObjectDataFactory(locatorFactory, encoderDecoder);
        IRootStorageHolder rootStorage = new RootStorageHolder(rootPath);
        IObjectDataStorageManager storageManager = new EncryptedFsObjectDataStorageManager(rootStorage, factory, new AesEncryptor(aesKey), new AesDecryptor(aesKey));
        IObjectDataWriter writer = new ObjectDataWriter(factory, storageManager);
        IObjectDataReader reader = new ObjectDataReader(storageManager);
        IObjectDataIndexer indexer = new ObjectDataIndexer(storageManager, compositeKeyCalculator);
        IObjectDataSearchIndexer searchIndexer = new ObjectDataSearchIndexer(storageManager, indexer);
        IObjectDataSearcher searcher = new ObjectDataSearcher(searchIndexer, reader, indexer);
        IObjectDataDeleter deleter = new ObjectDataDeleter(factory, storageManager, compositeKeyCalculator);
        IObjectDataArchiver archiver = new ObjectDataArchiver(factory, storageManager, compositeKeyCalculator);
        ObjectDataRepository repo = new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
        return new EncryptedProfileRepository(repo);
    }

    [UnitTest]
    public void RegisterPersonProfile()
    {
        When.A<ProfileManager>("registers a person profile",
            () => new ProfileManager(CreateRepository(nameof(RegisterPersonProfile))),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Test",
                    LastName = "User",
                    Phone = "555-1234",
                    Email = "test@example.com",
                };

                IProfile profile = manager.RegisterPersonProfile(registration);
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("ProfileHandle is not empty", p => !string.IsNullOrEmpty(p.ProfileHandle))
                .As<IProfile>("PersonHandle is not empty", p => !string.IsNullOrEmpty(p.PersonHandle))
                .As<IProfile>("Name is set", p => !string.IsNullOrEmpty(p.Name));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindProfileByProfileHandle()
    {
        When.A<ProfileManager>("finds a profile by profile handle",
            () => new ProfileManager(CreateRepository(nameof(FindProfileByProfileHandle))),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Find",
                    LastName = "ByHandle",
                    Handle = "findHandle1",
                };

                IProfile registered = manager.RegisterPersonProfile(registration);
                IProfile found = manager.FindProfileByHandle(registered.ProfileHandle);
                return new object[] { registered, found };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("found is not null", r => r[1] != null)
                .As<object[]>("ProfileHandle matches", r => ((IProfile)r[1])?.ProfileHandle == ((IProfile)r[0]).ProfileHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindProfileByPersonHandle()
    {
        When.A<ProfileManager>("finds a profile by person handle",
            () => new ProfileManager(CreateRepository(nameof(FindProfileByPersonHandle))),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Find",
                    LastName = "ByPerson",
                    Handle = "personHandle1",
                };

                IProfile registered = manager.RegisterPersonProfile(registration);
                IProfile found = manager.FindProfileByHandle(registered.PersonHandle);
                return new object[] { registered, found };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("found is not null", r => r[1] != null)
                .As<object[]>("PersonHandle matches", r => ((IProfile)r[1])?.PersonHandle == ((IProfile)r[0]).PersonHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetProfileCreatesIfNotExists()
    {
        When.A<ProfileManager>("creates profile if not exists",
            () => new ProfileManager(CreateRepository(nameof(GetProfileCreatesIfNotExists))),
            (manager) =>
            {
                IProfile profile = manager.GetProfile("nonexistent", createIfNotExists: true);
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("ProfileHandle is not empty", p => !string.IsNullOrEmpty(p.ProfileHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetProfileReturnsNullIfNotExists()
    {
        When.A<ProfileManager>("returns null if profile not exists",
            () => new ProfileManager(CreateRepository(nameof(GetProfileReturnsNullIfNotExists))),
            (manager) =>
            {
                IProfile profile = manager.GetProfile("nonexistent", createIfNotExists: false);
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", because.Result == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateProfile()
    {
        When.A<ProfileManager>("creates a blank profile",
            () => new ProfileManager(CreateRepository(nameof(CreateProfile))),
            (manager) =>
            {
                IProfile profile = manager.CreateProfile();
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("ProfileHandle is not empty", p => !string.IsNullOrEmpty(p.ProfileHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolvePublicKeyDigestDeterministicallyWhenDuplicateMaterialExists()
    {
        string testName = nameof(ResolvePublicKeyDigestDeterministicallyWhenDuplicateMaterialExists);
        string rootPath = $"./.bam/tests/{testName}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        DateTime baseline = DateTime.UtcNow;
        DuplicateFixture fixture = CreateDuplicateFixture(testName);

        When.A<ProfileManager>("resolves a public key digest with planted cross-handle duplicates",
            () => new ProfileManager(fixture.ProfileRepository),
            (manager) =>
            {
                PlantProfile(fixture.ObjectDataRepository, "late-claimant", "Late Claimant");
                PlantProfile(fixture.ObjectDataRepository, "original-owner", "Original Owner");
                // the later-created claim is INSERTED first so enumeration order cannot decide
                PlantKeySet(fixture.ObjectDataRepository, "late-claimant", "victim-material", baseline);
                PlantKeySet(fixture.ObjectDataRepository, "original-owner", "victim-material", baseline.AddHours(-1));

                string digest = "victim-material".Sha256();
                IProfile firstCall = manager.FindProfileByPublicKey(digest);
                IProfile secondCall = manager.FindProfileByPublicKey(digest);

                return new DigestDeterminismOutcome(firstCall?.ProfileHandle, secondCall?.ProfileHandle);
            })
        .TheTest
        .ShouldPass<DigestDeterminismOutcome>((because, outcome) =>
        {
            because.ItsTrue("the earliest-created registration wins", "original-owner".Equals(outcome.FirstHandle));
            because.ItsTrue("resolution is stable across calls", string.Equals(outcome.FirstHandle, outcome.SecondHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindProfileByPublicKeyPem()
    {
        string testName = nameof(FindProfileByPublicKeyPem);
        string rootPath = $"./.bam/tests/{testName}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        DateTime baseline = DateTime.UtcNow;
        DuplicateFixture fixture = CreateDuplicateFixture(testName);

        When.A<ProfileManager>("resolves a profile from full PEM material",
            () => new ProfileManager(fixture.ProfileRepository),
            (manager) =>
            {
                PlantProfile(fixture.ObjectDataRepository, "pem-owner", "Pem Owner");
                PlantKeySet(fixture.ObjectDataRepository, "pem-owner", "pem-material", baseline);

                IProfile found = manager.FindProfileByPublicKeyPem("pem-material");
                IProfile missing = manager.FindProfileByPublicKeyPem("no-such-material");

                return new PemLookupOutcome(found?.ProfileHandle, missing == null);
            })
        .TheTest
        .ShouldPass<PemLookupOutcome>((because, outcome) =>
        {
            because.ItsTrue("the key set's handle resolves its profile", "pem-owner".Equals(outcome.FoundHandle));
            because.ItsTrue("unregistered material resolves to null", outcome.MissingIsNull);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record DuplicateFixture(ObjectDataRepository ObjectDataRepository, IProfileRepository ProfileRepository);

    // Mirrors CreateRepository but keeps a handle on the underlying ObjectDataRepository so
    // tests can plant rows directly (bypassing the registrar) — the legacy/tampered-store
    // scenario the deterministic resolution exists for.
    private static DuplicateFixture CreateDuplicateFixture(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
        AesKey aesKey = new AesKey();
        ICompositeKeyCalculator compositeKeyCalculator = new CompositeKeyCalculator();
        IObjectDataIdentityCalculator identityCalculator = new ObjectDataIdentityCalculator();
        IObjectDataLocatorFactory locatorFactory = new ObjectDataLocatorFactory(identityCalculator);
        IObjectEncoderDecoder encoderDecoder = new JsonObjectDataEncoder();
        IObjectDataFactory factory = new ObjectDataFactory(locatorFactory, encoderDecoder);
        IRootStorageHolder rootStorage = new RootStorageHolder(rootPath);
        IObjectDataStorageManager storageManager = new EncryptedFsObjectDataStorageManager(rootStorage, factory, new AesEncryptor(aesKey), new AesDecryptor(aesKey));
        IObjectDataWriter writer = new ObjectDataWriter(factory, storageManager);
        IObjectDataReader reader = new ObjectDataReader(storageManager);
        IObjectDataIndexer indexer = new ObjectDataIndexer(storageManager, compositeKeyCalculator);
        IObjectDataSearchIndexer searchIndexer = new ObjectDataSearchIndexer(storageManager, indexer);
        IObjectDataSearcher searcher = new ObjectDataSearcher(searchIndexer, reader, indexer);
        IObjectDataDeleter deleter = new ObjectDataDeleter(factory, storageManager, compositeKeyCalculator);
        IObjectDataArchiver archiver = new ObjectDataArchiver(factory, storageManager, compositeKeyCalculator);
        ObjectDataRepository repo = new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
        return new DuplicateFixture(repo, new EncryptedProfileRepository(repo));
    }

    private static void PlantProfile(ObjectDataRepository repository, string profileHandle, string name)
    {
        repository.Create(new ProfileData
        {
            ProfileHandle = profileHandle,
            Name = name,
            Uuid = Guid.NewGuid().ToString(),
            Cuid = Bam.Cuid.Generate()
        });
    }

    private static void PlantKeySet(ObjectDataRepository repository, string handle, string rsaPem, DateTime created)
    {
        repository.Create(new PublicKeySetData
        {
            KeySetHandle = handle,
            PublicRsaKey = rsaPem,
            Created = created,
            Uuid = Guid.NewGuid().ToString(),
            Cuid = Bam.Cuid.Generate()
        });
    }

    private sealed record DigestDeterminismOutcome(string? FirstHandle, string? SecondHandle);

    private sealed record PemLookupOutcome(string? FoundHandle, bool MissingIsNull);
}
