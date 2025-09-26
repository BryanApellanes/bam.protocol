using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Newtonsoft.Json;

namespace Bam.Protocol;

//[JsonConverter(typeof(InterfaceTypeConverter<IDevice, DeviceData>))]
public interface IDevice : IActor
{
    DeviceTypes DeviceType { get; set; }
}