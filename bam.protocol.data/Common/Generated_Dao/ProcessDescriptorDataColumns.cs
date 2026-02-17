using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class ProcessDescriptorDataColumns: QueryFilter<ProcessDescriptorDataColumns>, IFilterToken
    {
        public ProcessDescriptorDataColumns() { }
        public ProcessDescriptorDataColumns(string columnName, bool isForeignKey = false)
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
        
		public ProcessDescriptorDataColumns KeyColumn => new ProcessDescriptorDataColumns("Id");

        public ProcessDescriptorDataColumns Id => new ProcessDescriptorDataColumns("Id");
        public ProcessDescriptorDataColumns Uuid => new ProcessDescriptorDataColumns("Uuid");
        public ProcessDescriptorDataColumns Cuid => new ProcessDescriptorDataColumns("Cuid");
        public ProcessDescriptorDataColumns InstanceIdentifier => new ProcessDescriptorDataColumns("InstanceIdentifier");
        public ProcessDescriptorDataColumns MachineId => new ProcessDescriptorDataColumns("MachineId");
        public ProcessDescriptorDataColumns HashAlgorithm => new ProcessDescriptorDataColumns("HashAlgorithm");
        public ProcessDescriptorDataColumns Hash => new ProcessDescriptorDataColumns("Hash");
        public ProcessDescriptorDataColumns MachineName => new ProcessDescriptorDataColumns("MachineName");
        public ProcessDescriptorDataColumns ProcessId => new ProcessDescriptorDataColumns("ProcessId");
        public ProcessDescriptorDataColumns StartTime => new ProcessDescriptorDataColumns("StartTime");
        public ProcessDescriptorDataColumns HasExited => new ProcessDescriptorDataColumns("HasExited");
        public ProcessDescriptorDataColumns ExitTime => new ProcessDescriptorDataColumns("ExitTime");
        public ProcessDescriptorDataColumns ExitCode => new ProcessDescriptorDataColumns("ExitCode");
        public ProcessDescriptorDataColumns FilePath => new ProcessDescriptorDataColumns("FilePath");
        public ProcessDescriptorDataColumns CommandLine => new ProcessDescriptorDataColumns("CommandLine");
        public ProcessDescriptorDataColumns Created => new ProcessDescriptorDataColumns("Created");


		public Type DaoType => typeof(ProcessDescriptorData);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}