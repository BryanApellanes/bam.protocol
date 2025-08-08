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
    public class OrganizationPagedQuery: PagedQuery<OrganizationColumns, Organization>
    { 
		public OrganizationPagedQuery(OrganizationColumns orderByColumn,OrganizationQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}