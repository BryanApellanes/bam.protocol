using Bam.Encryption;
using Bam.Protocol.Server;

namespace Bam.Protocol;

public class StartSessionRequest : BamRequest
{
    public EccPublicKey ClientPublicKey { get; set; }
}