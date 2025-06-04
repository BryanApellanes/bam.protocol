namespace Bam.Protocol.Server;

public class BamServerContext : IBamServerContext
{
    public RequestType RequestType { get; set; }
    public string RequestId { get; internal set; }
    public IBamRequest BamRequest { get; internal set; }
    public IBamResponse BamResponse { get; set; }
    public IBamActor Actor { get; set; }
    public IBamSessionState SessionState { get; set; }
    public IBamAuthorizationCalculation AuthorizationCalculation { get; set; }
}