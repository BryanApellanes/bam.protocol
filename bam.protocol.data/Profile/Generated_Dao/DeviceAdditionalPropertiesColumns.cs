using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class DeviceAdditionalPropertiesColumns: QueryFilter<DeviceAdditionalPropertiesColumns>, IFilterToken
    {
        public DeviceAdditionalPropertiesColumns() { }
        public DeviceAdditionalPropertiesColumns(string columnName, bool isForeignKey = false)
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
        
		public DeviceAdditionalPropertiesColumns KeyColumn => new DeviceAdditionalPropertiesColumns("Id");

        public DeviceAdditionalPropertiesColumns Id => new DeviceAdditionalPropertiesColumns("Id");
        public DeviceAdditionalPropertiesColumns Uuid => new DeviceAdditionalPropertiesColumns("Uuid");
        public DeviceAdditionalPropertiesColumns Cuid => new DeviceAdditionalPropertiesColumns("Cuid");
        public DeviceAdditionalPropertiesColumns DeviceHandle => new DeviceAdditionalPropertiesColumns("DeviceHandle");
        public DeviceAdditionalPropertiesColumns AdditionalPropertyHandle => new DeviceAdditionalPropertiesColumns("AdditionalPropertyHandle");
        public DeviceAdditionalPropertiesColumns Key => new DeviceAdditionalPropertiesColumns("Key");
        public DeviceAdditionalPropertiesColumns CompositeKeyId => new DeviceAdditionalPropertiesColumns("CompositeKeyId");
        public DeviceAdditionalPropertiesColumns CompositeKey => new DeviceAdditionalPropertiesColumns("CompositeKey");
        public DeviceAdditionalPropertiesColumns CreatedBy => new DeviceAdditionalPropertiesColumns("CreatedBy");
        public DeviceAdditionalPropertiesColumns ModifiedBy => new DeviceAdditionalPropertiesColumns("ModifiedBy");
        public DeviceAdditionalPropertiesColumns Modified => new DeviceAdditionalPropertiesColumns("Modified");
        public DeviceAdditionalPropertiesColumns Deleted => new DeviceAdditionalPropertiesColumns("Deleted");
        public DeviceAdditionalPropertiesColumns Created => new DeviceAdditionalPropertiesColumns("Created");


		public Type DaoType => typeof(DeviceAdditionalProperties);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}