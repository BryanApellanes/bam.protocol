using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientSessionDataColumns: QueryFilter<ClientSessionDataColumns>, IFilterToken
    {
        public ClientSessionDataColumns() { }
        public ClientSessionDataColumns(string columnName, bool isForeignKey = false)
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
        
		public ClientSessionDataColumns KeyColumn => new ClientSessionDataColumns("Id");

        public ClientSessionDataColumns Id => new ClientSessionDataColumns("Id");
        public ClientSessionDataColumns Uuid => new ClientSessionDataColumns("Uuid");
        public ClientSessionDataColumns Cuid => new ClientSessionDataColumns("Cuid");
        public ClientSessionDataColumns SessionId => new ClientSessionDataColumns("SessionId");
        public ClientSessionDataColumns Key => new ClientSessionDataColumns("Key");
        public ClientSessionDataColumns CompositeKeyId => new ClientSessionDataColumns("CompositeKeyId");
        public ClientSessionDataColumns CompositeKey => new ClientSessionDataColumns("CompositeKey");
        public ClientSessionDataColumns CreatedBy => new ClientSessionDataColumns("CreatedBy");
        public ClientSessionDataColumns ModifiedBy => new ClientSessionDataColumns("ModifiedBy");
        public ClientSessionDataColumns Modified => new ClientSessionDataColumns("Modified");
        public ClientSessionDataColumns Deleted => new ClientSessionDataColumns("Deleted");
        public ClientSessionDataColumns Created => new ClientSessionDataColumns("Created");


		public Type DaoType => typeof(ClientSessionData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}