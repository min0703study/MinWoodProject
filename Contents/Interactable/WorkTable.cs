using System;
using UnityEngine;
using UnityEngine.UI;

public class WorkTable : Building
{	
	[SerializeField]
	ProgressBar progressBar;
	
	[SerializeField]
	SpriteRenderer crfatingEntitySpriteRenderer;
	
	[SerializeField]
	GameObject progressPanel;
	
	[SerializeField]
	Transform openButtonPos; 

	bool isCraftingNow = false; 

	protected override void Init()
	{
		base.Init();
	}
	
	public override void OpenControlPopup() 
	{
		SoundManager.Instance.PlayButtonPressSound();
		UIManager.Instance.ShowPopupUI<UI_WorkTablePopup>();
	}
	
	private void Update() {
		if(CraftingServerData.Instance.CraftingItemStatusQueue.Count > 0) 
		{
			
			ServerData.CraftingItemStatus craftingItemStatus = CraftingServerData.Instance.CraftingItemStatusQueue.Peek();
			if(isCraftingNow == false) 
			{
				isCraftingNow = true;
				progressPanel.SetActive(true);
				uiInteractButton.SetActive(false);
				var craftingItem = ItemTable.Instance.GetItemDataByItemId(craftingItemStatus.ItemId);
				crfatingEntitySpriteRenderer.sprite = ResourceManager.Instance.Load<Sprite>(craftingItem.SpriteName);
				
			}
			TimeSpan timeSinceNowDate = craftingItemStatus.EndTime - DateTime.Now;
			progressBar.SetProgressBarValue(craftingItemStatus.DurationTime, (float)timeSinceNowDate.TotalSeconds);
			if(craftingItemStatus.EndTime < DateTime.Now) 
			{
				var curCraftingItemStatus =  CraftingServerData.Instance.CraftingItemStatusQueue.Dequeue();
				MapManager.Instance.Map.GenerateDropItem(CenterPos, curCraftingItemStatus.ItemId);
				QuestServerData.Instance.UpdateQuestCheck("craft_item", curCraftingItemStatus.ItemId, 1);
				isCraftingNow = false;
				uiInteractButton.SetActive(true);
			}
		} else 
		{
			progressPanel.SetActive(false);
		}
		
	}
}
