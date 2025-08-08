namespace Bam.Protocol.Server;

public interface IServerSessionState
{
    string SessionId { get; }
    object Get(string key);
    T Get<T>(string key);
    void Set(string key, object value);
    void Set<T>(string key, T value);
    
    object this[string key] { get; set; }
    
    string[] Keys { get; }
    object[] Values { get; }
}