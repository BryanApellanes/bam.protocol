using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupDataPersonDataColumns: QueryFilter<GroupDataPersonDataColumns>, IFilterToken
    {
        public GroupDataPersonDataColumns() { }
        public GroupDataPersonDataColumns(string columnName, bool isForeignKey = false)
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
        
		public GroupDataPersonDataColumns KeyColumn => new GroupDataPersonDataColumns("Id");

        public GroupDataPersonDataColumns Id => new GroupDataPersonDataColumns("Id");
        public GroupDataPersonDataColumns Uuid => new GroupDataPersonDataColumns("Uuid");

        public GroupDataPersonDataColumns GroupDataId => new GroupDataPersonDataColumns("GroupDataId", true);
        public GroupDataPersonDataColumns PersonDataId => new GroupDataPersonDataColumns("PersonDataId", true);

		public Type DaoType => typeof(GroupDataPersonData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}