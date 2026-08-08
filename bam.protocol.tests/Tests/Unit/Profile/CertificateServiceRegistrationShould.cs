using System.Reflection;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Test;
using NSubstitute;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("CertificateServiceRegistration Should", Selector = "csrs")]
public class CertificateServiceRegistrationShould : UnitTestMenuContainer
{
    public CertificateServiceRegistrationShould(ServiceRegistry serviceRegistry) : base(serviceRegistry) { }

    private static ServiceRegistry ComposeRegistry(IActor issuer)
    {
        ServiceRegistry registry = new ServiceRegistry();
        registry.For<IProfileRepository>().Use(Substitute.For<IProfileRepository>());
        registry.For<IKeyManager>().Use(Substitute.For<IKeyManager>());
        registry.For<ICompositeKeyCalculator>().Use(Substitute.For<ICompositeKeyCalculator>());
        return registry.AddCertificateManager(issuer);
    }

    [UnitTest]
    public void ResolveCertificateManagerFromComposedRegistry()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("issuerHandle");
        issuer.Name.Returns("Issuer Name");

        After.Setup(reg =>
        {
            reg.For<ServiceRegistry>().Use(ComposeRegistry(issuer));
        })
        .When<ServiceRegistry>("resolves the certificate stack from the composed registry", (registry) =>
        {
            return new ResolutionOutcome(
                registry.Get<ICertificateManager>(),
                registry.Get<CertificateAuthority>(),
                registry.Get<IActor>(),
                registry.Get<IX509NameProvider>(),
                registry.Get<ICertificateSerialNumberProvider>());
        })
        .TheTest
        .ShouldPass<ResolutionOutcome>((because, outcome) =>
        {
            because.ItsTrue("ICertificateManager resolves to CertificateManager", outcome.CertificateManager is CertificateManager);
            because.ItsTrue("CertificateAuthority resolves", outcome.CertificateAuthority != null);
            because.ItsTrue("IActor resolves to the supplied issuer", ReferenceEquals(outcome.Actor, issuer));
            because.ItsTrue("IX509NameProvider defaults to BamX509NameProvider", outcome.X509NameProvider is BamX509NameProvider);
            because.ItsTrue("ICertificateSerialNumberProvider defaults to CertificateSerialNumberProvider", outcome.SerialNumberProvider is CertificateSerialNumberProvider);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveSameCertificateManagerAcrossResolutions()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("singletonIssuer");
        issuer.Name.Returns("Singleton Issuer");

        After.Setup(reg =>
        {
            reg.For<ServiceRegistry>().Use(ComposeRegistry(issuer));
        })
        .When<ServiceRegistry>("resolves ICertificateManager twice", (registry) =>
        {
            ICertificateManager first = registry.Get<ICertificateManager>();
            ICertificateManager second = registry.Get<ICertificateManager>();
            return new SingletonOutcome(ReferenceEquals(first, second));
        })
        .TheTest
        .ShouldPass<SingletonOutcome>((because, outcome) =>
        {
            because.ItsTrue("both resolutions return the same CertificateManager instance", outcome.SameInstance);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RespectExistingX509NameProviderBinding()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("respectIssuer");
        issuer.Name.Returns("Respect Issuer");
        IX509NameProvider existingProvider = Substitute.For<IX509NameProvider>();

        After.Setup(reg =>
        {
            ServiceRegistry registry = new ServiceRegistry();
            registry.For<IProfileRepository>().Use(Substitute.For<IProfileRepository>());
            registry.For<IKeyManager>().Use(Substitute.For<IKeyManager>());
            registry.For<ICompositeKeyCalculator>().Use(Substitute.For<ICompositeKeyCalculator>());
            registry.For<IX509NameProvider>().Use(existingProvider);
            reg.For<ServiceRegistry>().Use(registry.AddCertificateManager(issuer));
        })
        .When<ServiceRegistry>("resolves IX509NameProvider after composing", (registry) =>
        {
            return registry.Get<IX509NameProvider>();
        })
        .TheTest
        .ShouldPass<IX509NameProvider>((because, provider) =>
        {
            because.ItsTrue("the pre-existing IX509NameProvider binding is respected", ReferenceEquals(provider, existingProvider));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RespectExistingSerialNumberProviderBinding()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("serialIssuer");
        issuer.Name.Returns("Serial Issuer");
        ICertificateSerialNumberProvider existingProvider = Substitute.For<ICertificateSerialNumberProvider>();

        After.Setup(reg =>
        {
            ServiceRegistry registry = new ServiceRegistry();
            registry.For<IProfileRepository>().Use(Substitute.For<IProfileRepository>());
            registry.For<IKeyManager>().Use(Substitute.For<IKeyManager>());
            registry.For<ICompositeKeyCalculator>().Use(Substitute.For<ICompositeKeyCalculator>());
            registry.For<ICertificateSerialNumberProvider>().Use(existingProvider);
            reg.For<ServiceRegistry>().Use(registry.AddCertificateManager(issuer));
        })
        .When<ServiceRegistry>("resolves ICertificateSerialNumberProvider after composing", (registry) =>
        {
            return registry.Get<ICertificateSerialNumberProvider>();
        })
        .TheTest
        .ShouldPass<ICertificateSerialNumberProvider>((because, provider) =>
        {
            because.ItsTrue("the pre-existing ICertificateSerialNumberProvider binding is respected", ReferenceEquals(provider, existingProvider));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveSameCertificateAuthorityComposedIntoTheManager()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("authorityIssuer");
        issuer.Name.Returns("Authority Issuer");

        After.Setup(reg =>
        {
            reg.For<ServiceRegistry>().Use(ComposeRegistry(issuer));
        })
        .When<ServiceRegistry>("resolves the authority twice and the manager it is composed into", (registry) =>
        {
            CertificateAuthority first = registry.Get<CertificateAuthority>();
            CertificateAuthority second = registry.Get<CertificateAuthority>();
            CertificateManager manager = (CertificateManager)registry.Get<ICertificateManager>();
            PropertyInfo authorityProperty = typeof(CertificateManager)
                .GetProperty("CertificateAuthority", BindingFlags.Instance | BindingFlags.NonPublic)!;
            CertificateAuthority composedIntoManager = (CertificateAuthority)authorityProperty.GetValue(manager)!;
            return new AuthoritySingletonOutcome(
                ReferenceEquals(first, second),
                ReferenceEquals(first, composedIntoManager));
        })
        .TheTest
        .ShouldPass<AuthoritySingletonOutcome>((because, outcome) =>
        {
            because.ItsTrue("both resolutions return the same CertificateAuthority instance", outcome.SameAuthorityAcrossResolutions);
            because.ItsTrue("the resolved CertificateManager composes that same CertificateAuthority singleton", outcome.ManagerSharesAuthority);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record ResolutionOutcome(
        ICertificateManager CertificateManager,
        CertificateAuthority CertificateAuthority,
        IActor Actor,
        IX509NameProvider X509NameProvider,
        ICertificateSerialNumberProvider SerialNumberProvider);

    private sealed record SingletonOutcome(bool SameInstance);

    private sealed record AuthoritySingletonOutcome(bool SameAuthorityAcrossResolutions, bool ManagerSharesAuthority);
}
