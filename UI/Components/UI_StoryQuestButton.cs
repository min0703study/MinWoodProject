using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

public class UI_StoryQuestButton : UI_Base
{	
	[Header("InProgress")]
	[SerializeField]
	Button InProgressButton;
	[SerializeField]
	TextMeshProUGUI questText;
	[SerializeField]
	UI_ProgressBar inProgressBar;
	
	[SerializeField]
	Image questIcon;
	
	[Header("Completed")]
	[SerializeField]
	Button CompletedButton;
	[SerializeField]
	TextMeshProUGUI rewardNameText;
	
	[SerializeField]
	TextMeshProUGUI rewardAmount;
	
	[SerializeField]
	Image rewardIcon;
	
	public Action OnRewardReceived;
	
	protected override void Init()
	{
		base.Init();
		
		InProgressButton.onClick.AddListener(OnClickInProgress);
		CompletedButton.onClick.AddListener(OnClickCompletedButton);
	}
	
	private void Start() {			
		Refresh();
	}
	
	public void Refresh()
	{
		if(QuestServerData.Instance.IsQuestCompleted()) 
		{
			CompletedButton.gameObject.SetActive(true);
			InProgressButton.gameObject.SetActive(false);
		} else 
		{			
			InProgressButton.gameObject.SetActive(true);
			CompletedButton.gameObject.SetActive(false);
			
			var currentStoryQuestIndex = QuestServerData.Instance.CurrentStoryQuest.StoryQuestIndex;
			var storyQuest = StoryQuestTable.Instance.GetStoryQuestByIndex(currentStoryQuestIndex);
			
			if(storyQuest.Type == "collect_item") 
			{
				var targetItem = ItemTable.Instance.GetItemDataByItemId(storyQuest.TargetId);
				var coditionCount = storyQuest.ConditionCount;
				questText.text = $"{targetItem.Name}를(을) {coditionCount}개 모으세요";
				
				Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(targetItem.SpriteName);
				questIcon.sprite = itemSprite;
				
				inProgressBar.Refresh(coditionCount, QuestServerData.Instance.CurrentStoryQuest.CurrentCount);
			}
			else if(storyQuest.Type == "craft_item") 
			{
				var targetItem = ItemTable.Instance.GetItemDataByItemId(storyQuest.TargetId);
				var coditionCount = storyQuest.ConditionCount;
				questText.text = $"{targetItem.Name}를(을) {coditionCount}개 제작하세요";
				
				Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(targetItem.SpriteName);
				questIcon.sprite = itemSprite;
				
				inProgressBar.Refresh(coditionCount, QuestServerData.Instance.CurrentStoryQuest.CurrentCount);
			} else if(storyQuest.Type == "build") 
			{
				var building = MyShopServerData.Instance.GetBuildingByBuildingId(storyQuest.TargetId);
				var coditionCount = storyQuest.ConditionCount;
				questText.text = $"{building.Name}를(을) 건설하세요";
				
				Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(building.SpriteName);
				questIcon.sprite = itemSprite;
				
				inProgressBar.Refresh(coditionCount, QuestServerData.Instance.CurrentStoryQuest.CurrentCount);
			} else if (storyQuest.Type == "kill_monster") 
			{
				var targetMonster = MonsterTable.Instance.GetMonsterDataById(storyQuest.TargetId);
				var coditionCount = storyQuest.ConditionCount;
				questText.text = $"{targetMonster.MonsterName}를(을) {coditionCount}마리 잡으세요";
				
				Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(targetMonster.SpriteName);
				questIcon.sprite = itemSprite;
				
				inProgressBar.Refresh(coditionCount, QuestServerData.Instance.CurrentStoryQuest.CurrentCount);
			}
		}
	}
	
	private void OnClickInProgress()
	{
		SoundManager.Instance.PlayButtonPressSound();
		var currentStoryQuestIndex = QuestServerData.Instance.CurrentStoryQuest.StoryQuestIndex;
		var storyQuest = StoryQuestTable.Instance.GetStoryQuestByIndex(currentStoryQuestIndex);
		
		if(storyQuest.Type == "collect_item") 
		{
			var tree = MapManager.Instance.Map.GetNeareastTree();
			CinemachineManager.Instance.SwitchFocusForSeconds(tree.transform.position);
		} else if(storyQuest.Type == "build") 
		{
			if(storyQuest.Id == 6) 
			{
				var workTable = MapManager.Instance.Map.GetNeareastBuilding<OrderBoard>();
				CinemachineManager.Instance.SwitchFocusForSeconds(workTable.transform.position);
			} else 
			{
				var workTable = MapManager.Instance.Map.GetNeareastBuilding<WorkTable>();
				CinemachineManager.Instance.SwitchFocusForSeconds(workTable.transform.position);
			}
		}
	}
	
	private void OnClickCompletedButton()
	{
		if(QuestServerData.Instance.IsQuestCompleted()) 
		{
			var currentStoryQuestIndex = QuestServerData.Instance.CurrentStoryQuest.StoryQuestIndex;
			var storyQuest = StoryQuestTable.Instance.GetStoryQuestByIndex(currentStoryQuestIndex);
			
			GameManager.Instance.Player.AddItem(storyQuest.RewardItemId, storyQuest.RewardItemAmount);
			
			QuestServerData.Instance.ToNextStoryQuest();
		}
		
		SoundManager.Instance.PlayQuestComplete();
		OnRewardReceived?.Invoke();
	}
}
