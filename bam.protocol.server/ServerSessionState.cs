using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;

namespace Bam.Protocol.Server;

public class ServerSessionState : IServerSessionState
{
    readonly Dictionary<string, object> _state = new Dictionary<string, object>();

    public ServerSessionState(ServerSession session, ServerSessionDataRepository repository)
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

    protected ServerSessionDataRepository Repository { get; set; }
    protected ServerSession Data { get; set; }
    
    public string SessionId { get; set; }
    public object Get(string name)
    {
        return _state.TryGetValue(name, out var value) ? value : null;
    }

    public T Get<T>(string name)
    {
        object val = Get(name);
        if (val == null)
        {
            return default(T);
        }
        return (T)val;
    }

    public void Set(string name, object value)
    {
        _state[name] = value;
        Save(name, value);
    }

    public void Set<T>(string name, T value)
    {
        _state[name] = value;
        Save(name, value);
    }

    public object this[string key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public string[] Keys => _state.Keys.ToArray();
    public object[] Values => _state.Values.ToArray();

    private void Save(string key, object value)
    {
        if (!_loading)
        {
            Data.KeyValues.Add(new ServerSessionKeyValuePair()
            {
                Key = key,
                Value = value?.ToString() ?? "null"
            });
            Data = Repository.Save(Data);
        }
    }
}