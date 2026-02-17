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
	public class DeviceDataWrapper: Bam.Protocol.Data.Common.DeviceData, IHasUpdatedXrefCollectionProperties
	{
		public DeviceDataWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public DeviceDataWrapper(DaoRepository repository) : this()
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

        System.Collections.Generic.List<Bam.Protocol.Data.Common.HostAddressData> _hostAddresses = null!;
		public override System.Collections.Generic.List<Bam.Protocol.Data.Common.HostAddressData> HostAddresses
		{
			get
			{
				if (_hostAddresses == null)
				{
					_hostAddresses = DaoRepository.ForeignKeyCollectionLoader<Bam.Protocol.Data.Common.DeviceData, Bam.Protocol.Data.Common.HostAddressData>(this).ToList();
				}
				return _hostAddresses;
			}
			set
			{
				_hostAddresses = value;
			}
		}        System.Collections.Generic.List<Bam.Protocol.Data.Common.NicData> _networkInterfaces = null!;
		public override System.Collections.Generic.List<Bam.Protocol.Data.Common.NicData> NetworkInterfaces
		{
			get
			{
				if (_networkInterfaces == null)
				{
					_networkInterfaces = DaoRepository.ForeignKeyCollectionLoader<Bam.Protocol.Data.Common.DeviceData, Bam.Protocol.Data.Common.NicData>(this).ToList();
				}
				return _networkInterfaces;
			}
			set
			{
				_networkInterfaces = value;
			}
		}



	}
	// -- generated
}																								
