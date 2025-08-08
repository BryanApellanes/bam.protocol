namespace Bam.Protocol.Client;

public class BamClientResponse : IBamClientResponse
{
    public BamClientResponse(HttpResponseMessage responseMessage)
    {
        this.ResponseMessage = responseMessage;
        this.Content = responseMessage.Content.ReadAsStringAsync().Result;
    }

    public BamClientResponse(string content)
    {
        this.Content = content;
    }
    
    public string Content { get; }
    
    protected HttpResponseMessage ResponseMessage { get; }
    
    public int StatusCode => (int)ResponseMessage.StatusCode;

    public virtual IBamClientResponse Authorize(IBamClientResponse clientResponse)
    {
        return clientResponse;
    }

    public Dictionary<string, string> Headers => ResponseMessage.Headers.ToDictionary(x=> x.Key, x=> string.Join(", ", x.Value.ToArray()));
}