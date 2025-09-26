using Bam.Data.Objects;
using Bam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Client
{
    public class BamClient<T> : BamClient
    {
        public BamClient(HostBinding httpBaseAddress) : base(new JsonObjectDataEncoder(), httpBaseAddress)
        {
        }

        public TR Invoke<TR>(string methodName, params object[] args)
        {

        }

    }
}
