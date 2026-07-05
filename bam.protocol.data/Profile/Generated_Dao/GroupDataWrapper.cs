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
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Data.Profile.Dao;

namespace Bam.Protocol.Data.Profile.Wrappers
{
	// generated
	[Serializable]
	public class GroupDataWrapper: Bam.Protocol.Data.Profile.GroupData, IHasUpdatedXrefCollectionProperties
	{
		public GroupDataWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public GroupDataWrapper(DaoRepository repository) : this()
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



        // left xref

// Left Xref property: Left -> GroupData ; Right -> PersonData
		List<Bam.Protocol.Data.Profile.PersonData> _personDatas;
		public override List<Bam.Protocol.Data.Profile.PersonData> PersonDatas
		{
			get
			{
				if(_personDatas == null || _personDatas.Count == 0)
				{
					var xref = new XrefDaoCollection<Bam.Protocol.Data.Profile.Dao.GroupDataPersonData, Bam.Protocol.Data.Profile.Dao.PersonData>(DaoRepository.GetDaoInstance(this), false);
					xref.Load(DaoRepository.Database);
					_personDatas = ((IEnumerable)xref).CopyAs<Bam.Protocol.Data.Profile.PersonData>().ToList();
					SetUpdatedXrefCollectionProperty("PersonDatas", this.GetType().GetProperty("PersonDatas"));					
				}

				return _personDatas;
			}
			set
			{
				_personDatas = value;
				SetUpdatedXrefCollectionProperty("PersonDatas", this.GetType().GetProperty("PersonDatas"));
			}
		}


	}
	// -- generated
}																								
