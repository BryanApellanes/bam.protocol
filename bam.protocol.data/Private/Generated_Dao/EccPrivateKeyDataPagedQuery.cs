/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class EccPrivateKeyDataPagedQuery: PagedQuery<EccPrivateKeyDataColumns, EccPrivateKeyData>
    { 
		public EccPrivateKeyDataPagedQuery(EccPrivateKeyDataColumns orderByColumn,EccPrivateKeyDataQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}