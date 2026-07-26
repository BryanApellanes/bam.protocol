using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;

namespace Bam.Protocol.Profile;

/// <summary>
/// Registers the certificate-management stack: a <see cref="CertificateAuthority"/> issuing on
/// behalf of a host-supplied issuer actor, and <see cref="CertificateManager"/> as the
/// <see cref="ICertificateManager"/> implementation, persisting issued certificates through the
/// host's <see cref="IProfileRepository"/>.
/// </summary>
public static class CertificateServiceRegistration
{
    /// <summary>
    /// Registers <see cref="ICertificateManager"/> backed by a <see cref="CertificateAuthority"/>
    /// that issues certificates on behalf of the specified <paramref name="issuer"/>.
    /// <para>
    /// The host registry must already provide <c>IProfileRepository</c> (e.g. via
    /// <c>AddEncryptedProfileRepository</c>), <c>IKeyManager</c>, and
    /// <c>ICompositeKeyCalculator</c>.  Existing <see cref="IX509NameProvider"/> and
    /// <see cref="ICertificateSerialNumberProvider"/> bindings are respected;
    /// <see cref="BamX509NameProvider"/> and <see cref="CertificateSerialNumberProvider"/> are
    /// bound only when none are present.
    /// </para>
    /// <para>
    /// The issuer is bound as the registry's <see cref="IActor"/> singleton — it is the identity
    /// certificates are issued on behalf of, and passing it here intentionally replaces any prior
    /// <see cref="IActor"/> binding.  The authority and manager are bound as singletons: the
    /// authority carries the fixed issuer identity and the manager's persistence must observe a
    /// single consistent store.
    /// </para>
    /// </summary>
    /// <param name="registry">The registry to add certificate-management bindings to.</param>
    /// <param name="issuer">The actor identity certificates are issued on behalf of.</param>
    /// <returns>The same registry, for fluent chaining.</returns>
    public static ServiceRegistry AddCertificateManager(this ServiceRegistry registry, IActor issuer)
    {
        if (!registry.Contains<IX509NameProvider>())
        {
            registry.For<IX509NameProvider>().Use<BamX509NameProvider>();
        }

        if (!registry.Contains<ICertificateSerialNumberProvider>())
        {
            registry.For<ICertificateSerialNumberProvider>().Use<CertificateSerialNumberProvider>();
        }

        registry
            .For<IActor>().UseSingleton(issuer)
            .For<CertificateAuthority>().UseSingleton<CertificateAuthority>()
            .For<ICertificateManager>().UseSingleton<CertificateManager>();

        return registry;
    }
}
