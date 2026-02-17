/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Net.Sockets;
using System.Text;
using System.Net;
using Bam.Logging;
using Bam.Configuration;
using Bam.Server;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// The main BAM protocol server that listens for HTTP, TCP, and UDP requests and processes them through an initialization pipeline.
    /// </summary>
    public class BamServer: Loggable, IAsyncManagedServer, IConfigurable, IDisposable
    {
        private bool _stopRequested;
        
        /// <summary>
        /// The default HTTP port.
        /// </summary>
        public const int DefaultHttpPort = 8080;

        /// <summary>
        /// The default TCP port.
        /// </summary>
        public const int DefaultTcpPort = 8413;

        /// <summary>
        /// The default UDP port.
        /// </summary>
        public const int DefaultUdpPort = 8414;

        /// <summary>
        /// Initializes a new instance of the <see cref="BamServer"/> class with default options.
        /// </summary>
        public BamServer() : this(new BamServerOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BamServer"/> class with the specified options.
        /// </summary>
        /// <param name="options">The server options.</param>
        public BamServer(BamServerOptions options)
        {
            Logger = options.Logger!;
            TcpPort = options.TcpPort;
            UdpPort = options.UdpPort;
            ServerName = options.ServerName;
            HttpHostBinding = options.HttpHostBinding;
            Options = options;
            _pipeline = new BamRequestPipeline(options);
            Started += (o, a) => Subscribe(Logger);
            Options.SubscribeEventHandlers(this);
        }

        private readonly BamRequestPipeline _pipeline;
        
        Encoding _encoding = null!;
        readonly object _encodingLock = new object();

        /// <summary>
        /// Gets or sets the text encoding used for request and response serialization. Defaults to UTF-8.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return _encodingLock.DoubleCheckLock(ref _encoding, () => Encoding.UTF8);
            }
            set => _encoding = value;
        }
        
        protected BamServerOptions Options { get; private set; }

        protected ICommunicationHandler CommunicationHandler => Options.GetCommunicationHandler()!;

        protected ITcpIPAddressProvider TcpIpAddressProvider => CommunicationHandler.TcpIpAddressProvider!;
        protected IUdpIPAddressProvider UdpIpAddressProvider => CommunicationHandler.UdpIpAddressProvider!;
        protected IBamServerContextProvider ServerContextProvider => CommunicationHandler.ServerContextProvider!;
        protected IBamResponseProvider ResponseProvider => CommunicationHandler.ResponseProvider!;
        protected IActorResolver ActorResolver => CommunicationHandler.ActorResolver!;
        protected IServerSessionManager ServerSessionManager => CommunicationHandler.ServerSessionManager!;
        protected IAuthorizationCalculator AuthorizationCalculator => CommunicationHandler.AuthorizationCalculator!;
        protected IBamRequestProcessor RequestProcessor => CommunicationHandler.RequestProcessor!;
        /// <summary>
        /// Gets the object encoder/decoder used for serializing and deserializing request/response content.
        /// </summary>
        public IObjectEncoderDecoder ObjectEncoderDecoder => CommunicationHandler.ObjectEncoderDecoder!;

        /// <summary>
        /// Gets or sets the name of this server to aid in identifying the process in logs.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets the HTTP host binding for this server.
        /// </summary>
        public HostBinding HttpHostBinding { get; }

        /// <summary>
        /// Gets or sets the TCP port number.
        /// </summary>
        public int TcpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the TCP IP address from the TCP IP address provider.
        /// </summary>
        public IPAddress TcpIPAddress => TcpIpAddressProvider.GetTcpIPAddress();

        /// <summary>
        /// Gets the UDP IP address from the UDP IP address provider.
        /// </summary>
        public IPAddress UdpIPAddress => UdpIpAddressProvider.GetUdpIPAddress();

        /// <summary>
        /// Gets or sets the UDP port number.
        /// </summary>
        public int UdpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the required property names for configuration validation.
        /// </summary>
        public string[] RequiredProperties => [nameof(TcpPort)];

        /// <summary>
        /// Gets or sets the message from the last exception that occurred.
        /// </summary>
        public string LastExceptionMessage
        {
            get;
            set;
        } = null!;

        /// <summary>
        /// Occurs when the server is about to start.
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Started")]
        public event EventHandler<BamServerEventArgs> Starting = null!;

        /// <summary>
        /// Occurs when the server has started successfully.
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Started")]
        public event EventHandler<BamServerEventArgs> Started;

        /// <summary>
        /// Occurs when the server is about to stop.
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Stopping")]
        public event EventHandler<BamServerEventArgs> Stopping = null!;

        /// <summary>
        /// Occurs when the server has stopped.
        /// </summary>
		[Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Stopped")]
        public event EventHandler<BamServerEventArgs> Stopped = null!;

        /// <summary>
        /// Occurs when an exception is thrown during server listener startup.
        /// </summary>
        [Verbosity(LogEventType.Error, SenderMessageFormat = "LastMessage: {LastExceptionMessage}")]
        public event EventHandler StartExceptionThrown = null!;

        /// <summary>
        /// Occurs when an exception is thrown while processing a request.
        /// </summary>
        [Verbosity(LogEventType.Error, SenderMessageFormat = "LastMessage: {LastExceptionMessage}")]
        public event EventHandler RequestExceptionThrown = null!;

        /// <summary>
        /// Occurs when an exception is thrown during the overall server start operation.
        /// </summary>
        [Verbosity(LogEventType.Error, SenderMessageFormat = "LastMessage: {LastExceptionMessage}")]
        public event EventHandler ServerStartException = null!;

        /// <summary>
        /// Occurs when an HTTP request is received.
        /// </summary>
        public event EventHandler HttpRequestReceived = null!;
        
        /// <summary>
        /// Occurs when a TCP client connects to the server.
        /// </summary>
        [Verbosity(LogEventType.Information,
            SenderMessageFormat =
                "Client Connected: LocalEndpoint={LocalEndpoint}, RemoteEndpoint={RemoteEndpoint}")]
        public event EventHandler<BamServerEventArgs> TcpClientConnected = null!;

        /// <summary>
        /// Occurs when UDP data is received.
        /// </summary>
        public event EventHandler<BamServerEventArgs> UdpDataReceived = null!;

        /// <summary>
        /// Occurs when server context creation starts for a request.
        /// </summary>
        public event EventHandler<BamServerEventArgs> CreateContextStarted = null!;

        /// <summary>
        /// Occurs when server context creation completes for a request.
        /// </summary>
        public event EventHandler<BamServerEventArgs> CreateContextComplete = null!;

        /// <summary>
        /// Occurs when server context initialization starts for a request.
        /// </summary>
        public event EventHandler<BamServerEventArgs> InitializeContextStarted = null!;

        /// <summary>
        /// Occurs when server context initialization completes for a request.
        /// </summary>
        public event EventHandler<BamServerEventArgs> InitializeContextComplete = null!;

        /// <summary>
        /// Gets an informational snapshot of this server's configuration.
        /// </summary>
        /// <returns>A <see cref="BamServerInfo"/> containing the server's current configuration.</returns>
        public BamServerInfo GetInfo()
        {
            BamServerInfo info = new BamServerInfo();
            info.CopyProperties(this);
            info.TcpIPAddress = TcpIPAddress.Equals(IPAddress.Any) ? "Any" : TcpIPAddress.ToString();
            info.UdpIPAddress = UdpIPAddress.Equals(IPAddress.Any) ? "Any" : UdpIPAddress.ToString();
            return info;
        }
        
        /// <summary>
        /// Disposes of the server by attempting to stop it.
        /// </summary>
        public void Dispose()
        {
            TryStop();
        }

        /// <summary>
        /// Stops the server and all listeners (HTTP, TCP, UDP).
        /// </summary>
        public void Stop()
        {
            FireEvent(Stopping);
            _stopRequested = true;
            UdpClient?.Close();
            TcpListener?.Stop();
            HttpServer?.Stop();
            FireEvent(Stopped);
        }

        /// <summary>
        /// Attempts to stop the server, logging any exceptions.
        /// </summary>
        public void TryStop()
        {
            try
            {
                Stop();
            }
            catch (Exception e)
            {
                Log.Error("Exception stopping server ({0})", this.ServerName, e);
            }
        }

        /// <summary>
        /// Stops the server asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous stop operation.</returns>
        public Task StopAsync()
        {
            return Task.Run(Stop);
        }

        /// <summary>
        /// Attempts to stop the server asynchronously, swallowing any exceptions.
        /// </summary>
        /// <returns>A task representing the asynchronous try-stop operation.</returns>
        public Task TryStopAsync()
        {
            return Task.Run(TryStop);
        }
        
        /// <summary>
        /// Starts the server asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous start operation.</returns>
        public Task StartAsync()
        {
            return Task.Run(Start);
        }
        
        /// <summary>
        /// Starts the server, initializing HTTP, TCP, and UDP listeners.
        /// </summary>
        public void Start()
        {
            try
            {
                FireEvent(Starting);
                try
                {
                    HttpServer = new HttpServer(HandleHttpRequests);
                    HttpServer.Start(HttpHostBinding);
                    TcpListener = new TcpListener(TcpIPAddress, TcpPort);
                    TcpListener.Start();
                    Task.Run(HandleTcpRequests);
                    Task.Run(HandleUdpRequests);
                }
                catch (Exception ex)
                {
                    LastExceptionMessage = ex.Message;
                    FireEvent(StartExceptionThrown, new ErrorEventArgs(ex));
                }
                FireEvent(Started);
            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
                FireEvent(ServerStartException, new ErrorEventArgs(ex));
            }
        }

        protected HttpServer HttpServer { get; set; } = null!;
        protected TcpListener TcpListener { get; set; } = null!;
        protected UdpClient UdpClient { get; set; } = null!;
        protected ILogger Logger { get; set; } = null!;

        protected void HandleHttpRequests(HttpListenerContext listenerContext)
        {
            try
            {
                string requestId = Cuid.Generate();

                FireEvent(HttpRequestReceived, new BamServerEventArgs() { Server = this });
                
                FireEvent(CreateContextStarted, new BamServerEventArgs() { Server = this });
                IBamServerContext serverContext = ServerContextProvider.CreateServerContext(listenerContext, requestId);
                serverContext.RequestType = RequestType.Http;
                FireEvent(CreateContextComplete, new BamServerEventArgs() { Server = this });
                
                
                FireEvent(InitializeContextStarted, new BamServerEventArgs() { Server = this });
                BamServerEventArgs args = new BamServerEventArgs(listenerContext, serverContext);
                BamServerInitializationContext initialization = InitializeServerContext(serverContext, args);
                FireEvent(InitializeContextComplete, new BamServerEventArgs() { Server = this });
                
                IBamResponse response = initialization.ServerContext.BamResponse
                    ?? ResponseProvider.CreateResponse(initialization);
                response.Send();
                try { listenerContext.Response.Close(); } catch { }
            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
                FireEvent(RequestExceptionThrown, new ErrorEventArgs(ex));
                try
                {
                    listenerContext.Response.StatusCode = 500;
                    listenerContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(ex.Message));
                    listenerContext.Response.Close();
                }
                catch (Exception innerEx)
                {
                    LastExceptionMessage = innerEx.Message;
                }
            }
        }

        protected void HandleUdpRequests()
        {
            UdpClient = new UdpClient(UdpPort);
            IPEndPoint groupEndpoint = new IPEndPoint(UdpIPAddress, UdpPort);
            while (UdpClient != null && !_stopRequested)
            {
                byte[] data = UdpClient.Receive(ref groupEndpoint);
                Task.Run(() =>
                {
                    string requestId = Cuid.Generate();
                    MemoryStream stream = new MemoryStream(data);
                    FireEvent(UdpDataReceived, new BamServerEventArgs(){UdpData = data, Server = this});
                    FireEvent(CreateContextStarted, new BamServerEventArgs(){Server = this});
                        
                    IBamServerContext serverContext = ServerContextProvider.CreateServerContext(stream, requestId);
                    serverContext.RequestType = RequestType.Udp;
                    
                    BamServerEventArgs args = new BamServerEventArgs(serverContext);
                    InitializeServerContext(serverContext, args);
                        
                    FireEvent(CreateContextComplete, new BamServerEventArgs(serverContext));
                });
            }
        }
        
        protected virtual void HandleTcpRequests()
        {
            while (TcpListener != null && !_stopRequested)
            {
                TcpClient client = TcpListener.AcceptTcpClient();
                Task.Run(() =>
                {
                    string requestId = Cuid.Generate();
                    try
                    {
                        FireEvent(TcpClientConnected, new BamServerEventArgs(client));
                        Logger.Info("Tcp Client Connected (CorrelationId={0}): LocalEndpoint={1}, RemoteEndpoint={2}", requestId, client.Client.LocalEndPoint!.ToString()!, client.Client.RemoteEndPoint!.ToString()!);
                        HandleTcpRequest(client, requestId);
                    }
                    catch (Exception ex)
                    {
                        Logger.AddEntry("Error parsing tcp request (CorrelationId={0}): {1}", ex, requestId, ex.Message);
                    }
                });
            }
        }

        protected internal virtual void HandleTcpRequest(TcpClient client, string requestId)
        {
            try
            {
                FireEvent(CreateContextStarted, new BamServerEventArgs(client));

                IBamServerContext serverContext = ServerContextProvider.CreateServerContext(client, requestId);
                serverContext.RequestType = RequestType.Tcp;

                BamServerEventArgs args = new BamServerEventArgs(client, serverContext) { Server = this };
                BamServerInitializationContext initialization = InitializeServerContext(serverContext, args);

                FireEvent(CreateContextComplete, new BamServerEventArgs(client, serverContext){Server = this});

                IBamResponse response = initialization.ServerContext.BamResponse
                    ?? ResponseProvider.CreateResponse(initialization);
                response.Send();
            }
            catch (Exception ex)
            {
                FireEvent(RequestExceptionThrown, new ErrorEventArgs(ex));
                Logger.AddEntry("Error processing tcp request (CorrelationId={0}): {1}", ex, requestId, ex.Message);
            }
            finally
            {
                try { client.Close(); } catch { }
            }
        }

        /// <summary>
        /// Configures this server using the specified configurer.
        /// </summary>
        /// <param name="configurer">The configurer to apply.</param>
        public void Configure(IConfigurer configurer)
        {
            configurer.Configure(this);
            this.CheckRequiredProperties();
        }

        /// <summary>
        /// Configures this server by copying properties from the specified configuration object.
        /// </summary>
        /// <param name="configuration">The configuration object to copy properties from.</param>
        public void Configure(object configuration)
        {
            this.CopyProperties(configuration);
            this.CheckRequiredProperties();
        }

        private BamServerInitializationContext InitializeServerContext(IBamServerContext serverContext, BamServerEventArgs args)
        {
            return _pipeline.RunPipeline(serverContext, args, this);
        }
    }
}