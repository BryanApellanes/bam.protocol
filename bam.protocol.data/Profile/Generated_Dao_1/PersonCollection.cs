using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonCollection: DaoCollection<PersonColumns, Person>
    { 
		public PersonCollection(){}
		public PersonCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PersonCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PersonCollection(IQuery<PersonColumns, Person> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PersonCollection(IDatabase db, IQuery<PersonColumns, Person> q, bool load) : base(db, q, load) { }
		public PersonCollection(IQuery<PersonColumns, Person> q, bool load) : base(q, load) { }
    }
}