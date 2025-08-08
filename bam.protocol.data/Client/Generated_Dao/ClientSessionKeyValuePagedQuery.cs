/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientSessionKeyValuePagedQuery: PagedQuery<ClientSessionKeyValueColumns, ClientSessionKeyValue>
    { 
		public ClientSessionKeyValuePagedQuery(ClientSessionKeyValueColumns orderByColumn,ClientSessionKeyValueQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}