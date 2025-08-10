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
    public class DeviceCertificateDataQuery: Query<DeviceCertificateDataColumns, DeviceCertificateData>
    { 
		public DeviceCertificateDataQuery(){}
		public DeviceCertificateDataQuery(WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public DeviceCertificateDataQuery(Func<DeviceCertificateDataColumns, QueryFilter<DeviceCertificateDataColumns>> where, OrderBy<DeviceCertificateDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public DeviceCertificateDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static DeviceCertificateDataQuery Where(WhereDelegate<DeviceCertificateDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static DeviceCertificateDataQuery Where(WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy = null, Database db = null)
        {
            return new DeviceCertificateDataQuery(where, orderBy, db);
        }

		public DeviceCertificateDataCollection Execute()
		{
			return new DeviceCertificateDataCollection(this, true);
		}
    }
}