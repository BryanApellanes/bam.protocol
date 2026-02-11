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
            (builder) => builder.Build())
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IBamClientRequest>("Host equals default HTTP address", r => BamClient.DefaultHttpBaseAddress.Equals(r.Host))
                .As<IBamClientRequest>("Path is null or empty", r => string.IsNullOrEmpty(r.Path))
                .As<IBamClientRequest>("QueryString is null or empty", r => string.IsNullOrEmpty(r.QueryString))
                .As<IBamClientRequest>("HttpMethod equals GET", r => HttpMethods.GET.Equals(r.HttpMethod))
                .As<IBamClientRequest>("Protocol equals HTTP", r => "HTTP".Equals(r.Protocol))
                .As<IBamClientRequest>("ProtocolVersion equals 1.1", r => "1.1".Equals(r.ProtocolVersion))
                .As<IBamClientRequest>("Content is null", r => r.Content == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateTcpRequestFromBuilder()
    {
        When.A<TcpBamClientRequestBuilder>("creates a TCP request",
            (builder) => builder.Build())
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IBamClientRequest>("Host equals default TCP address", r => BamClient.DefaultTcpBaseAddress.Equals(r.Host))
                .As<IBamClientRequest>("Path is null or empty", r => string.IsNullOrEmpty(r.Path))
                .As<IBamClientRequest>("QueryString is null or empty", r => string.IsNullOrEmpty(r.QueryString))
                .As<IBamClientRequest>("HttpMethod equals POST", r => HttpMethods.POST.Equals(r.HttpMethod))
                .As<IBamClientRequest>("Protocol equals BAM", r => "BAM".Equals(r.Protocol))
                .As<IBamClientRequest>("ProtocolVersion equals 2.0", r => "2.0".Equals(r.ProtocolVersion))
                .As<IBamClientRequest>("Content is null", r => r.Content == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateUdpRequestFromBuilder()
    {
        When.A<UdpBamClientRequestBuilder>("creates a UDP request",
            (builder) => builder.Build())
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IBamClientRequest>("Host equals default TCP address", r => BamClient.DefaultTcpBaseAddress.Equals(r.Host))
                .As<IBamClientRequest>("Path is null or empty", r => string.IsNullOrEmpty(r.Path))
                .As<IBamClientRequest>("QueryString is null or empty", r => string.IsNullOrEmpty(r.QueryString))
                .As<IBamClientRequest>("HttpMethod equals PUT", r => HttpMethods.PUT.Equals(r.HttpMethod))
                .As<IBamClientRequest>("Protocol equals BAM", r => "BAM".Equals(r.Protocol))
                .As<IBamClientRequest>("ProtocolVersion equals 2.0", r => "2.0".Equals(r.ProtocolVersion))
                .As<IBamClientRequest>("Content is null", r => r.Content == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
