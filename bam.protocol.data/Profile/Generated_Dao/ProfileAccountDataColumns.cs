using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileAccountDataColumns: QueryFilter<ProfileAccountDataColumns>, IFilterToken
    {
        public ProfileAccountDataColumns() { }
        public ProfileAccountDataColumns(string columnName, bool isForeignKey = false)
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
        
		public ProfileAccountDataColumns KeyColumn => new ProfileAccountDataColumns("Id");

        public ProfileAccountDataColumns Id => new ProfileAccountDataColumns("Id");
        public ProfileAccountDataColumns Uuid => new ProfileAccountDataColumns("Uuid");
        public ProfileAccountDataColumns Cuid => new ProfileAccountDataColumns("Cuid");
        public ProfileAccountDataColumns PersonId => new ProfileAccountDataColumns("PersonId");
        public ProfileAccountDataColumns PersonHandle => new ProfileAccountDataColumns("PersonHandle");
        public ProfileAccountDataColumns ProfileAccountHandle => new ProfileAccountDataColumns("ProfileAccountHandle");
        public ProfileAccountDataColumns DisplayName => new ProfileAccountDataColumns("DisplayName");
        public ProfileAccountDataColumns Key => new ProfileAccountDataColumns("Key");
        public ProfileAccountDataColumns CompositeKeyId => new ProfileAccountDataColumns("CompositeKeyId");
        public ProfileAccountDataColumns CompositeKey => new ProfileAccountDataColumns("CompositeKey");
        public ProfileAccountDataColumns CreatedBy => new ProfileAccountDataColumns("CreatedBy");
        public ProfileAccountDataColumns ModifiedBy => new ProfileAccountDataColumns("ModifiedBy");
        public ProfileAccountDataColumns Modified => new ProfileAccountDataColumns("Modified");
        public ProfileAccountDataColumns Deleted => new ProfileAccountDataColumns("Deleted");
        public ProfileAccountDataColumns Created => new ProfileAccountDataColumns("Created");


		public Type DaoType => typeof(ProfileAccountData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}