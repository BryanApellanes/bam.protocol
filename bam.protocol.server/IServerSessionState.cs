namespace Bam.Protocol.Server;

/// <summary>
/// Defines key-value state storage for a server session.
/// </summary>
public interface IServerSessionState
{
    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value, or null if not found.</returns>
    object Get(string key);

    /// <summary>
    /// Gets the value associated with the specified key, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the value to.</typeparam>
    /// <param name="key">The key to look up.</param>
    /// <returns>The typed value, or the default of <typeparamref name="T"/> if not found.</returns>
    T Get<T>(string key);

    /// <summary>
    /// Sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to store.</param>
    void Set(string key, object value);

    /// <summary>
    /// Sets the value associated with the specified key using a typed value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to store.</param>
    void Set<T>(string key, T value);

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to access.</param>
    /// <returns>The value associated with the key.</returns>
    object this[string key] { get; set; }

    /// <summary>
    /// Gets all keys currently stored in this session state.
    /// </summary>
    string[] Keys { get; }

    /// <summary>
    /// Gets all values currently stored in this session state.
    /// </summary>
    object[] Values { get; }
}