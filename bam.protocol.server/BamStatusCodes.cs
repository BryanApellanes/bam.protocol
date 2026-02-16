namespace Bam.Protocol.Server;

/// <summary>
/// Provides status code descriptions for the BAM protocol.
/// </summary>
public class BamStatusCodes
{
    static readonly Dictionary<int, string> _descriptions;
    static BamStatusCodes()
    {
        _descriptions = new Dictionary<int, string>
        {
            { 200, "OK" },
            { 401, "Unauthorized" },
            { 404, "NOT FOUND" }
        };
    }
    
    /// <summary>
    /// Gets the human-readable description for the specified status code.
    /// </summary>
    /// <param name="code">The numeric status code.</param>
    /// <returns>The description string, or an empty string if the code is not recognized.</returns>
    public static string GetDescription(int code)
    {
        if (_descriptions.ContainsKey(code))
        {
            return _descriptions[code];
        }

        return string.Empty;
    }
}