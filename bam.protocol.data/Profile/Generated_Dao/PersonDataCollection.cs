using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonDataCollection: DaoCollection<PersonDataColumns, PersonData>
    { 
		public PersonDataCollection(){}
		public PersonDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PersonDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PersonDataCollection(IQuery<PersonDataColumns, PersonData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PersonDataCollection(IDatabase db, IQuery<PersonDataColumns, PersonData> q, bool load) : base(db, q, load) { }
		public PersonDataCollection(IQuery<PersonDataColumns, PersonData> q, bool load) : base(q, load) { }
    }
}