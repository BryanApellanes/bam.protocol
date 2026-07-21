using Bam.Encryption;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("PrivateKeyManager Should", Selector = "pkms")]
public class PrivateKeyManagerShould : UnitTestMenuContainer
{
    private static PrivateKeyManager CreatePrivateKeyManager(string testName)
    {
        OpaqueFsKeyValuePairStorage opaqueStorage = new OpaqueFsKeyValuePairStorage(
            new FsSlottedStorage($"./.bam/tests/PrivateKeyManagerShould_{testName}"),
            new AesKey(),
            new HmacKeyProvider());
        return new PrivateKeyManager(opaqueStorage);
    }

    [UnitTest]
    public void RsaRoundTrip()
    {
        string testData = "PrivateKeyManagerShould RSA round trip data";

        When.A<PrivateKeyManager>("generates an RSA key and retrieves it by the returned public key",
            () => CreatePrivateKeyManager(nameof(RsaRoundTrip)),
            (privateKeyManager) =>
            {
                IPublicKey publicKey = privateKeyManager.GeneratePrivateRsaKey();
                IPrivateKey retrievedKey = privateKeyManager.GetPrivateRsaKey(publicKey);
                ISignature signature = retrievedKey.Sign(testData);
                ISignatureVerification verification = new RsaSignatureProvider().VerifySignature(signature, publicKey);
                return new RoundTripOutcome(retrievedKey, verification);
            })
        .TheTest
        .ShouldPass<RoundTripOutcome>((because, outcome) =>
        {
            because.ItsTrue("retrieved private key is not null", outcome.RetrievedKey != null);
            because.ItsTrue("signed data matches original", testData.Equals(outcome.Verification.Signature.Data));
            because.ItsTrue("signature verified against the returned public key", outcome.Verification.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void EccRoundTrip()
    {
        string testData = "PrivateKeyManagerShould ECC round trip data";

        When.A<PrivateKeyManager>("generates an ECC key and retrieves it by the returned public key",
            () => CreatePrivateKeyManager(nameof(EccRoundTrip)),
            (privateKeyManager) =>
            {
                IPublicKey publicKey = privateKeyManager.GeneratePrivateEccKey();
                IPrivateKey retrievedKey = privateKeyManager.GetPrivateEccKey(publicKey);
                ISignature signature = retrievedKey.Sign(testData);
                ISignatureVerification verification = new EccSignatureProvider().VerifySignature(signature, publicKey);
                return new RoundTripOutcome(retrievedKey, verification);
            })
        .TheTest
        .ShouldPass<RoundTripOutcome>((because, outcome) =>
        {
            because.ItsTrue("retrieved private key is not null", outcome.RetrievedKey != null);
            because.ItsTrue("signed data matches original", testData.Equals(outcome.Verification.Signature.Data));
            because.ItsTrue("signature verified against the returned public key", outcome.Verification.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RsaRoundTripWithReconstructedPublicKey()
    {
        string testData = "PrivateKeyManagerShould reconstructed public key data";

        When.A<PrivateKeyManager>("retrieves an RSA key by a public key reconstructed from its PEM string",
            () => CreatePrivateKeyManager(nameof(RsaRoundTripWithReconstructedPublicKey)),
            (privateKeyManager) =>
            {
                IPublicKey publicKey = privateKeyManager.GeneratePrivateRsaKey();
                string publicKeyPem = publicKey.Pem;
                RsaPublicKey reconstructedPublicKey = new RsaPublicKey(publicKeyPem);
                IPrivateKey retrievedKey = privateKeyManager.GetPrivateRsaKey(reconstructedPublicKey);
                ISignature signature = retrievedKey.Sign(testData);
                ISignatureVerification verification = new RsaSignatureProvider().VerifySignature(signature, reconstructedPublicKey);
                return new RoundTripOutcome(retrievedKey, verification);
            })
        .TheTest
        .ShouldPass<RoundTripOutcome>((because, outcome) =>
        {
            because.ItsTrue("retrieved private key is not null", outcome.RetrievedKey != null);
            because.ItsTrue("signature verified against the reconstructed public key", outcome.Verification.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ThrowForUnknownPublicKey()
    {
        When.A<PrivateKeyManager>("throws for a public key with no stored private key",
            () => CreatePrivateKeyManager(nameof(ThrowForUnknownPublicKey)),
            (privateKeyManager) =>
            {
                RsaKeyPair unknownKeyPair = new RsaKeyPair();
                try
                {
                    privateKeyManager.GetPrivateRsaKey(unknownKeyPair.PublicKey);
                    return new MissOutcome(false, null);
                }
                catch (InvalidOperationException ex)
                {
                    return new MissOutcome(true, ex.Message);
                }
            })
        .TheTest
        .ShouldPass<MissOutcome>((because, outcome) =>
        {
            because.ItsTrue("InvalidOperationException was thrown", outcome.ThrewInvalidOperationException);
            because.ItsTrue("message names the missing private key", outcome.Message != null && outcome.Message.Contains("No private key stored"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record RoundTripOutcome(IPrivateKey RetrievedKey, ISignatureVerification Verification);

    private sealed record MissOutcome(bool ThrewInvalidOperationException, string? Message);
}
