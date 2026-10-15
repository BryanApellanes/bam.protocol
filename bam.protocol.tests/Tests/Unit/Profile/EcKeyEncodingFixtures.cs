using Bam;
using Bam.Encryption;

namespace Bam.Protocol.Tests.Unit.Profile;

/// <summary>
/// One P-256 public key written by openssl in both curve encodings, and its fingerprint as the
/// documented openssl command computes it:
/// <c>openssl ec -pubin -in key.pem -param_enc named_curve -outform DER | sha256sum</c>.
/// </summary>
internal static class EcKeyEncodingFixtures
{
    /// <summary>The key with its named-curve OID (prime256v1), as <c>openssl ec -pubout</c> writes it.</summary>
    internal const string NamedCurvePem =
        "-----BEGIN PUBLIC KEY-----\n" +
        "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE3z51Z1uB07fK6X10tqf6L1DiwyjP\n" +
        "rwTKEK9qFwc3nP19jhVyDCzid58trbYgiI25hKv1zMkw1STmzULy0s7tGg==\n" +
        "-----END PUBLIC KEY-----\n";

    /// <summary>The same key with explicit curve parameters, as <c>openssl ec -param_enc explicit</c> writes it.</summary>
    internal const string ExplicitParametersPem =
        "-----BEGIN PUBLIC KEY-----\n" +
        "MIIBSzCCAQMGByqGSM49AgEwgfcCAQEwLAYHKoZIzj0BAQIhAP////8AAAABAAAA\n" +
        "AAAAAAAAAAAA////////////////MFsEIP////8AAAABAAAAAAAAAAAAAAAA////\n" +
        "///////////8BCBaxjXYqjqT57PrvVV2mIa8ZR0GsMxTsPY7zjw+J9JgSwMVAMSd\n" +
        "NgiG5wSTamZ44ROdJreBn36QBEEEaxfR8uEsQkf4vOblY6RA8ncDfYEt6zOg9KE5\n" +
        "RdiYwpZP40Li/hp/m47n60p8D54WK84zV2sxXs7LtkBoN79R9QIhAP////8AAAAA\n" +
        "//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABN8+dWdbgdO3yul9dLan+i9Q\n" +
        "4sMoz68EyhCvahcHN5z9fY4Vcgws4nefLa22IIiNuYSr9czJMNUk5s1C8tLO7Ro=\n" +
        "-----END PUBLIC KEY-----\n";

    /// <summary>The fingerprint the documented openssl command prints for either PEM above.</summary>
    internal const string OpensslFingerprint = "33b6e4f612db64916f00c81ed986526cfb497c9979dd820686d2979ff26ab294";

    /// <summary>
    /// The fingerprint a stored row carried before bam.protocol#47 when it registered
    /// <paramref name="pem"/>: the SHA-256 of BouncyCastle's re-encoding of the key as parsed.
    /// </summary>
    internal static string PreCanonicalFingerprint(string pem)
    {
        return Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pem.PemToKey()).GetDerEncoded().Sha256();
    }
}
