using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Data.Repositories;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
//using Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Repository;

namespace Bam.Net.CoreServices.ApplicationRegistration.Data
{
    [Serializable]
    public class ProcessDescriptor : KeyedAuditRepoData
    {
        public ProcessDescriptor()
        {
            //Machine = Machine.Current;
        }
        //#region client relevant
        //public long ApplicationId { get; set; }
        //public virtual MediaTypeNames.Application Application { get; set; }
        private string _instanceIdentifier;

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
        /*#endregion
        public ulong MachineId { get; set; }
        [JsonIgnore]
        public virtual Machine Machine { get; set; }*/
        //public virtual Client LocalClient { get; set; }
        public string HashAlgorithm { get; set; }

        private string _hash;
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
        public string MachineName { get; set; }
        [CompositeKey]
        public int ProcessId { get; set; }
        public DateTime StartTime { get; set; }
        public bool HasExited { get; set; }
        public DateTime ExitTime { get; set; }
        public int? ExitCode { get; set; }
        
        [CompositeKey]
        public string FilePath { get; set; }
        
        [CompositeKey]
        public string CommandLine { get; set; }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is ProcessDescriptor))
            {
                return false;
            }
            return obj.ToString().Equals(this.ToString());
        }

        static ProcessDescriptor _current;
        static object _currentLock = new object();
        public static ProcessDescriptor Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current, () =>
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    ProcessDescriptor result = new ProcessDescriptor
                    {
                        HashAlgorithm = HashAlgorithms.SHA1.ToString(),
                        MachineName = Environment.MachineName,
                        ProcessId = currentProcess.Id,
                        StartTime = currentProcess.StartTime,
                        FilePath = Assembly.GetEntryAssembly().GetFilePath(),
                        CommandLine = Environment.CommandLine
                    };                    
                    result.InstanceIdentifier = result.ToString();
                    //result.Hash = result.InstanceIdentifier.Sha1();
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

        static string _localIdentifier;
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

        public static ProcessDescriptor Parse(string processDescriptor)
        {
            string[] split = processDescriptor.Split('~');
            if(split.Length >= 3)
            {

                ProcessDescriptor result = new ProcessDescriptor
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
            return new ProcessDescriptor();
        }
    }
}
