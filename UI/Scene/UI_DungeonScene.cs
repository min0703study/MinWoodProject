using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_DungeonScene : UI_SceneBase
{
	#region TopPanel
	[Header("Top Panel")]
	[SerializeField]
	TextMeshProUGUI levelText;
	[SerializeField]
	UI_ProgressBar expProgressBar;
	[SerializeField]
	TextMeshProUGUI coinAmountText;
	[SerializeField]
	TextMeshProUGUI diaAmountText;
	#endregion

	#region CenterPanel
	[Header("Center Panel")]
	[SerializeField]
	UI_StoryQuestButton uiStoryQuestButton;
	#endregion
	
	
	[SerializeField]
	UI_CustomToggle weaponToggle;
	
	[SerializeField]
	GameObject swordSkillListGO, riffleSkillListGO;
	
	protected override void Init()
	{	
		base.Init();
	}
	
	private void Start() {
		weaponToggle.AddOnValueChangedListener(weaponToggleValueChanged);
		Bind();
		RefreshAll();
	}
	
	private void RefreshAll() 
	{	
		RefreshPlayerData();
		RefreshStoryQuest();
	}
	
	private void Bind()
	{
		UserServerData.Instance.OnPlayerChanged += RefreshPlayerData;
		QuestServerData.Instance.OnChagedStoryQuestData += RefreshStoryQuest;	
	}

	private void OnDestroy() {
		UserServerData.Instance.OnPlayerChanged -= RefreshPlayerData;
		QuestServerData.Instance.OnChagedStoryQuestData -= RefreshStoryQuest;
	}

	private void RefreshPlayerData() 
	{	
		//재화
		if(coinAmountText.text != UserServerData.Instance.Coin.ToString()) 
		{
			DOTween.To(() => int.Parse(coinAmountText.text), x => coinAmountText.text = ((int)x).ToString(), UserServerData.Instance.Coin, 0.5f)
				.SetEase(Ease.Linear);
		}
		
		//경험치
		expProgressBar.Refresh(LevelExpTable.Instance.GetRequiredExpForLevel(UserServerData.Instance.Level + 1), UserServerData.Instance.EXP);

		// Level
		if (levelText.text != UserServerData.Instance.Level.ToString())
		{
			var popup = UIManager.Instance.ShowPopupUI<UI_LevelUpPopup>();
			levelText.text = UserServerData.Instance.Level.ToString();
		}
	}

	public void RefreshStoryQuest()
	{
		uiStoryQuestButton.Refresh();
	}

	
	private void weaponToggleValueChanged(bool isOn) 
	{
		SoundManager.Instance.PlayButtonPressSound();
		if(isOn) 
		{
			GameManager.Instance.Player.ChangeWeaponMode(Define.WeaponMode.Sword);
			swordSkillListGO.SetActive(true);
			riffleSkillListGO.SetActive(false);
		} else 
		{
			GameManager.Instance.Player.ChangeWeaponMode(Define.WeaponMode.Riffle);
			riffleSkillListGO.SetActive(true);
			swordSkillListGO.SetActive(false);
		}
	}
	
	public void ClearDungeon() 
	{
		var popup = UIManager.Instance.ShowPopupUI<UI_DungeonClearPopup>();
		popup.OnClosedPopup += ()=>SceneChangeManager.Instance.LoadScene(Define.Scene.LobbyScene);
	}

}
