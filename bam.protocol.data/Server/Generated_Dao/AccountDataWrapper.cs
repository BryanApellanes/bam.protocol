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
	public class AccountDataWrapper: Bam.Protocol.Data.Server.AccountData, IHasUpdatedXrefCollectionProperties
	{
		public AccountDataWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public AccountDataWrapper(DaoRepository repository) : this()
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





	}
	// -- generated
}																								
