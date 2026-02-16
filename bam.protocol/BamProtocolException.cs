namespace Bam.Protocol;

/// <summary>
/// Represents errors that occur during Bam protocol operations.
/// </summary>
public class BamProtocolException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamProtocolException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BamProtocolException(string message) : base(message)
    {
    }
}