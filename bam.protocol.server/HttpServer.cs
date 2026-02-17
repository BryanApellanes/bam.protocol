/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Net;
using Bam.Logging;
using System.Collections.Concurrent;

namespace Bam.Server
{
    /// <summary>
    /// An HTTP server that listens on specified host bindings and dispatches requests to a handler delegate.
    /// </summary>
    public class HttpServer : IDisposable
    {
        private static readonly ConcurrentDictionary<HostBinding, HttpServer> _listening = new ConcurrentDictionary<HostBinding, HttpServer>();
        private readonly HttpListener _listener;
        private readonly Thread _handlerThread;
        private readonly ILogger _logger = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServer"/> class with the specified request handler.
        /// </summary>
        /// <param name="requestHandler">The delegate invoked for each incoming HTTP request.</param>
        /// <param name="logger">An optional logger. Defaults to <see cref="Log.Default"/>.</param>
        public HttpServer(Action<HttpListenerContext> requestHandler, ILogger? logger = null)
        {
            _logger = (logger ?? Log.Default)!;

            _listener = new HttpListener();
            _handlerThread = new Thread(HandleHttpListenerContextRequests);

            _hostPrefixes = new HashSet<HostBinding>();

            ProcessHttpContextListenerRequest = requestHandler;
        }

        HashSet<HostBinding> _hostPrefixes;
        /// <summary>
        /// Gets or sets the host binding prefixes this server listens on.
        /// </summary>
        public HostBinding[] HostPrefixes
        {
            get => _hostPrefixes.ToArray();
            set => _hostPrefixes = new HashSet<HostBinding>(value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to attempt to stop
        /// other HttpServers that are listening on the same port and 
        /// hostname.
        /// </summary>
        public bool Usurped
        {
            get;
            set;
        }

        /// <summary>
        /// Starts the server using the configured host prefixes.
        /// </summary>
        public void Start()
        {
            Start(HostPrefixes);
        }

        /// <summary>
        /// Starts the server on the specified host prefixes.
        /// </summary>
        /// <param name="hostPrefixes">The host bindings to listen on.</param>
        public void Start(params HostBinding[] hostPrefixes)
        {
            Start(Usurped, hostPrefixes);
        }

        static object _startLock = new object();
        /// <summary>
        /// Starts the server on the specified host prefixes, optionally stopping other servers on the same bindings.
        /// </summary>
        /// <param name="usurped">If true, stops other HttpServers already listening on the same bindings.</param>
        /// <param name="hostPrefixes">The host bindings to listen on.</param>
        public void Start(bool usurped, params HostBinding[] hostPrefixes)
        {
            if (hostPrefixes.Length == 0)
            {
                hostPrefixes = HostPrefixes;
            }
            lock (_startLock)
            {
                hostPrefixes.Each(hp =>
                {
                    if (!_listening.ContainsKey(hp))
                    {
                        AddHostBinding(hp);
                    }
                    else if (usurped && _listening.ContainsKey(hp))
                    {
                        _listening[hp].Stop();
                        _listening.TryRemove(hp, out _);
                        AddHostBinding(hp);
                    }
                    else
                    {
                        _logger.AddEntry("HttpServer: Another HttpServer is already listening for host {0}", LogEventType.Warning, hp.ToString());
                    }
                });

                _listener.Start();
                _handlerThread.Start();
            }
        }

        private void AddHostBinding(HostBinding hp)
        {
            _listening.TryAdd(hp, this);
            string path = hp.ToString();
            if (!path.EndsWith("/"))
            {
                path += "/";
            }
            _logger.AddEntry("HttpServer: {0}", path);
            _listener.Prefixes.Add(path);
        }

        /// <summary>
        /// Disposes of the server and stops listening.
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
            Stop();
        }

        /// <summary>
        /// Gets a value indicating whether this server has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the underlying listener is actively listening for connections.
        /// </summary>
        public bool IsListening => _listener.IsListening;

        private bool _stopping;
        private object _stopLock = new object();
        /// <summary>
        /// Stops the HTTP server and releases all associated resources.
        /// </summary>
        public void Stop()
        {
            lock (_stopLock)
            {
                if (!_stopping)
                {
                    _stopping = true;
                    try
                    {
                        _listener.Stop();
                        _logger.AddEntry("HttpServer listener stopped");
                    }
                    catch (Exception ex)
                    {
                        _logger.AddEntry("Error stopping HttpServer: {0}", ex, ex.Message);
                    }
                }
            }

            foreach (HostBinding hp in _listening.Keys)
            {
                try
                {
                    if (_listening[hp] == this)
                    {
                        if (_listening.TryRemove(hp, out HttpServer? server))
                        {
                            server?.Stop();
                        }
                    }
                }
                catch { }
            }

            try
            {
                if (_handlerThread.ThreadState == ThreadState.Running || _handlerThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    _handlerThread.Interrupt();
                    _handlerThread.Join(5000);
                }
            }
            catch { }
        }

        private void HandleHttpListenerContextRequests()
        {
            while (_listener != null && _listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Task.Run(() => ProcessHttpContextListenerRequest(context));
                }
                catch { }
            }
        }

        /// <summary>
        /// The delegate invoked to process each incoming HTTP listener context request.
        /// </summary>
        public Action<HttpListenerContext> ProcessHttpContextListenerRequest;
    }
}
