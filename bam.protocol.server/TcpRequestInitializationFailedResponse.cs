using System.Text;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents a TCP response sent when request initialization fails.
/// </summary>
public class TcpRequestInitializationFailedResponse : BamResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TcpRequestInitializationFailedResponse"/> class.
    /// </summary>
    /// <param name="initialization">The initialization context containing failure details.</param>
    public TcpRequestInitializationFailedResponse(BamServerInitializationContext initialization)
        : base(initialization.ServerContext.OutputStream!, 400)
    {
        this.Initialization = initialization;
    }

    protected BamServerInitializationContext Initialization { get; private set; }
    protected Encoding Encoding => Initialization.Server.Encoding;
    protected IObjectEncoderDecoder ObjectEncoderDecoder => Initialization.Server.ObjectEncoderDecoder;

    /// <summary>
    /// Sends the initialization failure response over the TCP stream.
    /// </summary>
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

    /// <summary>
    /// Sends the specified byte array as the response entity over the TCP stream, prefixed with a status line.
    /// </summary>
    /// <param name="responseEntity">The response bytes to send.</param>
    public override void Send(byte[] responseEntity)
    {
        string statusLine = $"BAM/2.0 {StatusCode}\n\n";
        byte[] header = Encoding.GetBytes(statusLine);
        OutputStream.Write(header, 0, header.Length);
        OutputStream.Write(responseEntity, 0, responseEntity.Length);
        OutputStream.Flush();
    }
}
