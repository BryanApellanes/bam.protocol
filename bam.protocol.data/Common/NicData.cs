using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Data.Repositories;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace Bam.Protocol.Data.Common
{
    [Serializable]
    public class NicData: RepoData, INic
    {
        public virtual ulong DeviceDataId { get; set; }

        [JsonIgnore]
        public virtual DeviceData DeviceData { get; set; }
    
        public virtual ulong MachineDataId { get; set; }
        
        public string Description { get; set; }
        [JsonIgnore]
        public virtual MachineData MachineData { get; set; }
        
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