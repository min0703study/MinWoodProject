using System;

using UnityEngine;
using UnityEngine.UI;

public class UI_CraftItemCell : UI_Base
{
	public int CraftableItemId { get; set; }
	public bool IsSelected { get; set; }
	public bool IsLocked { get; set; }
	
	[SerializeField]
	Image craftedItemImage;
	
	[SerializeField]
	GameObject selectedPanel, lockPanel;
	public Action<UI_CraftItemCell> CellButtonClicked;
	
	private Button cellButton;
	
	protected override void Init()
	{
		cellButton = GetComponent<Button>();
		cellButton.onClick.AddListener(OnCellButtonClick);
	}
	
	public void SetInfo(int craftableItemId)
	{
		CraftableItemId = craftableItemId;

		craftedItemImage.gameObject.SetActive(true);
		
		transform.localScale = Vector3.one;
		
		TableData.CraftableItem craftableItem = CraftableItemTable.Instance.GetCraftableItemById(craftableItemId);
		TableData.Item craftedItem = ItemTable.Instance.GetItemDataByItemId(craftableItem.CraftedItemID);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(craftedItem.SpriteName);
		craftedItemImage.sprite = itemSprite;
	}
	
	public void OnCellButtonClick() 
	{
		SoundManager.Instance.PlayButtonPressSound();
		CellButtonClicked?.Invoke(this);
	}
	
	public void SetSelectedState(bool isSelected) 
	{
		IsSelected = isSelected;
		selectedPanel.SetActive(isSelected);
	}
}
