using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Linq;
using Bam;
using Bam.Data;
using Bam.Data.Repositories;
using Newtonsoft.Json;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Common.Dao;

namespace Bam.Protocol.Data.Common.Wrappers
{
	// generated
	[Serializable]
	public class NicDataWrapper: Bam.Protocol.Data.Common.NicData, IHasUpdatedXrefCollectionProperties
	{
		public NicDataWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public NicDataWrapper(DaoRepository repository) : this()
		{
			this.DaoRepository = repository;
		}

		[JsonIgnore]
		public DaoRepository DaoRepository { get; set; } = null!;

		[JsonIgnore]
		public Dictionary<string, PropertyInfo> UpdatedXrefCollectionProperties { get; set; }

		protected void SetUpdatedXrefCollectionProperty(string propertyName, PropertyInfo correspondingProperty)
		{
			if(UpdatedXrefCollectionProperties != null && !UpdatedXrefCollectionProperties.ContainsKey(propertyName))
			{
				UpdatedXrefCollectionProperties.Add(propertyName, correspondingProperty);				
			}
			else if(UpdatedXrefCollectionProperties != null)
			{
				UpdatedXrefCollectionProperties[propertyName] = correspondingProperty;				
			}
		}


        Bam.Protocol.Data.Common.DeviceData _deviceData;
		public override Bam.Protocol.Data.Common.DeviceData DeviceData
		{
			get
			{
				if (_deviceData == null)
				{
					_deviceData = (Bam.Protocol.Data.Common.DeviceData)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Protocol.Data.Common.DeviceData));
				}
				return _deviceData;
			}
			set
			{
				_deviceData = value;
			}
		}        Bam.Protocol.Data.Common.MachineData _machineData;
		public override Bam.Protocol.Data.Common.MachineData MachineData
		{
			get
			{
				if (_machineData == null)
				{
					_machineData = (Bam.Protocol.Data.Common.MachineData)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Protocol.Data.Common.MachineData));
				}
				return _machineData;
			}
			set
			{
				_machineData = value;
			}
		}


	}
	// -- generated
}																								
