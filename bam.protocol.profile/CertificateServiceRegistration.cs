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
    /// <see cref="IActor"/> binding.  This replacement is a deliberate, accepted trade-off: it is
    /// safe because request-time actor identity is resolved through <c>IActorResolver.ResolveActor</c>,
    /// not by reading the registry's <see cref="IActor"/> singleton, so a host that also relies on a
    /// registry-bound <see cref="IActor"/> for another purpose must bind the certificate stack in its
    /// own registry.  The authority and manager are bound as singletons: the authority carries the
    /// fixed issuer identity and the manager's persistence must observe a single consistent store.
    /// </para>
    /// <para>
    /// The default <see cref="ICertificateSerialNumberProvider"/> is bound as a singleton so a single
    /// serial counter is shared across the authority; a transient binding would let a second
    /// resolution start a parallel counter and re-issue serial numbers.
    /// </para>
    /// <para>
    /// Registration is eager: the <see cref="CertificateAuthority"/> and <see cref="CertificateManager"/>
    /// singletons are constructed immediately by this call, so a missing prerequisite binding
    /// (<c>IProfileRepository</c>, <c>IKeyManager</c>, or <c>ICompositeKeyCalculator</c>) surfaces here
    /// as a <c>TypedBindingNotFoundException</c> at the call site rather than on first resolution.
    /// Because the singletons capture their collaborators at construction time, rebinding a
    /// prerequisite after this call has no effect on the already-constructed authority or manager.
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
            registry.For<ICertificateSerialNumberProvider>().UseSingleton<CertificateSerialNumberProvider>();
        }

        registry
            .For<IActor>().UseSingleton(issuer)
            .For<CertificateAuthority>().UseSingleton<CertificateAuthority>()
            .For<ICertificateManager>().UseSingleton<CertificateManager>();

        return registry;
    }
}
