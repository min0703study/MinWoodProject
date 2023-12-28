using System;
using UnityEngine;

namespace ServerData
{
	public class StoryQuest
	{
		public int StoryQuestIndex;
		public int CurrentCount;
		public bool RewardReceived; 
	}
}
public class QuestServerData : BaseTable<QuestServerData>
{
	
	public ServerData.StoryQuest CurrentStoryQuest { get; private set; } = new ServerData.StoryQuest();
	
	public event Action OnChagedStoryQuestData;

	protected override void init()
	{
		base.init();
		
		CurrentStoryQuest.StoryQuestIndex = 1;
		CurrentStoryQuest.CurrentCount = 0;
		CurrentStoryQuest.RewardReceived = false;
	}

	public bool IsQuestCompleted() 
	{

		return CurrentStoryQuest.CurrentCount >= StoryQuestTable.Instance.GetStoryQuestByIndex(CurrentStoryQuest.StoryQuestIndex).ConditionCount;
	}
	
	public void ToNextStoryQuest() 
	{
		CurrentStoryQuest.StoryQuestIndex += 1;
		CurrentStoryQuest.CurrentCount = 0;
		CurrentStoryQuest.RewardReceived = false;
		OnChagedStoryQuestData?.Invoke();
	}
	
	public void UpdateQuestCheck(string type, int targetId, int count) 
	{
		var storyQuest = StoryQuestTable.Instance.GetStoryQuestByIndex(CurrentStoryQuest.StoryQuestIndex);
		if(storyQuest.Type != type || storyQuest.TargetId != targetId) 
		{
			return;
		}
		
		if(CurrentStoryQuest.CurrentCount + count >= storyQuest.ConditionCount) 
		{
			CurrentStoryQuest.CurrentCount = storyQuest.ConditionCount;
			CurrentStoryQuest.RewardReceived = true;
		} else 
		{
			CurrentStoryQuest.CurrentCount += count; 
		}
		
		OnChagedStoryQuestData?.Invoke();		
	}
}
