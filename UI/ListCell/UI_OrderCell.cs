using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrderCell : UI_Base
{
	public int OrderId { get; set; }
		
	[SerializeField]
	Image itemIcon;
	
	[SerializeField]
	TextMeshProUGUI customerName, rewardGold, rewardDia;
	
	[SerializeField]
	TextMeshProUGUI deliveryButtonText;
	
	[SerializeField]
	Button deliveryButton;
	
	private void Awake() {
		//deliveryButton.onClick.AddListener(OnClickDeliveryButton);
	}
	
	public void SetInfo(int orderId)
	{	
		OrderId = orderId;
		Refresh();
	}
	
	public void Refresh()
	{		
		ServerData.Order order = OrderListServerData.Instance.GetOrderDataById(OrderId);
		
		TableData.Item orderItem = ItemTable.Instance.GetItemDataByItemId(order.ItemId);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(orderItem.SpriteName);
		itemIcon.sprite = itemSprite;
		
		//Npc 정보 매핑
		TableData.NPC customerNpc = NPCTable.Instance.GetNPCTableDataById(order.NpcId);
		customerName.text = customerNpc.Name;
		
		bool flag = false;
		//버튼 활상화 확인
		foreach(var area in MyShopServerData.Instance.SellingItemAreas)
		{
			if(area.isAddedItem && area.ItemId == orderItem.Id)
			{
				deliveryButtonText.text = "배송 가능";
				deliveryButton.interactable = true;
				flag = true;
				break;
			} 
		}
		
		if(flag == false) 
		{
			deliveryButtonText.text = "배송 불가";
			deliveryButton.interactable = false;
		}

	}
	
	public void OnClickDeliveryButton() 
	{
		ServerData.Order order = OrderListServerData.Instance.GetOrderDataById(OrderId);
		
		int bozagiIndex = MyShopServerData.Instance.FindBojagiIndexByItemId(order.ItemId);
		
		var cat = MapManager.Instance.Map.GetMapEntityGO<CatController>();
		(cat as CatController).Deliver(bozagiIndex);
		
		//GameManager.Instance.Player.AddCoin(order.RewardGold);
	}
}
