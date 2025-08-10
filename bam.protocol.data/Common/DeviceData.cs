using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Common;


public class DeviceData : MachineData, IDevice
{
    public DeviceData()
    {
        this.ProcessDescriptorData = ProcessDescriptorData.Current;
        // TODO: determine DeviceType
    }
    
    public virtual ulong ProcessDescriptorId { get; set; }
    
    public virtual ProcessDescriptorData ProcessDescriptorData { get; set; }
    public DeviceTypes DeviceType { get; set; }
    public string Key { get; }
    
    public string Handle { get; }
}