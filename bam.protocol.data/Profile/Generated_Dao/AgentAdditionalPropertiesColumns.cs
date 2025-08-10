using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class AgentAdditionalPropertiesColumns: QueryFilter<AgentAdditionalPropertiesColumns>, IFilterToken
    {
        public AgentAdditionalPropertiesColumns() { }
        public AgentAdditionalPropertiesColumns(string columnName, bool isForeignKey = false)
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
        
		public AgentAdditionalPropertiesColumns KeyColumn => new AgentAdditionalPropertiesColumns("Id");

        public AgentAdditionalPropertiesColumns Id => new AgentAdditionalPropertiesColumns("Id");
        public AgentAdditionalPropertiesColumns Uuid => new AgentAdditionalPropertiesColumns("Uuid");
        public AgentAdditionalPropertiesColumns Cuid => new AgentAdditionalPropertiesColumns("Cuid");
        public AgentAdditionalPropertiesColumns AgentHandle => new AgentAdditionalPropertiesColumns("AgentHandle");
        public AgentAdditionalPropertiesColumns AdditionalPropertyHandle => new AgentAdditionalPropertiesColumns("AdditionalPropertyHandle");
        public AgentAdditionalPropertiesColumns Key => new AgentAdditionalPropertiesColumns("Key");
        public AgentAdditionalPropertiesColumns CompositeKeyId => new AgentAdditionalPropertiesColumns("CompositeKeyId");
        public AgentAdditionalPropertiesColumns CompositeKey => new AgentAdditionalPropertiesColumns("CompositeKey");
        public AgentAdditionalPropertiesColumns CreatedBy => new AgentAdditionalPropertiesColumns("CreatedBy");
        public AgentAdditionalPropertiesColumns ModifiedBy => new AgentAdditionalPropertiesColumns("ModifiedBy");
        public AgentAdditionalPropertiesColumns Modified => new AgentAdditionalPropertiesColumns("Modified");
        public AgentAdditionalPropertiesColumns Deleted => new AgentAdditionalPropertiesColumns("Deleted");
        public AgentAdditionalPropertiesColumns Created => new AgentAdditionalPropertiesColumns("Created");


		public Type DaoType => typeof(AgentAdditionalProperties);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}