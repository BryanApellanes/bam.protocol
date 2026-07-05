namespace Bam.Protocol.Client;

public class BamClientResponse : IBamClientResponse
{
    public BamClientResponse(HttpResponseMessage responseMessage)
    {
        this.ResponseMessage = responseMessage;
        this.Content = responseMessage.Content.ReadAsStringAsync().Result;
        // HTTP framing is handled by HttpClient, so Content is already the entity body.
        this.Body = this.Content;
    }

    public BamClientResponse(string content)
    {
        this.Content = content;
        this.Body = ParseBamResponse(content);
    }

    public string Content { get; }

    public string Body { get; }

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

    /// <summary>
    /// Parses a raw BAM response (<c>BAM/2.0 {status}</c> status line, optional headers, a blank line,
    /// then the entity body), setting the status code and returning the entity body. When the raw text
    /// carries no BAM framing, the whole string is treated as the body.
    /// </summary>
    private string ParseBamResponse(string raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return raw;
        }

        // Parse "BAM/2.0 {statusCode}" from the first line
        int newlineIndex = raw.IndexOf('\n');
        string firstLine = newlineIndex >= 0 ? raw.Substring(0, newlineIndex).Trim() : raw.Trim();
        if (!firstLine.StartsWith("BAM/"))
        {
            // No BAM framing — the raw content is the body.
            return raw;
        }

        string[] parts = firstLine.Split(' ', 2);
        if (parts.Length >= 2 && int.TryParse(parts[1].Trim(), out int code))
        {
            _statusCode = code;
        }

        // The entity body follows the first blank line separating the status line/headers from the body.
        int separatorIndex = raw.IndexOf("\n\n", StringComparison.Ordinal);
        int separatorLength = 2;
        if (separatorIndex < 0)
        {
            separatorIndex = raw.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            separatorLength = 4;
        }

        return separatorIndex >= 0 ? raw.Substring(separatorIndex + separatorLength).Trim() : string.Empty;
    }
}
