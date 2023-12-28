using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UI_ItemIcon : UI_Base
{
	public int ItemId { get; set; }
	[SerializeField]
	Image icon;
	
	private void Awake() {
	}
	public void SetInfo(Sprite sprite)
	{
		icon.sprite = sprite;
	}

}
