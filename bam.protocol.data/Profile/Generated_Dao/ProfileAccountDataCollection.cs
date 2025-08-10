using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileAccountDataCollection: DaoCollection<ProfileAccountDataColumns, ProfileAccountData>
    { 
		public ProfileAccountDataCollection(){}
		public ProfileAccountDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ProfileAccountDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ProfileAccountDataCollection(IQuery<ProfileAccountDataColumns, ProfileAccountData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ProfileAccountDataCollection(IDatabase db, IQuery<ProfileAccountDataColumns, ProfileAccountData> q, bool load) : base(db, q, load) { }
		public ProfileAccountDataCollection(IQuery<ProfileAccountDataColumns, ProfileAccountData> q, bool load) : base(q, load) { }
    }
}