using System.Collections.Generic;

using Newtonsoft.Json;
using UnityEngine;

namespace TableData
{
	public class EnvEntity
	{
		public int Id { get; set; }
		public string TextId { get; set;}
		public string Name {get; set;}
		public Define.EnvObjectType EnvObjectType {get; set;}
		public string AssetRefLabel {get; set;}
		public string SpriteName {get; set;}
		public string Desc {get; set;}
		public int MaxHp { get; set; }
		public int DropItemId { get; set; }
	}
}

public class EnvEntityTable: BaseTable<EnvEntityTable>
{
	public List<TableData.EnvEntity> EnvEntities {get; private set;} = new List<TableData.EnvEntity>();
	public Dictionary<int, TableData.EnvEntity> EnvEntityDict { get; private set; } = new Dictionary<int, TableData.EnvEntity>();
	protected override void init()
	{
		base.init();
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		EnvEntities = JsonConvert.DeserializeObject<List<TableData.EnvEntity>>(textAsset.text);
		
		foreach (var envEntity in EnvEntities)
			EnvEntityDict.Add(envEntity.Id, envEntity);
	}

	public TableData.EnvEntity GetEnvEntityById(int itemId) 
	{
		Dictionary<int, TableData.EnvEntity> dict = EnvEntityDict;
		return dict[itemId];
	}
}
