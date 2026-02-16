using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents a serializable initialization failure containing the status and message.
/// </summary>
public class InitializationFailure
{
    /// <summary>
    /// Gets or sets the initialization status that represents the failure.
    /// </summary>
    public InitializationStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the message describing the failure.
    /// </summary>
    public string Message { get; set; }
}