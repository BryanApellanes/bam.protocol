using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupAdditionalPropertiesColumns: QueryFilter<GroupAdditionalPropertiesColumns>, IFilterToken
    {
        public GroupAdditionalPropertiesColumns() { }
        public GroupAdditionalPropertiesColumns(string columnName, bool isForeignKey = false)
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
        
		public GroupAdditionalPropertiesColumns KeyColumn => new GroupAdditionalPropertiesColumns("Id");

        public GroupAdditionalPropertiesColumns Id => new GroupAdditionalPropertiesColumns("Id");
        public GroupAdditionalPropertiesColumns Uuid => new GroupAdditionalPropertiesColumns("Uuid");
        public GroupAdditionalPropertiesColumns Cuid => new GroupAdditionalPropertiesColumns("Cuid");
        public GroupAdditionalPropertiesColumns GroupHandle => new GroupAdditionalPropertiesColumns("GroupHandle");
        public GroupAdditionalPropertiesColumns AdditionalPropertyHandle => new GroupAdditionalPropertiesColumns("AdditionalPropertyHandle");
        public GroupAdditionalPropertiesColumns Key => new GroupAdditionalPropertiesColumns("Key");
        public GroupAdditionalPropertiesColumns CompositeKeyId => new GroupAdditionalPropertiesColumns("CompositeKeyId");
        public GroupAdditionalPropertiesColumns CompositeKey => new GroupAdditionalPropertiesColumns("CompositeKey");
        public GroupAdditionalPropertiesColumns CreatedBy => new GroupAdditionalPropertiesColumns("CreatedBy");
        public GroupAdditionalPropertiesColumns ModifiedBy => new GroupAdditionalPropertiesColumns("ModifiedBy");
        public GroupAdditionalPropertiesColumns Modified => new GroupAdditionalPropertiesColumns("Modified");
        public GroupAdditionalPropertiesColumns Deleted => new GroupAdditionalPropertiesColumns("Deleted");
        public GroupAdditionalPropertiesColumns Created => new GroupAdditionalPropertiesColumns("Created");


		public Type DaoType => typeof(GroupAdditionalProperties);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}