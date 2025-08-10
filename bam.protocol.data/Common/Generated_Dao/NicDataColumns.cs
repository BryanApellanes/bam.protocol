using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class NicDataColumns: QueryFilter<NicDataColumns>, IFilterToken
    {
        public NicDataColumns() { }
        public NicDataColumns(string columnName, bool isForeignKey = false)
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
        
		public NicDataColumns KeyColumn => new NicDataColumns("Id");

        public NicDataColumns Id => new NicDataColumns("Id");
        public NicDataColumns Uuid => new NicDataColumns("Uuid");
        public NicDataColumns Cuid => new NicDataColumns("Cuid");
        public NicDataColumns AddressFamily => new NicDataColumns("AddressFamily");
        public NicDataColumns Address => new NicDataColumns("Address");
        public NicDataColumns MacAddress => new NicDataColumns("MacAddress");
        public NicDataColumns Key => new NicDataColumns("Key");
        public NicDataColumns CompositeKeyId => new NicDataColumns("CompositeKeyId");
        public NicDataColumns CompositeKey => new NicDataColumns("CompositeKey");
        public NicDataColumns CreatedBy => new NicDataColumns("CreatedBy");
        public NicDataColumns ModifiedBy => new NicDataColumns("ModifiedBy");
        public NicDataColumns Modified => new NicDataColumns("Modified");
        public NicDataColumns Deleted => new NicDataColumns("Deleted");
        public NicDataColumns Created => new NicDataColumns("Created");

        public NicDataColumns DeviceDataId => new NicDataColumns("DeviceDataId", true);
        public NicDataColumns MachineDataId => new NicDataColumns("MachineDataId", true);

		public Type DaoType => typeof(NicData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}