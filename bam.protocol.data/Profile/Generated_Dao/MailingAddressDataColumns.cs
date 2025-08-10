using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class MailingAddressDataColumns: QueryFilter<MailingAddressDataColumns>, IFilterToken
    {
        public MailingAddressDataColumns() { }
        public MailingAddressDataColumns(string columnName, bool isForeignKey = false)
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
        
		public MailingAddressDataColumns KeyColumn => new MailingAddressDataColumns("Id");

        public MailingAddressDataColumns Id => new MailingAddressDataColumns("Id");
        public MailingAddressDataColumns Uuid => new MailingAddressDataColumns("Uuid");
        public MailingAddressDataColumns Cuid => new MailingAddressDataColumns("Cuid");
        public MailingAddressDataColumns Address => new MailingAddressDataColumns("Address");
        public MailingAddressDataColumns City => new MailingAddressDataColumns("City");
        public MailingAddressDataColumns PostalCode => new MailingAddressDataColumns("PostalCode");
        public MailingAddressDataColumns Country => new MailingAddressDataColumns("Country");
        public MailingAddressDataColumns Handle => new MailingAddressDataColumns("Handle");
        public MailingAddressDataColumns Key => new MailingAddressDataColumns("Key");
        public MailingAddressDataColumns CompositeKeyId => new MailingAddressDataColumns("CompositeKeyId");
        public MailingAddressDataColumns CompositeKey => new MailingAddressDataColumns("CompositeKey");
        public MailingAddressDataColumns CreatedBy => new MailingAddressDataColumns("CreatedBy");
        public MailingAddressDataColumns ModifiedBy => new MailingAddressDataColumns("ModifiedBy");
        public MailingAddressDataColumns Modified => new MailingAddressDataColumns("Modified");
        public MailingAddressDataColumns Deleted => new MailingAddressDataColumns("Deleted");
        public MailingAddressDataColumns Created => new MailingAddressDataColumns("Created");


		public Type DaoType => typeof(MailingAddressData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}