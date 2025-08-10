/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionKeyValuePairPagedQuery: PagedQuery<ServerSessionKeyValuePairColumns, ServerSessionKeyValuePair>
    { 
		public ServerSessionKeyValuePairPagedQuery(ServerSessionKeyValuePairColumns orderByColumn,ServerSessionKeyValuePairQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}