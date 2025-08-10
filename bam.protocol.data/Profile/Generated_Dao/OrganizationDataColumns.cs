using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationDataColumns: QueryFilter<OrganizationDataColumns>, IFilterToken
    {
        public OrganizationDataColumns() { }
        public OrganizationDataColumns(string columnName, bool isForeignKey = false)
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
        
		public OrganizationDataColumns KeyColumn => new OrganizationDataColumns("Id");

        public OrganizationDataColumns Id => new OrganizationDataColumns("Id");
        public OrganizationDataColumns Uuid => new OrganizationDataColumns("Uuid");
        public OrganizationDataColumns Cuid => new OrganizationDataColumns("Cuid");
        public OrganizationDataColumns Handle => new OrganizationDataColumns("Handle");
        public OrganizationDataColumns Name => new OrganizationDataColumns("Name");
        public OrganizationDataColumns Key => new OrganizationDataColumns("Key");
        public OrganizationDataColumns CompositeKeyId => new OrganizationDataColumns("CompositeKeyId");
        public OrganizationDataColumns CompositeKey => new OrganizationDataColumns("CompositeKey");
        public OrganizationDataColumns CreatedBy => new OrganizationDataColumns("CreatedBy");
        public OrganizationDataColumns ModifiedBy => new OrganizationDataColumns("ModifiedBy");
        public OrganizationDataColumns Modified => new OrganizationDataColumns("Modified");
        public OrganizationDataColumns Deleted => new OrganizationDataColumns("Deleted");
        public OrganizationDataColumns Created => new OrganizationDataColumns("Created");


		public Type DaoType => typeof(OrganizationData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}