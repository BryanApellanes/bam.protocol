using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class AccountDataCollection: DaoCollection<AccountDataColumns, AccountData>
    { 
		public AccountDataCollection(){}
		public AccountDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public AccountDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public AccountDataCollection(IQuery<AccountDataColumns, AccountData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public AccountDataCollection(IDatabase db, IQuery<AccountDataColumns, AccountData> q, bool load) : base(db, q, load) { }
		public AccountDataCollection(IQuery<AccountDataColumns, AccountData> q, bool load) : base(q, load) { }
    }
}