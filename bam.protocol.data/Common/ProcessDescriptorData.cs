using System.Diagnostics;
using System.Reflection;
using Bam.Data.Repositories;
using Bam.Protocol.Data;
using Newtonsoft.Json;

namespace Bam.Protocol.Data.Common
{
    [Serializable]
    public class ProcessDescriptorData : RepoData, IProcessDescriptor
    {
        public ProcessDescriptorData()
        {
        }
        
        private string _instanceIdentifier = null!;

        public string InstanceIdentifier
        {
            get
            {
                if (string.IsNullOrEmpty(_instanceIdentifier))
                {
                    _instanceIdentifier = ToString();
                }

                return _instanceIdentifier;
            }
            set => _instanceIdentifier = value;
        }
        
        public ulong MachineId { get; set; }
        
        [JsonIgnore]
        public IMachine Machine { get; set; } = null!;

        public string HashAlgorithm { get; set; } = null!;

        private string _hash = null!;
        public string Hash
        {
            get
            {
                if (string.IsNullOrEmpty(_hash))
                {
                    _hash = ToString().HashHexString(Enum.Parse<HashAlgorithms>(HashAlgorithm));
                }

                return _hash;
            }
            set => _hash = value;
        }
       
        [CompositeKey]
        public string MachineName { get; set; } = null!;
        [CompositeKey]
        public int ProcessId { get; set; }
        public DateTime StartTime { get; set; }
        public bool HasExited { get; set; }
        public DateTime ExitTime { get; set; }
        public int? ExitCode { get; set; }
        
        [CompositeKey]
        public string FilePath { get; set; } = null!;

        [CompositeKey]
        public string CommandLine { get; set; } = null!;

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if(!(obj is ProcessDescriptorData))
            {
                return false;
            }
            return obj!.ToString()!.Equals(this.ToString());
        }

        static ProcessDescriptorData _current = null!;
        static object _currentLock = new object();
        public static ProcessDescriptorData Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current, () =>
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    ProcessDescriptorData result = new ProcessDescriptorData
                    {
                        HashAlgorithm = nameof(HashAlgorithms.SHA256),
                        MachineName = Environment.MachineName,
                        ProcessId = currentProcess.Id,
                        StartTime = currentProcess.StartTime,
                        FilePath = Assembly.GetEntryAssembly()!.GetFilePath(),
                        CommandLine = Environment.CommandLine
                    };                    
                    result.InstanceIdentifier = result.ToString();
                    return result;
                });
            }
        }
        
        /*public static ProcessDescriptor ForApplicationRegistration(ApplicationRegistrationRepository repo, string serverHost, int port, string applicationName, string organizationName = null)
        {
            Args.ThrowIfNull(repo, nameof(repo));
            Args.ThrowIfNullOrEmpty(serverHost, nameof(serverHost));
            Args.ThrowIfNullOrEmpty(applicationName, nameof(applicationName));            

            ProcessDescriptor result = new ProcessDescriptor();
            result.CopyProperties(Current);
            result.LocalClient = repo.GetOneClientWhere(m => m.MachineName == Machine.Current.Name && m.ApplicationName == Machine.Current.DnsName && m.ServerHost == serverHost && m.Port == port);
            result.Application = new Application { Name = applicationName, Organization = new Organization { Name = organizationName.Or(Organization.Public.Name) } };
            return result;
        }*/

        static string _localIdentifier = null!;
        public static string LocalIdentifier
        {
            get
            {
                if (_localIdentifier == null)
                {
                    _localIdentifier = Current.Hash;
                }

                return _localIdentifier;
            }
        }
        
        public override string ToString()
        {
            return $"{MachineName}~{ProcessId}~{FilePath}~::{CommandLine}";
        }

        public static ProcessDescriptorData Parse(string processDescriptor)
        {
            string[] split = processDescriptor.Split('~');
            if(split.Length >= 3)
            {

                ProcessDescriptorData result = new ProcessDescriptorData
                {
                    MachineName = split[0],
                    ProcessId = int.Parse(split[1]),
                    FilePath = split[2],
                };

                if(split.Length >= 4)
                {
                    result.CommandLine = split[3];
                }
                return result;
            }
            return new ProcessDescriptorData();
        }
    }
}
