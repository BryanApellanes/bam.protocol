using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bam.Protocol.Server;

public class BamRequestReader : IBamRequestReader
{
    public BamRequestReader(BamRequestReaderOptions options)
    {
        this.Options = options;
    }
    
    protected BamRequestReaderOptions Options { get; set; }

    public int BufferSize => Options.RequestBufferSize;

    public IBamRequest ReadRequest(HttpListenerRequest request)
    {
        BamRequest bamRequest = new BamRequest(GetBamRequestLine(request))
        {
            Headers = ReadHeaders(request),
            Content = ReadContentString(request.InputStream)
        };

        return bamRequest;
    }
    public IBamRequest ReadRequest(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] readBuffer = new byte[client.Available];
        stream.Read(readBuffer, 0, readBuffer.Length);
        string request = Encoding.UTF8.GetString(readBuffer);

        return new BamRequest(){Content = request};
    }
    
    public virtual IBamRequest ReadRequest(Stream stream)
    {
        BamRequestLine line = ReadRequestLine(stream);
        BamRequest bamRequest = new BamRequest(line)
        {
            Headers = ReadHeaders(stream),
            Content = ReadContentString(stream)
        };

        // TODO: ensure other request properties are set
        
        return bamRequest;
    }

    protected BamRequestLine ReadRequestLine(Stream stream)
    {
        return new BamRequestLine(ReadLineString(stream));
    }
    
    protected Dictionary<string, string> ReadHeaders(Stream stream)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        string line = ReadLineString(stream);
        while (!string.IsNullOrEmpty(line))
        {
            string[] split = line.DelimitSplit(":");
            if (split.Length == 2)
            {
                headers.Add(split[0].ToLowerInvariant(), split[1]);
            }

            line = ReadLineString(stream);
        }

        return headers;
    }
    
    protected string ReadLineString(Stream stream, Encoding encoding = null)
    {
        encoding = encoding ?? Encoding.ASCII;
        byte[] line = ReadLine(stream);
        return encoding.GetString(line).Trim();
    }

    protected byte[] ReadLine(Stream stream)
    {
        byte[] buffer = new byte[BufferSize];
        int totalBytesRead = 0;
        int bytesRead = 0;
        do
        {
            bytesRead = stream.Read(buffer, totalBytesRead, 1);
            byte[] currentBuffer = buffer.Trim();
            if (currentBuffer.TailEquals("\n"))
            {
                break;
            }

            totalBytesRead += bytesRead;
        } while (bytesRead > 0);

        return buffer.Trim();
    }

    protected string ReadContentString(Stream stream, Encoding encoding = null)
    {
        encoding = encoding ?? Encoding.ASCII;
        byte[] content = ReadContent(stream);
        return encoding.GetString(content).Trim();
    }
    
    protected byte[] ReadContent(Stream stream)
    {
        byte[] buffer = new byte[BufferSize];
        int totalBytesRead = 0;
        int bytesRead = 0;
        do
        {
            /*if (totalBytesRead == BufferSize)
            {
                byte[] newBuffer = new byte[buffer.Length + BufferSize];
                buffer.CopyTo(newBuffer, 0);
                buffer = newBuffer;
                totalBytesRead = 0;
            }*/

            bytesRead = stream.Read(buffer, totalBytesRead, 1);
            totalBytesRead += bytesRead;
        } while (bytesRead > 0);

        return buffer.Trim();
    }
    
    private BamRequestLine GetBamRequestLine(HttpListenerRequest request)
    {
        return new BamRequestLine($@"{request.HttpMethod} {request.Url.PathAndQuery} HTTP/{request.ProtocolVersion}");
    }
    
    private Dictionary<string, string> ReadHeaders(HttpListenerRequest request)
    {
        Dictionary<string, string>  result = new Dictionary<string, string>();
        foreach (string key in request.Headers.AllKeys)
        {
            result.Add(key, request.Headers[key]);
        }

        return result;
    }
}