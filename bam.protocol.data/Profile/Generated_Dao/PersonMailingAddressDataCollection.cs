using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonMailingAddressDataCollection: DaoCollection<PersonMailingAddressDataColumns, PersonMailingAddressData>
    { 
		public PersonMailingAddressDataCollection(){}
		public PersonMailingAddressDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public PersonMailingAddressDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public PersonMailingAddressDataCollection(IQuery<PersonMailingAddressDataColumns, PersonMailingAddressData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public PersonMailingAddressDataCollection(IDatabase db, IQuery<PersonMailingAddressDataColumns, PersonMailingAddressData> q, bool load) : base(db, q, load) { }
		public PersonMailingAddressDataCollection(IQuery<PersonMailingAddressDataColumns, PersonMailingAddressData> q, bool load) : base(q, load) { }
    }
}