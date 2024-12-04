using Bam.Protocol.Server;

namespace Bam.Protocol;

public class BamAuthentication
{
    public bool Success { get; }
    public string[] Messages { get; }

    public IBamActor Actor { get; }
    
    public IBamRequest Request { get; }
}