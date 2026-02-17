using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientSessionKeyValueColumns: QueryFilter<ClientSessionKeyValueColumns>, IFilterToken
    {
        public ClientSessionKeyValueColumns() { }
        public ClientSessionKeyValueColumns(string columnName, bool isForeignKey = false)
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
        
		public ClientSessionKeyValueColumns KeyColumn => new ClientSessionKeyValueColumns("Id");

        public ClientSessionKeyValueColumns Id => new ClientSessionKeyValueColumns("Id");
        public ClientSessionKeyValueColumns Uuid => new ClientSessionKeyValueColumns("Uuid");
        public ClientSessionKeyValueColumns Cuid => new ClientSessionKeyValueColumns("Cuid");
        public ClientSessionKeyValueColumns Key => new ClientSessionKeyValueColumns("Key");
        public ClientSessionKeyValueColumns Value => new ClientSessionKeyValueColumns("Value");
        public ClientSessionKeyValueColumns CompositeKeyId => new ClientSessionKeyValueColumns("CompositeKeyId");
        public ClientSessionKeyValueColumns CompositeKey => new ClientSessionKeyValueColumns("CompositeKey");
        public ClientSessionKeyValueColumns CreatedBy => new ClientSessionKeyValueColumns("CreatedBy");
        public ClientSessionKeyValueColumns ModifiedBy => new ClientSessionKeyValueColumns("ModifiedBy");
        public ClientSessionKeyValueColumns Modified => new ClientSessionKeyValueColumns("Modified");
        public ClientSessionKeyValueColumns Deleted => new ClientSessionKeyValueColumns("Deleted");
        public ClientSessionKeyValueColumns Created => new ClientSessionKeyValueColumns("Created");

        public ClientSessionKeyValueColumns ClientSessionDataId => new ClientSessionKeyValueColumns("ClientSessionDataId", true);

		public Type DaoType => typeof(ClientSessionKeyValue);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}