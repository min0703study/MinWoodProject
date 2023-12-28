using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TableData;
using System;
using DG.Tweening;
using Mono.Cecil;

public class UI_CollectedItemToast : UI_ToastBase
{
	[SerializeField]
	Image itemIcon;
	
	[SerializeField]
	TextMeshProUGUI itemName;
	
	[SerializeField]
	TextMeshProUGUI amount;

	protected override void Init()
	{
		base.Init();
	}

	public void ShowToastMessage(Sprite itemSprite, String itemName, int amount)
	{
		if(canvasGroup == null)
			return;
		this.itemIcon.sprite = itemSprite;
		this.itemName.text = itemName;
		this.amount.text = amount.ToString();
		
		StartCoroutine(CoShowingToastMessage());
	}
	
	public IEnumerator CoShowingToastMessage() 
	{
		canvasGroup.alpha = 0;
		DOTween.To(()=> 0.0f, x => canvasGroup.alpha = x, 1.0f, 2f);
		yield return new WaitForSeconds(dulation);
		DOTween.To(()=> 1, x => canvasGroup.alpha = x, 0, 1f);
		ResourceManager.Instance.Destroy(this.gameObject);
	}
}
