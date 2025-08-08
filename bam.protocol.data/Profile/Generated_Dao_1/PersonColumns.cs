using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonColumns: QueryFilter<PersonColumns>, IFilterToken
    {
        public PersonColumns() { }
        public PersonColumns(string columnName, bool isForeignKey = false)
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
        
		public PersonColumns KeyColumn => new PersonColumns("Id");

        public PersonColumns Id => new PersonColumns("Id");
        public PersonColumns Uuid => new PersonColumns("Uuid");
        public PersonColumns Cuid => new PersonColumns("Cuid");
        public PersonColumns Email => new PersonColumns("Email");
        public PersonColumns Phone => new PersonColumns("Phone");
        public PersonColumns FirstName => new PersonColumns("FirstName");
        public PersonColumns LastName => new PersonColumns("LastName");
        public PersonColumns MiddleName => new PersonColumns("MiddleName");
        public PersonColumns DisplayName => new PersonColumns("DisplayName");
        public PersonColumns Handle => new PersonColumns("Handle");
        public PersonColumns Key => new PersonColumns("Key");
        public PersonColumns CompositeKeyId => new PersonColumns("CompositeKeyId");
        public PersonColumns CompositeKey => new PersonColumns("CompositeKey");
        public PersonColumns CreatedBy => new PersonColumns("CreatedBy");
        public PersonColumns ModifiedBy => new PersonColumns("ModifiedBy");
        public PersonColumns Modified => new PersonColumns("Modified");
        public PersonColumns Deleted => new PersonColumns("Deleted");
        public PersonColumns Created => new PersonColumns("Created");


		public Type DaoType => typeof(Person);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}