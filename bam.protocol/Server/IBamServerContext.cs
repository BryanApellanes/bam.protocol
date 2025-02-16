/*
	Copyright © Bryan Apellanes 2022  
*/

namespace Bam.Protocol.Server
{
    public interface IBamServerContext
    {
        NetworkProtocols RequestProtocol { get; set; }
        string RequestId { get; }
        IBamRequest BamRequest { get; }
        IBamResponse BamResponse { get; set; }
        IBamActor Actor { get; set; }
        IBamSessionState SessionState { get; set; }
        IBamAuthorizationCalculation AuthorizationCalculation { get; set; }
    }
}