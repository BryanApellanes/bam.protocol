using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class DeviceCertificateDataColumns: QueryFilter<DeviceCertificateDataColumns>, IFilterToken
    {
        public DeviceCertificateDataColumns() { }
        public DeviceCertificateDataColumns(string columnName, bool isForeignKey = false)
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
        
		public DeviceCertificateDataColumns KeyColumn => new DeviceCertificateDataColumns("Id");

        public DeviceCertificateDataColumns Id => new DeviceCertificateDataColumns("Id");
        public DeviceCertificateDataColumns Uuid => new DeviceCertificateDataColumns("Uuid");
        public DeviceCertificateDataColumns Cuid => new DeviceCertificateDataColumns("Cuid");
        public DeviceCertificateDataColumns DeviceHandle => new DeviceCertificateDataColumns("DeviceHandle");
        public DeviceCertificateDataColumns CertificateHash => new DeviceCertificateDataColumns("CertificateHash");
        public DeviceCertificateDataColumns CertificateHashAlgorithm => new DeviceCertificateDataColumns("CertificateHashAlgorithm");
        public DeviceCertificateDataColumns Created => new DeviceCertificateDataColumns("Created");


		public Type DaoType => typeof(DeviceCertificateData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}