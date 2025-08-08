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
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao;

namespace Bam.Protocol.Data.Server.Wrappers
{
	// generated
	[Serializable]
	public class ServerSessionWrapper: Bam.Protocol.Data.Server.ServerSession, IHasUpdatedXrefCollectionProperties
	{
		public ServerSessionWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public ServerSessionWrapper(DaoRepository repository) : this()
		{
			this.DaoRepository = repository;
		}

		[JsonIgnore]
		public DaoRepository DaoRepository { get; set; }

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

        System.Collections.Generic.List<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> _keyValues;
		public override System.Collections.Generic.List<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> KeyValues
		{
			get
			{
				if (_keyValues == null)
				{
					_keyValues = DaoRepository.ForeignKeyCollectionLoader<Bam.Protocol.Data.Server.ServerSession, Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(this).ToList();
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
