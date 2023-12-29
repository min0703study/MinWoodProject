using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UI_LobbyScene : UI_SceneBase
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
	
	#region BottomPanel
	[Header("Bottom Panel")]
	[SerializeField]
	Toggle inventoryToggle;
	#endregion
	
	[SerializeField]
	Animator doorBellAnimator;
	
	[SerializeField]
	UI_ToastMessage toastMessage;
	
	[SerializeField]
	Button worldMapButton;

	public float spreadDistance = 280f;
	
	protected override void Init()
	{	
		inventoryToggle.onValueChanged.AddListener(OnValueChnagedInventoryToggle);
		worldMapButton.onClick.AddListener(OnClickWorldMapButton);
		
		uiStoryQuestButton.OnRewardReceived += StartStoryQuestRewardAnimation;
		
		levelText.text = UserServerData.Instance.Level.ToString();
		coinAmountText.text = UserServerData.Instance.Coin.ToString();
	}
	
	private void Start() {
		Bind();
		RefreshAll();
	}
	
	private void Bind()
	{	
		UserServerData.Instance.OnPlayerChanged += RefreshPlayerData;
		MyShopServerData.Instance.OnShopDataChanged += RefreshMyShopData;
		QuestServerData.Instance.OnChagedStoryQuestData += RefreshStoryQuest;
	}
	
	private void OnDestroy() {
		UserServerData.Instance.OnPlayerChanged -= RefreshPlayerData;
		MyShopServerData.Instance.OnShopDataChanged -= RefreshMyShopData;
		QuestServerData.Instance.OnChagedStoryQuestData -= RefreshStoryQuest;
	}
	
	private void OnValueChnagedInventoryToggle(bool isOn) 
	{
		SoundManager.Instance.PlayButtonPressSound();
		
		if(isOn) 
		{
			var popup = UIManager.Instance.ShowPopupUI<UI_InventoryPopup>();
			popup.OnClosedPopup += () => inventoryToggle.isOn = false;
		}
	}
	
	private void OnClickWorldMapButton() 
	{
		SoundManager.Instance.PlayButtonPressSound();
		var popup = UIManager.Instance.ShowPopupUI<UI_WorldMapPopup>();
	}
	
	private void RefreshAll() 
	{	
		RefreshPlayerData();
		RefreshMyShopData();
		RefreshStoryQuest();
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
	
	private void RefreshMyShopData() 
	{
		if(MyShopServerData.Instance.ToastMessageStack.Count > 0) 
		{
			toastMessage.ShowMessage(MyShopServerData.Instance.ToastMessageStack.Pop());	
		}
		
		if(MyShopServerData.Instance.EnteredNPCDict.Count <= 0 || MyShopServerData.Instance.EnteredNPCDict.First().Value.isWait) 
		{
			if(doorBellAnimator.GetBool("DoorBellOffTrigger") == false) 
			{				
				doorBellAnimator.ResetTrigger("DoorBellOnTrigger");
				doorBellAnimator.SetTrigger("DoorBellOffTrigger");
			}
		} else 
		{
			if(doorBellAnimator.GetBool("DoorBellOnTrigger") == false) 
			{
				SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_DoorBell");
				
				doorBellAnimator.ResetTrigger("DoorBellOffTrigger");
				doorBellAnimator.SetTrigger("DoorBellOnTrigger");
			}
		}
	}

	public void RefreshStoryQuest()
	{
		uiStoryQuestButton.Refresh();
	}

	void StartStoryQuestRewardAnimation()
	{
		var item = ItemTable.Instance.GetItemDataByItemId(103010020);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
		List<UI_ItemIcon> itemIcons = new List<UI_ItemIcon>(); 
		
		for(int i = 0; i < 20; i++) 
		{			
			var itemIcon = UIManager.Instance.MakeSceneUISubItem<UI_ItemIcon>();
			itemIcon.SetInfo(itemSprite);
			itemIcon.transform.position = uiStoryQuestButton.transform.position;
			itemIcons.Add(itemIcon);
		}
		
		// 각 Sprite에 대해 랜덤한 방향과 거리로 이동 애니메이션 적용
		foreach (var itemIcon in itemIcons)
		{
			// 랜덤한 방향과 거리 계산
			Vector2 randomDirection = Random.insideUnitCircle.normalized;
			float randomDistance = Random.Range(100f, spreadDistance);

			// 현재 위치에서 랜덤한 방향으로 랜덤한 거리만큼 이동하는 Tweener 생성
			 var spreadTweener = itemIcon.transform.DOMove(itemIcon.transform.position + (Vector3)randomDirection * randomDistance, 1f)
				.SetEase(Ease.OutQuad);
				
			// 퍼진 후에 지정한 위치로 바로 이동하는 Tweener 생성
			var moveToTargetTweener = itemIcon.transform.DOMove(coinAmountText.transform.position, 0.3f)
				.SetEase(Ease.InQuad)
				.Pause(); // 초기에는 일시정지 상태로 시작
			
			// spreadTweener가 끝날 때 moveToTargetTweener 실행
			spreadTweener.OnComplete(() => moveToTargetTweener.Play());

			moveToTargetTweener.OnComplete(() => { 
				ResourceManager.Instance.Destroy(itemIcon.gameObject); });
			
		}
	}
}
