using Bam;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("RsaKeySetRotationVerifier Should", Selector = "krv")]
public class RsaKeySetRotationVerifierShould : UnitTestMenuContainer
{
    private static byte[] Sign(RsaPublicPrivateKeyPair signingKeyPair, string currentPublicRsaKeyPem, PublicKeySetData proposed)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        string payload = KeySetRotationPayload.Compose(currentPublicRsaKeyPem.Sha256(), proposed);
        ISignature signature = signatureProvider.Sign(signingKeyPair, payload, RsaKeySetRotationVerifier.Algorithm);
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
                byte[] rotationSignature = Sign(currentKeyPair, current.PublicRsaKey, proposed);

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
                byte[] rotationSignature = Sign(currentKeyPair, current.PublicRsaKey, signedProposal);

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
                // the attacker signs the correct payload (the current key SHA is public) but with
                // their OWN private key — possession of the current key is what they lack
                byte[] forgedSignature = Sign(attackerKeyPair, current.PublicRsaKey, proposed);

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

    [UnitTest]
    public void RejectProofBoundToDifferentCurrentKey()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair otherCurrentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair nextKeyPair = new RsaPublicPrivateKeyPair();

        When.A<RsaKeySetRotationVerifier>("rejects a proof whose bound current-key SHA is not the registered key",
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
                // a proof made binding a DIFFERENT current key (even if signed by that key) must
                // not verify against the actually-registered current key (C2a key binding)
                byte[] mismatchedSignature = Sign(otherCurrentKeyPair, otherCurrentKeyPair.PublicKeyPem, proposed);

                ISignatureVerification verification = verifier.Verify(current, proposed, mismatchedSignature);
                return verification;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails when the proof is bound to a different current key", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
