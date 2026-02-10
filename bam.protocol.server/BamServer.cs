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
    public class BamServer: Loggable, IAsyncManagedServer, IConfigurable, IDisposable
    {
        private bool _stopRequested;
        
        public const int DefaultHttpPort = 8080;
        public const int DefaultTcpPort = 8413;
        public const int DefaultUdpPort = 8414;
        
        public BamServer() : this(new BamServerOptions())
        {
        }

        public BamServer(BamServerOptions options)
        {
            Logger = options.Logger;
            TcpPort = options.TcpPort;
            UdpPort = options.UdpPort;
            ServerName = options.ServerName;
            HttpHostBinding = options.HttpHostBinding;
            Options = options;
            Started += (o, a) => Subscribe(Logger);
            Options.SubscribeEventHandlers(this);
        }

        protected Dictionary<RequestType, Func<BamServerInitializationContext>> RequestTypeInitializationInstantiators =
            new Dictionary<RequestType, Func<BamServerInitializationContext>>()
            {
                { RequestType.Http, () => new HttpBamServerInitializationContext() },
                { RequestType.Tcp, () => new TcpBamServerInitializationContext() },
                { RequestType.Udp, () => new UdpBamServerInitializationContext() }
            };
        
        Encoding _encoding;
        readonly object _encodingLock = new object();

        public Encoding Encoding
        {
            get
            {
                return _encodingLock.DoubleCheckLock(ref _encoding, () => Encoding.UTF8);
            }
            set => _encoding = value;
        }
        
        protected BamServerOptions Options { get; private set; }

        protected ICommunicationHandler CommunicationHandler => Options.GetCommunicationHandler();

        protected ITcpIPAddressProvider TcpIpAddressProvider => CommunicationHandler.TcpIpAddressProvider;
        protected IUdpIPAddressProvider UdpIpAddressProvider => CommunicationHandler.UdpIpAddressProvider;
        protected IBamServerContextProvider ServerContextProvider => CommunicationHandler.ServerContextProvider;
        protected IBamResponseProvider ResponseProvider => CommunicationHandler.ResponseProvider;
        protected IActorResolver ActorResolver => CommunicationHandler.ActorResolver;
        protected IServerSessionManager ServerSessionManager => CommunicationHandler.ServerSessionManager;
        protected IAuthorizationCalculator AuthorizationCalculator => CommunicationHandler.AuthorizationCalculator;
        protected IBamRequestProcessor RequestProcessor => CommunicationHandler.RequestProcessor;
        public IObjectEncoderDecoder ObjectEncoderDecoder => CommunicationHandler.ObjectEncoderDecoder;  
        /// <summary>
        /// Gets or sets the name of this server to aid in identifying the process in logs.
        /// </summary>
        public string ServerName { get; set; }

        public HostBinding HttpHostBinding { get; }
        
        public int TcpPort
        {
            get;
            set;
        }

        public IPAddress TcpIPAddress => TcpIpAddressProvider.GetTcpIPAddress();

        public IPAddress UdpIPAddress => UdpIpAddressProvider.GetUdpIPAddress();

        public int UdpPort
        {
            get;
            set;
        }

        public string[] RequiredProperties => [nameof(TcpPort)];

        public string LastExceptionMessage
        {
            get;
            set;
        }

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Started")]
        public event EventHandler<BamServerEventArgs> Starting;

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Started")]
        public event EventHandler<BamServerEventArgs> Started;

        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Stopping")]
        public event EventHandler<BamServerEventArgs> Stopping;        
        
		[Verbosity(VerbosityLevel.Information, SenderMessageFormat = "BamHttpServer={Name};Port={Port};Stopped")]
        public event EventHandler<BamServerEventArgs> Stopped;

        [Verbosity(LogEventType.Error, SenderMessageFormat = "LastMessage: {LastExceptionMessage}")]
        public event EventHandler StartExceptionThrown;

        [Verbosity(LogEventType.Error, SenderMessageFormat = "LastMessage: {LastExceptionMessage}")]
        public event EventHandler RequestExceptionThrown;
        
        [Verbosity(LogEventType.Error, SenderMessageFormat = "LastMessage: {LastExceptionMessage}")]
        public event EventHandler ServerStartException;

        public event EventHandler HttpRequestReceived;
        
        [Verbosity(LogEventType.Information,
            SenderMessageFormat =
                "Client Connected: LocalEndpoint={LocalEndpoint}, RemoteEndpoint={RemoteEndpoint}")]
        public event EventHandler<BamServerEventArgs> TcpClientConnected;

        public event EventHandler<BamServerEventArgs> UdpDataReceived; 

        public event EventHandler<BamServerEventArgs> CreateContextStarted;
        public event EventHandler<BamServerEventArgs> CreateContextComplete; 
        
        public event EventHandler<BamServerEventArgs> InitializeContextStarted;
        
        public event EventHandler<BamServerEventArgs> InitializeContextComplete;

        public BamServerInfo GetInfo()
        {
            BamServerInfo info = new BamServerInfo();
            info.CopyProperties(this);
            info.TcpIPAddress = TcpIPAddress.Equals(IPAddress.Any) ? "Any" : TcpIPAddress.ToString();
            info.UdpIPAddress = UdpIPAddress.Equals(IPAddress.Any) ? "Any" : UdpIPAddress.ToString();
            return info;
        }
        
        public void Dispose()
        {
            TryStop();
        }

        public void Stop()
        {
            FireEvent(Stopping);
            _stopRequested = true;
            UdpClient.Close();
            TcpListener.Stop();
            HttpServer.Stop();
            FireEvent(Stopped);
        }

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

        public Task StopAsync()
        {
            return Task.Run(Stop);
        }

        public Task TryStopAsync()
        {
            return Task.Run(TryStop);
        }
        
        public Task StartAsync()
        {
            return Task.Run(Start);
        }
        
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

        protected HttpServer HttpServer { get; set; }
        protected TcpListener TcpListener { get; set; }
        protected UdpClient UdpClient { get; set; }
        protected ILogger Logger { get; set; }

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
                        Logger.Info("Tcp Client Connected (CorrelationId={0}): LocalEndpoint={1}, RemoteEndpoint={2}", requestId, client.Client.LocalEndPoint.ToString(), client.Client.RemoteEndPoint.ToString());
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
            int retryCount = 3;
            while (client.Connected)
            {
                try
                {
                    if (retryCount > 0)
                    {
                        FireEvent(CreateContextStarted, new BamServerEventArgs(client));
                        
                        IBamServerContext serverContext = ServerContextProvider.CreateServerContext(client, requestId);
                        serverContext.RequestType = RequestType.Tcp;
                        
                        BamServerEventArgs args = new BamServerEventArgs(client, serverContext) { Server = this };
                        InitializeServerContext(serverContext, args);
                        
                        FireEvent(CreateContextComplete, new BamServerEventArgs(client, serverContext){Server = this});
                    }
                }
                catch (Exception ex)
                {
                    FireEvent(RequestExceptionThrown, new ErrorEventArgs(ex));
                    Logger.AddEntry("Error processing request (retryCount={0}): {1}", ex, retryCount.ToString(), ex.Message);
                    Thread.Sleep(30);
                    --retryCount;
                }
            }            
        }

        public void Configure(IConfigurer configurer)
        {
            configurer.Configure(this);
            this.CheckRequiredProperties();
        }

        public void Configure(object configuration)
        {
            this.CopyProperties(configuration);
            this.CheckRequiredProperties();
        }

        private BamServerInitializationContext InitializeServerContext(IBamServerContext serverContext, BamServerEventArgs args)
        {
            IBamServerContextInitializer initializer = Options.GetServerContextInitializer();
            BamServerInitializationContext initialization =
                RequestTypeInitializationInstantiators[serverContext.RequestType]();
            initialization.Server = this;
            initialization.ServerContext = serverContext;
            initialization.EventArgs = args;
            return initializer.InitializeServerContext(initialization);
        }
    }
}