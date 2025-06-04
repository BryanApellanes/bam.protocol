using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

public interface IBamContextProvider
{
    IBamServerContext CreateContext(HttpListenerRequest httpRequest, string requestId);
    IBamServerContext CreateContext(TcpClient client, string requestId);
    IBamServerContext CreateContext(Stream stream, string requestId);
}