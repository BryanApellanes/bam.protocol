using Bam.DependencyInjection;
using Bam.Protocol.Server;
using Bam.Services;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("Request line should")]
public class RequestLineShould : UnitTestMenuContainer
{
    public RequestLineShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void ParseInputData()
    {
        string method = "POST";
        string uri = "/path/of/file";
        string protocolVersion = "BAM/2.0";
        string line = $"{method} {uri} {protocolVersion}";

        When.A<BamRequestLine>("parses input data correctly",
            () => new BamRequestLine(line),
            (requestLine) => requestLine)
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<BamRequestLine>("Method equals POST", r => HttpMethods.POST.Equals(r?.Method))
                .As<BamRequestLine>("RequestUri equals expected", r => uri.Equals(r?.RequestUri))
                .As<BamRequestLine>("ProtocolVersion equals expected", r => protocolVersion.Equals(r?.ProtocolVersion));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
