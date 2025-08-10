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
    public class ProcessDescriptorDataQuery: Query<ProcessDescriptorDataColumns, ProcessDescriptorData>
    { 
		public ProcessDescriptorDataQuery(){}
		public ProcessDescriptorDataQuery(WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ProcessDescriptorDataQuery(Func<ProcessDescriptorDataColumns, QueryFilter<ProcessDescriptorDataColumns>> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ProcessDescriptorDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ProcessDescriptorDataQuery Where(WhereDelegate<ProcessDescriptorDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static ProcessDescriptorDataQuery Where(WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, Database db = null)
        {
            return new ProcessDescriptorDataQuery(where, orderBy, db);
        }

		public ProcessDescriptorDataCollection Execute()
		{
			return new ProcessDescriptorDataCollection(this, true);
		}
    }
}