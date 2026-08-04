using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("RsaRevocationAuthority Should", Selector = "rra")]
public class RsaRevocationAuthorityShould : UnitTestMenuContainer
{
    private static RsaRevocationAuthority CreateAuthority(string? adminPublicRsaKey)
    {
        return new RsaRevocationAuthority(new RsaSignatureProvider(), new StaticAdminPublicKeySource(adminPublicRsaKey));
    }

    private static byte[] Sign(RsaPublicPrivateKeyPair signingKeyPair, PublicKeySetData target)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        ISignature signature = signatureProvider.Sign(signingKeyPair, RevocationPayload.Compose(target), RsaRevocationAuthority.Algorithm);
        return signature.SignatureBytes;
    }

    private static PublicKeySetData Target(string handle, string rsaPem)
    {
        return new PublicKeySetData { KeySetHandle = handle, PublicRsaKey = rsaPem, Uuid = Guid.NewGuid().ToString() };
    }

    [UnitTest]
    public void VerifyValidAdminProof()
    {
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        PublicKeySetData target = Target("holder", keyPair.PublicKeyPem);

        When.A<RsaRevocationAuthority>("verifies a target-bound proof signed by the admin key",
            () => CreateAuthority(adminKeyPair.PublicKeyPem),
            (authority) => authority.Verify(target, Sign(adminKeyPair, target)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification succeeds for a valid admin proof", v => v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectProofByNonAdminKey()
    {
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        PublicKeySetData target = Target("holder", keyPair.PublicKeyPem);

        When.A<RsaRevocationAuthority>("rejects a proof signed by a key other than the admin key",
            () => CreateAuthority(adminKeyPair.PublicKeyPem),
            (authority) => authority.Verify(target, Sign(attackerKeyPair, target)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails for a non-admin signature", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectProofBoundToDifferentTarget()
    {
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        PublicKeySetData signedTarget = Target("holder", keyPair.PublicKeyPem);
        PublicKeySetData otherTarget = Target("holder", keyPair.PublicKeyPem); // different Uuid

        When.A<RsaRevocationAuthority>("rejects a proof bound to a different target than the one presented",
            () => CreateAuthority(adminKeyPair.PublicKeyPem),
            (authority) => authority.Verify(otherTarget, Sign(adminKeyPair, signedTarget)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails when the proof is bound to a different target", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FailClosedWhenAdminKeyUnconfigured()
    {
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        PublicKeySetData target = Target("holder", keyPair.PublicKeyPem);

        When.A<RsaRevocationAuthority>("fails closed when no admin key is configured",
            () => CreateAuthority(null),
            (authority) => authority.Verify(target, Sign(adminKeyPair, target)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails closed with no admin key configured", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FailClosedWhenAdminKeyMalformed()
    {
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        PublicKeySetData target = Target("holder", keyPair.PublicKeyPem);

        When.A<RsaRevocationAuthority>("fails closed (returns a failed verification, not a throw) when the admin key is malformed",
            () => CreateAuthority("not a pem"),
            (authority) => authority.Verify(target, Sign(adminKeyPair, target)))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ISignatureVerification>("verification fails closed for malformed admin key material", v => !v.Success);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
