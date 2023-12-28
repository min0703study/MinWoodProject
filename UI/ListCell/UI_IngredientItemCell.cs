using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using TableData;

public class UI_IngredientItemCell : UI_Base
{
	public int CraftingInfoId { get; set; }
	
	
	[SerializeField]
	Image ingredientItemImage;
	
	[SerializeField]
	TextMeshProUGUI needAmountTMP,haveAmountTMP;
	
	private Button cellButton;
	
	protected override void Init()
	{
		cellButton = GetComponent<Button>();
		cellButton.onClick.AddListener(OnItemButtonClick);
	}
	
	public void SetInfo(int itemId, int amount, int haveAmount)
	{
		if(itemId == -1)
			return;
			
		CraftingInfoId = itemId;
		
		transform.localScale = Vector3.one;
		TableData.Item item = ItemTable.Instance.GetItemDataByItemId(itemId);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
		ingredientItemImage.sprite = itemSprite;
		needAmountTMP.text = amount.ToString();
		haveAmountTMP.text = haveAmount.ToString();
	}
	
	public void OnItemButtonClick() 
	{
	}
}
