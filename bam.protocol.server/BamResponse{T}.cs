using System.Net;
using System.Text;
using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// A typed BAM response that serializes and sends content of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the response content.</typeparam>
public class BamResponse<T> : BamResponse, IBamResponse<T>
{
    BamResponse(BamServerInitializationContext initializationContext, T content, int statusCode = 404) : this(initializationContext, statusCode)
    {
        this.Content = content;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamResponse{T}"/> class with the specified initialization context and status code.
    /// </summary>
    /// <param name="initializationContext">The server initialization context.</param>
    /// <param name="statusCode">The HTTP status code. Defaults to 404.</param>
    public BamResponse(BamServerInitializationContext initializationContext, int statusCode = 404) : base(initializationContext.ServerContext.OutputStream, statusCode)
    {
        this.BamServerInitializationContext = initializationContext;
    }

    protected BamServerInitializationContext BamServerInitializationContext { get; set; }
    protected BamServer Server => BamServerInitializationContext.Server;
    protected IBamServerContext BamServerContext => BamServerInitializationContext.ServerContext;

    protected IObjectEncoderDecoder BamObjectEncoderDecoder => Server.ObjectEncoderDecoder;

    protected HttpListenerResponse? Response => BamServerContext.HttpContext?.Response;

    protected Encoding Encoding => Server.Encoding;

    protected void Close()
    {
        if (Response != null)
        {
            Response.OutputStream.Flush();
            Response.Close();
        }
        else
        {
            OutputStream.Flush();
        }
    }

    /// <summary>
    /// Sends the response, serializing the content if present.
    /// </summary>
    public override void Send()
    {
       if (Content != null)
       {
           string encoded = BamObjectEncoderDecoder.Stringify(Content);
           Send(Encoding.GetBytes(encoded));
           return;
       }

       if (Response != null)
       {
           Response.StatusCode = StatusCode;
       }
       Close();
    }

    /// <summary>
    /// Sends the specified byte array as the response entity.
    /// </summary>
    /// <param name="responseEntity">The response bytes to send.</param>
    public override void Send(byte[] responseEntity)
    {
        if (Response != null)
        {
            Response.StatusCode = StatusCode;
            Response.OutputStream.Write(responseEntity, 0, responseEntity.Length);
            Close();
        }
        else
        {
            string statusLine = $"BAM/2.0 {StatusCode}\n\n";
            byte[] header = Encoding.GetBytes(statusLine);
            OutputStream.Write(header, 0, header.Length);
            OutputStream.Write(responseEntity, 0, responseEntity.Length);
            OutputStream.Flush();
        }
    }

    /// <summary>
    /// Gets or sets the typed content of this response.
    /// </summary>
    public T Content { get; set; }

}
