using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class AdditionalPropertyColumns: QueryFilter<AdditionalPropertyColumns>, IFilterToken
    {
        public AdditionalPropertyColumns() { }
        public AdditionalPropertyColumns(string columnName, bool isForeignKey = false)
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
        
		public AdditionalPropertyColumns KeyColumn => new AdditionalPropertyColumns("Id");

        public AdditionalPropertyColumns Id => new AdditionalPropertyColumns("Id");
        public AdditionalPropertyColumns Uuid => new AdditionalPropertyColumns("Uuid");
        public AdditionalPropertyColumns Cuid => new AdditionalPropertyColumns("Cuid");
        public AdditionalPropertyColumns Name => new AdditionalPropertyColumns("Name");
        public AdditionalPropertyColumns Value => new AdditionalPropertyColumns("Value");
        public AdditionalPropertyColumns Handle => new AdditionalPropertyColumns("Handle");
        public AdditionalPropertyColumns Key => new AdditionalPropertyColumns("Key");
        public AdditionalPropertyColumns CompositeKeyId => new AdditionalPropertyColumns("CompositeKeyId");
        public AdditionalPropertyColumns CompositeKey => new AdditionalPropertyColumns("CompositeKey");
        public AdditionalPropertyColumns CreatedBy => new AdditionalPropertyColumns("CreatedBy");
        public AdditionalPropertyColumns ModifiedBy => new AdditionalPropertyColumns("ModifiedBy");
        public AdditionalPropertyColumns Modified => new AdditionalPropertyColumns("Modified");
        public AdditionalPropertyColumns Deleted => new AdditionalPropertyColumns("Deleted");
        public AdditionalPropertyColumns Created => new AdditionalPropertyColumns("Created");


		public Type DaoType => typeof(AdditionalProperty);

		public string Operator { get; set; } = null!;

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}