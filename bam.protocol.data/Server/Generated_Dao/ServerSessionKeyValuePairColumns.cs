using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionKeyValuePairColumns: QueryFilter<ServerSessionKeyValuePairColumns>, IFilterToken
    {
        public ServerSessionKeyValuePairColumns() { }
        public ServerSessionKeyValuePairColumns(string columnName, bool isForeignKey = false)
            : base(columnName)
        { 
            _isForeignKey = isForeignKey;
        }
        
        public bool IsKey()
        {
            return (bool)ColumnName?.Equals(KeyColumn.ColumnName)!;
        }

        private bool? _isForeignKey;
        public bool IsForeignKey
        {
            get
            {
                if (_isForeignKey == null)
                {
                    PropertyInfo? prop = DaoType
                        .GetProperties()
                        .FirstOrDefault(pi => ((MemberInfo) pi)
                            .HasCustomAttributeOfType<ForeignKeyAttribute>(out ForeignKeyAttribute foreignKeyAttribute)
                                && foreignKeyAttribute.Name.Equals(ColumnName));
                        _isForeignKey = prop != null;
                }

                return _isForeignKey!.Value;
            }
            set => _isForeignKey = value;
        }
        
		public ServerSessionKeyValuePairColumns KeyColumn => new ServerSessionKeyValuePairColumns("Id");

        public ServerSessionKeyValuePairColumns Id => new ServerSessionKeyValuePairColumns("Id");
        public ServerSessionKeyValuePairColumns Uuid => new ServerSessionKeyValuePairColumns("Uuid");
        public ServerSessionKeyValuePairColumns Cuid => new ServerSessionKeyValuePairColumns("Cuid");
        public ServerSessionKeyValuePairColumns Key => new ServerSessionKeyValuePairColumns("Key");
        public ServerSessionKeyValuePairColumns Value => new ServerSessionKeyValuePairColumns("Value");
        public ServerSessionKeyValuePairColumns CompositeKeyId => new ServerSessionKeyValuePairColumns("CompositeKeyId");
        public ServerSessionKeyValuePairColumns CompositeKey => new ServerSessionKeyValuePairColumns("CompositeKey");
        public ServerSessionKeyValuePairColumns CreatedBy => new ServerSessionKeyValuePairColumns("CreatedBy");
        public ServerSessionKeyValuePairColumns ModifiedBy => new ServerSessionKeyValuePairColumns("ModifiedBy");
        public ServerSessionKeyValuePairColumns Modified => new ServerSessionKeyValuePairColumns("Modified");
        public ServerSessionKeyValuePairColumns Deleted => new ServerSessionKeyValuePairColumns("Deleted");
        public ServerSessionKeyValuePairColumns Created => new ServerSessionKeyValuePairColumns("Created");

        public ServerSessionKeyValuePairColumns ServerSessionId => new ServerSessionKeyValuePairColumns("ServerSessionId", true);

		public Type DaoType => typeof(ServerSessionKeyValuePair);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}