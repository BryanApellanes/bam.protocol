using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// A BAM server that acts as a proxy, forwarding requests based on proxy configuration.
    /// </summary>
    public class BamProxyServer : BamServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BamProxyServer"/> class with the specified proxy options.
        /// </summary>
        /// <param name="options">The proxy server options.</param>
        public BamProxyServer(BamProxyServerOptions options) : base(options)
        {
            this.Options = options;
        }

        protected new BamProxyServerOptions Options { get; private set; }
    }
}
