using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

public interface IAuthorizationCalculation
{
    string[] Messages { get; }
    BamAccess Access { get; }
    IBamRequest Request { get; }
    IBamResponse Response { get; }
    IActor Actor { get; }
}