using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

/// <summary>
/// Issues X.509 certificates on behalf of a fixed issuer actor, signing with that actor's
/// key material resolved through <see cref="IKeyManager"/>.  Persistence of issued
/// certificates is the responsibility of <see cref="ICertificateManager"/> implementations
/// (e.g. <see cref="CertificateManager"/>), which compose an instance of this authority —
/// this type deliberately takes no dependency on certificate storage.
/// </summary>
public class CertificateAuthority : CertificateIssuer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateAuthority"/> class issuing
    /// on behalf of the specified <paramref name="issuer"/>.
    /// </summary>
    /// <param name="issuer">The actor identity certificates are issued on behalf of.</param>
    /// <param name="keyManager">Resolves the issuer's signing key.</param>
    /// <param name="x509NameProvider">Maps actors and names to X.509 distinguished names.</param>
    /// <param name="compositeKeyCalculator">Calculates composite keys for issued-certificate bookkeeping.</param>
    /// <param name="serialNumberProvider">Provides unique serial numbers for issued certificates.</param>
    public CertificateAuthority(IActor issuer, IKeyManager keyManager,
        IX509NameProvider x509NameProvider, ICompositeKeyCalculator compositeKeyCalculator,
        ICertificateSerialNumberProvider serialNumberProvider) : base(serialNumberProvider)
    {
        this.Issuer = issuer;
        this.KeyManager = keyManager;
        this.X509NameProvider = x509NameProvider;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    /// <summary>
    /// Gets the actor identity this authority issues certificates on behalf of.
    /// </summary>
    protected IActor Issuer { get; }

    /// <summary>
    /// Gets the key manager used to resolve the issuer's signing key.
    /// </summary>
    protected IKeyManager KeyManager { get; }

    /// <summary>
    /// Gets the provider that maps actors and names to X.509 distinguished names.
    /// </summary>
    protected IX509NameProvider X509NameProvider { get; }

    /// <summary>
    /// Gets the calculator used for composite-key bookkeeping of issued certificates.
    /// </summary>
    protected ICompositeKeyCalculator CompositeKeyCalculator { get; }

    /// <summary>
    /// Creates a certificate from fully-specified generation options, using the issuer and
    /// subject names and keys carried by <paramref name="options"/> rather than this
    /// authority's configured <see cref="Issuer"/>.
    /// </summary>
    /// <param name="options">The names and key material to generate the certificate from.</param>
    /// <returns>The generated certificate.</returns>
    public X509Certificate CreateCertificate(GenerateCertificateOptions options)
    {
        return CreateCertificate(options.IssuerName(), options.SubjectName(), options.IssuerPrivateKey(),
            options.SubjectPublicKey());
    }

    /// <summary>
    /// Creates a certificate for the specified subject, issued and signed by this authority's
    /// configured <see cref="Issuer"/> using its signing key from <see cref="KeyManager"/>,
    /// preserving historical defaults (a certificate-authority certificate — see
    /// <see cref="CertificateIssuanceOptions.Default"/>).
    /// </summary>
    /// <param name="subject">The subject name the certificate is issued to.</param>
    /// <param name="subjectPublic">The subject's public key embedded in the certificate.</param>
    /// <returns>The generated certificate.</returns>
    public X509Certificate CreateCertificate(string subject, IPublicKey subjectPublic)
    {
        return CreateCertificate(subject, subjectPublic, CertificateIssuanceOptions.Default());
    }

    /// <summary>
    /// Creates a certificate for the specified subject, issued and signed by this authority's
    /// configured <see cref="Issuer"/> using its signing key from <see cref="KeyManager"/>,
    /// applying the supplied per-call issuance <paramref name="options"/> — for example
    /// <see cref="CertificateIssuanceOptions.EndEntity"/> to issue a non-CA leaf certificate
    /// rather than a certificate authority.
    /// </summary>
    /// <param name="subject">The subject name the certificate is issued to.</param>
    /// <param name="subjectPublic">The subject's public key embedded in the certificate.</param>
    /// <param name="options">The per-call issuance options controlling validity and CA status.</param>
    /// <returns>The generated certificate.</returns>
    public X509Certificate CreateCertificate(string subject, IPublicKey subjectPublic, CertificateIssuanceOptions options)
    {
        X509Name issuerName = X509NameProvider.GetName(Issuer);
        X509Name subjectName = X509NameProvider.GetName(subject);
        return base.CreateCertificate(issuerName, subjectName, KeyManager.GetSigningKey(Issuer).Value, subjectPublic.Value, options);
    }
}