using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ProfileColumns: QueryFilter<ProfileColumns>, IFilterToken
    {
        public ProfileColumns() { }
        public ProfileColumns(string columnName, bool isForeignKey = false)
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
        
		public ProfileColumns KeyColumn => new ProfileColumns("Id");

        public ProfileColumns Id => new ProfileColumns("Id");
        public ProfileColumns Uuid => new ProfileColumns("Uuid");
        public ProfileColumns Cuid => new ProfileColumns("Cuid");
        public ProfileColumns Handle => new ProfileColumns("Handle");
        public ProfileColumns PersonHandle => new ProfileColumns("PersonHandle");
        public ProfileColumns Created => new ProfileColumns("Created");


		public Type DaoType => typeof(Profile);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}