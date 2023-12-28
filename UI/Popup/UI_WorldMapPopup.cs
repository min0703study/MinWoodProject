using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_WorldMapPopup : UI_PopupBase
{
	
	[SerializeField]
	Button backButton;
	
	#region Selected Dungeon
	[Header("Selected Dungeon")]
	[SerializeField]
	Button entryDungeonButton;
	
	#endregion
	
	protected override void Init() 
	{
		base.Init();

		backButton.onClick.AddListener(OnClickBackButton);
		entryDungeonButton.onClick.AddListener(OnClickDungeonButton);
	}
	
	private void OnClickDungeonButton() 
	{
		SceneChangeManager.Instance.LoadScene(Define.Scene.DungeonScene);
		UIManager.Instance.ClosePopupUI(this); 
	}

	private void OnClickBackButton() 
	{
		ClosePopupUI();
	}
}
