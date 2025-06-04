/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Configuration;
using Bam.ServiceProxy;
using System.Diagnostics;
using Bam.Protocol.Server;

namespace Bam.Server
{
    public class HostBinding
    {
        /// <summary>
        /// Instantiate a HostBinding with the hostname of "localhost" and port set to 8080.
        /// </summary>
        public HostBinding()
        {
            this.HostName = "localhost";
            this._port = BamServer.DefaultTcpPort;
            this.Ssl = false;
        }

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

        public string HostName { get; protected set; }
        protected int _port;

        public virtual int Port
        {
            get => _port;
            set => _port = value;
        }

        public bool Ssl { get; set; }

        public override string ToString()
        {
            string protocol = Ssl ? "https://" : "http://";
            return $"{protocol}{HostName}:{Port}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is HostBinding compareTo)
            {
                return compareTo.ToString().Equals(this.ToString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().ToSha1Int();
        }

        public HostBinding FromServiceSubdomain(ServiceSubdomainAttribute attr)
        {
            HostBinding result = this.CopyAs<HostBinding>();
            result.HostName = $"{attr.Subdomain}.{HostName}";
            return result;
        }

        public static HostBinding[] FromHostAppMaps(IEnumerable<HostAppMap> hostAppMaps)
        {
            return hostAppMaps.Select(hm => new HostBinding { HostName = hm.Host, Port = 80 }).ToArray();
        }

        public static HostBinding[] FromBamProcessConfig(string defaultHostName = "localhost", int defaultPort = 80)
        {
            int port = int.Parse(Config.Current["Port", defaultPort.ToString()]);
            bool ssl = Config.Current["Ssl"].IsAffirmative();
            List<HostBinding> results = new List<HostBinding>();
            foreach (string hostName in Config.Current["HostNames"].Or(defaultHostName).DelimitSplit(",", true))
            {
                AddHostBinding(hostName, port, ssl, results);
            }
            return results.ToArray();
        }

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