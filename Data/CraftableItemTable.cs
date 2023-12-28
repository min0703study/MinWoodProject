using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace TableData
{
	// 제작에 필요한 재료 테이블
	public class CraftingIngredient
	{
		public int CraftableItemId { get; set;}
		public int IngredientItemId {get; set;}
		public int IngredientItemAmount {get; set;}
	}
	
	public class CraftableItem
	{
		public int Id { get; set; }
		public int CraftedItemID { get; set;}
		public int DurationTime {get; set;}

		public List<CraftingIngredient> CraftingIngredients { get; set; }
	}
}

public class CraftableItemTable: BaseTable<CraftableItemTable>
{
	public List<TableData.CraftableItem> CraftableItems {get; private set;} = new List<TableData.CraftableItem>();
	public Dictionary<int, TableData.CraftableItem> CraftableItemDict { get; private set; } = new Dictionary<int, TableData.CraftableItem>();

	protected override void init()
	{
		base.init();
	}
	
	public void Load(string craftableItemsAssetLabel, string craftingIngredientTableAssetLabel)
	{
		TextAsset craftableItemsAsset = ResourceManager.Instance.Load<TextAsset>(craftableItemsAssetLabel);
		CraftableItems = JsonConvert.DeserializeObject<List<TableData.CraftableItem>>(craftableItemsAsset.text);
		
		TextAsset ingredientTableAssets = ResourceManager.Instance.Load<TextAsset>(craftingIngredientTableAssetLabel);
		List<TableData.CraftingIngredient> craftingIngredients = JsonConvert.DeserializeObject<List<TableData.CraftingIngredient>>(ingredientTableAssets.text);
		
		foreach (TableData.CraftableItem craftableItem in CraftableItems) 
		{
			craftableItem.CraftingIngredients = new List<TableData.CraftingIngredient>();
			foreach (TableData.CraftingIngredient craftingIngredient in craftingIngredients) 
			{
				if(craftableItem.Id == craftingIngredient.CraftableItemId)
				{
					craftableItem.CraftingIngredients.Add(craftingIngredient);
				}
			}
			
			CraftableItemDict.Add(craftableItem.Id, craftableItem);
		}
	}

	public TableData.CraftableItem GetCraftableItemById(int craftingInfoId) 
	{
		Dictionary<int, TableData.CraftableItem> dict = CraftableItemDict;
		return dict[craftingInfoId];
	}
}
