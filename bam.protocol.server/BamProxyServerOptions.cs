using Bam.DependencyInjection;
using Bam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    public class BamProxyServerOptions : BamServerOptions
    {
        public BamProxyServerOptions(ServiceRegistry proxyRegistry, params Type[] proxyTypes) : base()
        {
            this.ProxyRegistry = proxyRegistry ?? throw new ArgumentNullException(nameof(proxyRegistry));
            this.ProxyTypes.UnionWith(proxyTypes);
        }

        public BamProxyServerOptions(params Type[] proxyTypes) : base()
        {
            this.ProxyTypes.UnionWith(proxyTypes);
        }

        public ServiceRegistry ProxyRegistry { get; } = new ServiceRegistry();

        public static BamProxyServerOptions Create<THandler>(params Type[] proxyTypes) where THandler : CommunicationHandler, new()
        {
            return new BamProxyServerOptions();
        }

        public HashSet<Type> ProxyTypes { get; } = new HashSet<Type>();

        public BamProxyServerOptions WithHostBinding(HostBinding hostBinding)
        {
            this.HttpHostBinding = hostBinding;
            return this;
        }
    }
}
