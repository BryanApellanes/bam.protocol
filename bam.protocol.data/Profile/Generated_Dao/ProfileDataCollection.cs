using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileDataCollection: DaoCollection<ProfileDataColumns, ProfileData>
    { 
		public ProfileDataCollection(){}
		public ProfileDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ProfileDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ProfileDataCollection(IQuery<ProfileDataColumns, ProfileData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ProfileDataCollection(IDatabase db, IQuery<ProfileDataColumns, ProfileData> q, bool load) : base(db, q, load) { }
		public ProfileDataCollection(IQuery<ProfileDataColumns, ProfileData> q, bool load) : base(q, load) { }
    }
}