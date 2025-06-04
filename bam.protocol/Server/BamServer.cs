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
    public class BamServer: Loggable, IManagedServer, IConfigurable, IDisposable
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
        
        Encoding _encoding;
        readonly object _encodingLock = new object();

        protected Encoding Encoding
        {
            get
            {
                return _encodingLock.DoubleCheckLock(ref _encoding, () => Encoding.UTF8);
            }
            set => _encoding = value;
        }
        
        protected BamServerOptions Options { get; private set; }

        protected IBamCommunicationHandler CommunicationHandler => Options.GetCommunicationHandler();

        protected ITcpIPAddressProvider TcpIpAddressProvider => CommunicationHandler.TcpIpAddressProvider;
        protected IUdpIPAddressProvider UdpIpAddressProvider => CommunicationHandler.UdpIpAddressProvider;
        protected IBamContextProvider ContextProvider => CommunicationHandler.ContextProvider;
        protected IBamResponseProvider ResponseProvider => CommunicationHandler.ResponseProvider;
        protected IBamActorResolver ActorResolver => CommunicationHandler.ActorResolver;
        protected IBamSessionStateProvider SessionStateProvider => CommunicationHandler.SessionStateProvider;
        protected IBamAuthorizationCalculator AuthorizationCalculator => CommunicationHandler.AuthorizationCalculator;
        protected IBamRequestProcessor RequestProcessor => CommunicationHandler.RequestProcessor;
        
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
        public event EventHandler InitializationException;

        public event EventHandler HttpRequestReceived;
        
        [Verbosity(LogEventType.Information,
            SenderMessageFormat =
                "Client Connected: LocalEndpoint={LocalEndpoint}, RemoteEndpoint={RemoteEndpoint}")]
        public event EventHandler<BamServerEventArgs> TcpClientConnected;

        public event EventHandler<BamServerEventArgs> UdpDataReceived; 

        public event EventHandler<BamServerEventArgs> CreateContextStarted;
        public event EventHandler<BamServerEventArgs> CreateContextComplete; 
        public event EventHandler<BamServerEventArgs> ResolveActorStarted;
        public event EventHandler<BamServerEventArgs> ResolveActorComplete;
        public event EventHandler<BamServerEventArgs> AuthorizeRequestStarted;
        public event EventHandler<BamServerEventArgs> AuthorizeRequestComplete;
        public event EventHandler<BamServerEventArgs> ResolveSessionStateStarted;
        public event EventHandler<BamServerEventArgs> ResolveSessionStateComplete;

        public event EventHandler<BamServerEventArgs> CreateResponseStarted;
        public event EventHandler<BamServerEventArgs> CreateResponseComplete;

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
            Stop();
        }

        public void Stop()
        {
            FireEvent(Stopping);
            _stopRequested = true;
            TcpListener.Stop();
            FireEvent(Stopped);
        }

        public void TryStop()
        {
            throw new NotImplementedException();
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
                FireEvent(InitializationException, new ErrorEventArgs(ex));
            }
        }

        protected HttpServer HttpServer { get; set; }
        protected TcpListener TcpListener { get; set; }
        protected UdpClient UdpClient { get; set; }
        protected ILogger Logger { get; set; }

        protected void HandleHttpRequests(HttpListenerContext listenerContext)
        {
            throw new NotImplementedException("this is not complete");
            System.Console.WriteLine(listenerContext.Request.Url);
            listenerContext.Response.Close();
            
            
            FireEvent(HttpRequestReceived, new BamServerEventArgs(){Server = this});
            FireEvent(CreateContextStarted, new BamServerEventArgs(){Server = this});
            string requestId = Cuid.Generate();
            IBamServerContext serverContext = ContextProvider.CreateContext(listenerContext.Request, requestId);
            serverContext.RequestType = RequestType.Http;
            BamServerEventArgs args = new BamServerEventArgs(listenerContext, serverContext);
            ResolveContextPipeline(serverContext, args);
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
                        
                    IBamServerContext serverContext = ContextProvider.CreateContext(stream, requestId);
                    serverContext.RequestType = RequestType.Udp;
                    
                    BamServerEventArgs args = new BamServerEventArgs(serverContext);
                    ResolveContextPipeline(serverContext, args);
                        
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
                        
                        IBamServerContext serverContext = ContextProvider.CreateContext(client, requestId);
                        serverContext.RequestType = RequestType.Tcp;
                        
                        BamServerEventArgs args = new BamServerEventArgs(client, serverContext) { Server = this };
                        ResolveContextPipeline(serverContext, args);
                        
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

        private void ResolveContextPipeline(IBamServerContext serverContext, BamServerEventArgs args)
        {
            // TODO: move this to
            // Bam.Protocol.Server.BamContextResolutionPipeline
            FireEvent(ResolveActorStarted, args);
            serverContext.Actor = ActorResolver.ResolveActor(serverContext.BamRequest);
            FireEvent(ResolveActorComplete, args);
            FireEvent(AuthorizeRequestStarted, args);
            serverContext.AuthorizationCalculation = AuthorizationCalculator.CalculateAuthorization(serverContext);
            FireEvent(AuthorizeRequestComplete, args);
            FireEvent(ResolveSessionStateStarted, args);
            serverContext.SessionState = SessionStateProvider.GetSession(serverContext);
            FireEvent(ResolveSessionStateComplete, args);
            FireEvent(CreateResponseStarted, args);
            serverContext.BamResponse = ResponseProvider.CreateResponse(serverContext);
            FireEvent(CreateResponseComplete, args);
        }
    }
}