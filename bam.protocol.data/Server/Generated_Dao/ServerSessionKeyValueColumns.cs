using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionKeyValueColumns: QueryFilter<ServerSessionKeyValueColumns>, IFilterToken
    {
        public ServerSessionKeyValueColumns() { }
        public ServerSessionKeyValueColumns(string columnName, bool isForeignKey = false)
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
        
		public ServerSessionKeyValueColumns KeyColumn => new ServerSessionKeyValueColumns("Id");

        public ServerSessionKeyValueColumns Id => new ServerSessionKeyValueColumns("Id");
        public ServerSessionKeyValueColumns Uuid => new ServerSessionKeyValueColumns("Uuid");
        public ServerSessionKeyValueColumns Cuid => new ServerSessionKeyValueColumns("Cuid");
        public ServerSessionKeyValueColumns Key => new ServerSessionKeyValueColumns("Key");
        public ServerSessionKeyValueColumns Value => new ServerSessionKeyValueColumns("Value");
        public ServerSessionKeyValueColumns CompositeKeyId => new ServerSessionKeyValueColumns("CompositeKeyId");
        public ServerSessionKeyValueColumns CompositeKey => new ServerSessionKeyValueColumns("CompositeKey");
        public ServerSessionKeyValueColumns CreatedBy => new ServerSessionKeyValueColumns("CreatedBy");
        public ServerSessionKeyValueColumns ModifiedBy => new ServerSessionKeyValueColumns("ModifiedBy");
        public ServerSessionKeyValueColumns Modified => new ServerSessionKeyValueColumns("Modified");
        public ServerSessionKeyValueColumns Deleted => new ServerSessionKeyValueColumns("Deleted");
        public ServerSessionKeyValueColumns Created => new ServerSessionKeyValueColumns("Created");

        public ServerSessionKeyValueColumns ServerSessionId => new ServerSessionKeyValueColumns("ServerSessionId", true);

		public Type DaoType => typeof(ServerSessionKeyValue);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}