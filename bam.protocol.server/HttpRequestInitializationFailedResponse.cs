using System.Net;
using System.Text;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents an HTTP response sent when request initialization fails.
/// </summary>
public class HttpRequestInitializationFailedResponse : BamResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestInitializationFailedResponse"/> class.
    /// </summary>
    /// <param name="initialization">The initialization context containing failure details.</param>
    public HttpRequestInitializationFailedResponse(BamServerInitializationContext initialization) : base(initialization.EventArgs.HttpContext.Response.OutputStream, 400)
    {
        this.Initialization = initialization;
        this.CopyProperties(Response);
    }

    protected BamServerInitializationContext Initialization { get; private set; }
    protected HttpListenerResponse Response => Initialization.EventArgs.HttpContext.Response;
    protected Encoding Encoding => Initialization.Server.Encoding;
    protected IObjectEncoderDecoder ObjectEncoderDecoder => Initialization.Server.ObjectEncoderDecoder;
    
    /// <summary>
    /// Sends the initialization failure response to the HTTP client.
    /// </summary>
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

    /// <summary>
    /// Sends the specified byte array as the response entity to the HTTP client.
    /// </summary>
    /// <param name="responseEntity">The response bytes to send.</param>
    public override void Send(byte[] responseEntity)
    {
        Response.StatusCode = 400;
        Response.OutputStream.Write(responseEntity, 0, responseEntity.Length);
        Response.OutputStream.Flush();
        Response.Close();
    }
}