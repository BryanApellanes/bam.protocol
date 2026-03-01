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
using Bam.Protocol.Data.Client;
using Bam.Protocol.Data.Client.Dao;

namespace Bam.Protocol.Data.Client.Wrappers
{
	// generated
	[Serializable]
	public class ClientSessionDataWrapper: Bam.Protocol.Data.Client.ClientSessionData, IHasUpdatedXrefCollectionProperties
	{
		public ClientSessionDataWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public ClientSessionDataWrapper(DaoRepository repository) : this()
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

        System.Collections.Generic.List<Bam.Protocol.Data.Client.ClientSessionKeyValue> _keyValues;
		public override System.Collections.Generic.List<Bam.Protocol.Data.Client.ClientSessionKeyValue> KeyValues
		{
			get
			{
				if (_keyValues == null)
				{
					_keyValues = DaoRepository.ForeignKeyCollectionLoader<Bam.Protocol.Data.Client.ClientSessionData, Bam.Protocol.Data.Client.ClientSessionKeyValue>(this).ToList();
				}
				return _keyValues;
			}
			set
			{
				_keyValues = value;
			}
		}



	}
	// -- generated
}																								
