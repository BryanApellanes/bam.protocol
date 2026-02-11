using Bam.Encryption;
using Bam.Protocol.Server;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamJwtToken should")]
public class BamJwtTokenShould : UnitTestMenuContainer
{
    [UnitTest]
    public void EncodeAndDecode()
    {
        string sessionId = 16.RandomLetters();
        string actorHandle = 8.RandomLetters();
        string issuer = "bam-test";

        When.A<BamJwtToken>("encodes and decodes a JWT",
            () => new BamJwtToken(sessionId, actorHandle, issuer),
            (token) =>
            {
                EccPublicPrivateKeyPair keyPair = new EccPublicPrivateKeyPair();
                PrivateKeyProvider privateKey = new PrivateKeyProvider(keyPair);
                string encoded = token.Encode(privateKey.GetPrivateKey());
                return BamJwtToken.Decode(encoded);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            BamJwtToken decoded = because.TheResult.As<BamJwtToken>();
            because.ItsTrue("ActorHandle matches", decoded.ActorHandle == actorHandle);
            because.ItsTrue("SessionId matches", decoded.SessionId == sessionId);
            because.ItsTrue("Issuer matches", decoded.Issuer == issuer);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void VerifyValidSignature()
    {
        string sessionId = 16.RandomLetters();
        string actorHandle = 8.RandomLetters();

        When.A<EccPublicPrivateKeyPair>("verifies a valid JWT signature",
            () => new EccPublicPrivateKeyPair(),
            (keyPair) =>
            {
                PrivateKeyProvider privateKey = new PrivateKeyProvider(keyPair);
                BamJwtToken token = new BamJwtToken(sessionId, actorHandle, "bam-test");
                string encoded = token.Encode(privateKey.GetPrivateKey());
                return BamJwtToken.Verify(encoded, keyPair.PublicKeyPem.PemToKey());
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.Is<bool>("signature is valid", b => b);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectTamperedToken()
    {
        string sessionId = 16.RandomLetters();
        string actorHandle = 8.RandomLetters();

        When.A<EccPublicPrivateKeyPair>("rejects a tampered JWT",
            () => new EccPublicPrivateKeyPair(),
            (keyPair) =>
            {
                PrivateKeyProvider privateKey = new PrivateKeyProvider(keyPair);
                BamJwtToken token = new BamJwtToken(sessionId, actorHandle, "bam-test");
                string encoded = token.Encode(privateKey.GetPrivateKey());

                // Tamper with the payload
                string[] parts = encoded.Split('.');
                parts[1] = parts[1] + "x";
                string tampered = string.Join(".", parts);

                return BamJwtToken.Verify(tampered, keyPair.PublicKeyPem.PemToKey());
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.Is<bool>("tampered token is rejected", b => !b);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
