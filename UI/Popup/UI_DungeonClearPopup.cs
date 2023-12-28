using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonClearPopup : UI_PopupBase
{	
	[SerializeField]
	Button backButton;
	
	protected override void Init() 
	{
		base.Init();
		
		backButton.onClick.AddListener(OnClickBackButton);
		
		SoundManager.Instance.PlayLevelUpSound();
	}

	public void OnLevelUpAnimationCompleted() 
	{
		ClosePopupUI();
	}
	
	public void OnClickBackButton() 
	{
		SceneChangeManager.Instance.LoadScene(Define.Scene.LobbyScene);
	}
}
