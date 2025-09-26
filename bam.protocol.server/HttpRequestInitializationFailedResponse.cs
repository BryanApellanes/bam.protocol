using System.Net;
using System.Text;

namespace Bam.Protocol.Server;

public class HttpRequestInitializationFailedResponse : BamResponse
{
    public HttpRequestInitializationFailedResponse(BamServerInitializationContext initialization) : base(initialization.EventArgs.HttpContext.Response.OutputStream, 400)
    {
        this.Initialization = initialization;
        this.CopyProperties(Response);
    }

    protected BamServerInitializationContext Initialization { get; private set; }
    protected HttpListenerResponse Response => Initialization.EventArgs.HttpContext.Response;
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
        Send(Initialization.Server.Encoding.GetBytes(encoded));
    }

    public override void Send(byte[] responseEntity)
    {
        Response.StatusCode = 400;
        Response.OutputStream.Write(responseEntity, 0, responseEntity.Length);
        Response.OutputStream.Flush();
        Response.Close();
    }
}