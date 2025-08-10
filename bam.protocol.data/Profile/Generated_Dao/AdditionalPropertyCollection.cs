using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class AdditionalPropertyCollection: DaoCollection<AdditionalPropertyColumns, AdditionalProperty>
    { 
		public AdditionalPropertyCollection(){}
		public AdditionalPropertyCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public AdditionalPropertyCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public AdditionalPropertyCollection(IQuery<AdditionalPropertyColumns, AdditionalProperty> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public AdditionalPropertyCollection(IDatabase db, IQuery<AdditionalPropertyColumns, AdditionalProperty> q, bool load) : base(db, q, load) { }
		public AdditionalPropertyCollection(IQuery<AdditionalPropertyColumns, AdditionalProperty> q, bool load) : base(q, load) { }
    }
}