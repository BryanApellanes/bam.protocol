using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionColumns: QueryFilter<ServerSessionColumns>, IFilterToken
    {
        public ServerSessionColumns() { }
        public ServerSessionColumns(string columnName, bool isForeignKey = false)
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
        
		public ServerSessionColumns KeyColumn => new ServerSessionColumns("Id");

        public ServerSessionColumns Id => new ServerSessionColumns("Id");
        public ServerSessionColumns Uuid => new ServerSessionColumns("Uuid");
        public ServerSessionColumns Cuid => new ServerSessionColumns("Cuid");
        public ServerSessionColumns SessionId => new ServerSessionColumns("SessionId");
        public ServerSessionColumns CompositeKeyId => new ServerSessionColumns("CompositeKeyId");
        public ServerSessionColumns CompositeKey => new ServerSessionColumns("CompositeKey");
        public ServerSessionColumns CreatedBy => new ServerSessionColumns("CreatedBy");
        public ServerSessionColumns ModifiedBy => new ServerSessionColumns("ModifiedBy");
        public ServerSessionColumns Modified => new ServerSessionColumns("Modified");
        public ServerSessionColumns Deleted => new ServerSessionColumns("Deleted");
        public ServerSessionColumns Created => new ServerSessionColumns("Created");


		public Type DaoType => typeof(ServerSession);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}