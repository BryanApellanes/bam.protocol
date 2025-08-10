using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonAdditionalPropertiesCollection: DaoCollection<PersonAdditionalPropertiesColumns, PersonAdditionalProperties>
    { 
		public PersonAdditionalPropertiesCollection(){}
		public PersonAdditionalPropertiesCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PersonAdditionalPropertiesCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PersonAdditionalPropertiesCollection(IQuery<PersonAdditionalPropertiesColumns, PersonAdditionalProperties> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PersonAdditionalPropertiesCollection(IDatabase db, IQuery<PersonAdditionalPropertiesColumns, PersonAdditionalProperties> q, bool load) : base(db, q, load) { }
		public PersonAdditionalPropertiesCollection(IQuery<PersonAdditionalPropertiesColumns, PersonAdditionalProperties> q, bool load) : base(q, load) { }
    }
}