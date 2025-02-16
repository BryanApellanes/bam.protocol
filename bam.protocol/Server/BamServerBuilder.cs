using System.Net;
using Bam.DependencyInjection;
using Bam.Services;

namespace Bam.Protocol.Server;

public class BamServerBuilder
{
    private readonly BamServerEventHandlers _serverEventHandlers;
    private readonly BamRequestEventHandlers _requestEventHandlers;
    private readonly BamServerOptions _options;
    private ServiceRegistry _applicationServiceRegistry;
    public BamServerBuilder()
    {
        _serverEventHandlers = new BamServerEventHandlers();
        _requestEventHandlers = new BamRequestEventHandlers();
        _options = new BamServerOptions();
        _applicationServiceRegistry = new ServiceRegistry();
    }

    internal int TcpPort()
    {
        return _options.TcpPort;
    }
    
    public BamServerBuilder TcpPort(int port)
    {
        _options.TcpPort = port;
        return this;
    }

    public BamServerBuilder UdpPort(int port)
    {
        _options.UdpPort = port;
        return this;
    }

    public BamServerBuilder TcpIPAddress(string ipAddress)
    {
        _options.TcpIPAddress = IPAddress.Parse(ipAddress);
        return this;
    }

    public BamServerBuilder UdpIPAddress(string ipAddress)
    {
        _options.UdpIPAddress = IPAddress.Parse(ipAddress);
        return this;
    }

    public BamServerBuilder ServerName(string name)
    {
        _options.ServerName = name;
        return this;
    }
    
    public BamServerBuilder ComponentRegistry(ServiceRegistry componentRegistry)
    {
        _applicationServiceRegistry = componentRegistry;
        return this;
    }

    public BamServerBuilder UseNameBasedPort(bool value = true)
    {
        _options.UseNameBasedPort = value;
        return this;
    }
    
    public BamServerBuilder ForApplicationComponentUse<I, T>(T instance)
    {
        if (instance == null)
        {
            return this;
        }
        ForApplicationComponent<I>().Use(instance);
        return this;
    }

    public BamServerBuilder OnStarting(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StartingHandlers.Add(new BamEventListener(nameof(BamServer.Starting), handler));
        return this;
    }
    
    public BamServerBuilder OnStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StartedHandlers.Add(new BamEventListener(nameof(BamServer.Started), handler));
        return this;
    }

    public BamServerBuilder OnTcpClientConnected(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.TcpClientConnectedHandlers.Add(new BamEventListener(nameof(BamServer.TcpClientConnected), handler));
        return this;
    }

    public BamServerBuilder OnUdpDataReceived(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.UdpDataReceivedHandlers.Add(new BamEventListener(nameof(BamServer.UdpDataReceived), handler));
        return this;
    }

    public BamServerBuilder OnStopping(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StoppingHandlers.Add(new BamEventListener(nameof(BamServer.Stopping), handler));
        return this;
    }
    
    public BamServerBuilder OnStopped(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StoppedHandlers.Add(new BamEventListener(nameof(BamServer.Stopped), handler));
        return this;
    }

    public BamServerBuilder OnCreateContextStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.CreateContextStartedHandlers.Add(new BamEventListener(nameof(BamServer.CreateContextStarted), handler));
        return this;
    }

    public BamServerBuilder OnCreateContextComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.CreateContextCompleteHandlers.Add(new BamEventListener(nameof(BamServer.CreateContextComplete), handler));
        return this;
    }
    
    public BamServerBuilder OnResolveUserStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveUserStartedHandlers.Add(new BamEventListener(nameof(BamServer.ResolveActorStarted), handler));
        return this;
    }
    
    public BamServerBuilder OnResolveUserComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveUserCompleteHandlers.Add(new BamEventListener(nameof(BamServer.ResolveActorComplete), handler));
        return this;
    }
    
    public BamServerBuilder OnAuthorizeRequestStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.AuthorizeRequestStartedHandlers.Add(new BamEventListener(nameof(BamServer.AuthorizeRequestStarted), handler));
        return this;
    }
    
    public BamServerBuilder OnAuthorizeRequestComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.AuthorizeRequestCompleteHandlers.Add(new BamEventListener(nameof(BamServer.AuthorizeRequestComplete), handler));
        return this;
    }
    
    public BamServerBuilder OnResolveSessionStateStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveSessionStateStartedHandlers.Add(new BamEventListener(nameof(BamServer.ResolveSessionStateStarted), handler));
        return this;
    }
    
    public BamServerBuilder OnResolveSessionStateComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveSessionStateCompleteHandlers.Add(new BamEventListener(nameof(BamServer.ResolveSessionStateComplete), handler));
        return this;
    }
    
    public BamServerBuilder OnCreateResponseStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.CreateResponseStartedHandlers.Add(new BamEventListener(nameof(BamServer.CreateResponseStarted), handler));
        return this;
    }
    
    public BamServerBuilder OnCreateResponseComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.CreateResponseCompleteHandlers.Add(new BamEventListener(nameof(BamServer.CreateResponseComplete), handler));
        return this;
    }
    
    public FluentServiceRegistryContext<I> ForApplicationComponent<I>()
    {
        return _applicationServiceRegistry.For<I>();
    }

    public BamServer Build()
    {
        _options.ComponentRegistry.CombineWith(_applicationServiceRegistry); 
        _options.ServerEventHandlers = _serverEventHandlers;
        _options.RequestEventHandlers = _requestEventHandlers;
        return new BamServer(_options);
    }
}