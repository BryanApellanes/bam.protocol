using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class HostAddressDataCollection: DaoCollection<HostAddressDataColumns, HostAddressData>
    { 
		public HostAddressDataCollection(){}
		public HostAddressDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public HostAddressDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public HostAddressDataCollection(IQuery<HostAddressDataColumns, HostAddressData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public HostAddressDataCollection(IDatabase db, IQuery<HostAddressDataColumns, HostAddressData> q, bool load) : base(db, q, load) { }
		public HostAddressDataCollection(IQuery<HostAddressDataColumns, HostAddressData> q, bool load) : base(q, load) { }
    }
}