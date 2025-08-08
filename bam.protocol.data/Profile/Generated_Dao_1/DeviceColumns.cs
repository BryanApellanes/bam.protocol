using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class DeviceColumns: QueryFilter<DeviceColumns>, IFilterToken
    {
        public DeviceColumns() { }
        public DeviceColumns(string columnName, bool isForeignKey = false)
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
        
		public DeviceColumns KeyColumn => new DeviceColumns("Id");

        public DeviceColumns Id => new DeviceColumns("Id");
        public DeviceColumns Uuid => new DeviceColumns("Uuid");
        public DeviceColumns Cuid => new DeviceColumns("Cuid");
        public DeviceColumns HostName => new DeviceColumns("HostName");
        public DeviceColumns Key => new DeviceColumns("Key");
        public DeviceColumns CompositeKeyId => new DeviceColumns("CompositeKeyId");
        public DeviceColumns CompositeKey => new DeviceColumns("CompositeKey");
        public DeviceColumns CreatedBy => new DeviceColumns("CreatedBy");
        public DeviceColumns ModifiedBy => new DeviceColumns("ModifiedBy");
        public DeviceColumns Modified => new DeviceColumns("Modified");
        public DeviceColumns Deleted => new DeviceColumns("Deleted");
        public DeviceColumns Created => new DeviceColumns("Created");


		public Type DaoType => typeof(Device);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}