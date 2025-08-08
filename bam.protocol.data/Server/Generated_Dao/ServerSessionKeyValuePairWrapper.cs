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
	public class ServerSessionKeyValuePairWrapper: Bam.Protocol.Data.Server.ServerSessionKeyValuePair, IHasUpdatedXrefCollectionProperties
	{
		public ServerSessionKeyValuePairWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public ServerSessionKeyValuePairWrapper(DaoRepository repository) : this()
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


        Bam.Protocol.Data.Server.ServerSession _serverSession;
		public override Bam.Protocol.Data.Server.ServerSession ServerSession
		{
			get
			{
				if (_serverSession == null)
				{
					_serverSession = (Bam.Protocol.Data.Server.ServerSession)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Protocol.Data.Server.ServerSession));
				}
				return _serverSession;
			}
			set
			{
				_serverSession = value;
			}
		}


	}
	// -- generated
}																								
