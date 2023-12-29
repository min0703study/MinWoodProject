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
		deliveryButton.onClick.AddListener(OnClickDeliveryButton);
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
		itemIcon.SetNativeSize();
		
		rewardGold.text = order.RewardGold.ToString();
		
		//Npc 정보 매핑
		TableData.NPC customerNpc = NPCTable.Instance.GetNPCTableDataById(order.NpcId);
		customerName.text = $"{customerNpc.Name}의 주문 요청";
		
		bool flag = false;
		
		int inventoryAmount = InventoryServerData.Instance.GetItemAmount(orderItem.Id);

		//버튼 활상화 확인
		foreach(var area in MyShopServerData.Instance.SellingItemAreas)
		{
			if(inventoryAmount > 0)
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
		TableData.Item orderItem = ItemTable.Instance.GetItemDataByItemId(order.ItemId);
		
		InventoryServerData.Instance.RemoveItem(orderItem.Id, 1);
					
		var currentStoryQuestIndex = QuestServerData.Instance.CurrentStoryQuest.StoryQuestIndex;
		var storyQuest = StoryQuestTable.Instance.GetStoryQuestByIndex(currentStoryQuestIndex);
		
		GameManager.Instance.Player.AddItem(storyQuest.RewardItemId, storyQuest.RewardItemAmount);
			
		SoundManager.Instance.PlayQuestComplete();
	}
}
