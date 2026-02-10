using Bam.Protocol.Client;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamClientRequestBuilder should")]
public class BamClientRequestBuilderShould : UnitTestMenuContainer
{
    [UnitTest]
    public void CreateHttpRequestFromBuilder()
    {
        When.A<HttpBamClientRequestBuilder>("creates an HTTP request",
            (builder) =>
            {
                IBamClientRequest request = builder.Build();
                return new object?[] { request.Host, request.Path, request.QueryString, request.HttpMethod, request.Protocol, request.ProtocolVersion, request.Content };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] r = (object?[])because.Result;
            because.ItsTrue("Host equals default HTTP address", BamClient.DefaultHttpBaseAddress.Equals(r[0]));
            because.ItsTrue("Path is null or empty", string.IsNullOrEmpty((string?)r[1]));
            because.ItsTrue("QueryString is null or empty", string.IsNullOrEmpty((string?)r[2]));
            because.ItsTrue("HttpMethod equals GET", HttpMethods.GET.Equals(r[3]));
            because.ItsTrue("Protocol equals HTTP", "HTTP".Equals(r[4]));
            because.ItsTrue("ProtocolVersion equals 1.1", "1.1".Equals(r[5]));
            because.ItsTrue("Content is null", r[6] == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateTcpRequestFromBuilder()
    {
        When.A<TcpBamClientRequestBuilder>("creates a TCP request",
            (builder) =>
            {
                IBamClientRequest request = builder.Build();
                return new object?[] { request.Host, request.Path, request.QueryString, request.HttpMethod, request.Protocol, request.ProtocolVersion, request.Content };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] r = (object?[])because.Result;
            because.ItsTrue("Host equals default TCP address", BamClient.DefaultTcpBaseAddress.Equals(r[0]));
            because.ItsTrue("Path is null or empty", string.IsNullOrEmpty((string?)r[1]));
            because.ItsTrue("QueryString is null or empty", string.IsNullOrEmpty((string?)r[2]));
            because.ItsTrue("HttpMethod equals POST", HttpMethods.POST.Equals(r[3]));
            because.ItsTrue("Protocol equals BAM", "BAM".Equals(r[4]));
            because.ItsTrue("ProtocolVersion equals 2.0", "2.0".Equals(r[5]));
            because.ItsTrue("Content is null", r[6] == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateUdpRequestFromBuilder()
    {
        When.A<UdpBamClientRequestBuilder>("creates a UDP request",
            (builder) =>
            {
                IBamClientRequest request = builder.Build();
                return new object?[] { request.Host, request.Path, request.QueryString, request.HttpMethod, request.Protocol, request.ProtocolVersion, request.Content };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] r = (object?[])because.Result;
            because.ItsTrue("Host equals default TCP address", BamClient.DefaultTcpBaseAddress.Equals(r[0]));
            because.ItsTrue("Path is null or empty", string.IsNullOrEmpty((string?)r[1]));
            because.ItsTrue("QueryString is null or empty", string.IsNullOrEmpty((string?)r[2]));
            because.ItsTrue("HttpMethod equals PUT", HttpMethods.PUT.Equals(r[3]));
            because.ItsTrue("Protocol equals BAM", "BAM".Equals(r[4]));
            because.ItsTrue("ProtocolVersion equals 2.0", "2.0".Equals(r[5]));
            because.ItsTrue("Content is null", r[6] == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
