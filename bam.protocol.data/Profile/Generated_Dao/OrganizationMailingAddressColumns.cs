using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationMailingAddressColumns: QueryFilter<OrganizationMailingAddressColumns>, IFilterToken
    {
        public OrganizationMailingAddressColumns() { }
        public OrganizationMailingAddressColumns(string columnName, bool isForeignKey = false)
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
        
		public OrganizationMailingAddressColumns KeyColumn => new OrganizationMailingAddressColumns("Id");

        public OrganizationMailingAddressColumns Id => new OrganizationMailingAddressColumns("Id");
        public OrganizationMailingAddressColumns Uuid => new OrganizationMailingAddressColumns("Uuid");
        public OrganizationMailingAddressColumns Cuid => new OrganizationMailingAddressColumns("Cuid");
        public OrganizationMailingAddressColumns OrganizationHandle => new OrganizationMailingAddressColumns("OrganizationHandle");
        public OrganizationMailingAddressColumns MailingAddressHandle => new OrganizationMailingAddressColumns("MailingAddressHandle");
        public OrganizationMailingAddressColumns Key => new OrganizationMailingAddressColumns("Key");
        public OrganizationMailingAddressColumns CompositeKeyId => new OrganizationMailingAddressColumns("CompositeKeyId");
        public OrganizationMailingAddressColumns CompositeKey => new OrganizationMailingAddressColumns("CompositeKey");
        public OrganizationMailingAddressColumns CreatedBy => new OrganizationMailingAddressColumns("CreatedBy");
        public OrganizationMailingAddressColumns ModifiedBy => new OrganizationMailingAddressColumns("ModifiedBy");
        public OrganizationMailingAddressColumns Modified => new OrganizationMailingAddressColumns("Modified");
        public OrganizationMailingAddressColumns Deleted => new OrganizationMailingAddressColumns("Deleted");
        public OrganizationMailingAddressColumns Created => new OrganizationMailingAddressColumns("Created");


		public Type DaoType => typeof(OrganizationMailingAddress);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}