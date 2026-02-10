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
            (requestLine) => new object[] { requestLine.Method, requestLine.RequestUri, requestLine.ProtocolVersion })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            because.ItsTrue("Method equals POST", HttpMethods.POST.Equals(results[0]));
            because.ItsTrue("RequestUri equals expected", uri.Equals(results[1]));
            because.ItsTrue("ProtocolVersion equals expected", protocolVersion.Equals(results[2]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
