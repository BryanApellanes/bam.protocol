using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;

namespace Bam.Protocol.Server;

/// <summary>
/// Implements server session state backed by a persistent repository, storing key-value pairs.
/// </summary>
public class ServerSessionState : IServerSessionState
{
    readonly Dictionary<string, object> _state = new Dictionary<string, object>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSessionState"/> class from a session entity and repository.
    /// </summary>
    /// <param name="session">The server session entity.</param>
    /// <param name="repository">The repository for persisting session key-value pairs.</param>
    public ServerSessionState(ServerSession session, ServerSessionSchemaRepository repository)
    {
        this.Repository = repository;
        this.Data = session;
        if (session != null)
        {
            Load(session);
        }
    }

    bool _loading = false;
    private void Load(ServerSession session)
    {
        _loading = true;
        SessionId = session.SessionId;
        foreach (ServerSessionKeyValuePair value in session.KeyValues)
        {
            Set(value.Key, value.Value);
        }

        _loading = false;
    }

    protected ServerSessionSchemaRepository Repository { get; set; }
    protected ServerSession Data { get; set; }
    
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    public string SessionId { get; set; } = null!;

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="name">The key to look up.</param>
    /// <returns>The value, or null if not found.</returns>
    public object Get(string name)
    {
        return _state.TryGetValue(name, out var value) ? value : null!;
    }

    /// <summary>
    /// Gets the value associated with the specified key, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the value to.</typeparam>
    /// <param name="name">The key to look up.</param>
    /// <returns>The typed value, or the default of <typeparamref name="T"/> if not found.</returns>
    public T Get<T>(string name)
    {
        object val = Get(name);
        if (val == null)
        {
            return default(T)!;
        }
        return (T)val;
    }

    /// <summary>
    /// Sets the value associated with the specified key and persists it to the repository.
    /// </summary>
    /// <param name="name">The key to set.</param>
    /// <param name="value">The value to store.</param>
    public void Set(string name, object value)
    {
        _state[name] = value;
        Save(name, value);
    }

    /// <summary>
    /// Sets the value associated with the specified key using a typed value and persists it to the repository.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="name">The key to set.</param>
    /// <param name="value">The value to store.</param>
    public void Set<T>(string name, T value)
    {
        _state[name] = value!;
        Save(name, value!);
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to access.</param>
    /// <returns>The value associated with the key.</returns>
    public object this[string key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    /// <summary>
    /// Gets all keys currently stored in this session state.
    /// </summary>
    public string[] Keys => _state.Keys.ToArray();

    /// <summary>
    /// Gets all values currently stored in this session state.
    /// </summary>
    public object[] Values => _state.Values.ToArray();

    private void Save(string key, object value)
    {
        if (!_loading && Data != null)
        {
            Data.KeyValues.Add(new ServerSessionKeyValuePair()
            {
                Key = key,
                Value = value?.ToString() ?? "null"
            });
            if (Repository != null)
            {
                Data = Repository.Save(Data);
            }
        }
    }
}