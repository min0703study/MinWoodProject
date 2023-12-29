using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;
namespace ServerData
{
	public class Order
	{
		public int OrderId;
		public int NpcId;
		public int ItemId;
		public int RewardGold;
		public int RewardExp;
		
		public Define.OrderState OrderState;
	}
	
	public class Delivery
	{
		public Order deliveryOrder;
		public float StartTime;
		public float EndTime;
	}
}

public class OrderListServerData : BaseTable<OrderListServerData>
{
	public const int MAX_ORDER_COUNT = 3;
	public List<ServerData.Order> Orders { get; private set; } = new List<ServerData.Order>();
	
	//OnPlayerChanged
	public event Action OnChangedOrderListData;
	
	public void AddOrder(int npcId, int itemId, int rewardGold, int rewardExp)
	{
		if(Orders.Count < MAX_ORDER_COUNT) 
		{
			var newOrder = new ServerData.Order();
			Random random = new Random();
			newOrder.OrderId = random.Next();
			newOrder.NpcId = npcId;
			newOrder.ItemId = itemId;
			newOrder.RewardGold = rewardGold;
			newOrder.RewardExp = rewardExp;
			Orders.Add(newOrder);
			
			OnChangedOrderListData?.Invoke();
		}
	}
	
	public void DeleteOrder(int orderId)
	{
		foreach(var order in Orders) 
		{
			if(order.OrderId == orderId) 
			{
				Orders.Remove(order);
				OnChangedOrderListData?.Invoke();
				break;
			}
		}
	}
	
	public ServerData.Order GetOrderDataById(int orderId) 
	{
		for (int i = 0; i < MAX_ORDER_COUNT; i++)
		{
			if(Orders[i].OrderId == orderId)
			{
				return Orders[i];
			}
		}
		
		return null;
	}

}
