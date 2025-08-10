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
	public class PersonDataWrapper: Bam.Protocol.Data.Profile.PersonData, IHasUpdatedXrefCollectionProperties
	{
		public PersonDataWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public PersonDataWrapper(DaoRepository repository) : this()
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



        // left xref

// Left Xref property: Left -> PersonData ; Right -> OrganizationData
		List<Bam.Protocol.Data.Profile.OrganizationData> _organizationDatas;
		public override List<Bam.Protocol.Data.Profile.OrganizationData> Organizations
		{
			get
			{
				if(_organizationDatas == null || _organizationDatas.Count == 0)
				{
					var xref = new XrefDaoCollection<Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData, Bam.Protocol.Data.Profile.Dao.OrganizationData>(DaoRepository.GetDaoInstance(this), false);
					xref.Load(DaoRepository.Database);
					_organizationDatas = ((IEnumerable)xref).CopyAs<Bam.Protocol.Data.Profile.OrganizationData>().ToList();
					SetUpdatedXrefCollectionProperty("OrganizationDatas", this.GetType().GetProperty("Organizations"));					
				}

				return _organizationDatas;
			}
			set
			{
				_organizationDatas = value;
				SetUpdatedXrefCollectionProperty("OrganizationDatas", this.GetType().GetProperty("Organizations"));
			}
		}

        // right xref

// Right Xref property: Left -> GroupData ; Right -> PersonData
		List<Bam.Protocol.Data.Profile.GroupData> _groupDatas;
		public override List<Bam.Protocol.Data.Profile.GroupData> GroupDatas
		{
			get
			{
				if(_groupDatas == null || _groupDatas.Count == 0)
				{
					var xref = new XrefDaoCollection<Bam.Protocol.Data.Profile.Dao.GroupDataPersonData, Bam.Protocol.Data.Profile.Dao.GroupData>(DaoRepository.GetDaoInstance(this), false);
					xref.Load(DaoRepository.Database);
					_groupDatas = ((IEnumerable)xref).CopyAs<Bam.Protocol.Data.Profile.GroupData>().ToList();
					SetUpdatedXrefCollectionProperty("GroupDatas", this.GetType().GetProperty("GroupDatas"));					
				}

				return _groupDatas;
			}
			set
			{
				_groupDatas = value;
				SetUpdatedXrefCollectionProperty("GroupDatas", this.GetType().GetProperty("GroupDatas"));
			}
		}

	}
	// -- generated
}																								
