using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class EccPrivateKeyDataColumns: QueryFilter<EccPrivateKeyDataColumns>, IFilterToken
    {
        public EccPrivateKeyDataColumns() { }
        public EccPrivateKeyDataColumns(string columnName, bool isForeignKey = false)
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
        
		public EccPrivateKeyDataColumns KeyColumn => new EccPrivateKeyDataColumns("Id");

        public EccPrivateKeyDataColumns Id => new EccPrivateKeyDataColumns("Id");
        public EccPrivateKeyDataColumns Uuid => new EccPrivateKeyDataColumns("Uuid");
        public EccPrivateKeyDataColumns Cuid => new EccPrivateKeyDataColumns("Cuid");
        public EccPrivateKeyDataColumns Pem => new EccPrivateKeyDataColumns("Pem");
        public EccPrivateKeyDataColumns PublicKeyHash => new EccPrivateKeyDataColumns("PublicKeyHash");
        public EccPrivateKeyDataColumns PublicKeyHashAlgorithm => new EccPrivateKeyDataColumns("PublicKeyHashAlgorithm");
        public EccPrivateKeyDataColumns Created => new EccPrivateKeyDataColumns("Created");


		public Type DaoType => typeof(EccPrivateKeyData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}