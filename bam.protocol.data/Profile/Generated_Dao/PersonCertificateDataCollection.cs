using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonCertificateDataCollection: DaoCollection<PersonCertificateDataColumns, PersonCertificateData>
    { 
		public PersonCertificateDataCollection(){}
		public PersonCertificateDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PersonCertificateDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PersonCertificateDataCollection(IQuery<PersonCertificateDataColumns, PersonCertificateData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PersonCertificateDataCollection(IDatabase db, IQuery<PersonCertificateDataColumns, PersonCertificateData> q, bool load) : base(db, q, load) { }
		public PersonCertificateDataCollection(IQuery<PersonCertificateDataColumns, PersonCertificateData> q, bool load) : base(q, load) { }
    }
}