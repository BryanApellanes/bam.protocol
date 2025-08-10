using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonDataOrganizationDataColumns: QueryFilter<PersonDataOrganizationDataColumns>, IFilterToken
    {
        public PersonDataOrganizationDataColumns() { }
        public PersonDataOrganizationDataColumns(string columnName, bool isForeignKey = false)
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
        
		public PersonDataOrganizationDataColumns KeyColumn => new PersonDataOrganizationDataColumns("Id");

        public PersonDataOrganizationDataColumns Id => new PersonDataOrganizationDataColumns("Id");
        public PersonDataOrganizationDataColumns Uuid => new PersonDataOrganizationDataColumns("Uuid");

        public PersonDataOrganizationDataColumns PersonDataId => new PersonDataOrganizationDataColumns("PersonDataId", true);
        public PersonDataOrganizationDataColumns OrganizationDataId => new PersonDataOrganizationDataColumns("OrganizationDataId", true);

		public Type DaoType => typeof(PersonDataOrganizationData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}