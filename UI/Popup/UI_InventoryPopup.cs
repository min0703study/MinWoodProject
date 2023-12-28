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
	private UI_CustomToggle itemToggle, craftEntityToggle;

	protected override void Init()
	{
		base.Init();
		
		RefreshInventoryList();
	
		backgroundButton.onClick.AddListener(ClosePopupUI);
		
		itemToggle.AddOnValueChangedListener(ItemToggleValueChanged);
		craftEntityToggle.AddOnValueChangedListener(craftEntityToggleValueChanged);
	}
	
	private void RefreshInventoryList() 
	{
		Util.DestroyChilds(itemListGO);
		
		foreach (var item in InventoryServerData.Instance.InventoryItems) 
		{
			var inventoryCell = UIManager.Instance.MakeSubItem<UI_InventoryCell>(itemListGO.transform);
			inventoryCell.SetInfo(item.ItemId, item.Amount);
		}
	}
	
	private void ItemToggleValueChanged(bool isOn) 
	{
		SoundManager.Instance.PlayButtonPressSound();
		itemToggle.SwitchToggle(isOn);
	}

	private void craftEntityToggleValueChanged(bool isOn) 
	{
		SoundManager.Instance.PlayButtonPressSound();
		craftEntityToggle.SwitchToggle(isOn);
	}
	
	

}
