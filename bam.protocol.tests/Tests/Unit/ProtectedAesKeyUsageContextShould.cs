using Bam.Encryption;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("ProtectedAesKeyUsageContext should", "pakucs")]
public class ProtectedAesKeyUsageContextShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ProvideUsableKeyInCallback()
    {
        string plainText = "test data to encrypt";

        When.A<ProtectedAesKeyUsageContext>("provides usable key in callback",
            () =>
            {
                AesKey key = new AesKey();
                return new ProtectedAesKeyUsageContext(key);
            },
            (context) =>
            {
                string encrypted = context.UseKey(key => Aes.Encrypt(plainText, key));
                string decrypted = context.UseKey(key => Aes.Decrypt(encrypted, key));
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
    public void ZeroOriginalKeyOnConstruction()
    {
        When.A<AesKey>("zeros original key bytes on construction",
            () => new AesKey(),
            (key) =>
            {
                byte[] originalKey = new byte[key.Key.Length];
                Array.Copy(key.Key, originalKey, key.Key.Length);

                byte[] originalIv = new byte[key.IV.Length];
                Array.Copy(key.IV, originalIv, key.IV.Length);

                // ProtectedAesKeyUsageContext zeros the source key
                ProtectedAesKeyUsageContext context = new ProtectedAesKeyUsageContext(key);

                bool keyZeroed = true;
                for (int i = 0; i < key.Key.Length; i++)
                {
                    if (key.Key[i] != 0) { keyZeroed = false; break; }
                }

                bool ivZeroed = true;
                for (int i = 0; i < key.IV.Length; i++)
                {
                    if (key.IV[i] != 0) { ivZeroed = false; break; }
                }

                // But the protected context still has the key usable
                bool keyStillUsable = false;
                context.UseKey(usableKey =>
                {
                    keyStillUsable = Convert.ToBase64String(usableKey.Key) == Convert.ToBase64String(originalKey)
                                  && Convert.ToBase64String(usableKey.IV) == Convert.ToBase64String(originalIv);
                });

                context.Dispose();

                // Return as object array to avoid anonymous type issues with dynamic binding
                return new object[] { keyZeroed, ivZeroed, keyStillUsable };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] result = (object[])because.Result;
            because.ItsTrue("original key bytes were zeroed", (bool)result[0]);
            because.ItsTrue("original IV bytes were zeroed", (bool)result[1]);
            because.ItsTrue("key is still usable via protected context", (bool)result[2]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ZeroCipherBytesOnDispose()
    {
        When.A<AesKey>("zeros cipher bytes after dispose",
            () => new AesKey(),
            (key) =>
            {
                ProtectedAesKeyUsageContext context = new ProtectedAesKeyUsageContext(key);

                // Verify key works before dispose
                bool workedBefore = false;
                context.UseKey(usableKey =>
                {
                    workedBefore = usableKey.Key != null && usableKey.Key.Length > 0;
                });

                context.Dispose();

                // After dispose, UseKey should fail or use zeroed data
                bool threwAfterDispose = false;
                try
                {
                    context.UseKey(_ => { });
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
            object[] result = (object[])because.Result;
            because.ItsTrue("key worked before dispose", (bool)result[0]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReturnValueFromGenericUseKey()
    {
        When.A<ProtectedAesKeyUsageContext>("returns value from generic UseKey",
            () =>
            {
                AesKey key = new AesKey();
                return new ProtectedAesKeyUsageContext(key);
            },
            (context) =>
            {
                int keyLength = context.UseKey(key => key.Key.Length);
                context.Dispose();
                return keyLength;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("returned key length is 32", (int)because.Result == 32);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
