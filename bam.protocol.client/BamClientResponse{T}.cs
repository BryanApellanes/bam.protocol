namespace Bam.Protocol.Client;

public class BamClientResponse<T>: BamClientResponse, IBamClientResponse<T>
{
    public BamClientResponse(HttpResponseMessage responseMessage) : base(responseMessage)
    {
    }
}