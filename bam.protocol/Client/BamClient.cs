using System.Net.Sockets;
using System.Text;
using Amazon.Runtime.Internal;
using bam.protocol;
using Bam.Web;
using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol.Client;

public class BamClient : IBamClient
{
    public static readonly HostBinding DefaultTcpBaseAddress = new BamHostBinding("localhost", BamServer.DefaultTcpPort);

    public static readonly HostBinding DefaultHttpBaseAddress = new HostBinding("localhost", BamServer.DefaultHttpPort);

    public static readonly HostBinding DefaultUdpBaseAddress = new BamHostBinding("localhost", BamServer.DefaultUdpPort);
    
    public BamClient(IObjectEncoderDecoder objectEncoderDecoder) : this(objectEncoderDecoder, DefaultHttpBaseAddress, DefaultTcpBaseAddress, DefaultUdpBaseAddress)
    {
    }

    public BamClient(IObjectEncoderDecoder objectEncoderDecoder, HostBinding httpBaseAddress) : this(objectEncoderDecoder, httpBaseAddress, DefaultTcpBaseAddress, DefaultUdpBaseAddress)
    {
    }

    public BamClient(IObjectEncoderDecoder objectEncoderDecoder, HostBinding httpBaseAddress, HostBinding tcpBaseAddress) : this(objectEncoderDecoder, httpBaseAddress, tcpBaseAddress, DefaultUdpBaseAddress)
    {
    }
    
    public BamClient(IObjectEncoderDecoder objectEncoderDecoder,HostBinding httpBaseAddress, HostBinding tcpBaseAddress, HostBinding udpBaseAddress)
    {
        this.ObjectEncoderDecoder = objectEncoderDecoder;
        this.HttpBaseAddress = httpBaseAddress;
        this.BaseAddress = tcpBaseAddress;
        this.UdpBaseAddress = udpBaseAddress;
        this.Initialize();
    }

    private void Initialize()
    {
        HttpMethods = new Dictionary<HttpMethods, HttpMethod>()
        {
            { Protocol.HttpMethods.GET, HttpMethod.Get },
            { Protocol.HttpMethods.POST, HttpMethod.Post },
            { Protocol.HttpMethods.PUT, HttpMethod.Put },
            { Protocol.HttpMethods.PATCH, HttpMethod.Patch },
            { Protocol.HttpMethods.DELETE, HttpMethod.Delete },
            { Protocol.HttpMethods.HEAD, HttpMethod.Head },
            { Protocol.HttpMethods.OPTIONS, HttpMethod.Options },
            { Protocol.HttpMethods.TRACE, HttpMethod.Trace },
        };
        RequestBuilders = new Dictionary<BamClientProtocols, Func<IBamClientRequestBuilder>>()
        {
            { BamClientProtocols.Http, () => new HttpBamClientRequestBuilder() },
            { BamClientProtocols.Tcp, () => new TcpBamClientRequestBuilder() },
            { BamClientProtocols.Udp, () => new UdpBamClientRequestBuilder() }
        };
        ResponseHandlers = new Dictionary<Type, Func<IBamClientRequest, Task<IBamClientResponse>>>()
        {
            { typeof(HttpClientRequest), ReceiveHttpResponseAsync},
            { typeof(TcpClientRequest), ReceiveTcpResponseAsync},
            
        };
    }

    private HttpClient HttpClient // content
    {
        get; 
        set;
    }

    private TcpClient TcpClient // rpc
    {
        get;
        set;
    }
    
    private Socket UdpSocket // data and event broadcast
    {
        get;
        set;
    }
    
    private Dictionary<HttpMethods, HttpMethod> HttpMethods
    {
        get;
        set;
    }

    private Dictionary<BamClientProtocols, Func<IBamClientRequestBuilder>> RequestBuilders
    {
        get;
        set;
    }

    private Dictionary<Type, Func<IBamClientRequest, Task<IBamClientResponse>>> ResponseHandlers
    {
        get;
        set;
    }
    
    private IObjectEncoderDecoder ObjectEncoderDecoder { get; }
    
    public HostBinding HttpBaseAddress { get; set; }
    public HostBinding BaseAddress { get; set; }
    public HostBinding UdpBaseAddress { get; set; }

    public IBamClientRequest CreateHttpRequest(string path)
    {
        return CreateRequestBuilder(BamClientProtocols.Http)
            .BaseAddress(HttpBaseAddress)
            .Path(path)
            .Build();
    }

    public IBamClientRequest CreateTcpRequest(string path)
    {
        return CreateRequestBuilder(BamClientProtocols.Tcp)
            .BaseAddress(BaseAddress)
            .Path(path)
            .Build();
    }

    public IBamClientRequest CreateUdpRequest(string path, object content)
    {
        return CreateRequestBuilder(BamClientProtocols.Udp)
            .BaseAddress(UdpBaseAddress)
            .Path(path)
            .Content(content)
            .Build();
    }
    
    public IBamClientRequestBuilder CreateRequestBuilder(BamClientProtocols protocol)
    {
        return RequestBuilders[protocol]();
    }

    public async Task<IBamClientResponse> ReceiveResponseAsync(IBamClientRequest request)
    {
        // TODO: determine if the request is an http request, tcp request or udp request
        // See https://github.com/BryanApellanes/Bam.Net/blob/e6f1132b6eedb4fd1372011ce945fdaf775cf588/legacy/Bam.Net.Server/Streaming/StreamingClient.cs#L12
        // and https://github.com/BryanApellanes/Bam.Net/blob/e6f1132b6eedb4fd1372011ce945fdaf775cf588/legacy/Bam.Net.Server/Streaming/StreamingServer.cs#L19
        Type requestType = request.GetType();
        if (ResponseHandlers.ContainsKey(requestType))
        {
            return await ResponseHandlers[requestType](request);
        }

        throw new UnsupportedRequestTypeException(requestType);
    }

    public async Task<IBamClientResponse> ReceiveHttpResponseAsync(IBamClientRequest request)
    {
        HttpRequestMessage requestMessage = CreateHttpRequestMessage(request);
        HttpClient client = new HttpClient();
        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        return new BamClientResponse(responseMessage);
    }
    
    public async Task<IBamClientResponse> ReceiveTcpResponseAsync(IBamClientRequest bamRequest)
    {
        TcpClientRequest request = (TcpClientRequest)bamRequest;
        TcpClient client = new TcpClient(BaseAddress.HostName, BaseAddress.Port);
        NetworkStream stream = client.GetStream();
        byte[] data = CreateRequestData(request);
        stream.Write(data, 0, data.Length);
        byte[] readBuffer = new byte[client.Available];
        string response = string.Empty;
        while (stream.Read(readBuffer, 0,  readBuffer.Length) != 0)
        {
            response = Encoding.UTF8.GetString(readBuffer);
        }

        return new BamClientResponse(response);
    }

    private byte[] CreateRequestData(TcpClientRequest request)
    {
        StringBuilder data = new StringBuilder();
        data.AppendLine(request.GetRequestLine().ToString());
        if (request.Headers?.Count > 0)
        {
            foreach (KeyValuePair<string, string> keyValuePair in request.Headers)
            {
                data.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
            }
        }

        if (request.Content != null)
        {
            IObjectEncoding encoding = ObjectEncoderDecoder.Encode(request.Content);
            string content = encoding.Encoding.GetString(encoding.Value);
            data.AppendLine();
            data.AppendLine(content);
        }

        return Encoding.UTF8.GetBytes(data.ToString());
    }
    
    
    
    private HttpRequestMessage CreateHttpRequestMessage(IBamClientRequest request)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethods[request.HttpMethod], request.GetUrl(this));
        requestMessage.Headers.Add(Headers.ProcessMode, ProcessMode.Current.Mode.ToString());
        requestMessage.Headers.Add(Headers.ProcessLocalIdentifier, ProcessDescriptor.LocalIdentifier);
        requestMessage.Headers.Add(Headers.ProcessDescriptor, ProcessDescriptor.Current.ToString());
        return requestMessage;
    }
}