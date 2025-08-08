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
    public class DeviceQuery: Query<DeviceColumns, Device>
    { 
		public DeviceQuery(){}
		public DeviceQuery(WhereDelegate<DeviceColumns> where, OrderBy<DeviceColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public DeviceQuery(Func<DeviceColumns, QueryFilter<DeviceColumns>> where, OrderBy<DeviceColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public DeviceQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static DeviceQuery Where(WhereDelegate<DeviceColumns> where)
        {
            return Where(where, null, null);
        }

        public static DeviceQuery Where(WhereDelegate<DeviceColumns> where, OrderBy<DeviceColumns> orderBy = null, Database db = null)
        {
            return new DeviceQuery(where, orderBy, db);
        }

		public DeviceCollection Execute()
		{
			return new DeviceCollection(this, true);
		}
    }
}