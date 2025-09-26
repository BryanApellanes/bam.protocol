using System.Net;
using System.Text;
using Bam.Protocol.Server;

namespace Bam.Protocol;

public class BamResponse<T> : BamResponse, IBamResponse<T>
{
    BamResponse(BamServerInitializationContext initializationContext, T content, int statusCode = 404) : this(initializationContext, statusCode)
    {
        this.Content = content;
    }

    public BamResponse(BamServerInitializationContext initializationContext, int statusCode = 404) : base(initializationContext.ServerContext.HttpContext.Response.OutputStream, statusCode)
    {
        this.BamServerInitializationContext = initializationContext;
    }

    protected BamServerInitializationContext BamServerInitializationContext { get; set; }
    protected BamServer Server => BamServerInitializationContext.Server;
    protected IBamServerContext BamServerContext => BamServerInitializationContext.ServerContext;

    protected IObjectEncoderDecoder BamObjectEncoderDecoder => Server.ObjectEncoderDecoder;

    protected HttpListenerResponse Response => BamServerContext.HttpContext.Response;

    protected Encoding Encoding => Server.Encoding;

    protected void Close()
    {
        Response.OutputStream.Flush();
        Response.Close();
    }
    
    public override void Send()
    {
       if (Content != null)
       {
           string encoded = BamObjectEncoderDecoder.Stringify(Content);
           Send(Encoding.GetBytes(encoded));
           return;
       }

       Response.StatusCode = StatusCode;
       Close();
    }

    public override void Send(byte[] responseEntity)
    {
        Response.StatusCode = StatusCode;
        Response.OutputStream.Write(responseEntity, 0, responseEntity.Length);
        Close();
    }

    public T Content { get; set; }
    
}