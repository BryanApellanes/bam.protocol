using System.Net.Sockets;
using System.Text;
using Amazon.Runtime.Internal;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
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
        this.HttpClient = new HttpClient();
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
            { typeof(UdpClientRequest), SendUdpAsync},
        };
    }

    public HttpClient HttpClient // content
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

    public IClientSessionState SessionState { get; set; }
    private ClientRequestSecurityProvider SecurityProvider { get; } = new ClientRequestSecurityProvider();

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
        HttpClient client = this.HttpClient ?? new HttpClient();
        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        return new BamClientResponse(responseMessage);
    }
    
    public async Task<IBamClientResponse> ReceiveTcpResponseAsync(IBamClientRequest bamRequest)
    {
        TcpClientRequest request = (TcpClientRequest)bamRequest;
        TcpClient client = new TcpClient(BaseAddress.HostName, BaseAddress.Port);
        NetworkStream stream = client.GetStream();
        byte[] data = CreateRequestData(request);
        await stream.WriteAsync(data, 0, data.Length);
        client.Client.Shutdown(SocketShutdown.Send);

        using MemoryStream responseBuffer = new MemoryStream();
        byte[] readBuffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length)) > 0)
        {
            responseBuffer.Write(readBuffer, 0, bytesRead);
        }

        client.Close();
        string response = Encoding.UTF8.GetString(responseBuffer.ToArray());
        return new BamClientResponse(response);
    }

    public async Task<IBamClientResponse> SendUdpAsync(IBamClientRequest bamRequest)
    {
        UdpClientRequest request = (UdpClientRequest)bamRequest;
        byte[] data = CreateRequestData(request);
        using UdpClient client = new UdpClient();
        await client.SendAsync(data, data.Length, UdpBaseAddress.HostName, UdpBaseAddress.Port);
        return new BamClientResponse("UDP_SENT");
    }

    private byte[] CreateRequestData(BamClientRequest request)
    {
        StringBuilder data = new StringBuilder();
        data.AppendLine(request.GetRequestLine().ToString());

        string body = null;
        if (request.Content != null)
        {
            IObjectEncoding encoding = ObjectEncoderDecoder.Encode(request.Content);
            body = encoding.Encoding.GetString(encoding.Value);
        }

        if (SessionState != null && body != null)
        {
            body = SecurityProvider.PrepareRequest(request, body, SessionState);
        }

        if (request.Headers?.Count > 0)
        {
            foreach (KeyValuePair<string, string> keyValuePair in request.Headers)
            {
                data.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
            }
        }

        if (body != null)
        {
            data.AppendLine();
            data.AppendLine(body);
        }

        return Encoding.UTF8.GetBytes(data.ToString());
    }
    
    
    
    private HttpRequestMessage CreateHttpRequestMessage(IBamClientRequest request)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethods[request.HttpMethod], request.GetUrl(this));
        requestMessage.Headers.Add(Headers.ProcessMode, ProcessMode.Current.Mode.ToString());
        requestMessage.Headers.Add(Headers.ProcessLocalIdentifier, ProcessDescriptorData.LocalIdentifier);
        requestMessage.Headers.Add(Headers.ProcessDescriptor, ProcessDescriptorData.Current.ToString());

        string body = null;
        if (request.Content != null)
        {
            body = request.Content is string s ? s : System.Text.Json.JsonSerializer.Serialize(request.Content);
        }

        if (SessionState != null && body != null)
        {
            SecurityProvider.PrepareHttpRequest(requestMessage, body, SessionState);
        }
        else if (body != null)
        {
            requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        return requestMessage;
    }
}