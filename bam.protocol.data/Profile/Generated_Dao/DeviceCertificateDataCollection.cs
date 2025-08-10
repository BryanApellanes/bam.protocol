using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class DeviceCertificateDataCollection: DaoCollection<DeviceCertificateDataColumns, DeviceCertificateData>
    { 
		public DeviceCertificateDataCollection(){}
		public DeviceCertificateDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public DeviceCertificateDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public DeviceCertificateDataCollection(IQuery<DeviceCertificateDataColumns, DeviceCertificateData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public DeviceCertificateDataCollection(IDatabase db, IQuery<DeviceCertificateDataColumns, DeviceCertificateData> q, bool load) : base(db, q, load) { }
		public DeviceCertificateDataCollection(IQuery<DeviceCertificateDataColumns, DeviceCertificateData> q, bool load) : base(q, load) { }
    }
}