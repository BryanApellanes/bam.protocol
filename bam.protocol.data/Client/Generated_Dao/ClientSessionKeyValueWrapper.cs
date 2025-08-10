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
	public class ClientSessionKeyValueWrapper: Bam.Protocol.Data.Client.ClientSessionKeyValue, IHasUpdatedXrefCollectionProperties
	{
		public ClientSessionKeyValueWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public ClientSessionKeyValueWrapper(DaoRepository repository) : this()
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


        Bam.Protocol.Data.Client.ClientSessionData _clientSessionData;
		public override Bam.Protocol.Data.Client.ClientSessionData ClientSessionData
		{
			get
			{
				if (_clientSessionData == null)
				{
					_clientSessionData = (Bam.Protocol.Data.Client.ClientSessionData)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Protocol.Data.Client.ClientSessionData));
				}
				return _clientSessionData;
			}
			set
			{
				_clientSessionData = value;
			}
		}


	}
	// -- generated
}																								
