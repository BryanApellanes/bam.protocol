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
    public class ClientSessionDataPagedQuery: PagedQuery<ClientSessionDataColumns, ClientSessionData>
    { 
		public ClientSessionDataPagedQuery(ClientSessionDataColumns orderByColumn,ClientSessionDataQuery query, Database db = null!) : base(orderByColumn, query, db) { }
    }
}