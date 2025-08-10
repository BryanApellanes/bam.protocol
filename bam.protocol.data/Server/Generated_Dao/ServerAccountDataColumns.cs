using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerAccountDataColumns: QueryFilter<ServerAccountDataColumns>, IFilterToken
    {
        public ServerAccountDataColumns() { }
        public ServerAccountDataColumns(string columnName, bool isForeignKey = false)
            : base(columnName)
        { 
            _isForeignKey = isForeignKey;
        }
        
        public bool IsKey()
        {
            return (bool)ColumnName?.Equals(KeyColumn.ColumnName);
        }

        private bool? _isForeignKey;
        public bool IsForeignKey
        {
            get
            {
                if (_isForeignKey == null)
                {
                    PropertyInfo prop = DaoType
                        .GetProperties()
                        .FirstOrDefault(pi => ((MemberInfo) pi)
                            .HasCustomAttributeOfType<ForeignKeyAttribute>(out ForeignKeyAttribute foreignKeyAttribute)
                                && foreignKeyAttribute.Name.Equals(ColumnName));
                        _isForeignKey = prop != null;
                }

                return _isForeignKey.Value;
            }
            set => _isForeignKey = value;
        }
        
		public ServerAccountDataColumns KeyColumn => new ServerAccountDataColumns("Id");

        public ServerAccountDataColumns Id => new ServerAccountDataColumns("Id");
        public ServerAccountDataColumns Uuid => new ServerAccountDataColumns("Uuid");
        public ServerAccountDataColumns Cuid => new ServerAccountDataColumns("Cuid");
        public ServerAccountDataColumns Issuer => new ServerAccountDataColumns("Issuer");
        public ServerAccountDataColumns ProfileHandle => new ServerAccountDataColumns("ProfileHandle");
        public ServerAccountDataColumns CompositeKeyId => new ServerAccountDataColumns("CompositeKeyId");
        public ServerAccountDataColumns CompositeKey => new ServerAccountDataColumns("CompositeKey");
        public ServerAccountDataColumns CreatedBy => new ServerAccountDataColumns("CreatedBy");
        public ServerAccountDataColumns ModifiedBy => new ServerAccountDataColumns("ModifiedBy");
        public ServerAccountDataColumns Modified => new ServerAccountDataColumns("Modified");
        public ServerAccountDataColumns Deleted => new ServerAccountDataColumns("Deleted");
        public ServerAccountDataColumns Created => new ServerAccountDataColumns("Created");


		public Type DaoType => typeof(ServerAccountData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}