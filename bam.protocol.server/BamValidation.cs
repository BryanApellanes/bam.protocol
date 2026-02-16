namespace Bam.Protocol.Server;

/// <summary>
/// Represents the result of a BAM validation operation.
/// </summary>
public class BamValidation
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation succeeded.
    /// </summary>
    public bool Success { get; internal set; }

    /// <summary>
    /// Gets or sets the validation messages.
    /// </summary>
    public string[] Messages { get; internal set; }
}