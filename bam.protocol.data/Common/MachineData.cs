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
    public class MachineData: KeyedAuditRepoData, IMachine
    {
        public MachineData()
        {
            Name = Environment.MachineName;
            DnsName = Dns.GetHostName();
            SetNics();
        }

        public MachineData(string dnsName)
        {
            Name = Environment.MachineName;
            DnsName = dnsName;
            SetNics();
        }

        List<HostAddressData> _hostAddresses;
        public virtual List<HostAddressData> HostAddresses
        {
            get
            {
                if(_hostAddresses == null || _hostAddresses.Count == 0)
                {
                    SetHostAddresses();
                }
                return _hostAddresses;
            }
            set => _hostAddresses = value;
        }
        
        [CompositeKey]
        public string Name { get; set; }
        
        [CompositeKey]
        public string DnsName { get; set; }

        private List<NicData> _nics;
        public virtual List<NicData> NetworkInterfaces
        {
            get
            {
                if(_nics == null || _nics.Count == 0)
                {
                    SetNics();
                }
                return _nics;
            }
            set => _nics = value;
        }

        public override string ToString()
        {
            return $"{Name}@{DnsName}";
        }
        static MachineData _current;
        static object _currentLock = new object();
        public static MachineData Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current, () => new MachineData());
            }
        }

        public override RepoData Save(IRepository repo)
        {
            MachineData existing = repo.Query<MachineData>(new { Name = Name }).FirstOrDefault();
            if(existing == null)
            {
                existing = repo.Save(this);
            }                        
            return existing;
        }

        /// <summary>
        /// Gets the MAC address of the first Network Interface Card in NetworkInterfaces.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string GetFirstMac()
        {
            INic nic = NetworkInterfaces.FirstOrDefault();
            if (nic == null)
            {
                throw new InvalidOperationException("Unable to retrieve first Nic of current host");
            }

            string mac = nic.MacAddress;
            return mac;
        }
        
        public string ToJson()
        {
            return this.ToJson(new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
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
