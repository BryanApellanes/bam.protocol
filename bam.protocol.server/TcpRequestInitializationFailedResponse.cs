using System.Text;

namespace Bam.Protocol.Server;

public class TcpRequestInitializationFailedResponse : BamResponse
{
    public TcpRequestInitializationFailedResponse(BamServerInitializationContext initialization)
        : base(initialization.ServerContext.OutputStream, 400)
    {
        this.Initialization = initialization;
    }

    protected BamServerInitializationContext Initialization { get; private set; }
    protected Encoding Encoding => Initialization.Server.Encoding;
    protected IObjectEncoderDecoder ObjectEncoderDecoder => Initialization.Server.ObjectEncoderDecoder;

    public override void Send()
    {
        InitializationFailure failure = new InitializationFailure()
        {
            Status = Initialization.Status,
            Message = Initialization.Message,
        };
        string encoded = ObjectEncoderDecoder.Stringify(failure);
        Send(Encoding.GetBytes(encoded));
    }

    public override void Send(byte[] responseEntity)
    {
        string statusLine = $"BAM/2.0 {StatusCode}\n\n";
        byte[] header = Encoding.GetBytes(statusLine);
        OutputStream.Write(header, 0, header.Length);
        OutputStream.Write(responseEntity, 0, responseEntity.Length);
        OutputStream.Flush();
    }
}
