using Bam.DependencyInjection;
using Bam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// Configuration options for a <see cref="BamProxyServer"/>, including proxy type registrations.
    /// </summary>
    public class BamProxyServerOptions : BamServerOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BamProxyServerOptions"/> class with a service registry and proxy types.
        /// </summary>
        /// <param name="proxyRegistry">The service registry for proxy type resolution.</param>
        /// <param name="proxyTypes">The types to proxy.</param>
        public BamProxyServerOptions(ServiceRegistry proxyRegistry, params Type[] proxyTypes) : base()
        {
            this.ProxyRegistry = proxyRegistry ?? throw new ArgumentNullException(nameof(proxyRegistry));
            this.ProxyTypes.UnionWith(proxyTypes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BamProxyServerOptions"/> class with the specified proxy types.
        /// </summary>
        /// <param name="proxyTypes">The types to proxy.</param>
        public BamProxyServerOptions(params Type[] proxyTypes) : base()
        {
            this.ProxyTypes.UnionWith(proxyTypes);
        }

        /// <summary>
        /// Gets the service registry used for proxy type resolution.
        /// </summary>
        public ServiceRegistry ProxyRegistry { get; } = new ServiceRegistry();

        /// <summary>
        /// Creates a new <see cref="BamProxyServerOptions"/> instance configured for the specified communication handler type.
        /// </summary>
        /// <typeparam name="THandler">The type of communication handler to use.</typeparam>
        /// <param name="proxyTypes">The types to proxy.</param>
        /// <returns>A new <see cref="BamProxyServerOptions"/> instance.</returns>
        public static BamProxyServerOptions Create<THandler>(params Type[] proxyTypes) where THandler : CommunicationHandler, new()
        {
            return new BamProxyServerOptions();
        }

        /// <summary>
        /// Gets the set of types that this proxy server handles.
        /// </summary>
        public HashSet<Type> ProxyTypes { get; } = new HashSet<Type>();

        /// <summary>
        /// Sets the HTTP host binding for this proxy server.
        /// </summary>
        /// <param name="hostBinding">The host binding to use.</param>
        /// <returns>This instance for fluent chaining.</returns>
        public BamProxyServerOptions WithHostBinding(HostBinding hostBinding)
        {
            this.HttpHostBinding = hostBinding;
            return this;
        }
    }
}
