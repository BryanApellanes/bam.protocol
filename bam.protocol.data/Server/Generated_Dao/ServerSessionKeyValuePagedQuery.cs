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
    public class ServerSessionKeyValuePagedQuery: PagedQuery<ServerSessionKeyValueColumns, ServerSessionKeyValue>
    { 
		public ServerSessionKeyValuePagedQuery(ServerSessionKeyValueColumns orderByColumn,ServerSessionKeyValueQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}