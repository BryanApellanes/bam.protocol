using System.IO;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

public interface IBamRequestReader
{
    IBamRequest ReadRequest(TcpClient client);
    IBamRequest ReadRequest(Stream stream);
}