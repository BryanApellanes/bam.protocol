using Bam.Protocol.Data;
using Bam.Protocol.Server;

namespace Bam.Protocol;

public class BamAuthentication
{
    public BamAuthentication(bool success, IActor actor, IBamRequest request, string[] messages = null)
    {
        Success = success;
        Actor = actor;
        Request = request;
        Messages = messages ?? Array.Empty<string>();
    }

    public bool Success { get; }
    public string[] Messages { get; }

    public IActor Actor { get; }

    public IBamRequest Request { get; }
}