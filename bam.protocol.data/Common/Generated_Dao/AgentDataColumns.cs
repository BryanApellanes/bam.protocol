using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class AgentDataColumns: QueryFilter<AgentDataColumns>, IFilterToken
    {
        public AgentDataColumns() { }
        public AgentDataColumns(string columnName, bool isForeignKey = false)
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
        
		public AgentDataColumns KeyColumn => new AgentDataColumns("Id");

        public AgentDataColumns Id => new AgentDataColumns("Id");
        public AgentDataColumns Uuid => new AgentDataColumns("Uuid");
        public AgentDataColumns Cuid => new AgentDataColumns("Cuid");
        public AgentDataColumns ActorDataId => new AgentDataColumns("ActorDataId");
        public AgentDataColumns DeviceDataId => new AgentDataColumns("DeviceDataId");
        public AgentDataColumns ProcessDescriptorDataId => new AgentDataColumns("ProcessDescriptorDataId");
        public AgentDataColumns Created => new AgentDataColumns("Created");


		public Type DaoType => typeof(AgentData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}