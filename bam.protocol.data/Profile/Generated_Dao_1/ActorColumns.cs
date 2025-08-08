using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ActorColumns: QueryFilter<ActorColumns>, IFilterToken
    {
        public ActorColumns() { }
        public ActorColumns(string columnName, bool isForeignKey = false)
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
        
		public ActorColumns KeyColumn => new ActorColumns("Id");

        public ActorColumns Id => new ActorColumns("Id");
        public ActorColumns Uuid => new ActorColumns("Uuid");
        public ActorColumns Cuid => new ActorColumns("Cuid");
        public ActorColumns DisplayName => new ActorColumns("DisplayName");
        public ActorColumns Handle => new ActorColumns("Handle");
        public ActorColumns Key => new ActorColumns("Key");
        public ActorColumns CompositeKeyId => new ActorColumns("CompositeKeyId");
        public ActorColumns CompositeKey => new ActorColumns("CompositeKey");
        public ActorColumns CreatedBy => new ActorColumns("CreatedBy");
        public ActorColumns ModifiedBy => new ActorColumns("ModifiedBy");
        public ActorColumns Modified => new ActorColumns("Modified");
        public ActorColumns Deleted => new ActorColumns("Deleted");
        public ActorColumns Created => new ActorColumns("Created");


		public Type DaoType => typeof(Actor);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}