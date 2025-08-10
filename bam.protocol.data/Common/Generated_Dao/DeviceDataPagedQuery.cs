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
    public class DeviceDataPagedQuery: PagedQuery<DeviceDataColumns, DeviceData>
    { 
		public DeviceDataPagedQuery(DeviceDataColumns orderByColumn,DeviceDataQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}