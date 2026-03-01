using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class OutboxDataColumns: QueryFilter<OutboxDataColumns>, IFilterToken
    {
        public OutboxDataColumns() { }
        public OutboxDataColumns(string columnName, bool isForeignKey = false)
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
        
		public OutboxDataColumns KeyColumn => new OutboxDataColumns("Id");

        public OutboxDataColumns Id => new OutboxDataColumns("Id");
        public OutboxDataColumns Uuid => new OutboxDataColumns("Uuid");
        public OutboxDataColumns Cuid => new OutboxDataColumns("Cuid");
        public OutboxDataColumns AccountDataId => new OutboxDataColumns("AccountDataId");
        public OutboxDataColumns Key => new OutboxDataColumns("Key");
        public OutboxDataColumns CompositeKeyId => new OutboxDataColumns("CompositeKeyId");
        public OutboxDataColumns CompositeKey => new OutboxDataColumns("CompositeKey");
        public OutboxDataColumns CreatedBy => new OutboxDataColumns("CreatedBy");
        public OutboxDataColumns ModifiedBy => new OutboxDataColumns("ModifiedBy");
        public OutboxDataColumns Modified => new OutboxDataColumns("Modified");
        public OutboxDataColumns Deleted => new OutboxDataColumns("Deleted");
        public OutboxDataColumns Created => new OutboxDataColumns("Created");


		public Type DaoType => typeof(OutboxData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}