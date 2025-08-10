using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupDataColumns: QueryFilter<GroupDataColumns>, IFilterToken
    {
        public GroupDataColumns() { }
        public GroupDataColumns(string columnName, bool isForeignKey = false)
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
        
		public GroupDataColumns KeyColumn => new GroupDataColumns("Id");

        public GroupDataColumns Id => new GroupDataColumns("Id");
        public GroupDataColumns Uuid => new GroupDataColumns("Uuid");
        public GroupDataColumns Cuid => new GroupDataColumns("Cuid");
        public GroupDataColumns Name => new GroupDataColumns("Name");
        public GroupDataColumns Description => new GroupDataColumns("Description");
        public GroupDataColumns Key => new GroupDataColumns("Key");
        public GroupDataColumns CompositeKeyId => new GroupDataColumns("CompositeKeyId");
        public GroupDataColumns CompositeKey => new GroupDataColumns("CompositeKey");
        public GroupDataColumns CreatedBy => new GroupDataColumns("CreatedBy");
        public GroupDataColumns ModifiedBy => new GroupDataColumns("ModifiedBy");
        public GroupDataColumns Modified => new GroupDataColumns("Modified");
        public GroupDataColumns Deleted => new GroupDataColumns("Deleted");
        public GroupDataColumns Created => new GroupDataColumns("Created");


		public Type DaoType => typeof(GroupData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}