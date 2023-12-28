using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildCondition : UI_WordSpaceBase
{
	[SerializeField]
	public Button accessConditionButton;
	
	[SerializeField]
	Image itemIcon;
	
	[SerializeField]
	TextMeshProUGUI itemAmountText; 
	
	Building building;

	Action OnCompletedBuilding;
	
	int BuildId => building.BuildingInfo.BuildingId;

	[SerializeField]
	CanvasGroup canvasGroup;

	protected override void Init()
	{
		base.Init();
		
		accessConditionButton.onClick.AddListener(OnClickAccessConditionButton);
	}
	
	public void SetInfo(Transform worldSpaceTransform, Building building) 
	{
		this.building = building;
		this.worldSpaceTransform = worldSpaceTransform;
		Refresh();
	}

	public void Refresh() 
	{
		var building = MyShopServerData.Instance.GetBuildingByBuildingId(BuildId);
		TableData.Item item = ItemTable.Instance.GetItemDataByItemId(building.RequiredItemId);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
		itemIcon.sprite = itemSprite;
		
		itemAmountText.text = building.RemaningItemAmount.ToString();
	}
	
	private void OnClickAccessConditionButton() 
	{
	
		if(building.BuildingInfo.IsConstructionComplete) 
		{
			StartCoroutine(CoUsingPlayerItem());
		} else 
		{
			Refresh();
		}	
	}
	
	IEnumerator CoUsingPlayerItem()
	{
		MapManager.Instance.Map.ShowBuildEffect(this.building.CenterPos, null);
		
		var item = ItemTable.Instance.GetItemDataByItemId(building.BuildingInfo.RequiredItemId);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
				
		Vector3 playerPos = CinemachineManager.Instance.MainCamera.WorldToScreenPoint(GameManager.Instance.Player.CenterPos);
		Vector3 buildingPos = CinemachineManager.Instance.MainCamera.WorldToScreenPoint(this.building.CenterPos);

		List<UI_ItemIcon> itemIcons = new List<UI_ItemIcon>();
		for(int i = 0; i < 5; i++) 
		{
			var itemIcon = UIManager.Instance.MakeSceneUISubItem<UI_ItemIcon>();
			itemIcon.SetInfo(itemSprite);
			itemIcon.transform.position = playerPos;
			itemIcons.Add(itemIcon);
		}
		
		foreach (var itemIcon in itemIcons)
		{			
			itemIcon.transform.DOMove(buildingPos, 1f)
				.SetEase(Ease.OutQuad)
				.OnComplete(() => ResourceManager.Instance.Destroy(itemIcon.gameObject));

			itemIcon.transform.DORotate(new Vector3(0f, 0f, 360), 1f, RotateMode.FastBeyond360)
				.SetEase(Ease.InOutQuad);
				
			yield return new WaitForSeconds(0.1f);
		}
		
		
		DOTween.To(() => int.Parse(itemAmountText.text), x => itemAmountText.text = ((int)x).ToString(), building.BuildingInfo.RemaningItemAmount, 0.5f)
			.SetEase(Ease.Linear);

		yield return new WaitForSeconds(0.5f);
		
		canvasGroup.alpha = 0;
		
		building.BuildComplete();
		CinemachineManager.Instance.SwitchFocusForSeconds(this.building.transform.position);
		yield return new WaitForSeconds(2f);
		
		building.ActiveInteractButton();
		
		int inventoryItemAmount = InventoryServerData.Instance.GetItemAmount(building.BuildingInfo.RequiredItemId);
		int useItemAmount = Mathf.Clamp(inventoryItemAmount, inventoryItemAmount, building.BuildingInfo.RemaningItemAmount);
		
		InventoryServerData.Instance.RemoveItem(building.BuildingInfo.RequiredItemId, useItemAmount);
		MyShopServerData.Instance.ContentEntityMinusRemaningItemAmount(BuildId, useItemAmount);
		
		OnCompletedBuilding?.Invoke();

		ResourceManager.Instance.Destroy(this.gameObject);
	}
}
