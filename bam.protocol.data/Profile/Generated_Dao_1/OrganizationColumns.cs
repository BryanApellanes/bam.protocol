using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationColumns: QueryFilter<OrganizationColumns>, IFilterToken
    {
        public OrganizationColumns() { }
        public OrganizationColumns(string columnName, bool isForeignKey = false)
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
        
		public OrganizationColumns KeyColumn => new OrganizationColumns("Id");

        public OrganizationColumns Id => new OrganizationColumns("Id");
        public OrganizationColumns Uuid => new OrganizationColumns("Uuid");
        public OrganizationColumns Cuid => new OrganizationColumns("Cuid");
        public OrganizationColumns Name => new OrganizationColumns("Name");
        public OrganizationColumns Created => new OrganizationColumns("Created");


		public Type DaoType => typeof(Organization);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}