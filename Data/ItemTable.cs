using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

using Newtonsoft.Json;
using UnityEngine;

namespace TableData
{
	public class Item
	{
		public int Id { get; set; }
		public string TextId { get; set;}
		public string Name {get; set;}
		public string ItemType {get; set;}
		public string SpriteName {get; set;}
		public int Price { get; set; }
		public string PrefabName { get; set; }		
		public string Desc {get; set;}
		public bool CanInInventory;
	}
}

public class ItemTable: BaseTable<ItemTable>
{
	public List<TableData.Item> Items { get; private set; } = new List<TableData.Item>();
	public Dictionary<int, TableData.Item> ItemDict { get; private set; } = new Dictionary<int, TableData.Item>();

	protected override void init()
	{
		base.init();
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		Items = JsonConvert.DeserializeObject<List<TableData.Item>>(textAsset.text);
		
		foreach (TableData.Item item in Items)
			ItemDict.Add(item.Id, item);
	}

	public TableData.Item GetItemDataByItemId(int itemId) 
	{
		Dictionary<int, TableData.Item> dict = ItemDict;
		dict.TryGetValue(itemId, out TableData.Item returnItem);
		return returnItem;
	}
}
