using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TableData
{
	public class Customer
	{
		public int CustomerId { get; set; }
		public string Name { get; set;}
		public string IconAssetLabel { get; set; }
		public string Desc {get; set;}
	}
}

public class CustomerTable: BaseTable<CustomerTable>
{
	public List<TableData.Customer> Customers {get; private set;} = new List<TableData.Customer>();
	public Dictionary<int, TableData.Customer> CustomerDict { get; private set; } = new Dictionary<int, TableData.Customer>();
	protected override void init()
	{
		base.init();
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		Customers = JsonConvert.DeserializeObject<List<TableData.Customer>>(textAsset.text);
		
		foreach (TableData.Customer customer in Customers)
			CustomerDict.Add(customer.CustomerId, customer);
	}

	public TableData.Customer GetCustomerTableDataById(int customerId) 
	{
		Dictionary<int, TableData.Customer> dict = CustomerDict;
		return dict[customerId];
	}
}
