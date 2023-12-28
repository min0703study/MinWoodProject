using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
	#region Player
	public PlayerController Player { get; set; }
	#endregion
	
	#region GameData
	public TableData.Map CurrentMapData { get; set; }
	#endregion

	public TargetMarker TargetMarker { get; set; }
	

	protected override void init()
	{
		base.init();
		
		CurrentMapData = new TableData.Map();
		CurrentMapData.Id = 1;
		
		if(MyShopServerData.Instance.EnteredNPCDict.Count <= 0)
		{
			MapEntity orderPos = MapManager.Instance.Map.GetMapEntityGO<ExitDoor>();
			CustomerController npc = MapManager.Instance.Map.SpawnNPC(orderPos.CenterPos, 102010001);
			npc.EnterShop();
		}
		
		DOTween.Init();
	}

	public void GeneratePlayer(Vector3 position)
	{
		GameObject playerGO = ResourceManager.Instance.Instantiate("Player.prefab");
		playerGO.transform.position = position;
		DontDestroyOnLoad(playerGO);

		PlayerController playerController = Util.GetOrAddComponent<PlayerController>(playerGO);
		Instance.Player = playerController;

		MyShopServerData.Instance.AddSalesItem(103010004, 1);
		MyShopServerData.Instance.AddSalesItem(103010012, 6);

		GameObject targetMarkGO = ResourceManager.Instance.Instantiate("TargetMarker");
		DontDestroyOnLoad(targetMarkGO);
		TargetMarker = Util.GetOrAddComponent<TargetMarker>(targetMarkGO);
	}

}
