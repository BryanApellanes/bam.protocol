using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PrivateKeySetColumns: QueryFilter<PrivateKeySetColumns>, IFilterToken
    {
        public PrivateKeySetColumns() { }
        public PrivateKeySetColumns(string columnName, bool isForeignKey = false)
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
        
		public PrivateKeySetColumns KeyColumn => new PrivateKeySetColumns("Id");

        public PrivateKeySetColumns Id => new PrivateKeySetColumns("Id");
        public PrivateKeySetColumns Uuid => new PrivateKeySetColumns("Uuid");
        public PrivateKeySetColumns Cuid => new PrivateKeySetColumns("Cuid");
        public PrivateKeySetColumns PrivateRsaKey => new PrivateKeySetColumns("PrivateRsaKey");
        public PrivateKeySetColumns PrivateEccKey => new PrivateKeySetColumns("PrivateEccKey");
        public PrivateKeySetColumns Identifier => new PrivateKeySetColumns("Identifier");
        public PrivateKeySetColumns PersonHandleHmac => new PrivateKeySetColumns("PersonHandleHmac");
        public PrivateKeySetColumns PublicRsaKey => new PrivateKeySetColumns("PublicRsaKey");
        public PrivateKeySetColumns PublicEccKey => new PrivateKeySetColumns("PublicEccKey");
        public PrivateKeySetColumns Created => new PrivateKeySetColumns("Created");


		public Type DaoType => typeof(PrivateKeySet);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}