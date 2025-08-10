using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class MachineDataColumns: QueryFilter<MachineDataColumns>, IFilterToken
    {
        public MachineDataColumns() { }
        public MachineDataColumns(string columnName, bool isForeignKey = false)
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
        
		public MachineDataColumns KeyColumn => new MachineDataColumns("Id");

        public MachineDataColumns Id => new MachineDataColumns("Id");
        public MachineDataColumns Uuid => new MachineDataColumns("Uuid");
        public MachineDataColumns Cuid => new MachineDataColumns("Cuid");
        public MachineDataColumns Name => new MachineDataColumns("Name");
        public MachineDataColumns DnsName => new MachineDataColumns("DnsName");
        public MachineDataColumns Key => new MachineDataColumns("Key");
        public MachineDataColumns CompositeKeyId => new MachineDataColumns("CompositeKeyId");
        public MachineDataColumns CompositeKey => new MachineDataColumns("CompositeKey");
        public MachineDataColumns CreatedBy => new MachineDataColumns("CreatedBy");
        public MachineDataColumns ModifiedBy => new MachineDataColumns("ModifiedBy");
        public MachineDataColumns Modified => new MachineDataColumns("Modified");
        public MachineDataColumns Deleted => new MachineDataColumns("Deleted");
        public MachineDataColumns Created => new MachineDataColumns("Created");


		public Type DaoType => typeof(MachineData);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}