using Bam.DependencyInjection;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("ProfileRepositoryServiceRegistration Should", Selector = "prsr")]
public class ProfileRepositoryServiceRegistrationShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ResolveProfileRepositoryHonoringRegisteredRegistrar()
    {
        string rootPath = $"./.bam/tests/{nameof(ResolveProfileRepositoryHonoringRegisteredRegistrar)}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }

        When.A<IProfileRepository>("resolves IProfileRepository using the registry-supplied IPublicKeySetRegistrar",
            () =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.AddEncryptedProfileRepository(rootPath);
                // Override the registrar with a marker AFTER the default registration. If the
                // IProfileRepository factory honors the registry (SF1 fix), resolving and calling
                // SavePublicKeySet routes through the marker. The old first-satisfiable-ctor
                // wiring would self-compose the default policy and never see this marker.
                registry.For<IPublicKeySetRegistrar>().Use(new MarkerRegistrar());
                return registry.Get<IProfileRepository>();
            },
            (repository) =>
            {
                bool markerInvoked = false;
                try
                {
                    repository.SavePublicKeySet(new PublicKeySetData { KeySetHandle = "any", PublicRsaKey = "any" });
                }
                catch (MarkerInvokedException)
                {
                    markerInvoked = true;
                }
                return markerInvoked;
            })
        .TheTest
        .ShouldPass<bool>((because, _, markerInvoked) =>
        {
            because.ItsTrue("the registry-supplied registrar was used, not the self-composed default", markerInvoked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed class MarkerRegistrar : IPublicKeySetRegistrar
    {
        public PublicKeySetData Register(PublicKeySetData publicKeySetData)
        {
            throw new MarkerInvokedException();
        }

        public PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature)
        {
            throw new MarkerInvokedException();
        }

        public PublicKeySetData? Resolve(string keySetHandle)
        {
            throw new MarkerInvokedException();
        }
    }

    private sealed class MarkerInvokedException : Exception
    {
    }
}
