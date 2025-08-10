/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class AgentDataQuery: Query<AgentDataColumns, AgentData>
    { 
		public AgentDataQuery(){}
		public AgentDataQuery(WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public AgentDataQuery(Func<AgentDataColumns, QueryFilter<AgentDataColumns>> where, OrderBy<AgentDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public AgentDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static AgentDataQuery Where(WhereDelegate<AgentDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static AgentDataQuery Where(WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy = null, Database db = null)
        {
            return new AgentDataQuery(where, orderBy, db);
        }

		public AgentDataCollection Execute()
		{
			return new AgentDataCollection(this, true);
		}
    }
}