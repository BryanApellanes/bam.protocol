using System.Net;
using System.Net.Sockets;
using System.Text;
using Bam.Logging;

namespace Bam.Protocol.Server;

public class BamRequestReader : Loggable, IBamRequestReader
{
    public BamRequestReader(BamRequestReaderOptions options)
    {
        this.Options = options;
    }
    
    protected BamRequestReaderOptions Options { get; set; }

    public int BufferSize => Options.RequestBufferSize;

    public event EventHandler? ReadRequestStarted;

    public IBamRequest ReadRequest(HttpListenerRequest request)
    {
        FireEvent(ReadRequestStarted, this, new EventArgs());
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
        return ReadRequest(stream);
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
        Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string line = ReadLineString(stream);
        while (!string.IsNullOrEmpty(line))
        {
            string[] split = line.DelimitSplit(":", true);
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
        MemoryStream lineBuffer = new MemoryStream();
        byte[] singleByte = new byte[1];
        int bytesRead;
        do
        {
            bytesRead = stream.Read(singleByte, 0, 1);
            if (bytesRead > 0)
            {
                lineBuffer.WriteByte(singleByte[0]);
                if (singleByte[0] == (byte)'\n')
                {
                    break;
                }
            }
        } while (bytesRead > 0);

        return lineBuffer.ToArray();
    }

    protected string ReadContentString(Stream stream, Encoding encoding = null)
    {
        encoding = encoding ?? Encoding.ASCII;
        byte[] content = ReadContent(stream);
        return encoding.GetString(content).Trim();
    }

    protected byte[] ReadContent(Stream stream)
    {
        MemoryStream contentBuffer = new MemoryStream();
        byte[] chunk = new byte[BufferSize];
        int bytesRead;
        do
        {
            bytesRead = stream.Read(chunk, 0, chunk.Length);
            if (bytesRead > 0)
            {
                contentBuffer.Write(chunk, 0, bytesRead);
            }
        } while (bytesRead > 0);

        return contentBuffer.ToArray();
    }
    
    private BamRequestLine GetBamRequestLine(HttpListenerRequest request)
    {
        return new BamRequestLine($@"{request.HttpMethod} {request.Url} HTTP/{request.ProtocolVersion}");
    }
    
    private Dictionary<string, string> ReadHeaders(HttpListenerRequest request)
    {
        Dictionary<string, string>  result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string key in request.Headers.AllKeys)
        {
            result.Add(key, request.Headers[key]);
        }

        return result;
    }
}