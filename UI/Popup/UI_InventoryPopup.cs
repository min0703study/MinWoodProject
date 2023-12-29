using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryPopup : UI_PopupBase
{
	[SerializeField]
	Button backgroundButton;
	
	[SerializeField]
	private GameObject itemListGO;
	
	[SerializeField]
	private GameObject contentPanel;

	protected override void Init()
	{
		base.Init();
		
		RefreshInventoryList();
	
		backgroundButton.onClick.AddListener(ClosePopupUI);
	}
	
	private void Start() {
		PopupOpenAnimation(contentPanel);
	}
	
	private void RefreshInventoryList() 
	{
		Util.DestroyChilds(itemListGO);
		
		foreach (var item in InventoryServerData.Instance.InventoryItems) 
		{
			var inventoryCell = UIManager.Instance.MakeSubItem<UI_InventoryCell>();
			inventoryCell.transform.SetParent(itemListGO.transform, false);
			inventoryCell.SetInfo(item.ItemId, item.Amount);
		}
	}
}
