namespace Bam.Protocol;

public class ServerIdentity : IServerIdentity
{
    public ServerIdentity(string serverName)
    {
        ServerName = serverName;
    }

    public string ServerName { get; }
}
