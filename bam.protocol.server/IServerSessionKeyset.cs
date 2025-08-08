using Bam.Encryption;

namespace Bam.Protocol.Server;

public interface IServerSessionKeySet
{
    string SessionId { get; set; }
    string Nonce { get; set; }
    IPublicKey ClientPublicKey { get; set; }
    
}