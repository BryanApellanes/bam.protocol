namespace Bam.Protocol.Client;

public interface IBamClientResponse
{
    int StatusCode { get; }
    string Content { get; }
    IBamClientResponse Authorize(IBamClientResponse clientResponse);
    Dictionary<string, string> Headers { get; }
}