using System;
using Cinemachine;
using UnityEngine;

public class DungeonScene : BaseScene
{
	public CinemachineVirtualCamera MainVirtualCamera;
	
	protected override void Init()
	{	
		base.Init();
		SceneType = Define.Scene.DungeonScene;
		
		var dungeonScene = UIManager.Instance.OpenSceneUI<UI_DungeonScene>();

		UIManager.Instance.OpenJoystickUI<UI_Joystick>();
		
		MapManager.Instance.LoadOrGenerateMap(Define.Map.Dungeon, 106010004);
		
		SoundManager.Instance.Play(SoundManager.SoundType.Bgm, "Sound_DungeonBGM");
		
		// VCamera Setting
		MainVirtualCamera.m_Follow = GameManager.Instance.Player.gameObject.transform;
		
		CinemachineManager.Instance.AddVirtualCamera("Main", MainVirtualCamera);
		CinemachineManager.Instance.ActivateCamera("Main");
		
		MapManager.Instance.Map.OnClearDungon += dungeonScene.ClearDungeon;

	}
	
	private void Start() {
		GameManager.Instance.Player.ChangeWeaponMode(Define.WeaponMode.Sword);
	}
}
