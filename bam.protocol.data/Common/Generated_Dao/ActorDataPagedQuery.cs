/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class ActorDataPagedQuery: PagedQuery<ActorDataColumns, ActorData>
    { 
		public ActorDataPagedQuery(ActorDataColumns orderByColumn,ActorDataQuery query, Database db = null!) : base(orderByColumn, query, db) { }
    }
}