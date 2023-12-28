using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace TableData
{
	public class StoryQuest
	{
		public int Id { get; set;}
		public string Type { get; set; }
		public int TargetId { get; set; }
		public int ConditionCount { get; set; }
		
		public int RewardItemId { get; set; }
		public int RewardItemAmount { get; set; }
		
		public TableData.Item ItemTableData;
	}
}

public class StoryQuestTable: BaseTable<StoryQuestTable>
{
	public List<TableData.StoryQuest> StoryQuests { get; private set; } = new List<TableData.StoryQuest>();
	public Dictionary<int, TableData.StoryQuest> StoryQuestDict { get; private set; } = new Dictionary<int, TableData.StoryQuest>();

	protected override void init()
	{
		base.init();
		
		
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		StoryQuests = JsonConvert.DeserializeObject<List<TableData.StoryQuest>>(textAsset.text);
		
		foreach (var storyQuest in StoryQuests) 
		{
			StoryQuestDict.Add(storyQuest.Id, storyQuest);
		}
	}

	public void LoadAfter()
	{
		foreach (var storyQuest in StoryQuests) 
		{
			storyQuest.ItemTableData = ItemTable.Instance.GetItemDataByItemId(storyQuest.RewardItemId);
		}
	}


	public TableData.StoryQuest GetStoryQuestByIndex(int index) 
	{
		Dictionary<int, TableData.StoryQuest> dict = StoryQuestDict;
		return dict[index];
	}
	
	public TableData.StoryQuest GetNextStoryQuest(int id) 
	{
		Dictionary<int, TableData.StoryQuest> dict = StoryQuestDict;
		return dict[id];
	}
}
