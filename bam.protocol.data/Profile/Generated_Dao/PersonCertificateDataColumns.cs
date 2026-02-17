using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonCertificateDataColumns: QueryFilter<PersonCertificateDataColumns>, IFilterToken
    {
        public PersonCertificateDataColumns() { }
        public PersonCertificateDataColumns(string columnName, bool isForeignKey = false)
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
        
		public PersonCertificateDataColumns KeyColumn => new PersonCertificateDataColumns("Id");

        public PersonCertificateDataColumns Id => new PersonCertificateDataColumns("Id");
        public PersonCertificateDataColumns Uuid => new PersonCertificateDataColumns("Uuid");
        public PersonCertificateDataColumns Cuid => new PersonCertificateDataColumns("Cuid");
        public PersonCertificateDataColumns PersonHandle => new PersonCertificateDataColumns("PersonHandle");
        public PersonCertificateDataColumns CertificateHash => new PersonCertificateDataColumns("CertificateHash");
        public PersonCertificateDataColumns CertificateHashAlgorithm => new PersonCertificateDataColumns("CertificateHashAlgorithm");
        public PersonCertificateDataColumns Created => new PersonCertificateDataColumns("Created");


		public Type DaoType => typeof(PersonCertificateData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}