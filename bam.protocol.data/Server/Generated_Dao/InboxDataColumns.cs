using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class InboxDataColumns: QueryFilter<InboxDataColumns>, IFilterToken
    {
        public InboxDataColumns() { }
        public InboxDataColumns(string columnName, bool isForeignKey = false)
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
        
		public InboxDataColumns KeyColumn => new InboxDataColumns("Id");

        public InboxDataColumns Id => new InboxDataColumns("Id");
        public InboxDataColumns Uuid => new InboxDataColumns("Uuid");
        public InboxDataColumns Cuid => new InboxDataColumns("Cuid");
        public InboxDataColumns AccountDataId => new InboxDataColumns("AccountDataId");
        public InboxDataColumns Key => new InboxDataColumns("Key");
        public InboxDataColumns CompositeKeyId => new InboxDataColumns("CompositeKeyId");
        public InboxDataColumns CompositeKey => new InboxDataColumns("CompositeKey");
        public InboxDataColumns CreatedBy => new InboxDataColumns("CreatedBy");
        public InboxDataColumns ModifiedBy => new InboxDataColumns("ModifiedBy");
        public InboxDataColumns Modified => new InboxDataColumns("Modified");
        public InboxDataColumns Deleted => new InboxDataColumns("Deleted");
        public InboxDataColumns Created => new InboxDataColumns("Created");


		public Type DaoType => typeof(InboxData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}