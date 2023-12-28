using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

using Random = UnityEngine.Random;

public class LobbyScene : BaseScene
{	
	public CinemachineVirtualCamera HomeVirtualamera, ShopVitualCamera, BackyardVirtualCamera, WorkplaceVirtualCamera;
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
		
		// VCamera Setting
		HomeVirtualamera.m_Follow = GameManager.Instance.Player.gameObject.transform;
		ShopVitualCamera.m_Follow = GameManager.Instance.Player.gameObject.transform;
		BackyardVirtualCamera.m_Follow = GameManager.Instance.Player.gameObject.transform;
		WorkplaceVirtualCamera.m_Follow = GameManager.Instance.Player.gameObject.transform;
		
		CinemachineManager.Instance.MainCamera = MainCamera;
		CinemachineManager.Instance.AddVirtualCamera("Home", HomeVirtualamera);
		CinemachineManager.Instance.AddVirtualCamera("Shop", ShopVitualCamera);
		CinemachineManager.Instance.AddVirtualCamera("Backyard", BackyardVirtualCamera);
		CinemachineManager.Instance.AddVirtualCamera("Workplace", WorkplaceVirtualCamera);
		
		CinemachineManager.Instance.ActivateCamera("Home");
		
		SoundManager.Instance.Play(SoundManager.SoundType.Bgm, "Sound_LobbyBGM");

		MapEntity orderPos = MapManager.Instance.Map.GetMapEntityGO<CatCarpet>();
		MapManager.Instance.Map.SpawnCat(orderPos.CenterPos);
		
		Bind();
	}
	
	private void Bind() 
	{
		//MyShopServerData.Instance.OnShopDataChanged += ChangedShopData;
	}
	
	private void OnDestroy() {
		//MyShopServerData.Instance.OnShopDataChanged -= ChangedShopData;
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
