using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

using Newtonsoft.Json;
using UnityEngine;

namespace TableData
{
	public class Monster
	{
		public int Id { get; set; }
		public string TextId { get; set;}
		public string MonsterName {get; set;}
		public string PrefabAssetLabel {get; set;}
		public string Desc {get; set;}
		public int MaxHp { get; set; }
		public string SpriteName { get; set; }
		public int AtkPower { get; set; }
		public int DropItemId { get; set; }
	}
}

public class MonsterTable: BaseTable<MonsterTable>
{
	public List<TableData.Monster> Monsters {get; private set;} = new List<TableData.Monster>();
	public Dictionary<int, TableData.Monster> MonsterDict { get; private set; } = new Dictionary<int, TableData.Monster>();
	protected override void init()
	{
		base.init();
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		Monsters = JsonConvert.DeserializeObject<List<TableData.Monster>>(textAsset.text);
		
		foreach (TableData.Monster monster in Monsters)
			MonsterDict.Add(monster.Id, monster);
	}

	public TableData.Monster GetMonsterDataById(int monsterId) 
	{
		Dictionary<int, TableData.Monster> dict = MonsterDict;
		return dict[monsterId];
	}
}
