namespace Bam.Protocol.Client;

public class BamClientResponse : IBamClientResponse
{
    public BamClientResponse(HttpResponseMessage responseMessage)
    {
        this.ResponseMessage = responseMessage;
    }

    public BamClientResponse(string response)
    {
        this.Response = response;
    }
    
    public string Response { get; }
    
    protected HttpResponseMessage ResponseMessage { get; }
    
    public int StatusCode => (int)ResponseMessage.StatusCode;

    public virtual IBamClientResponse Authorize(IBamClientResponse clientResponse)
    {
        return clientResponse;
    }
}