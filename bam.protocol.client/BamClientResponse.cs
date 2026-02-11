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
        ParseBamResponse(content);
    }

    public string Content { get; }

    protected HttpResponseMessage? ResponseMessage { get; }

    private int _statusCode;
    private Dictionary<string, string> _headers = new Dictionary<string, string>();

    public int StatusCode => ResponseMessage != null
        ? (int)ResponseMessage.StatusCode
        : _statusCode;

    public virtual IBamClientResponse Authorize(IBamClientResponse clientResponse)
    {
        return clientResponse;
    }

    public Dictionary<string, string> Headers => ResponseMessage != null
        ? ResponseMessage.Headers.ToDictionary(x=> x.Key, x=> string.Join(", ", x.Value.ToArray()))
        : _headers;

    private void ParseBamResponse(string raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return;
        }

        // Parse "BAM/2.0 {statusCode}" from the first line
        int newlineIndex = raw.IndexOf('\n');
        string firstLine = newlineIndex >= 0 ? raw.Substring(0, newlineIndex).Trim() : raw.Trim();
        if (firstLine.StartsWith("BAM/"))
        {
            string[] parts = firstLine.Split(' ', 2);
            if (parts.Length >= 2 && int.TryParse(parts[1].Trim(), out int code))
            {
                _statusCode = code;
            }
        }
    }
}
