using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Bam.Data.Repositories;
using Bam.Protocol.Data.Profile;
using Newtonsoft.Json;

namespace Bam.Protocol.Data.Common
{
    public class MachineData : RepoData, IMachine, IHasHandle
    {
        public MachineData(): this(true)
        {
        }

        public MachineData(bool initialize = true)
        {
            if (initialize)
            {
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            if (!IsInitialized)
            {
                Name = Environment.MachineName;
                DnsName = Dns.GetHostName();
                SetHostAddresses();
                SetNics();
                IsInitialized = true;
            }
        }

        protected bool IsInitialized { get; set; }

        List<HostAddressData> _hostAddresses;
        public virtual List<HostAddressData> HostAddresses
        {
            get => _hostAddresses ??= new List<HostAddressData>();
            set => _hostAddresses = value;
        }

        [CompositeKey]
        [CompositeHandle]
        public string Name { get; set; }

        [CompositeKey]
        public string DnsName { get; set; }

        private List<NicData> _nics;
        public virtual List<NicData> NetworkInterfaces
        {
            get => _nics ??= new List<NicData>();
            set => _nics = value;
        }

        [CompositeHandle]
        public string[] Macs
        {
            get
            {
                return NetworkInterfaces.Select(n => n.MacAddress).ToArray();
            }
        }

        public override string ToString()
        {
            return $"{Name}@{DnsName}";
        }

        static MachineData _current;
        static object _currentLock = new object();
        [JsonIgnore]
        public static MachineData Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current, () => new MachineData());
            }
        }

        public string Handle { get; set; } = string.Empty;

        public override RepoData Save(IRepository repo)
        {
            MachineData existing = repo.Query<MachineData>(new { Name = Name }).FirstOrDefault();
            if(existing == null)
            {
                existing = repo.Save(this);
            }                        
            return existing;
        }
        
        private void SetNics()
        {
            var context = new { Nics = new List<NicData>() };
            List<NetworkInterface> nicList = new List<NetworkInterface>();
            NetworkInterface.GetAllNetworkInterfaces().Each(context, (ctx, nic) =>
            {
                IPInterfaceProperties nicProperties = nic.GetIPProperties();
                foreach (UnicastIPAddressInformation unicast in nicProperties.UnicastAddresses.Where(a => !a.Address.Equals(IPAddress.Loopback) && !a.Address.Equals(IPAddress.IPv6Loopback)))
                {
                    ctx.Nics.Add(
                        new NicData
                        {
                            AddressFamily = unicast.Address.AddressFamily.ToString(),
                            Address = unicast.Address.ToString(),
                            Description = nic.Description,
                            MacAddress = nic.GetPhysicalAddress().ToString()
                        });
                }
            });
            _nics = context.Nics;
        }

        IPAddress[] _hostIps;
        private void SetHostAddresses()
        {
            if(_hostIps == null)
            {
                _hostIps = Dns.GetHostAddresses(DnsName);
            }
            List<HostAddressData> hostAddresses = new List<HostAddressData>();
            _hostIps.Each(ip => hostAddresses.Add(new HostAddressData { HostName = DnsName, AddressFamily = ip.AddressFamily.ToString(), IpAddress = ip.ToString(), MachineData = this, MachineDataId = Id }));
            _hostAddresses = hostAddresses;
        }
    }
}
