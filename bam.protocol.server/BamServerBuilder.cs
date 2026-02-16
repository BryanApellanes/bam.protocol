using System.Net;
using Bam.DependencyInjection;
using Bam.Services;

namespace Bam.Protocol.Server;

/// <summary>
/// Provides a fluent API for constructing and configuring a <see cref="BamServer"/> instance.
/// </summary>
public class BamServerBuilder
{
    private readonly BamServerEventHandlers _serverEventHandlers;
    private readonly BamRequestEventHandlers _requestEventHandlers;
    private readonly BamServerOptions _options;
    private ServiceRegistry _applicationServiceRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerBuilder"/> class with default settings.
    /// </summary>
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
    
    /// <summary>
    /// Sets the TCP port for the server.
    /// </summary>
    /// <param name="port">The TCP port number.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder TcpPort(int port)
    {
        _options.TcpPort = port;
        return this;
    }

    /// <summary>
    /// Sets the UDP port for the server.
    /// </summary>
    /// <param name="port">The UDP port number.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder UdpPort(int port)
    {
        _options.UdpPort = port;
        return this;
    }

    /// <summary>
    /// Sets the TCP IP address for the server to bind to.
    /// </summary>
    /// <param name="ipAddress">The IP address string to parse and bind to.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder TcpIPAddress(string ipAddress)
    {
        _options.TcpIPAddress = IPAddress.Parse(ipAddress);
        return this;
    }

    /// <summary>
    /// Sets the UDP IP address for the server to bind to.
    /// </summary>
    /// <param name="ipAddress">The IP address string to parse and bind to.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder UdpIPAddress(string ipAddress)
    {
        _options.UdpIPAddress = IPAddress.Parse(ipAddress);
        return this;
    }

    /// <summary>
    /// Sets the server name for identification in logs and configuration.
    /// </summary>
    /// <param name="name">The server name.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder ServerName(string name)
    {
        _options.ServerName = name;
        return this;
    }
    
    /// <summary>
    /// Sets the application-level service registry for dependency injection.
    /// </summary>
    /// <param name="componentRegistry">The service registry to use.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder ComponentRegistry(ServiceRegistry componentRegistry)
    {
        _applicationServiceRegistry = componentRegistry;
        return this;
    }

    /// <summary>
    /// Enables or disables deriving TCP and UDP ports deterministically from the server name.
    /// </summary>
    /// <param name="value">True to enable name-based port derivation; false to disable.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder UseNameBasedPort(bool value = true)
    {
        _options.UseNameBasedPort = value;
        return this;
    }
    
    /// <summary>
    /// Registers a specific instance for a service interface in the application component registry.
    /// </summary>
    /// <typeparam name="I">The service interface type.</typeparam>
    /// <typeparam name="T">The implementation type.</typeparam>
    /// <param name="instance">The instance to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder ForApplicationComponentUse<I, T>(T instance)
    {
        if (instance == null)
        {
            return this;
        }
        ForApplicationComponent<I>().Use(instance);
        return this;
    }

    /// <summary>
    /// Registers an event handler for the server Starting event.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnStarting(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StartingHandlers.Add(new BamEventListener(nameof(BamServer.Starting), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for the server Started event.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StartedHandlers.Add(new BamEventListener(nameof(BamServer.Started), handler));
        return this;
    }

    /// <summary>
    /// Registers an event handler for when a TCP client connects.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnTcpClientConnected(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.TcpClientConnectedHandlers.Add(new BamEventListener(nameof(BamServer.TcpClientConnected), handler));
        return this;
    }

    /// <summary>
    /// Registers an event handler for when UDP data is received.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnUdpDataReceived(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.UdpDataReceivedHandlers.Add(new BamEventListener(nameof(BamServer.UdpDataReceived), handler));
        return this;
    }

    /// <summary>
    /// Registers an event handler for the server Stopping event.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnStopping(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StoppingHandlers.Add(new BamEventListener(nameof(BamServer.Stopping), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for the server Stopped event.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnStopped(EventHandler<BamServerEventArgs> handler)
    {
        this._serverEventHandlers.StoppedHandlers.Add(new BamEventListener(nameof(BamServer.Stopped), handler));
        return this;
    }

    /// <summary>
    /// Registers an event handler for when server context creation starts.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnCreateContextStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.CreateContextStartedHandlers.Add(new BamEventListener(nameof(BamServer.CreateContextStarted), handler));
        return this;
    }

    /// <summary>
    /// Registers an event handler for when server context creation completes.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnCreateContextComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.CreateContextCompleteHandlers.Add(new BamEventListener(nameof(BamServer.CreateContextComplete), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for when user resolution starts.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnResolveUserStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveUserStartedHandlers.Add(new BamEventListener(nameof(BamServerContextInitializer.ResolveActorStarted), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for when user resolution completes.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnResolveUserComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveUserCompleteHandlers.Add(new BamEventListener(nameof(BamServerContextInitializer.ResolveActorComplete), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for when request authorization starts.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnAuthorizeRequestStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.AuthorizeRequestStartedHandlers.Add(new BamEventListener(nameof(BamServerContextInitializer.AuthorizeRequestStarted), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for when request authorization completes.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnAuthorizeRequestComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.AuthorizeRequestCompleteHandlers.Add(new BamEventListener(nameof(BamServerContextInitializer.AuthorizeRequestComplete), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for when session state resolution starts.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnResolveSessionStateStarted(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveSessionStateStartedHandlers.Add(new BamEventListener(nameof(BamServerContextInitializer.ResolveSessionStateStarted), handler));
        return this;
    }
    
    /// <summary>
    /// Registers an event handler for when session state resolution completes.
    /// </summary>
    /// <param name="handler">The event handler to register.</param>
    /// <returns>This builder for fluent chaining.</returns>
    public BamServerBuilder OnResolveSessionStateComplete(EventHandler<BamServerEventArgs> handler)
    {
        this._requestEventHandlers.ResolveSessionStateCompleteHandlers.Add(new BamEventListener(nameof(BamServerContextInitializer.ResolveSessionStateComplete), handler));
        return this;
    }
    
    /// <summary>
    /// Gets a fluent registration context for the specified service interface in the application component registry.
    /// </summary>
    /// <typeparam name="I">The service interface type to configure.</typeparam>
    /// <returns>A fluent service registry context for the specified type.</returns>
    public FluentServiceRegistryContext<I> ForApplicationComponent<I>()
    {
        return _applicationServiceRegistry.For<I>();
    }

    /// <summary>
    /// Builds and returns a configured <see cref="BamServer"/> instance.
    /// </summary>
    /// <returns>The configured <see cref="BamServer"/>.</returns>
    public BamServer Build()
    {
        _options.ComponentRegistry.CombineWith(_applicationServiceRegistry); 
        _options.ServerEventHandlers = _serverEventHandlers;
        _options.RequestEventHandlers = _requestEventHandlers;
        return new BamServer(_options);
    }
}