using System.IO;

namespace Bam.Protocol.Server;

public interface IBamRequestReader
{
    IBamRequest ReadRequest(Stream stream);
}