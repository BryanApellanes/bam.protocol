using Bam.Encryption;

namespace Bam.Protocol.Client;

public interface IClientSessionState : IDisposable
{
    string SessionId { get; }
    string Nonce { get; }
    string AuthorizationToken { get; }
    EccPublicKey ServerPublicKey { get; }

    AesKey DeriveSessionAesKey();
    void UseSessionKey(Action<AesKey> action);
    T UseSessionKey<T>(Func<AesKey, T> func);
}
