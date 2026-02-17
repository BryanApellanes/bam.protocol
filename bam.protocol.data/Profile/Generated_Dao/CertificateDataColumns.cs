using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class CertificateDataColumns: QueryFilter<CertificateDataColumns>, IFilterToken
    {
        public CertificateDataColumns() { }
        public CertificateDataColumns(string columnName, bool isForeignKey = false)
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
        
		public CertificateDataColumns KeyColumn => new CertificateDataColumns("Id");

        public CertificateDataColumns Id => new CertificateDataColumns("Id");
        public CertificateDataColumns Uuid => new CertificateDataColumns("Uuid");
        public CertificateDataColumns Cuid => new CertificateDataColumns("Cuid");
        public CertificateDataColumns Hash => new CertificateDataColumns("Hash");
        public CertificateDataColumns HashAlgorithm => new CertificateDataColumns("HashAlgorithm");
        public CertificateDataColumns Pem => new CertificateDataColumns("Pem");
        public CertificateDataColumns Key => new CertificateDataColumns("Key");
        public CertificateDataColumns CompositeKeyId => new CertificateDataColumns("CompositeKeyId");
        public CertificateDataColumns CompositeKey => new CertificateDataColumns("CompositeKey");
        public CertificateDataColumns CreatedBy => new CertificateDataColumns("CreatedBy");
        public CertificateDataColumns ModifiedBy => new CertificateDataColumns("ModifiedBy");
        public CertificateDataColumns Modified => new CertificateDataColumns("Modified");
        public CertificateDataColumns Deleted => new CertificateDataColumns("Deleted");
        public CertificateDataColumns Created => new CertificateDataColumns("Created");


		public Type DaoType => typeof(CertificateData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}