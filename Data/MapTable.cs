using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using TableData;

namespace TableData
{
	public class MapMonsterSpawn
	{
		public int Id { get; set; }
		public int MapId { get; set; } 
		public int MonsterId { get; set; }
		public int MinCount { get; set; }
		public int MaxCount { get; set; } 
		
		public int SpawnSpotIndex { get; set; }
	}

	public class Map
	{
		public int Id { get; set; }
		
		public string AssetLabelRef {get; set;}
		
		public List<MapMonsterSpawn> MapMonsterSpawns;
	}
}

public class MapTable : BaseTable<MapTable>
{
	public List<TableData.Map> Maps {get; private set;} = new List<TableData.Map>();
	public Dictionary<int, TableData.Map> MapDict { get; private set; } = new Dictionary<int, TableData.Map>();
	
	public void Load(string mapAssetLabel, string monsterEntitySpawnAssetLabel)
	{
		TextAsset mapAsset = ResourceManager.Instance.Load<TextAsset>(mapAssetLabel);
		Maps = JsonConvert.DeserializeObject<List<TableData.Map>>(mapAsset.text);
		
		TextAsset monsterSpawnAsset = ResourceManager.Instance.Load<TextAsset>(monsterEntitySpawnAssetLabel);
		var mapMonsterSpawns = JsonConvert.DeserializeObject<List<TableData.MapMonsterSpawn>>(monsterSpawnAsset.text);
		
		foreach (TableData.Map map in Maps) 
		{
			map.MapMonsterSpawns = new List<TableData.MapMonsterSpawn>();
			foreach (var monsterSpawn in mapMonsterSpawns) 
			{
				if(monsterSpawn.MapId == map.Id)
					map.MapMonsterSpawns.Add(monsterSpawn);
			}
			
			MapDict.Add(map.Id, map);
		}
	}
	
	public TableData.Map GetMapTableDataById(int mapId) 
	{
		Dictionary<int, TableData.Map> dict = MapDict;
		dict.TryGetValue(mapId, out TableData.Map returnDict);
		return returnDict;
	}
}
