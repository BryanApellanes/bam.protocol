using System.Net;
using System.Net.Sockets;
using Bam.Logging;

namespace Bam.Protocol.Server;

public interface IBamRequestReader
{
    public event EventHandler ReadRequestStarted;
    
    IBamRequest ReadRequest(HttpListenerRequest request);
    IBamRequest ReadRequest(TcpClient client);
    IBamRequest ReadRequest(Stream stream);
}