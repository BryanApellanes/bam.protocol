using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class AccountDataColumns: QueryFilter<AccountDataColumns>, IFilterToken
    {
        public AccountDataColumns() { }
        public AccountDataColumns(string columnName, bool isForeignKey = false)
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
        
		public AccountDataColumns KeyColumn => new AccountDataColumns("Id");

        public AccountDataColumns Id => new AccountDataColumns("Id");
        public AccountDataColumns Uuid => new AccountDataColumns("Uuid");
        public AccountDataColumns Cuid => new AccountDataColumns("Cuid");
        public AccountDataColumns PersonHandle => new AccountDataColumns("PersonHandle");
        public AccountDataColumns Key => new AccountDataColumns("Key");
        public AccountDataColumns CompositeKeyId => new AccountDataColumns("CompositeKeyId");
        public AccountDataColumns CompositeKey => new AccountDataColumns("CompositeKey");
        public AccountDataColumns CreatedBy => new AccountDataColumns("CreatedBy");
        public AccountDataColumns ModifiedBy => new AccountDataColumns("ModifiedBy");
        public AccountDataColumns Modified => new AccountDataColumns("Modified");
        public AccountDataColumns Deleted => new AccountDataColumns("Deleted");
        public AccountDataColumns Created => new AccountDataColumns("Created");


		public Type DaoType => typeof(AccountData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}