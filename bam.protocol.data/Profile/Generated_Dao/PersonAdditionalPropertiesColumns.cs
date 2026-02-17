using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonAdditionalPropertiesColumns: QueryFilter<PersonAdditionalPropertiesColumns>, IFilterToken
    {
        public PersonAdditionalPropertiesColumns() { }
        public PersonAdditionalPropertiesColumns(string columnName, bool isForeignKey = false)
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
        
		public PersonAdditionalPropertiesColumns KeyColumn => new PersonAdditionalPropertiesColumns("Id");

        public PersonAdditionalPropertiesColumns Id => new PersonAdditionalPropertiesColumns("Id");
        public PersonAdditionalPropertiesColumns Uuid => new PersonAdditionalPropertiesColumns("Uuid");
        public PersonAdditionalPropertiesColumns Cuid => new PersonAdditionalPropertiesColumns("Cuid");
        public PersonAdditionalPropertiesColumns PersonHandle => new PersonAdditionalPropertiesColumns("PersonHandle");
        public PersonAdditionalPropertiesColumns AdditionalPropertyHandle => new PersonAdditionalPropertiesColumns("AdditionalPropertyHandle");
        public PersonAdditionalPropertiesColumns Key => new PersonAdditionalPropertiesColumns("Key");
        public PersonAdditionalPropertiesColumns CompositeKeyId => new PersonAdditionalPropertiesColumns("CompositeKeyId");
        public PersonAdditionalPropertiesColumns CompositeKey => new PersonAdditionalPropertiesColumns("CompositeKey");
        public PersonAdditionalPropertiesColumns CreatedBy => new PersonAdditionalPropertiesColumns("CreatedBy");
        public PersonAdditionalPropertiesColumns ModifiedBy => new PersonAdditionalPropertiesColumns("ModifiedBy");
        public PersonAdditionalPropertiesColumns Modified => new PersonAdditionalPropertiesColumns("Modified");
        public PersonAdditionalPropertiesColumns Deleted => new PersonAdditionalPropertiesColumns("Deleted");
        public PersonAdditionalPropertiesColumns Created => new PersonAdditionalPropertiesColumns("Created");


		public Type DaoType => typeof(PersonAdditionalProperties);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}