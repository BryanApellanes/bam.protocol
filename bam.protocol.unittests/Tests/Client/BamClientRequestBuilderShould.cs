

using Bam.Net;
using Bam.Net.CoreServices;
using Bam.Protocol.Client;
using Bam.Testing;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamClientRequestBuilder should")]
public class BamClientRequestBuilderShould 
{
    [UnitTest]
    public void CreateHttpRequestFromBuilder()
    {
        BamClientRequestBuilder requestBuilder = new HttpBamClientRequestBuilder();
        IBamClientRequest request = requestBuilder.Build();
        
        request.Host.ShouldBeEqualTo(BamClient.DefaultHttpBaseAddress);
        request.Path.ShouldBeNullOrEmpty();
        request.QueryString.ShouldBeNullOrEmpty();
        request.HttpMethod.ShouldBeEqualTo(HttpMethods.GET);
        request.Protocol.ShouldBeEqualTo("HTTP");
        request.ProtocolVersion.ShouldBeEqualTo("1.1");
        request.Content.ShouldBeNull();
    }

    [UnitTest]
    public void CreateTcpRequestFromBuilder()
    {
        BamClientRequestBuilder requestBuilder = new TcpBamClientRequestBuilder();
        IBamClientRequest request = requestBuilder.Build();
        
        request.Host.ShouldBeEqualTo(BamClient.DefaultTcpBaseAddress);
        request.Path.ShouldBeNullOrEmpty();
        request.QueryString.ShouldBeNullOrEmpty();
        request.HttpMethod.ShouldBeEqualTo(HttpMethods.POST);
        request.Protocol.ShouldBeEqualTo("BAM");
        request.ProtocolVersion.ShouldBeEqualTo("2.0");
        request.Content.ShouldBeNull();
    }
    
    [UnitTest]
    public void CreateUdpRequestFromBuilder()
    {
        BamClientRequestBuilder requestBuilder = new UdpBamClientRequestBuilder();
        IBamClientRequest request = requestBuilder.Build();
        
        request.Host.ShouldBeEqualTo(BamClient.DefaultTcpBaseAddress);
        request.Path.ShouldBeNullOrEmpty();
        request.QueryString.ShouldBeNullOrEmpty();
        request.HttpMethod.ShouldBeEqualTo(HttpMethods.PUT);
        request.Protocol.ShouldBeEqualTo("BAM");
        request.ProtocolVersion.ShouldBeEqualTo("2.0");
        request.Content.ShouldBeNull();
    }

    /*public BamClientRequestBuilderShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }*/
}