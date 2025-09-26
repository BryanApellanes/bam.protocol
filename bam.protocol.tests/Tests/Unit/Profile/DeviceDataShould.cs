using Bam.Console;
using Bam.Data;
using Bam.Data.Repositories;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;


[UnitTestMenu("DeviceData Should", Selector = "dds")]
public class DeviceDataShould : UnitTestMenuContainer
{
    [UnitTest]
    public async Task SerializeAndDeserialize()
    {
        DeviceData deviceData = new DeviceData(true);
        string json = deviceData.ToJson(true);
        Message.PrintLine(json);
        
        DeviceData deserialized = json.FromJson<DeviceData>();
        deserialized.HostAddresses.Each(ha=> Message.PrintLine(ha.ToString()));
    }
}