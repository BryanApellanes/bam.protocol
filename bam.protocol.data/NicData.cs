using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Data.Repositories;

namespace Bam.Protocol.Data
{
    [Serializable]
    public class NicData: KeyedAuditRepoData, INicData
    {
        public ulong MachineId { get; set; }
        
        public virtual IMachine Machine { get; set; }
        
        [CompositeKey]
        public string AddressFamily { get; set; }
        
        [CompositeKey]
        public string Address { get; set; }
        
        [CompositeKey]
        public string MacAddress { get; set; }
        
        public override int GetHashCode()
        {
            return $"{AddressFamily}:{Address}:{MacAddress}".GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if(obj is NicData input)
            {
                return input.AddressFamily.Equals(AddressFamily) && input.Address.Equals(Address);
            }
            return false;
        }
    }
}