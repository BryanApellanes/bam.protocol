using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    public class BamProxyServer : BamServer
    {
        public BamProxyServer(BamProxyServerOptions options) : base(options)
        {
            this.Options = options;
        }

        protected new BamProxyServerOptions Options { get; private set; }
    }
}
