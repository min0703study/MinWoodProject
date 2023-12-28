using System.Collections.Generic;
using UnityEngine;


public class MapManager : BaseManager<MapManager>
{
	public MapController Map { get; set; }

	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("Map_Root");
			if (root == null)
				root = new GameObject { name = "Map_Root" };
			return root;
		}
	}

	protected override void init()
	{
		base.init();
	}

	public void LoadOrGenerateMap(Define.Map mapType, int mapId = 1)
	{		
		// 개발용으로 넣어논 맵 제거
		Util.DestroyChilds(Root);
				
		//map 생성
		TableData.Map mapTableData = MapTable.Instance.GetMapTableDataById(mapId);
		GameObject mapGO = ResourceManager.Instance.Instantiate(mapTableData.AssetLabelRef, Root.transform);
		MapController map = Util.GetOrAddComponent<MapController>(mapGO);
		Map = map;
	
		//Load or Generate
		ServerData.MapData mapData = MapServerData.Instance.GetMapDataById(mapId);
		if(mapData != null) 
		{
			Map.LoadMap(mapId, mapData.Monsters, mapData.DropItems, mapData.InteractableObjects);
		} else 
		{
			Map.SetForTheFirstTime(mapId);
		}
		
		Map.IntoPlayer();
	}
	
	public void ClearMap()
	{
		Map.gameObject.transform.parent = null;
		Map.gameObject.SetActive(false);
	}
	
	public void SaveMapData()
	{				
		List<ServerData.Monster> monsters = new List<ServerData.Monster>();
		List<ServerData.DropItem> dropItems = new List<ServerData.DropItem>();
		List<ServerData.MapObject> mapObjects = new List<ServerData.MapObject>();
		
		foreach (MonsterController monster in Map.Monsters) 
		{
			monsters.Add(new ServerData.Monster()
			{
				MonsterId = monster.MonsterTableData.Id,
				Position = monster.CenterPos,
			});
		}
		
		foreach (DropItemController dropItem in Map.DropItems) 
		{
			dropItems.Add(new ServerData.DropItem()
			{
				ItemId = dropItem.ItemData.Id,
				Position = dropItem.transform.position,
			});
		}
		
		foreach (MapEntity mapEntity in Map.MapEntities) 
		{
			//if(mapObject.EnvObjectData == null)
				//continue;
				
			// mapObjects.Add(new ServerData.MapObject()
			// {
			// 	MapObjectId = mapObject.EnvObjectData.Id,
			// 	Position = mapObject.transform.position,
			// });
		}
		
		MapServerData.Instance.UpdateMapData(Map.MapData.Id, monsters, dropItems, mapObjects);
	}
	
	public void ChangeMap()
	{				
		SaveMapData();
		ClearMap();
	}
	
	
}