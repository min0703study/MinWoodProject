
using System;
using System.Collections.Generic;
using ServerData;
using TableData;

using Random = UnityEngine.Random;

namespace ServerData
{
	public class SellingItemArea
	{
		public bool isAddedItem;
		public int ItemId;
	}
	
	public class SellingItem
	{
		public int ItemId;
		public int ItemAmount;		
		public int SoldAmount;
		public bool IsSoldOut;
	}
	
	public class EnteredNPC
	{
		public int NPCId;
		public int OrderId;
		
		public bool isWait;
	}
}

public class MyShopServerData : BaseTable<MyShopServerData>
{
	public ServerData.SellingItemArea[] SellingItemAreas { get; private set; } = new ServerData.SellingItemArea[6];
	public Dictionary<int, ServerData.EnteredNPC> EnteredNPCDict { get; private set; } = new Dictionary<int, ServerData.EnteredNPC>();
	public Dictionary<int, SellingItem> SellingItemDict { get; private set; } = new Dictionary<int, SellingItem>();
	public Dictionary<int, BuildingInfoSO> BuildingDict { get; private set; } = new Dictionary<int, BuildingInfoSO>();

	public Stack<string> ToastMessageStack {get; private set; } = new Stack<string>();
	
	public bool VisitCustomer = false;
	
	public event Action OnShopDataChanged;
	
	override protected void init() 
	{
		for (int i = 0; i < 6; i++)
		{
			SellingItemAreas[i] = new ServerData.SellingItemArea();
			SellingItemAreas[i].ItemId = -1;
		}
	}
	
	public void AddSellingItem(int areaIndex, int itemId)
	{
		SellingItemAreas[areaIndex].isAddedItem = true;
		SellingItemAreas[areaIndex].ItemId = itemId;
		
		OnShopDataChanged?.Invoke();
	}
	
	
	public void AddSalesItem(int itemId, int amount)
	{
		var addSalesItem = new SellingItem();
		addSalesItem.ItemId = itemId;
		addSalesItem.ItemAmount = amount;
		addSalesItem.SoldAmount = 0;
		
		int randomSellingId = Random.Range(0, 300);
		while (SellingItemDict.ContainsKey(randomSellingId))
		{
			randomSellingId = Random.Range(0, 300);
		};

		SellingItemDict.Add(randomSellingId, addSalesItem);
		OnShopDataChanged?.Invoke();
	}
	
	public void AddBuilding(BuildingInfoSO buildingInfoSO)
	{	
		BuildingDict.TryAdd(buildingInfoSO.BuildingId, buildingInfoSO);
		buildingInfoSO.RemaningItemAmount = buildingInfoSO.RequiredItemAmount;
		OnShopDataChanged?.Invoke();
	}
	
	public void ContentEntityMinusRemaningItemAmount(int buildingId, int ItemAmount)
	{		
		if(BuildingDict.TryGetValue(buildingId, out var buidling)) 
		{
			buidling.RemaningItemAmount -= ItemAmount;
			if(buidling.RemaningItemAmount <= 0) 
			{
				buidling.IsConstructionComplete = true;
			}
		}

		OnShopDataChanged?.Invoke();
	}
	
	public BuildingInfoSO GetBuildingByBuildingId(int buildingId) 
	{
		if(BuildingDict.TryGetValue(buildingId, out var building)) 
		{
			return building;
		}
		
		return null;
	}
	
	
	public void AddToastMessage(string meesage)
	{
		ToastMessageStack.Push(meesage);
		OnShopDataChanged?.Invoke();
	}
	
	
	public void SoldSellingItem(int sellingItemId, int soldAmount)
	{
		var soldItem = SellingItemDict[sellingItemId];
		if(soldItem.IsSoldOut)
			return;
			
		soldItem.SoldAmount += soldAmount;
		if(soldItem.ItemAmount - soldItem.SoldAmount <= 0) 
		{
			soldItem.IsSoldOut = true;
		}
		
		OnShopDataChanged?.Invoke();
	}

	public void EnterNPC(int NPCId)
	{
		EnteredNPCDict[NPCId] = new ServerData.EnteredNPC();
		EnteredNPCDict[NPCId].NPCId = NPCId;
		EnteredNPCDict[NPCId].OrderId = 1;
		EnteredNPCDict[NPCId].isWait = true;
		
		
		OnShopDataChanged?.Invoke();
	}
	
	public void RealEnterNPC(int NPCId)
	{
		EnteredNPCDict[NPCId] = new ServerData.EnteredNPC();
		EnteredNPCDict[NPCId].isWait = false;
	}
	
	
	public void ExitNPC(int NPCId)
	{
		EnteredNPCDict.Remove(NPCId);
		OnShopDataChanged?.Invoke();
	}

	public int FindBojagiIndexByItemId(int itemId)
	{
		for (int i = 0; i < 6; i++)
		{
			if(SellingItemAreas[i].ItemId == itemId) 
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public SellingItem GetSellingItemById(int sellingItemId) 
	{
		return SellingItemDict[sellingItemId];
	}
	
	public void OnVisitCustomer() 
	{
		VisitCustomer = true;
		OnShopDataChanged?.Invoke();
	}
}
