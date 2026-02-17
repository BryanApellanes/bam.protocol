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
    public class RsaPrivateKeyDataPagedQuery: PagedQuery<RsaPrivateKeyDataColumns, RsaPrivateKeyData>
    { 
		public RsaPrivateKeyDataPagedQuery(RsaPrivateKeyDataColumns orderByColumn,RsaPrivateKeyDataQuery query, Database db = null!) : base(orderByColumn, query, db) { }
    }
}