namespace Bam.Protocol.Data;

public interface INicData
{
    ulong MachineId { get; set; }
    IMachine Machine { get; set; }
    string AddressFamily { get; set; }
    string Address { get; set; }
    string MacAddress { get; set; }
}