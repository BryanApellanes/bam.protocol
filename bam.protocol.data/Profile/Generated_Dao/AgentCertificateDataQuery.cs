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
    public class AgentCertificateDataQuery: Query<AgentCertificateDataColumns, AgentCertificateData>
    { 
		public AgentCertificateDataQuery(){}
		public AgentCertificateDataQuery(WhereDelegate<AgentCertificateDataColumns> where, OrderBy<AgentCertificateDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public AgentCertificateDataQuery(Func<AgentCertificateDataColumns, QueryFilter<AgentCertificateDataColumns>> where, OrderBy<AgentCertificateDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public AgentCertificateDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static AgentCertificateDataQuery Where(WhereDelegate<AgentCertificateDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static AgentCertificateDataQuery Where(WhereDelegate<AgentCertificateDataColumns> where, OrderBy<AgentCertificateDataColumns> orderBy = null!, Database db = null!)
        {
            return new AgentCertificateDataQuery(where, orderBy, db);
        }

		public AgentCertificateDataCollection Execute()
		{
			return new AgentCertificateDataCollection(this, true);
		}
    }
}