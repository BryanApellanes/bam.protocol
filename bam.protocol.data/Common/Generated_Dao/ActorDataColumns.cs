using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class ActorDataColumns: QueryFilter<ActorDataColumns>, IFilterToken
    {
        public ActorDataColumns() { }
        public ActorDataColumns(string columnName, bool isForeignKey = false)
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

                return _isForeignKey.Value;
            }
            set => _isForeignKey = value;
        }
        
		public ActorDataColumns KeyColumn => new ActorDataColumns("Id");

        public ActorDataColumns Id => new ActorDataColumns("Id");
        public ActorDataColumns Uuid => new ActorDataColumns("Uuid");
        public ActorDataColumns Cuid => new ActorDataColumns("Cuid");
        public ActorDataColumns Name => new ActorDataColumns("Name");
        public ActorDataColumns Handle => new ActorDataColumns("Handle");
        public ActorDataColumns Created => new ActorDataColumns("Created");


		public Type DaoType => typeof(ActorData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}