using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonDataColumns: QueryFilter<PersonDataColumns>, IFilterToken
    {
        public PersonDataColumns() { }
        public PersonDataColumns(string columnName, bool isForeignKey = false)
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
        
		public PersonDataColumns KeyColumn => new PersonDataColumns("Id");

        public PersonDataColumns Id => new PersonDataColumns("Id");
        public PersonDataColumns Uuid => new PersonDataColumns("Uuid");
        public PersonDataColumns Cuid => new PersonDataColumns("Cuid");
        public PersonDataColumns Phone => new PersonDataColumns("Phone");
        public PersonDataColumns Email => new PersonDataColumns("Email");
        public PersonDataColumns FirstName => new PersonDataColumns("FirstName");
        public PersonDataColumns LastName => new PersonDataColumns("LastName");
        public PersonDataColumns MiddleName => new PersonDataColumns("MiddleName");
        public PersonDataColumns Name => new PersonDataColumns("Name");
        public PersonDataColumns Handle => new PersonDataColumns("Handle");
        public PersonDataColumns Key => new PersonDataColumns("Key");
        public PersonDataColumns CompositeKeyId => new PersonDataColumns("CompositeKeyId");
        public PersonDataColumns CompositeKey => new PersonDataColumns("CompositeKey");
        public PersonDataColumns CreatedBy => new PersonDataColumns("CreatedBy");
        public PersonDataColumns ModifiedBy => new PersonDataColumns("ModifiedBy");
        public PersonDataColumns Modified => new PersonDataColumns("Modified");
        public PersonDataColumns Deleted => new PersonDataColumns("Deleted");
        public PersonDataColumns Created => new PersonDataColumns("Created");


		public Type DaoType => typeof(PersonData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}