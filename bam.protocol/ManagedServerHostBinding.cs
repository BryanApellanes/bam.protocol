using System;
using System.Collections.Generic;
using System.Text;
using Bam.Protocol;

namespace Bam.Server
{
    public class ManagedServerHostBinding : HostBinding
    {
        public ManagedServerHostBinding(IManagedServer server)
        {
            this.Server = server;
        }

        /// <summary>
        /// Create an instance of ManagedServerHostBinding using the specified server name.
        /// </summary>
        /// <param name="serverName"></param>
        public ManagedServerHostBinding(string serverName)
        {
            this.ServerName = serverName;
        }

        IManagedServer _server;
        protected internal IManagedServer Server
        {
            get => _server;
            set
            {
                _server = value;
                Port = ServerName.GetUnprivilegedPortForName();
            }
        }

        string _serverName;
        /// <summary>
        /// Gets or sets the server name.  Not to be confused with the hostname, the ServerName is an identifier assigned to the server for programmatic reference.
        /// </summary>
        public string ServerName
        {
            get => Server?.ServerName ?? _serverName;
            set => _serverName = value;
        }

        public override int Port
        {
            get => ServerName.GetUnprivilegedPortForName();
            set
            {
                // always use derived port
            }
        }
    }
}