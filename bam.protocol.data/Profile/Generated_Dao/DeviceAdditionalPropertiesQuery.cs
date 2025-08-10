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
    public class DeviceAdditionalPropertiesQuery: Query<DeviceAdditionalPropertiesColumns, DeviceAdditionalProperties>
    { 
		public DeviceAdditionalPropertiesQuery(){}
		public DeviceAdditionalPropertiesQuery(WhereDelegate<DeviceAdditionalPropertiesColumns> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public DeviceAdditionalPropertiesQuery(Func<DeviceAdditionalPropertiesColumns, QueryFilter<DeviceAdditionalPropertiesColumns>> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public DeviceAdditionalPropertiesQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static DeviceAdditionalPropertiesQuery Where(WhereDelegate<DeviceAdditionalPropertiesColumns> where)
        {
            return Where(where, null, null);
        }

        public static DeviceAdditionalPropertiesQuery Where(WhereDelegate<DeviceAdditionalPropertiesColumns> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy = null, Database db = null)
        {
            return new DeviceAdditionalPropertiesQuery(where, orderBy, db);
        }

		public DeviceAdditionalPropertiesCollection Execute()
		{
			return new DeviceAdditionalPropertiesCollection(this, true);
		}
    }
}