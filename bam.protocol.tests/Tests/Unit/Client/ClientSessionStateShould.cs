using System.Text;
using Bam.Encryption;
using Bam.Protocol.Client;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("ClientSessionState should", "csss")]
public class ClientSessionStateShould : UnitTestMenuContainer
{
    [UnitTest]
    public void DeriveSessionAesKey()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();

        When.A<ClientSessionState>("derives a session AES key",
            () => new ClientSessionState(
                "test-session-id",
                "test-nonce",
                serverKeyPair.GetEccPublicKey(),
                clientKeyPair),
            (state) => state.DeriveSessionAesKey())
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            AesKey key = because.TheResult.As<AesKey>();
            because.ItsTrue("key bytes are not null", key.Key != null);
            because.ItsTrue("IV bytes are not null", key.IV != null);
            because.ItsTrue("key has 32 bytes", key.Key.Length == 32);
            because.ItsTrue("IV has 16 bytes", key.IV.Length == 16);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DeriveMatchingKeysWithServer()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();

        When.A<ClientSessionState>("derives key matching server ECDH",
            () => new ClientSessionState(
                "test-session-id",
                "test-nonce",
                serverKeyPair.GetEccPublicKey(),
                clientKeyPair),
            (state) =>
            {
                AesKey clientDerivedKey = state.DeriveSessionAesKey();

                // Server side: derive using server private + client public (same as RequestSecurityValidator)
                AesKey serverDerivedKey = serverKeyPair.GetSharedAesKey(clientKeyPair.PublicKeyPem);

                string clientKeyB64 = Convert.ToBase64String(clientDerivedKey.Key);
                string serverKeyB64 = Convert.ToBase64String(serverDerivedKey.Key);
                string clientIvB64 = Convert.ToBase64String(clientDerivedKey.IV);
                string serverIvB64 = Convert.ToBase64String(serverDerivedKey.IV);

                return new object[] { clientKeyB64, serverKeyB64, clientIvB64, serverIvB64 };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("keys match", r => ((string)r[0]).Equals(r[1]))
                .As<object[]>("IVs match", r => ((string)r[2]).Equals(r[3]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void UseSessionKeyViaProtectedContext()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();
        string plainText = "test message for encryption";

        When.A<ClientSessionState>("uses session key via protected context",
            () => new ClientSessionState(
                "test-session-id",
                "test-nonce",
                serverKeyPair.GetEccPublicKey(),
                clientKeyPair),
            (state) =>
            {
                // Encrypt using the protected key context
                string encrypted = state.UseSessionKey(key => Aes.Encrypt(plainText, key));

                // Decrypt using server-derived key
                AesKey serverKey = serverKeyPair.GetSharedAesKey(clientKeyPair.PublicKeyPem);
                string decrypted = Aes.Decrypt(encrypted, serverKey);

                return decrypted;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            because.ItsTrue("roundtrip matches", plainText.Equals(because.Result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DisposeClientKeyPair()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();
        string serverPublicKeyPem = serverKeyPair.PublicKeyPem;

        When.A<ClientSessionState>("disposes key pair on dispose",
            () => new ClientSessionState(
                "test-session-id",
                "test-nonce",
                serverKeyPair.GetEccPublicKey(),
                clientKeyPair),
            (state) =>
            {
                // Key works before dispose
                AesKey keyBefore = state.DeriveSessionAesKey();
                bool workedBefore = keyBefore != null && keyBefore.Key.Length > 0;

                state.Dispose();

                // After dispose, the key pair should be unusable
                bool threwAfterDispose = false;
                try
                {
                    clientKeyPair.GetSharedAesKey(serverPublicKeyPem);
                }
                catch
                {
                    threwAfterDispose = true;
                }

                return new object[] { workedBefore, threwAfterDispose };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("key worked before dispose", r => (bool)r[0])
                .As<object[]>("key pair unusable after dispose", r => (bool)r[1]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
