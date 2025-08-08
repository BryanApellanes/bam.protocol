using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class RsaPrivateKeyDataColumns: QueryFilter<RsaPrivateKeyDataColumns>, IFilterToken
    {
        public RsaPrivateKeyDataColumns() { }
        public RsaPrivateKeyDataColumns(string columnName, bool isForeignKey = false)
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
        
		public RsaPrivateKeyDataColumns KeyColumn => new RsaPrivateKeyDataColumns("Id");

        public RsaPrivateKeyDataColumns Id => new RsaPrivateKeyDataColumns("Id");
        public RsaPrivateKeyDataColumns Uuid => new RsaPrivateKeyDataColumns("Uuid");
        public RsaPrivateKeyDataColumns Cuid => new RsaPrivateKeyDataColumns("Cuid");
        public RsaPrivateKeyDataColumns Pem => new RsaPrivateKeyDataColumns("Pem");
        public RsaPrivateKeyDataColumns PublicKeyHash => new RsaPrivateKeyDataColumns("PublicKeyHash");
        public RsaPrivateKeyDataColumns PublicKeyHashAlgorithm => new RsaPrivateKeyDataColumns("PublicKeyHashAlgorithm");
        public RsaPrivateKeyDataColumns Created => new RsaPrivateKeyDataColumns("Created");


		public Type DaoType => typeof(RsaPrivateKeyData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}