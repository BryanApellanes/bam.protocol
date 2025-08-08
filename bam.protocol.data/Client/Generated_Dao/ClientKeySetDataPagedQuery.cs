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
    public class ClientKeySetDataPagedQuery: PagedQuery<ClientKeySetDataColumns, ClientKeySetData>
    { 
		public ClientKeySetDataPagedQuery(ClientKeySetDataColumns orderByColumn,ClientKeySetDataQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}