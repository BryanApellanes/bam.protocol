using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileDataColumns: QueryFilter<ProfileDataColumns>, IFilterToken
    {
        public ProfileDataColumns() { }
        public ProfileDataColumns(string columnName, bool isForeignKey = false)
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
        
		public ProfileDataColumns KeyColumn => new ProfileDataColumns("Id");

        public ProfileDataColumns Id => new ProfileDataColumns("Id");
        public ProfileDataColumns Uuid => new ProfileDataColumns("Uuid");
        public ProfileDataColumns Cuid => new ProfileDataColumns("Cuid");
        public ProfileDataColumns PersonId => new ProfileDataColumns("PersonId");
        public ProfileDataColumns PersonHandle => new ProfileDataColumns("PersonHandle");
        public ProfileDataColumns ProfileHandle => new ProfileDataColumns("ProfileHandle");
        public ProfileDataColumns Name => new ProfileDataColumns("Name");
        public ProfileDataColumns ShowFirstName => new ProfileDataColumns("ShowFirstName");
        public ProfileDataColumns ShowLastName => new ProfileDataColumns("ShowLastName");
        public ProfileDataColumns ShowEmail => new ProfileDataColumns("ShowEmail");
        public ProfileDataColumns ShowPhone => new ProfileDataColumns("ShowPhone");
        public ProfileDataColumns MailingAddressHandles => new ProfileDataColumns("MailingAddressHandles");
        public ProfileDataColumns Key => new ProfileDataColumns("Key");
        public ProfileDataColumns CompositeKeyId => new ProfileDataColumns("CompositeKeyId");
        public ProfileDataColumns CompositeKey => new ProfileDataColumns("CompositeKey");
        public ProfileDataColumns CreatedBy => new ProfileDataColumns("CreatedBy");
        public ProfileDataColumns ModifiedBy => new ProfileDataColumns("ModifiedBy");
        public ProfileDataColumns Modified => new ProfileDataColumns("Modified");
        public ProfileDataColumns Deleted => new ProfileDataColumns("Deleted");
        public ProfileDataColumns Created => new ProfileDataColumns("Created");


		public Type DaoType => typeof(ProfileData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}