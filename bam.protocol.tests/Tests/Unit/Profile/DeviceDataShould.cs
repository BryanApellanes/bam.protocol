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
    public void SerializeAndDeserialize()
    {
        When.A<DeviceData>("serializes and deserializes",
            () => new DeviceData(true),
            (deviceData) =>
            {
                string json = deviceData.ToJson(true);
                DeviceData deserialized = json.FromJson<DeviceData>();
                return deserialized;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
