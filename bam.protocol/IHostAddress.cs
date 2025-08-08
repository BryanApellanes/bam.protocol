namespace Bam.Protocol.Data;

public interface IHostAddress
{
    ulong MachineId { get; set; }
    IMachine Machine { get; set; }
    string IpAddress { get; set; }
    string AddressFamily { get; set; }
    string HostName { get; set; }
}