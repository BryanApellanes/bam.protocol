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
    public class DevicePagedQuery: PagedQuery<DeviceColumns, Device>
    { 
		public DevicePagedQuery(DeviceColumns orderByColumn,DeviceQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}