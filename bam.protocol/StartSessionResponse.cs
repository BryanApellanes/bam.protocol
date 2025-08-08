using Bam.Encryption;
using Bam.Protocol.Server;

namespace Bam.Protocol;

public class StartSessionResponse : BamResponse
{
    public StartSessionResponse(string nonce, EccPublicKey serverPublicKey, Stream outputStream, int statusCode = 404) : base(outputStream, statusCode)
    {
        Nonce = nonce;
        ServerPublicKey = serverPublicKey;
    }

    public string Nonce { get; set; }
    public EccPublicKey ServerPublicKey { get; set; }
    public override void Send()
    {
        throw new NotImplementedException();
    }

    public override void Send(byte[] responseEntity)
    {
        throw new NotImplementedException();
    }
}