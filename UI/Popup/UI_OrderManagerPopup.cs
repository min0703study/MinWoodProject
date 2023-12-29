using UnityEngine;
using UnityEngine.UI;

public class UI_OrderManagerPopup : UI_PopupBase
{
	[SerializeField]
	Button backgroundButton;
	
	[SerializeField]
	GameObject deliveryListGO;
	
	[SerializeField]
	GameObject contentPanel;
	
	protected override void Init() 
	{
		backgroundButton.onClick.AddListener(ClosePopupUI);
		
		Util.DestroyChilds(deliveryListGO);
		
		if(OrderListServerData.Instance.Orders.Count <= 0) 
		{
			UIManager.Instance.MakeSubItem<UI_OrderEmptyCell>(deliveryListGO.transform);
		}
		
		foreach (var order in OrderListServerData.Instance.Orders) 
		{				
			var deliveryCell = UIManager.Instance.MakeSubItem<UI_OrderCell>(deliveryListGO.transform);
			deliveryCell.SetInfo(order.OrderId);
		}
	}
	
	private void Start() {
		PopupOpenAnimation(contentPanel);
	}
	
	private void Update() {

	}

}
