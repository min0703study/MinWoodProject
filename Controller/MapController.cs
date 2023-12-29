using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

[Serializable]
public class MonsetSpawnSpot
{
	public int Index;
	public Transform SpotTransform;
}

public class MapController : BaseController
{
	public TableData.Map MapData { get; set; }
	
	[SerializeField]
	List<MonsetSpawnSpot> MonsterSpawnSpots = new List<MonsetSpawnSpot>();

	public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
	public HashSet<DropItemController> DropItems { get; } = new HashSet<DropItemController>();
	public HashSet<MapEntity> MapEntities { get; } = new HashSet<MapEntity>();
	public HashSet<MonsterController> ChasingMonsters { get; } = new HashSet<MonsterController>();
	
	protected Tilemap floorTileMap, collisionTileMap;
	protected Tilemap monsterSpawnTilemap;
	protected EntryPos entryPos;
	public HashSet<Vector3Int> ObstaclePositions { get; set; } = new HashSet<Vector3Int>();

	public Action OnClearDungon;
	
	protected override void Init()
	{
		base.Init();
		InitTilemap();
	}

	protected virtual void InitTilemap()
	{
		// Get tilemaps in children.
		Tilemap[] tilemaps = gameObject.GetComponentsInChildren<Tilemap>();
		foreach (Tilemap tilemap in tilemaps)
		{
			if (tilemap.gameObject.tag == "FloorTilemap")
			{
				floorTileMap = tilemap;
			}
			else if (tilemap.gameObject.tag == "CollisionTilemap")
			{
				collisionTileMap = tilemap;
				tilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
			}
			else if (tilemap.gameObject.tag == "MonsterSpawnTilemap")
			{
				monsterSpawnTilemap = tilemap;
			}
		}

		BoxCollider2D[] colliders = gameObject.GetComponentsInChildren<BoxCollider2D>();
		foreach (Collider2D collider in colliders)
		{
			// Box Collider 2D의 Bounds를 가져옵니다.
			Bounds colliderBounds = collider.bounds;

			// 최소 및 최대 Bounds의 위치를 Tilemap의 Cell Position으로 변환합니다.
			Vector3Int minCellPosition = floorTileMap.WorldToCell(colliderBounds.min);
			Vector3Int maxCellPosition = floorTileMap.WorldToCell(colliderBounds.max);
			
			//Bounds 내의 모든 Tilemap Cell Position을 반복합니다.
			for (int x = minCellPosition.x; x <= maxCellPosition.x; x++)
			{
				for (int y = minCellPosition.y; y <= maxCellPosition.y; y++)
				{
					ObstaclePositions.Add(new Vector3Int(x, y, 0));
				}
			}
		}

		MapEntity[] mapEntities = gameObject.GetComponentsInChildren<MapEntity>();
		foreach (MapEntity mapEntity in mapEntities)
		{
			MapEntities.Add(mapEntity);
		}

		EntryPos entryPos = gameObject.GetComponentInChildren<EntryPos>();
		if (entryPos != null)
		{
			this.entryPos = entryPos;
		}
	}

	public void LoadMap(int mapId, List<ServerData.Monster> monsters, List<ServerData.DropItem> items, List<ServerData.MapObject> mapObjects)
	{
		this.MapData = MapTable.Instance.GetMapTableDataById(mapId);

		foreach (var monster in monsters)
		{
			GenerateMonster(monster.Position, monster.MonsterId);
		}

		foreach (var item in items)
		{
			GenerateDropItem(item.Position, item.ItemId);
		}

		foreach (var mapObject in mapObjects)
		{
			SpawnEnvEntity(mapObject.Position, mapObject.MapObjectId);
		}
	}

	public virtual void SetForTheFirstTime(int mapId)
	{
		this.MapData = MapTable.Instance.GetMapTableDataById(mapId);

		foreach (Vector3Int position in collisionTileMap.cellBounds.allPositionsWithin)
		{
			Tile tile = collisionTileMap.GetTile<Tile>(position);
			if (tile != null)
			{
				ObstaclePositions.Add(position);
			}
		}

		if (this.MapData.MapMonsterSpawns.Count > 0)
		{
			Dictionary<int, Transform> monsterSpawnSpotDict = new Dictionary<int, Transform>();
			foreach (var monsterSpawnSpot in MonsterSpawnSpots)
			{
				monsterSpawnSpotDict.Add(monsterSpawnSpot.Index, monsterSpawnSpot.SpotTransform);
			}

			foreach (var mapMonsterSpawn in MapData.MapMonsterSpawns)
			{
				int spawnCount = 0;
				while (spawnCount < mapMonsterSpawn.MinCount)
				{
					if(monsterSpawnSpotDict.TryGetValue(mapMonsterSpawn.SpawnSpotIndex, out Transform spotTransform)) 
					{
						Vector3 randomPosition = new Vector3(Random.Range(-1.0f, 1.0f) + spotTransform.position.x, Random.Range(-1.0f, 1.0f) + spotTransform.position.y, 0);
						GenerateMonster(randomPosition, mapMonsterSpawn.MonsterId);
					}
					
					spawnCount++;
				}
			}
		}
	}

	public void IntoPlayer()
	{
		var player = GameManager.Instance.Player;
		player.SetPosition(entryPos.transform.position);
	}

	public void GenerateMonster(Vector3 position, int monsterId)
	{
		TableData.Monster monsterData = MonsterTable.Instance.GetMonsterDataById(monsterId);
		GameObject monsterGO = ResourceManager.Instance.Instantiate(monsterData.PrefabAssetLabel, gameObject.transform);
		MonsterController monsterController = Util.GetOrAddComponent<MonsterController>(monsterGO);

		monsterController.transform.position = position;
		monsterController.SetInfo(monsterData);
		monsterController.DestroyAction += DestroyMonster;
		monsterController.OnChasePlayer += MonsterOnChasePlayer;
		monsterController.StoppedChasePlayer += MonsterStoppedChasePlayer;
		Monsters.Add(monsterController);
	}

	public void GenerateDropItem(Vector3 position, int itemId)
	{
		GameObject dropItemGO = ResourceManager.Instance.Instantiate("DropItem.prefab", gameObject.transform);
		DropItemController dropItemController = Util.GetOrAddComponent<DropItemController>(dropItemGO);

		dropItemController.transform.position = position;
		dropItemController.SetInfo(itemId);
		dropItemController.DestroyAction += DestroyDropItem;
		dropItemController.StartDropping();
		DropItems.Add(dropItemController);
	}

	public void SpawnEnvEntity(Vector3 position, int envEntityTableId)
	{
		TableData.EnvEntity envObjectData = EnvEntityTable.Instance.GetEnvEntityById(envEntityTableId);
		GameObject envEntityGO = ResourceManager.Instance.Instantiate(envObjectData.AssetRefLabel, gameObject.transform);
		EnvEntity envEntity = Util.GetOrAddComponent<EnvEntity>(envEntityGO);

		envEntity.SetInfo(envObjectData.Id);
		envEntity.gameObject.transform.position = position;

		MapEntities.Add(envEntity);
	}

	public CustomerController SpawnNPC(Vector3 position, int npcId)
	{
		var npcTableData = NPCTable.Instance.GetNPCTableDataById(npcId);
		var npcGO = ResourceManager.Instance.Instantiate(npcTableData.PrefabAddress);
		npcGO.transform.position = position;

		CustomerController npcController = Util.GetOrAddComponent<CustomerController>(npcGO);
		npcController.SetInfo(npcId);
		
		MapEntities.Add(npcController);
		
		return npcController;
	}
	
	public CatController SpawnCat(Vector3 position)
	{
		//var npcTableData = NPCTable.Instance.GetNPCTableDataById(npcId);
		var catGO = ResourceManager.Instance.Instantiate("Cat");
		catGO.transform.position = position;

		CatController catController = Util.GetOrAddComponent<CatController>(catGO);
		MapEntities.Add(catController);
		
		return catController;
	}
	
	public void DestroyNPC(CustomerController customerController)
	{
		MapEntities.Remove(customerController);
		ResourceManager.Instance.Destroy(customerController.gameObject);
	}
	public void MonsterOnChasePlayer(MonsterController monster) 
	{
		ChasingMonsters.Add(monster);
	}
	
	public void MonsterStoppedChasePlayer(MonsterController monster) 
	{
		ChasingMonsters.Remove(monster);
	}
	
	public virtual void DestroyMonster(MonsterController monster)
	{
		if(ChasingMonsters.Contains(monster)) 
		{
			ChasingMonsters.Remove(monster);
		}

		SetTargetNeareastMonster();
		
		Monsters.Remove(monster);
		ResourceManager.Instance.Destroy(monster.gameObject);
		
		if(Monsters.Count <= 0) 
		{
			OnClearDungon?.Invoke();
		}
	}

	public virtual void DestroyDropItem(DropItemController dropItemController)
	{
		DropItems.Remove(dropItemController);
		ResourceManager.Instance.Destroy(dropItemController.gameObject);
	}

	public virtual void DestroyMapEntity(MapEntity mapEntity)
	{
		MapEntities.Remove(mapEntity);
		ResourceManager.Instance.Destroy(mapEntity.gameObject);
	}

	public MapEntity GetNeareastInteractableEntity()
	{
		MapEntity neareastEntity = null;
		float curNeareastEntityRange = 100.0f;
		
		foreach (var mapEntity in MapEntities) 
		{
			if(mapEntity is IInteractable) 
			{
				if(((IInteractable)mapEntity).InteractionLocked) 
					continue;
					
				var pointsVector = mapEntity.CenterPos - GameManager.Instance.Player.CenterPos;
				if(pointsVector.magnitude > GameManager.Instance.Player.InteractableRange) 
				{
					continue;
				}
				
				if(pointsVector.magnitude < curNeareastEntityRange) 
				{
					neareastEntity = mapEntity;
					curNeareastEntityRange = pointsVector.sqrMagnitude;
			}	
			} 
		}

		return neareastEntity;
	}
	public MapEntity GetNeareastTree()
	{
		MapEntity neareastEntity = null;
		float curNeareastEntityRange = 100.0f;
		
		foreach (var mapEntity in MapEntities) 
		{
			if(mapEntity is Tree) 
			{
				if(((IInteractable)mapEntity).InteractionLocked) 
					continue;
					
				var pointsVector = mapEntity.CenterPos - GameManager.Instance.Player.CenterPos;
				if(pointsVector.magnitude < curNeareastEntityRange) 
				{
					neareastEntity = mapEntity;
					curNeareastEntityRange = pointsVector.sqrMagnitude;
			}	
			} 
		}

		return neareastEntity;
	}
	
	public MapEntity GetNeareastBuilding<T>()
	{
		MapEntity neareastEntity = null;
		float curNeareastEntityRange = 10000.0f;
		
		foreach (var mapEntity in MapEntities) 
		{
			if(mapEntity is T) 
			{
				if(((IInteractable)mapEntity).InteractionLocked) 
					continue;
					
				var pointsVector = mapEntity.CenterPos - GameManager.Instance.Player.CenterPos;
				if(pointsVector.magnitude < curNeareastEntityRange) 
				{
					neareastEntity = mapEntity;
					curNeareastEntityRange = pointsVector.sqrMagnitude;
				}	
			} 
		}

		return neareastEntity;
	}
	
	
	public void SetTargetNeareastInteractableEntity()
	{
		MapEntity neareastEntity = GetNeareastInteractableEntity();
		GameManager.Instance.TargetMarker.SetTargetObject(neareastEntity);
	}
	
	public void SetTargetNeareastMonster()
	{
		MapEntity neareastEntity = null;
		float curNeareastEntityRange = 100.0f;
		
		foreach (var mapEntity in ChasingMonsters) 
		{
			var pointsVector = mapEntity.CenterPos - GameManager.Instance.Player.CenterPos;
			if(pointsVector.magnitude < curNeareastEntityRange) 
			{
				neareastEntity = mapEntity;
				curNeareastEntityRange = pointsVector.sqrMagnitude;
			}	
		}
		
		if(neareastEntity != null) 
		{
			GameManager.Instance.TargetMarker.SetTargetObject(neareastEntity);
		}
	}
	
	public List<MonsterController> GetNearestMonsters(int count = 1, int distanceThreshold = 0)
	{
		List<MonsterController> monsterList = Monsters.OrderBy(monster => (GameManager.Instance.Player.CenterPos - monster.CenterPos).sqrMagnitude).ToList();

		if (distanceThreshold > 0)
			monsterList = monsterList.Where(monster => (GameManager.Instance.Player.CenterPos - monster.CenterPos).magnitude > distanceThreshold).ToList();

		int min = Mathf.Min(count, monsterList.Count);

		List<MonsterController> nearestMonsters = monsterList.Take(min).ToList();

		if (nearestMonsters.Count == 0) return null;

		while (nearestMonsters.Count < count)
		{
			nearestMonsters.Add(nearestMonsters.Last());
		}

		return nearestMonsters;
	}
	
	public MapEntity GetMapEntityGO<T>() where T: MapEntity
	{
		List<MapEntity> interactables = MapEntities.Where(interactable =>interactable is T).ToList();

		if (interactables.Count <= 0)
			return null;

		return interactables.First();
	}
	
	public List<T> GetMapEntityGOList<T>() where T: MapEntity
	{
		List<T> interactables = MapEntities.Where(interactable => interactable is T).Select(interactables => interactables as T).ToList();

		if (interactables.Count <= 0)
			return null;

		return interactables;
	}
	
	public List<Vector3> FindPathToPosition(Vector3 startPosition, Vector3 targetPosition)
	{		
		var startCellPosition = floorTileMap.WorldToCell(startPosition);
		var targetCellPosition = floorTileMap.WorldToCell(targetPosition);
		List<Vector3Int> findCellPositions = Astar.FindPath(startCellPosition, targetCellPosition);
		
		if(findCellPositions != null) 
		{
			List<Vector3> findWorldPositions = new List<Vector3>();
			findCellPositions.ForEach(convertPath => findWorldPositions.Add(floorTileMap.CellToWorld(convertPath) + new Vector3(0.5f, 0.5f, 0)));
			return findWorldPositions;
		}

		return null;
	}
	
	public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
	{
		string prefabName;
		if (isCritical)
			prefabName = "CriticalDamageFont";
		else
			prefabName = "DamageFont";

		GameObject go = ResourceManager.Instance.Instantiate(prefabName, pooling: true);
		DamageFont damageText = Util.GetOrAddComponent<DamageFont>(go);
		damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
	}

	public void ShowShootEffect(Vector2 pos, Transform parent)
	{
		string prefabAssetLabel = "ShootEffect";

		GameObject go = ResourceManager.Instance.Instantiate(prefabAssetLabel, pooling: true);
		VisualEffect visualEffect = Util.GetOrAddComponent<VisualEffect>(go);
		visualEffect.SetInfo(pos, parent);
	}
	
	public void ShowBrokenEffect(Vector2 pos, Transform parent)
	{
		string prefabAssetLabel = "BrokenEffect";

		GameObject go = ResourceManager.Instance.Instantiate(prefabAssetLabel, pooling: true);
		VisualEffect visualEffect = Util.GetOrAddComponent<VisualEffect>(go);
		visualEffect.SetInfo(pos, parent);
	}
	
	public void ShowBuildEffect(Vector2 pos, Transform parent)
	{
		string prefabAssetLabel = "BuildEffect";

		GameObject go = ResourceManager.Instance.Instantiate(prefabAssetLabel, pooling: true);
		go.transform.position = pos;
	}

	public void ShowSkillEffect(Vector2 pos, Transform parent)
	{
		string prefabAssetLabel = "SkillEffect";

		GameObject go = ResourceManager.Instance.Instantiate(prefabAssetLabel, pooling: true);
		go.transform.position = pos;
	}
	
}