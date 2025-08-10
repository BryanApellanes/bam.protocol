using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileAdditionalPropertiesColumns: QueryFilter<ProfileAdditionalPropertiesColumns>, IFilterToken
    {
        public ProfileAdditionalPropertiesColumns() { }
        public ProfileAdditionalPropertiesColumns(string columnName, bool isForeignKey = false)
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
        
		public ProfileAdditionalPropertiesColumns KeyColumn => new ProfileAdditionalPropertiesColumns("Id");

        public ProfileAdditionalPropertiesColumns Id => new ProfileAdditionalPropertiesColumns("Id");
        public ProfileAdditionalPropertiesColumns Uuid => new ProfileAdditionalPropertiesColumns("Uuid");
        public ProfileAdditionalPropertiesColumns Cuid => new ProfileAdditionalPropertiesColumns("Cuid");
        public ProfileAdditionalPropertiesColumns ProfileHandle => new ProfileAdditionalPropertiesColumns("ProfileHandle");
        public ProfileAdditionalPropertiesColumns AdditionalPropertyHandle => new ProfileAdditionalPropertiesColumns("AdditionalPropertyHandle");
        public ProfileAdditionalPropertiesColumns Key => new ProfileAdditionalPropertiesColumns("Key");
        public ProfileAdditionalPropertiesColumns CompositeKeyId => new ProfileAdditionalPropertiesColumns("CompositeKeyId");
        public ProfileAdditionalPropertiesColumns CompositeKey => new ProfileAdditionalPropertiesColumns("CompositeKey");
        public ProfileAdditionalPropertiesColumns CreatedBy => new ProfileAdditionalPropertiesColumns("CreatedBy");
        public ProfileAdditionalPropertiesColumns ModifiedBy => new ProfileAdditionalPropertiesColumns("ModifiedBy");
        public ProfileAdditionalPropertiesColumns Modified => new ProfileAdditionalPropertiesColumns("Modified");
        public ProfileAdditionalPropertiesColumns Deleted => new ProfileAdditionalPropertiesColumns("Deleted");
        public ProfileAdditionalPropertiesColumns Created => new ProfileAdditionalPropertiesColumns("Created");


		public Type DaoType => typeof(ProfileAdditionalProperties);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}