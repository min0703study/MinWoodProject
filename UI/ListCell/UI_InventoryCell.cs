using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_InventoryCell : UI_Base
{
	public int ItemId { get; set; }
	[SerializeField]
	Image icon;
	
	[SerializeField]
	TextMeshProUGUI countText;
	
	[SerializeField]
	GameObject countTextGO;
	
	private void Awake() {
	}
	public void SetInfo(int itemId, int count)
	{
		if(itemId == -1)
			return;
			
		ItemId = itemId;

		icon.gameObject.SetActive(true);
		countTextGO.SetActive(true);
		
		transform.localScale = Vector3.one;
		TableData.Item item = ItemTable.Instance.GetItemDataByItemId(itemId);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
		icon.sprite = itemSprite;
		
		countText.text = count.ToString();
	}

}
