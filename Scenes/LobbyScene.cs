using System;
using System.Collections;

using Cinemachine;
using UnityEngine;

using Random = UnityEngine.Random;

public class LobbyScene : BaseScene
{	
	public CinemachineVirtualCamera MainVirtualCamera;
	public Camera MainCamera;
	
	protected override void Init()
	{
		base.Init();
		SceneType = Define.Scene.LobbyScene;

		// Open Scene UI 
		UIManager.Instance.OpenSceneUI<UI_LobbyScene>();
		UIManager.Instance.OpenJoystickUI<UI_Joystick>();

		// Open Map
		MapManager.Instance.LoadOrGenerateMap(Define.Map.Home, 106010001);
		
		CinemachineManager.Instance.MainCamera = MainCamera;
		CinemachineManager.Instance.AddVirtualCamera("Main", MainVirtualCamera);
		CinemachineManager.Instance.ActivateCamera("Main");
		CinemachineManager.Instance.SetActiveCameraTarget(GameManager.Instance.Player.transform);
		
		SoundManager.Instance.Play(SoundManager.SoundType.Bgm, "Sound_LobbyBGM");

		MapEntity orderPos = MapManager.Instance.Map.GetMapEntityGO<CatCarpet>();
		MapManager.Instance.Map.SpawnCat(orderPos.CenterPos);
	}
	
	public void Update() 
	{
		if(!MyShopServerData.Instance.VisitCustomer) {} 
		else if(MyShopServerData.Instance.EnteredNPCDict.Count > 0) {}
		else if(OrderListServerData.Instance.Orders.Count >= 1) {}
		else
		{	
			MapEntity orderPos = MapManager.Instance.Map.GetMapEntityGO<ExitDoor>();
			CustomerController customer = MapManager.Instance.Map.SpawnNPC(orderPos.CenterPos, 102010001);
			StartCoroutine(CoStartVisitCustomer(customer));
		}
	}
	
	IEnumerator CoStartVisitCustomer(CustomerController customer) 
	{	
		yield return new WaitForSeconds(Random.Range(5, 15));

		customer.EnterShop();
		
		yield return new WaitForSeconds(0.5f);
		MyShopServerData.Instance.AddToastMessage("손님이 찾아왔습니다!");
	}
}
