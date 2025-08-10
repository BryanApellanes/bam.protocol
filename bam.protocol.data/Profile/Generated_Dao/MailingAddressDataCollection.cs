using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class MailingAddressDataCollection: DaoCollection<MailingAddressDataColumns, MailingAddressData>
    { 
		public MailingAddressDataCollection(){}
		public MailingAddressDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public MailingAddressDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public MailingAddressDataCollection(IQuery<MailingAddressDataColumns, MailingAddressData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public MailingAddressDataCollection(IDatabase db, IQuery<MailingAddressDataColumns, MailingAddressData> q, bool load) : base(db, q, load) { }
		public MailingAddressDataCollection(IQuery<MailingAddressDataColumns, MailingAddressData> q, bool load) : base(q, load) { }
    }
}