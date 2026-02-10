using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Bam.Protocol.Server;

public class InitializationFailure
{
    public InitializationStatus Status { get; set; }
    public string Message { get; set; }
}