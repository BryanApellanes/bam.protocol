using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientKeySetDataColumns: QueryFilter<ClientKeySetDataColumns>, IFilterToken
    {
        public ClientKeySetDataColumns() { }
        public ClientKeySetDataColumns(string columnName, bool isForeignKey = false)
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
        
		public ClientKeySetDataColumns KeyColumn => new ClientKeySetDataColumns("Id");

        public ClientKeySetDataColumns Id => new ClientKeySetDataColumns("Id");
        public ClientKeySetDataColumns Uuid => new ClientKeySetDataColumns("Uuid");
        public ClientKeySetDataColumns Cuid => new ClientKeySetDataColumns("Cuid");
        public ClientKeySetDataColumns MachineName => new ClientKeySetDataColumns("MachineName");
        public ClientKeySetDataColumns ClientHostName => new ClientKeySetDataColumns("ClientHostName");
        public ClientKeySetDataColumns ServerHostName => new ClientKeySetDataColumns("ServerHostName");
        public ClientKeySetDataColumns Key => new ClientKeySetDataColumns("Key");
        public ClientKeySetDataColumns CompositeKeyId => new ClientKeySetDataColumns("CompositeKeyId");
        public ClientKeySetDataColumns CompositeKey => new ClientKeySetDataColumns("CompositeKey");
        public ClientKeySetDataColumns CreatedBy => new ClientKeySetDataColumns("CreatedBy");
        public ClientKeySetDataColumns ModifiedBy => new ClientKeySetDataColumns("ModifiedBy");
        public ClientKeySetDataColumns Modified => new ClientKeySetDataColumns("Modified");
        public ClientKeySetDataColumns Deleted => new ClientKeySetDataColumns("Deleted");
        public ClientKeySetDataColumns Created => new ClientKeySetDataColumns("Created");


		public Type DaoType => typeof(ClientKeySetData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}