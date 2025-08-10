using System.Text.Json.Serialization;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;

namespace Bam.Protocol;

[JsonConverter(typeof(InterfaceTypeConverter<IProcessDescriptor, ProcessDescriptorData>))]
public interface IProcessDescriptor
{
    string CommandLine { get; set; }
    int? ExitCode { get; set; }
    DateTime ExitTime { get; set; }
    string FilePath { get; set; }
    bool HasExited { get; set; }
    string Hash { get; set; }
    string HashAlgorithm { get; set; }
    string InstanceIdentifier { get; set; }
    IMachine Machine { get; set; }
    ulong MachineId { get; set; }
    string MachineName { get; set; }
    int ProcessId { get; set; }
    DateTime StartTime { get; set; }
}