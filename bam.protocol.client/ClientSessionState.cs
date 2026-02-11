using Bam.Encryption;

namespace Bam.Protocol.Client;

public class ClientSessionState : IClientSessionState
{
    private bool _disposed = false;
    private ProtectedAesKeyUsageContext _protectedSessionKey;
    private readonly object _keyLock = new object();

    public ClientSessionState(string sessionId, string nonce, EccPublicKey serverPublicKey, EccPublicPrivateKeyPair clientKeyPair)
    {
        SessionId = sessionId;
        Nonce = nonce;
        ServerPublicKey = serverPublicKey;
        ClientKeyPair = clientKeyPair;
    }

    public string SessionId { get; }
    public string Nonce { get; }
    public string AuthorizationToken { get; set; }
    public EccPublicKey ServerPublicKey { get; }
    protected internal EccPublicPrivateKeyPair ClientKeyPair { get; }

    public AesKey DeriveSessionAesKey()
    {
        return ClientKeyPair.GetSharedAesKey(ServerPublicKey.Pem);
    }

    public void UseSessionKey(Action<AesKey> action)
    {
        EnsureProtectedKey();
        _protectedSessionKey.UseKey(action);
    }

    public T UseSessionKey<T>(Func<AesKey, T> func)
    {
        EnsureProtectedKey();
        return _protectedSessionKey.UseKey(func);
    }

    private void EnsureProtectedKey()
    {
        if (_protectedSessionKey != null)
        {
            return;
        }

        lock (_keyLock)
        {
            if (_protectedSessionKey != null)
            {
                return;
            }

            AesKey derivedKey = DeriveSessionAesKey();
            _protectedSessionKey = new ProtectedAesKeyUsageContext(derivedKey);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _protectedSessionKey?.Dispose();
            ClientKeyPair?.Dispose();
        }

        _disposed = true;
    }
}
