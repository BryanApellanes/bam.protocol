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
    public class ProfilePagedQuery: PagedQuery<ProfileColumns, Profile>
    { 
		public ProfilePagedQuery(ProfileColumns orderByColumn,ProfileQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}