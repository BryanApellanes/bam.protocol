using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class ProcessDescriptorDataCollection: DaoCollection<ProcessDescriptorDataColumns, ProcessDescriptorData>
    { 
		public ProcessDescriptorDataCollection(){}
		public ProcessDescriptorDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public ProcessDescriptorDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public ProcessDescriptorDataCollection(IQuery<ProcessDescriptorDataColumns, ProcessDescriptorData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public ProcessDescriptorDataCollection(IDatabase db, IQuery<ProcessDescriptorDataColumns, ProcessDescriptorData> q, bool load) : base(db, q, load) { }
		public ProcessDescriptorDataCollection(IQuery<ProcessDescriptorDataColumns, ProcessDescriptorData> q, bool load) : base(q, load) { }
    }
}