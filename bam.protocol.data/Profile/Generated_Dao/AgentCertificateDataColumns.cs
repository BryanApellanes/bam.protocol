using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class AgentCertificateDataColumns: QueryFilter<AgentCertificateDataColumns>, IFilterToken
    {
        public AgentCertificateDataColumns() { }
        public AgentCertificateDataColumns(string columnName, bool isForeignKey = false)
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
        
		public AgentCertificateDataColumns KeyColumn => new AgentCertificateDataColumns("Id");

        public AgentCertificateDataColumns Id => new AgentCertificateDataColumns("Id");
        public AgentCertificateDataColumns Uuid => new AgentCertificateDataColumns("Uuid");
        public AgentCertificateDataColumns Cuid => new AgentCertificateDataColumns("Cuid");
        public AgentCertificateDataColumns AgentHandle => new AgentCertificateDataColumns("AgentHandle");
        public AgentCertificateDataColumns CertificateHash => new AgentCertificateDataColumns("CertificateHash");
        public AgentCertificateDataColumns CertificateHashAlgorithm => new AgentCertificateDataColumns("CertificateHashAlgorithm");
        public AgentCertificateDataColumns Created => new AgentCertificateDataColumns("Created");


		public Type DaoType => typeof(AgentCertificateData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}