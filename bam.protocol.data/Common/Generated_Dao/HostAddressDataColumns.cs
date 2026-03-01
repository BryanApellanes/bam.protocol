using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class HostAddressDataColumns: QueryFilter<HostAddressDataColumns>, IFilterToken
    {
        public HostAddressDataColumns() { }
        public HostAddressDataColumns(string columnName, bool isForeignKey = false)
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
        
		public HostAddressDataColumns KeyColumn => new HostAddressDataColumns("Id");

        public HostAddressDataColumns Id => new HostAddressDataColumns("Id");
        public HostAddressDataColumns Uuid => new HostAddressDataColumns("Uuid");
        public HostAddressDataColumns Cuid => new HostAddressDataColumns("Cuid");
        public HostAddressDataColumns IpAddress => new HostAddressDataColumns("IpAddress");
        public HostAddressDataColumns AddressFamily => new HostAddressDataColumns("AddressFamily");
        public HostAddressDataColumns HostName => new HostAddressDataColumns("HostName");
        public HostAddressDataColumns Created => new HostAddressDataColumns("Created");

        public HostAddressDataColumns DeviceDataId => new HostAddressDataColumns("DeviceDataId", true);
        public HostAddressDataColumns MachineDataId => new HostAddressDataColumns("MachineDataId", true);

		public Type DaoType => typeof(HostAddressData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}