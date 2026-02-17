using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationCertificateDataColumns: QueryFilter<OrganizationCertificateDataColumns>, IFilterToken
    {
        public OrganizationCertificateDataColumns() { }
        public OrganizationCertificateDataColumns(string columnName, bool isForeignKey = false)
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
        
		public OrganizationCertificateDataColumns KeyColumn => new OrganizationCertificateDataColumns("Id");

        public OrganizationCertificateDataColumns Id => new OrganizationCertificateDataColumns("Id");
        public OrganizationCertificateDataColumns Uuid => new OrganizationCertificateDataColumns("Uuid");
        public OrganizationCertificateDataColumns Cuid => new OrganizationCertificateDataColumns("Cuid");
        public OrganizationCertificateDataColumns OrganizationHandle => new OrganizationCertificateDataColumns("OrganizationHandle");
        public OrganizationCertificateDataColumns CertificateHash => new OrganizationCertificateDataColumns("CertificateHash");
        public OrganizationCertificateDataColumns CertificateHashAlgorithm => new OrganizationCertificateDataColumns("CertificateHashAlgorithm");
        public OrganizationCertificateDataColumns Created => new OrganizationCertificateDataColumns("Created");


		public Type DaoType => typeof(OrganizationCertificateData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}