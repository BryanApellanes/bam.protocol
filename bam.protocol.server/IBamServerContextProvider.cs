using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

public interface IBamServerContextProvider
{
    IBamServerContext CreateServerContext(HttpListenerContext httpContext, string requestId);
    IBamServerContext CreateServerContext(TcpClient client, string requestId);
    IBamServerContext CreateServerContext(Stream stream, string requestId);
}