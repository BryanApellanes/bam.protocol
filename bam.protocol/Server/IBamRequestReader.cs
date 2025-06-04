using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

public interface IBamRequestReader
{
    IBamRequest ReadRequest(HttpListenerRequest request);
    IBamRequest ReadRequest(TcpClient client);
    IBamRequest ReadRequest(Stream stream);
}