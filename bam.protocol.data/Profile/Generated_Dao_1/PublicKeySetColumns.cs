using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PublicKeySetColumns: QueryFilter<PublicKeySetColumns>, IFilterToken
    {
        public PublicKeySetColumns() { }
        public PublicKeySetColumns(string columnName, bool isForeignKey = false)
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
        
		public PublicKeySetColumns KeyColumn => new PublicKeySetColumns("Id");

        public PublicKeySetColumns Id => new PublicKeySetColumns("Id");
        public PublicKeySetColumns Uuid => new PublicKeySetColumns("Uuid");
        public PublicKeySetColumns Cuid => new PublicKeySetColumns("Cuid");
        public PublicKeySetColumns Identifier => new PublicKeySetColumns("Identifier");
        public PublicKeySetColumns PersonHandleHmac => new PublicKeySetColumns("PersonHandleHmac");
        public PublicKeySetColumns PublicRsaKey => new PublicKeySetColumns("PublicRsaKey");
        public PublicKeySetColumns PublicEccKey => new PublicKeySetColumns("PublicEccKey");
        public PublicKeySetColumns Created => new PublicKeySetColumns("Created");


		public Type DaoType => typeof(PublicKeySet);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}