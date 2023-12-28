using System;

namespace ServerData
{
	public class InventoryItem
	{
		public bool IsEmpty { get; set;}
		public int ItemId { get; set;}
		public int Amount { get; set; } 
	}
}

public class InventoryServerData : BaseTable<InventoryServerData>
{
	public const int INVENTORY_SIZE = 20;
	public ServerData.InventoryItem[] InventoryItems { get; private set; } = new ServerData.InventoryItem[INVENTORY_SIZE];
	
	override protected void init() 
	{
		base.init();
		for (int i = 0; i < INVENTORY_SIZE; i++)
		{
			InventoryItems[i] = new ServerData.InventoryItem();
			InventoryItems[i].IsEmpty = true;
			InventoryItems[i].ItemId = -1;
			InventoryItems[i].Amount = -1;
		}
	}
	
	public bool AddItem(int newItemId, int amount = 1)
	{	
		bool isAreadyExist = false;
		
		// 이미 아이템이 있을 경우
		for (int i = 0; i < INVENTORY_SIZE; ++i)
		{
			if (InventoryItems[i].ItemId == newItemId)
			{
				UpdateItem(i, newItemId, InventoryItems[i].Amount + amount);
				isAreadyExist = true;
				break;
			}
		}
		
		if(isAreadyExist == false) 
		{
			for (int i = 0; i < INVENTORY_SIZE; ++i)
			{
				if (InventoryItems[i].ItemId == -1)
				{
					UpdateItem(i, newItemId, amount);
					break;
				}
			}
		}

		//새로운 아이템

		
		return true;

		//bool areadyMaxStack = remainingToFit == 0;
		//return areadyMaxStack;
	}

	
	public void UpdateItem(int invenIndex, int itemId, int amount)
	{
		InventoryItems[invenIndex].IsEmpty = amount == 0;
		InventoryItems[invenIndex].ItemId = itemId;
		InventoryItems[invenIndex].Amount = amount;
	}
	
	public bool HasItemAmount(int itemId, int requiredAmount)
	{
		var item = Array.Find(InventoryItems, inventoryItem => inventoryItem.ItemId == itemId);

		if(item != null)
		{
			return item.Amount >= requiredAmount;
		}

		return false;
	}
	
	public int GetItemAmount(int itemId)
	{
		var item = Array.Find(InventoryItems, inventoryItem => inventoryItem.ItemId == itemId);
		if(item == null) 
		{
			return 0;
		}
		return item.Amount;
	}
	
	public void RemoveItem(int itemId, int requiredAmount)
	{
		var item = Array.Find(InventoryItems, inventoryItem => inventoryItem.ItemId == itemId);

		if(item != null)
		{
			item.Amount = item.Amount - requiredAmount;
			if(item.Amount <= 0) 
			{
				item.IsEmpty = true;
				item.ItemId = -1;
			}
		}
	}
}
