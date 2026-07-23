using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("RsaKeySetRotationVerifier Should", Selector = "krv")]
public class RsaKeySetRotationVerifierShould : UnitTestMenuContainer
{
    private static byte[] Sign(RsaPublicPrivateKeyPair signingKeyPair, PublicKeySetData proposed)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        ISignature signature = signatureProvider.Sign(signingKeyPair, KeySetRotationPayload.Compose(proposed), RsaKeySetRotationVerifier.Algorithm);
        return signature.SignatureBytes;
    }

    [UnitTest]
    public void VerifyValidRotationSignature()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair nextKeyPair = new RsaPublicPrivateKeyPair();

        When.A<RsaKeySetRotationVerifier>("verifies a rotation signature made by the current key",
            () => new RsaKeySetRotationVerifier(new RsaSignatureProvider()),
            (verifier) =>
            {
                PublicKeySetData current = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = currentKeyPair.PublicKeyPem,
                };
                PublicKeySetData proposed = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = nextKeyPair.PublicKeyPem,
                };
                byte[] rotationSignature = Sign(currentKeyPair, proposed);

                ISignatureVerification verification = verifier.Verify(current, proposed, rotationSignature);
                return verification;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification succeeds for a signature by the current key", v => v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectTamperedPayload()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair nextKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair swappedInKeyPair = new RsaPublicPrivateKeyPair();

        When.A<RsaKeySetRotationVerifier>("rejects a signature when the proposed key set was altered after signing",
            () => new RsaKeySetRotationVerifier(new RsaSignatureProvider()),
            (verifier) =>
            {
                PublicKeySetData current = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = currentKeyPair.PublicKeyPem,
                };
                PublicKeySetData signedProposal = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = nextKeyPair.PublicKeyPem,
                };
                byte[] rotationSignature = Sign(currentKeyPair, signedProposal);

                // the key set presented for rotation differs from what was signed
                PublicKeySetData tamperedProposal = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = swappedInKeyPair.PublicKeyPem,
                };

                ISignatureVerification verification = verifier.Verify(current, tamperedProposal, rotationSignature);
                return verification;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails for a payload that differs from what was signed", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectSignatureByDifferentKey()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();

        When.A<RsaKeySetRotationVerifier>("rejects a signature made by a key other than the current key",
            () => new RsaKeySetRotationVerifier(new RsaSignatureProvider()),
            (verifier) =>
            {
                PublicKeySetData current = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = currentKeyPair.PublicKeyPem,
                };
                PublicKeySetData proposed = new PublicKeySetData
                {
                    KeySetHandle = "holder",
                    PublicRsaKey = attackerKeyPair.PublicKeyPem,
                };
                byte[] forgedSignature = Sign(attackerKeyPair, proposed);

                ISignatureVerification verification = verifier.Verify(current, proposed, forgedSignature);
                return verification;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails for a signature by a non-matching key", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
