namespace Bam.Protocol;


public interface IDevice : IActor
{
    DeviceTypes DeviceType { get; set; }
}