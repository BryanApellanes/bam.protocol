using Bam.Data.Repositories;

namespace Bam.Protocol.Data;


public class DeviceData : MachineData, IDevice
{
    public DeviceData()
    {
        this.ProcessDescriptor = ProcessDescriptor.Current;
    }
    
    public virtual ulong ProcessDescriptorId { get; set; }
    
    public virtual ProcessDescriptor ProcessDescriptor { get; set; }
    public DeviceTypes DeviceType { get; set; }
    public string Key { get; }
    
    public string Handle { get; }
}