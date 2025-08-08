using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileCollection: DaoCollection<ProfileColumns, Profile>
    { 
		public ProfileCollection(){}
		public ProfileCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ProfileCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ProfileCollection(IQuery<ProfileColumns, Profile> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ProfileCollection(IDatabase db, IQuery<ProfileColumns, Profile> q, bool load) : base(db, q, load) { }
		public ProfileCollection(IQuery<ProfileColumns, Profile> q, bool load) : base(q, load) { }
    }
}