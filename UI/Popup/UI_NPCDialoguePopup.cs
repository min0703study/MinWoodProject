
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_NPCDialoguePopup : UI_PopupBase
{
	private CustomerController customer;
	
	[SerializeField]
	private TextMeshProUGUI orderItemName;
	
	[SerializeField]
	private Image orderItemIcon;
	
	[SerializeField]
	private TextMeshProUGUI rewardCoinValue;

	[SerializeField]
	Button acceptButton, refuseButton;
	
	private void Awake() {
		acceptButton.onClick.AddListener(OnAcceptButton);
		refuseButton.onClick.AddListener(OnRefuseButton);	
	}
	
	public void SetInfo(CustomerController customer)
	{
		this.customer = customer;
		
		var orderItem = ItemTable.Instance.GetItemDataByItemId(103010017);
		
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(orderItem.SpriteName);
		orderItemIcon.sprite = itemSprite;
		orderItemIcon.SetNativeSize();
		
		rewardCoinValue.text = 30.ToString();
		
		orderItemName.text = orderItem.Name;
	}
	
	public void OnAcceptButton() 
	{
		OrderListServerData.Instance.AddOrder(customer.NPCTableData.NPCId, 103010017, 30, 100);
		customer.PlayerAcceptedOrder();
		ClosePopupUI();
	}
	
	public void OnRefuseButton() 
	{
		customer.PlayerRefuseOrder();
		ClosePopupUI();
	}
}
