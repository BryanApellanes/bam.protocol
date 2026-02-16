using Bam.Encryption;

namespace Bam.Protocol.Client;

/// <summary>
/// Represents the state of an established client session, providing access to session keys and tokens.
/// </summary>
public interface IClientSessionState : IDisposable
{
    /// <summary>
    /// Gets the unique identifier for this session.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// Gets the nonce value used for key derivation.
    /// </summary>
    string Nonce { get; }

    /// <summary>
    /// Gets the authorization token for authenticated requests.
    /// </summary>
    string AuthorizationToken { get; }

    /// <summary>
    /// Gets the server's ECC public key for this session.
    /// </summary>
    EccPublicKey ServerPublicKey { get; }

    /// <summary>
    /// Derives an AES key from the session parameters for symmetric encryption.
    /// </summary>
    /// <returns>The derived AES key.</returns>
    AesKey DeriveSessionAesKey();

    /// <summary>
    /// Executes the specified action with the session AES key.
    /// </summary>
    /// <param name="action">The action to execute with the session key.</param>
    void UseSessionKey(Action<AesKey> action);

    /// <summary>
    /// Executes the specified function with the session AES key and returns the result.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="func">The function to execute with the session key.</param>
    /// <returns>The result of the function.</returns>
    T UseSessionKey<T>(Func<AesKey, T> func);
}
