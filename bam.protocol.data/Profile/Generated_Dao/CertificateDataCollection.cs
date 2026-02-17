using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class CertificateDataCollection: DaoCollection<CertificateDataColumns, CertificateData>
    { 
		public CertificateDataCollection(){}
		public CertificateDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public CertificateDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public CertificateDataCollection(IQuery<CertificateDataColumns, CertificateData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public CertificateDataCollection(IDatabase db, IQuery<CertificateDataColumns, CertificateData> q, bool load) : base(db, q, load) { }
		public CertificateDataCollection(IQuery<CertificateDataColumns, CertificateData> q, bool load) : base(q, load) { }
    }
}