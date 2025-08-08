using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class PrivateKeySetDataColumns: QueryFilter<PrivateKeySetDataColumns>, IFilterToken
    {
        public PrivateKeySetDataColumns() { }
        public PrivateKeySetDataColumns(string columnName, bool isForeignKey = false)
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
        
		public PrivateKeySetDataColumns KeyColumn => new PrivateKeySetDataColumns("Id");

        public PrivateKeySetDataColumns Id => new PrivateKeySetDataColumns("Id");
        public PrivateKeySetDataColumns Uuid => new PrivateKeySetDataColumns("Uuid");
        public PrivateKeySetDataColumns Cuid => new PrivateKeySetDataColumns("Cuid");
        public PrivateKeySetDataColumns PublicEccKeyHash => new PrivateKeySetDataColumns("PublicEccKeyHash");
        public PrivateKeySetDataColumns PublicEccKeyHashAlgorithm => new PrivateKeySetDataColumns("PublicEccKeyHashAlgorithm");
        public PrivateKeySetDataColumns PublicRsaKeyHash => new PrivateKeySetDataColumns("PublicRsaKeyHash");
        public PrivateKeySetDataColumns PublicRsaKeyHashAlgorithm => new PrivateKeySetDataColumns("PublicRsaKeyHashAlgorithm");
        public PrivateKeySetDataColumns KeySetHandle => new PrivateKeySetDataColumns("KeySetHandle");
        public PrivateKeySetDataColumns PublicRsaKey => new PrivateKeySetDataColumns("PublicRsaKey");
        public PrivateKeySetDataColumns PublicEccKey => new PrivateKeySetDataColumns("PublicEccKey");
        public PrivateKeySetDataColumns Created => new PrivateKeySetDataColumns("Created");


		public Type DaoType => typeof(PrivateKeySetData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}