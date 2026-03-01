using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class DeviceDataColumns: QueryFilter<DeviceDataColumns>, IFilterToken
    {
        public DeviceDataColumns() { }
        public DeviceDataColumns(string columnName, bool isForeignKey = false)
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
        
		public DeviceDataColumns KeyColumn => new DeviceDataColumns("Id");

        public DeviceDataColumns Id => new DeviceDataColumns("Id");
        public DeviceDataColumns Uuid => new DeviceDataColumns("Uuid");
        public DeviceDataColumns Cuid => new DeviceDataColumns("Cuid");
        public DeviceDataColumns ProcessDescriptorId => new DeviceDataColumns("ProcessDescriptorId");
        public DeviceDataColumns Handle => new DeviceDataColumns("Handle");
        public DeviceDataColumns Name => new DeviceDataColumns("Name");
        public DeviceDataColumns DnsName => new DeviceDataColumns("DnsName");
        public DeviceDataColumns Created => new DeviceDataColumns("Created");


		public Type DaoType => typeof(DeviceData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}