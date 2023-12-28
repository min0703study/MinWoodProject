using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerData
{
	public class CraftingItemStatus
	{
		public int ItemId { get; set; }
		
		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; } 
		
		public float DurationTime { get; set; }
	}
}

public class CraftingServerData : BaseTable<CraftingServerData>
{
	public Queue<ServerData.CraftingItemStatus> CraftingItemStatusQueue { get; private set; } = new Queue<ServerData.CraftingItemStatus>();
	
	override protected void init() 
	{
	}
	
	public void AddCraftingItem(int craftingInfoId, int amount)
	{
		TableData.CraftableItem craftingInfo = CraftableItemTable.Instance.GetCraftableItemById(craftingInfoId);
		
		for (int i = 0; i < amount ; i ++) 
		{
			ServerData.CraftingItemStatus craftingItemStatus = new ServerData.CraftingItemStatus(); 
			craftingItemStatus.ItemId = craftingInfo.CraftedItemID;
			craftingItemStatus.DurationTime = craftingInfo.DurationTime;
			craftingItemStatus.StartTime = DateTime.Now.AddSeconds(craftingInfo.DurationTime * i);
			craftingItemStatus.EndTime = craftingItemStatus.StartTime.AddSeconds(craftingInfo.DurationTime);
			CraftingItemStatusQueue.Enqueue(craftingItemStatus);
		}	
	}
}
