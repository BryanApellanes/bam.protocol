/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Configuration;
using Bam.ServiceProxy;
using System.Diagnostics;
using Bam.Protocol;

namespace Bam.Server
{
    /// <summary>
    /// Represents a network host binding, consisting of a hostname, port, and SSL setting.
    /// </summary>
    public class HostBinding
    {
        /// <summary>
        /// Instantiate a HostBinding with the hostname of "localhost" and port set to 8080.
        /// </summary>
        public HostBinding()
        {
            this.HostName = "localhost";
            this._port = DefaultPorts.DefaultTcpPort;
            this.Ssl = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostBinding"/> class with localhost and the specified port.
        /// </summary>
        /// <param name="port">The port number.</param>
        public HostBinding(int port) : this("localhost", port)
        {
        }

        /// <summary>
        /// Instantiate a HostBinding with the specified hostname and port.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public HostBinding(string hostName, int port = 8080)
        {
            this.HostName = hostName;
            this._port = port;
            this.Ssl = false;
        }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        public string HostName { get; protected set; }
        protected int _port;

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        public virtual int Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether SSL/TLS should be used.
        /// </summary>
        public bool Ssl { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            string protocol = Ssl ? "https://" : "http://";
            return $"{protocol}{HostName}:{Port}";
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is HostBinding compareTo)
            {
                return compareTo.ToString().Equals(this.ToString());
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToString().ToSha1Int();
        }

        /// <summary>
        /// Creates a new <see cref="HostBinding"/> with the subdomain from the specified attribute prepended to the hostname.
        /// </summary>
        /// <param name="attr">The service subdomain attribute.</param>
        /// <returns>A new <see cref="HostBinding"/> with the subdomain-prefixed hostname.</returns>
        public HostBinding FromServiceSubdomain(ServiceSubdomainAttribute attr)
        {
            HostBinding result = this.CopyAs<HostBinding>();
            result.HostName = $"{attr.Subdomain}.{HostName}";
            return result;
        }

        /// <summary>
        /// Creates an array of host bindings from the specified host-to-application mappings, each on port 80.
        /// </summary>
        /// <param name="hostAppMaps">The host-to-application mappings.</param>
        /// <returns>An array of <see cref="HostBinding"/> instances.</returns>
        public static HostBinding[] FromHostAppMaps(IEnumerable<HostAppMap> hostAppMaps)
        {
            return hostAppMaps.Select(hm => new HostBinding { HostName = hm.Host, Port = 80 }).ToArray();
        }

        /// <summary>
        /// Creates host bindings from the current Bam process configuration.
        /// </summary>
        /// <param name="defaultHostName">The default hostname if not configured.</param>
        /// <param name="defaultPort">The default port if not configured.</param>
        /// <returns>An array of <see cref="HostBinding"/> instances from the configuration.</returns>
        public static HostBinding[] FromBamProcessConfig(string defaultHostName = "localhost", int defaultPort = 80)
        {
            int port = int.Parse(Config.Current["Port", defaultPort.ToString()]!);
            bool ssl = Config.Current["Ssl"]!.IsAffirmative();
            List<HostBinding> results = new List<HostBinding>();
            foreach (string hostName in Config.Current["HostNames"]!.Or(defaultHostName).DelimitSplit(",", true))
            {
                AddHostBinding(hostName, port, ssl, results);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Creates host bindings from the default application configuration settings.
        /// </summary>
        /// <param name="defaultHostName">The default hostname if not configured.</param>
        /// <param name="defaultPort">The default port if not configured.</param>
        /// <returns>An array of <see cref="HostBinding"/> instances from the configuration.</returns>
        public static HostBinding[] FromDefaultConfiguration(string defaultHostName = "localhost", int defaultPort = 80)
        {
            int port = int.Parse(DefaultConfiguration.GetAppSetting("Port", defaultPort.ToString()));
            bool ssl = DefaultConfiguration.GetAppSetting("Ssl").IsAffirmative();
            List<HostBinding> results = new List<HostBinding>();
            foreach (string hostName in DefaultConfiguration.GetAppSetting("HostNames").Or(defaultHostName).DelimitSplit(",", true))
            {
                AddHostBinding(hostName, port, ssl, results);
            }
            return results.ToArray();
        }

        private static void AddHostBinding(string hostName, int port, bool ssl, List<HostBinding> results)
        {
            HostBinding hostPrefix = new HostBinding()
            {
                HostName = hostName,
                Port = port,
                Ssl = ssl
            };
            results.Add(hostPrefix);
            Trace.Write($"Default Config Hostname: {hostPrefix.ToString()}");
        }
    }
}