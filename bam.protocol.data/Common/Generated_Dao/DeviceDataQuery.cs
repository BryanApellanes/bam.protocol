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
    public class DeviceDataQuery: Query<DeviceDataColumns, DeviceData>
    { 
		public DeviceDataQuery(){}
		public DeviceDataQuery(WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public DeviceDataQuery(Func<DeviceDataColumns, QueryFilter<DeviceDataColumns>> where, OrderBy<DeviceDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public DeviceDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static DeviceDataQuery Where(WhereDelegate<DeviceDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static DeviceDataQuery Where(WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy = null!, Database db = null!)
        {
            return new DeviceDataQuery(where, orderBy, db);
        }

		public DeviceDataCollection Execute()
		{
			return new DeviceDataCollection(this, true);
		}
    }
}