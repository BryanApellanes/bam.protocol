using Bam.Protocol.Server;
using System.Net;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamServer builder should")]
public class BamServerBuilderShould : UnitTestMenuContainer
{
    [UnitTest]
    public void BuildServer()
    {
        When.A<BamServerBuilder>("builds a server",
            (builder) => builder.Build())
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BuildBamProtocolServer()
    {
        int testTcpPort = RandomNumber.Between(1, 50);
        int testUdpPort = RandomNumber.Between(51, 100);
        string tcpIpAddress = "10.0.0.10";
        string udpIpAddress = "10.0.0.11";
        string serverName = "Test Server Name: ".RandomLetters(8);

        When.A<BamServerBuilder>("builds a configured server",
            () => new BamServerBuilder()
                .TcpPort(testTcpPort)
                .UdpPort(testUdpPort)
                .ServerName(serverName)
                .TcpIPAddress(tcpIpAddress)
                .UdpIPAddress(udpIpAddress),
            (builder) =>
            {
                BamServer server = builder.Build();
                BamServerInfo info = server.GetInfo();
                return new object[] { info, server.HttpHostBinding };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            BamServerInfo info = (BamServerInfo)results[0];
            object httpHostBinding = results[1];
            because.ItsTrue("ServerName equals expected", serverName.Equals(info.ServerName));
            because.ItsTrue("TcpPort equals expected", testTcpPort == info.TcpPort);
            because.ItsTrue("UdpPort equals expected", testUdpPort == info.UdpPort);
            because.ItsTrue("TcpIPAddress equals expected", IPAddress.Parse(tcpIpAddress).ToString().Equals(info.TcpIPAddress));
            because.ItsTrue("UdpIPAddress equals expected", IPAddress.Parse(udpIpAddress).ToString().Equals(info.UdpIPAddress));
            because.ItsTrue("HttpHostBinding is not null", info.HttpHostBinding != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
