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
    public class PersonCertificateDataQuery: Query<PersonCertificateDataColumns, PersonCertificateData>
    { 
		public PersonCertificateDataQuery(){}
		public PersonCertificateDataQuery(WhereDelegate<PersonCertificateDataColumns> where, OrderBy<PersonCertificateDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PersonCertificateDataQuery(Func<PersonCertificateDataColumns, QueryFilter<PersonCertificateDataColumns>> where, OrderBy<PersonCertificateDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PersonCertificateDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PersonCertificateDataQuery Where(WhereDelegate<PersonCertificateDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static PersonCertificateDataQuery Where(WhereDelegate<PersonCertificateDataColumns> where, OrderBy<PersonCertificateDataColumns> orderBy = null, Database db = null)
        {
            return new PersonCertificateDataQuery(where, orderBy, db);
        }

		public PersonCertificateDataCollection Execute()
		{
			return new PersonCertificateDataCollection(this, true);
		}
    }
}