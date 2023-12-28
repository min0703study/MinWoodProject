using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelUpPopup : UI_PopupBase
{
	[SerializeField]
	TextMeshProUGUI levelText;
	
	protected override void Init() 
	{
		base.Init();
		
		SoundManager.Instance.PlayLevelUpSound();
		levelText.text = UserServerData.Instance.Level.ToString();
	}

	public void OnLevelUpAnimationCompleted() 
	{
		ClosePopupUI();
	}
}
