using Newtonsoft.Json;

namespace Bam.Protocol;

public interface IArgument
{
    string ParameterName { get; set; }
    object Value { get; set; }
}