using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UI_WorkTablePopup : UI_PopupBase
{
	[SerializeField]
	Button backgroundButton, startCraftingButton;

	[SerializeField]
	Image selectedItemImage;
	
	[SerializeField]
	TextMeshProUGUI selectedItemName, selectedItemDesc;
	
	[SerializeField]
	GameObject craftItemListGO;
	
	[SerializeField]
	GameObject ingredientItemListGO;
	
	[SerializeField]
	GameObject disableCraftButtonPanelGO, activeCraftButtonPanelGO;
	
	UI_CraftItemCell selectedCraftableItemCell = null;
	
	protected override void Init() 
	{
		base.Init();

		backgroundButton.onClick.AddListener(ClosePopupUI);
		startCraftingButton.onClick.AddListener(OnClickStartCraftingButton);
		
		refreshCraftItemList();
		refreshSelectedItemInfo();
	}
	
	private void refreshSelectedItemInfo() 
	{
		if(selectedCraftableItemCell == null)
			return;
			
		int selectedCraftableItemId = selectedCraftableItemCell.CraftableItemId;
		TableData.CraftableItem selectedCraftableItem = CraftableItemTable.Instance.GetCraftableItemById(selectedCraftableItemId);
		
		// 재료 목록 새로고침
		Util.DestroyChilds(ingredientItemListGO);
		
		bool isIngredientSufficient = true;
		foreach (var craftingIngredient in selectedCraftableItem.CraftingIngredients) 
		{
			UI_IngredientItemCell itemCell = UIManager.Instance.MakeSubItem<UI_IngredientItemCell>(ingredientItemListGO.transform);
			itemCell.SetInfo(craftingIngredient.IngredientItemId, craftingIngredient.IngredientItemAmount, InventoryServerData.Instance.GetItemAmount(craftingIngredient.IngredientItemId));
			if(InventoryServerData.Instance.HasItemAmount(craftingIngredient.IngredientItemId, craftingIngredient.IngredientItemAmount) == false) 
			{
				isIngredientSufficient = false;
			};
		}
		
		activeCraftButtonPanelGO.SetActive(isIngredientSufficient);
		disableCraftButtonPanelGO.SetActive(!isIngredientSufficient);
		
		// 상단 정보 새로 고침
		TableData.Item tableDataItem = ItemTable.Instance.GetItemDataByItemId(selectedCraftableItem.CraftedItemID);
		selectedItemImage.sprite = ResourceManager.Instance.Load<Sprite>(tableDataItem.SpriteName);
		selectedItemName.text = tableDataItem.Name;
		selectedItemDesc.text = tableDataItem.Desc;
	}
	
	private void refreshCraftItemList() 
	{
		Util.DestroyChilds(craftItemListGO);
		
		foreach (var craftableItem in CraftableItemTable.Instance.CraftableItems) 
		{
			UI_CraftItemCell itemCell = UIManager.Instance.MakeSubItem<UI_CraftItemCell>(craftItemListGO.transform);
			itemCell.SetInfo(craftableItem.Id);
			itemCell.CellButtonClicked += (ui_craftItemCell) => 
			{
				if(selectedCraftableItemCell != null) 
				{
					selectedCraftableItemCell.SetSelectedState(false);
				}
				
				this.selectedCraftableItemCell = ui_craftItemCell;
				selectedCraftableItemCell.SetSelectedState(true);
				refreshSelectedItemInfo();
			};
		}
		
		if(this.selectedCraftableItemCell == null) 
		{
			GameObject firstCraftItemCellGO = craftItemListGO.transform.GetChild(0).gameObject;
			this.selectedCraftableItemCell = firstCraftItemCellGO.GetComponent<UI_CraftItemCell>();
			this.selectedCraftableItemCell.SetSelectedState(true);
		}
	}
	
	private void OnClickStartCraftingButton() 
	{
		TableData.CraftableItem selectedCraftableItem = CraftableItemTable.Instance.GetCraftableItemById(selectedCraftableItemCell.CraftableItemId);
		foreach (var craftingIngredient in selectedCraftableItem.CraftingIngredients) 
		{
			//UI_IngredientItemCell itemCell = UIManager.Instance.MakeSubItem<UI_IngredientItemCell>(ingredientItemListGO.transform);
			//itemCell.SetInfo(craftingIngredient.IngredientItemId, craftingIngredient.IngredientItemAmount);
			if(InventoryServerData.Instance.HasItemAmount(craftingIngredient.IngredientItemId, craftingIngredient.IngredientItemAmount) == false) 
			{
				return;
			};
			InventoryServerData.Instance.RemoveItem(craftingIngredient.IngredientItemId, craftingIngredient.IngredientItemAmount);
			
		}
			
		int selectedCraftableItemId = selectedCraftableItemCell.CraftableItemId;
		CraftingServerData.Instance.AddCraftingItem(selectedCraftableItemId, 1);
		
		ClosePopupUI();
	}
}
