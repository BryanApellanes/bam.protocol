using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

public interface IBamServerContextProvider
{
    IBamServerContext CreateServerContext(HttpListenerRequest httpRequest, string requestId);
    IBamServerContext CreateServerContext(TcpClient client, string requestId);
    IBamServerContext CreateServerContext(Stream stream, string requestId);
}