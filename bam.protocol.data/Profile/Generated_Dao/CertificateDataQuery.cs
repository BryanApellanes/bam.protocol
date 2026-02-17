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
    public class CertificateDataQuery: Query<CertificateDataColumns, CertificateData>
    { 
		public CertificateDataQuery(){}
		public CertificateDataQuery(WhereDelegate<CertificateDataColumns> where, OrderBy<CertificateDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public CertificateDataQuery(Func<CertificateDataColumns, QueryFilter<CertificateDataColumns>> where, OrderBy<CertificateDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public CertificateDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static CertificateDataQuery Where(WhereDelegate<CertificateDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static CertificateDataQuery Where(WhereDelegate<CertificateDataColumns> where, OrderBy<CertificateDataColumns> orderBy = null!, Database db = null!)
        {
            return new CertificateDataQuery(where, orderBy, db);
        }

		public CertificateDataCollection Execute()
		{
			return new CertificateDataCollection(this, true);
		}
    }
}