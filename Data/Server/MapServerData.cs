using System.Collections;
using System.Collections.Generic;
using ServerData;
using UnityEngine;

namespace ServerData
{
	public class MapData
	{
		public int MapDataId;
		public List<ServerData.Monster> Monsters { get; set; } = new List<ServerData.Monster>();
		public List<ServerData.DropItem> DropItems { get; set; } = new List<DropItem>();
		public List<ServerData.MapObject> InteractableObjects { get; set; } = new List<MapObject>();
	}

	public class Monster
	{
		public Vector3 Position;
		public int MonsterId;
		public int curHp;
	}
	
	public class MapObject
	{
		public Vector3 Position;
		public int MapObjectId;
	}
	
	public class DropItem
	{
		public Vector3 Position;
		public int ItemId;
	}
}

public class MapServerData : BaseTable<MapServerData>
{
	public int Id { get; private set; }
	
	public Dictionary<int, ServerData.MapData> MapDict = new Dictionary<int, ServerData.MapData>();
	
	override protected void init() 
	{
	}
	
	public ServerData.MapData GetMapDataById(int mapId) 
	{
		Dictionary<int, ServerData.MapData> dict = MapDict;
		
		dict.TryGetValue(mapId, out ServerData.MapData returnDict);
		return returnDict;
	}
	
	public void UpdateMapData(int mapId, List<ServerData.Monster> monsters, List<ServerData.DropItem> dropItems, List<ServerData.MapObject> mapObjects) 
	{
		Dictionary<int, ServerData.MapData> dict = MapDict;
		
		dict.TryGetValue(mapId, out ServerData.MapData findMapData);
		if(findMapData == null) 
		{
			findMapData = new ServerData.MapData();
			dict.Add(mapId, findMapData);
		}
		
		findMapData.DropItems = dropItems;
		findMapData.Monsters = monsters;
		findMapData.InteractableObjects = mapObjects;
	}
}
