using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationAdditionalPropertiesColumns: QueryFilter<OrganizationAdditionalPropertiesColumns>, IFilterToken
    {
        public OrganizationAdditionalPropertiesColumns() { }
        public OrganizationAdditionalPropertiesColumns(string columnName, bool isForeignKey = false)
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
        
		public OrganizationAdditionalPropertiesColumns KeyColumn => new OrganizationAdditionalPropertiesColumns("Id");

        public OrganizationAdditionalPropertiesColumns Id => new OrganizationAdditionalPropertiesColumns("Id");
        public OrganizationAdditionalPropertiesColumns Uuid => new OrganizationAdditionalPropertiesColumns("Uuid");
        public OrganizationAdditionalPropertiesColumns Cuid => new OrganizationAdditionalPropertiesColumns("Cuid");
        public OrganizationAdditionalPropertiesColumns OrganizationHandle => new OrganizationAdditionalPropertiesColumns("OrganizationHandle");
        public OrganizationAdditionalPropertiesColumns AdditionalPropertyHandle => new OrganizationAdditionalPropertiesColumns("AdditionalPropertyHandle");
        public OrganizationAdditionalPropertiesColumns Key => new OrganizationAdditionalPropertiesColumns("Key");
        public OrganizationAdditionalPropertiesColumns CompositeKeyId => new OrganizationAdditionalPropertiesColumns("CompositeKeyId");
        public OrganizationAdditionalPropertiesColumns CompositeKey => new OrganizationAdditionalPropertiesColumns("CompositeKey");
        public OrganizationAdditionalPropertiesColumns CreatedBy => new OrganizationAdditionalPropertiesColumns("CreatedBy");
        public OrganizationAdditionalPropertiesColumns ModifiedBy => new OrganizationAdditionalPropertiesColumns("ModifiedBy");
        public OrganizationAdditionalPropertiesColumns Modified => new OrganizationAdditionalPropertiesColumns("Modified");
        public OrganizationAdditionalPropertiesColumns Deleted => new OrganizationAdditionalPropertiesColumns("Deleted");
        public OrganizationAdditionalPropertiesColumns Created => new OrganizationAdditionalPropertiesColumns("Created");


		public Type DaoType => typeof(OrganizationAdditionalProperties);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}