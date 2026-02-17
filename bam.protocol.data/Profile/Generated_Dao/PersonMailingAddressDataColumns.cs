using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonMailingAddressDataColumns: QueryFilter<PersonMailingAddressDataColumns>, IFilterToken
    {
        public PersonMailingAddressDataColumns() { }
        public PersonMailingAddressDataColumns(string columnName, bool isForeignKey = false)
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
        
		public PersonMailingAddressDataColumns KeyColumn => new PersonMailingAddressDataColumns("Id");

        public PersonMailingAddressDataColumns Id => new PersonMailingAddressDataColumns("Id");
        public PersonMailingAddressDataColumns Uuid => new PersonMailingAddressDataColumns("Uuid");
        public PersonMailingAddressDataColumns Cuid => new PersonMailingAddressDataColumns("Cuid");
        public PersonMailingAddressDataColumns PersonHandle => new PersonMailingAddressDataColumns("PersonHandle");
        public PersonMailingAddressDataColumns MailingAddressHandle => new PersonMailingAddressDataColumns("MailingAddressHandle");
        public PersonMailingAddressDataColumns Created => new PersonMailingAddressDataColumns("Created");


		public Type DaoType => typeof(PersonMailingAddressData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}