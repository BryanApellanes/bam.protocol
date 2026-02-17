/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class AgentAdditionalPropertiesQuery: Query<AgentAdditionalPropertiesColumns, AgentAdditionalProperties>
    { 
		public AgentAdditionalPropertiesQuery(){}
		public AgentAdditionalPropertiesQuery(WhereDelegate<AgentAdditionalPropertiesColumns> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public AgentAdditionalPropertiesQuery(Func<AgentAdditionalPropertiesColumns, QueryFilter<AgentAdditionalPropertiesColumns>> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public AgentAdditionalPropertiesQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static AgentAdditionalPropertiesQuery Where(WhereDelegate<AgentAdditionalPropertiesColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static AgentAdditionalPropertiesQuery Where(WhereDelegate<AgentAdditionalPropertiesColumns> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy = null!, Database db = null!)
        {
            return new AgentAdditionalPropertiesQuery(where, orderBy, db);
        }

		public AgentAdditionalPropertiesCollection Execute()
		{
			return new AgentAdditionalPropertiesCollection(this, true);
		}
    }
}