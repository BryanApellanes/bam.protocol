using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileAdditionalPropertiesCollection: DaoCollection<ProfileAdditionalPropertiesColumns, ProfileAdditionalProperties>
    { 
		public ProfileAdditionalPropertiesCollection(){}
		public ProfileAdditionalPropertiesCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ProfileAdditionalPropertiesCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ProfileAdditionalPropertiesCollection(IQuery<ProfileAdditionalPropertiesColumns, ProfileAdditionalProperties> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ProfileAdditionalPropertiesCollection(IDatabase db, IQuery<ProfileAdditionalPropertiesColumns, ProfileAdditionalProperties> q, bool load) : base(db, q, load) { }
		public ProfileAdditionalPropertiesCollection(IQuery<ProfileAdditionalPropertiesColumns, ProfileAdditionalProperties> q, bool load) : base(q, load) { }
    }
}