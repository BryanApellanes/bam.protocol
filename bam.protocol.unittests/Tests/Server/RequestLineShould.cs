using Bam.Net;
using Bam.Net.CoreServices;
using Bam.Protocol.Server;
using Bam.Testing;

namespace Bam.Protocol.Tests;

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
        BamRequestLine bamRequestLine = new BamRequestLine(line);

        bamRequestLine.Method.ShouldBeEqualTo(HttpMethods.POST);
        bamRequestLine.RequestUri.ShouldBeEqualTo(uri);
        
        bamRequestLine.ProtocolVersion.ShouldBeEqualTo(protocolVersion);
    }
}