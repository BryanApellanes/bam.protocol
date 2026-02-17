using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PublicKeySetDataColumns: QueryFilter<PublicKeySetDataColumns>, IFilterToken
    {
        public PublicKeySetDataColumns() { }
        public PublicKeySetDataColumns(string columnName, bool isForeignKey = false)
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
        
		public PublicKeySetDataColumns KeyColumn => new PublicKeySetDataColumns("Id");

        public PublicKeySetDataColumns Id => new PublicKeySetDataColumns("Id");
        public PublicKeySetDataColumns Uuid => new PublicKeySetDataColumns("Uuid");
        public PublicKeySetDataColumns Cuid => new PublicKeySetDataColumns("Cuid");
        public PublicKeySetDataColumns KeySetHandle => new PublicKeySetDataColumns("KeySetHandle");
        public PublicKeySetDataColumns PublicRsaKey => new PublicKeySetDataColumns("PublicRsaKey");
        public PublicKeySetDataColumns PublicEccKey => new PublicKeySetDataColumns("PublicEccKey");
        public PublicKeySetDataColumns Created => new PublicKeySetDataColumns("Created");


		public Type DaoType => typeof(PublicKeySetData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}