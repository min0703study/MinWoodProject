using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System;

public class UI_PopupBase : UI_Base
{
	public Action OnClosedPopup { get; set; }
	protected override void Init()
	{
		base.Init();
		UIManager.Instance.SetCanvas(gameObject, true);
	}
	
	public virtual void ClosePopupUI()
	{
		UIManager.Instance.ClosePopupUI(this);
		OnClosedPopup?.Invoke();
	}
	
	public void PopupOpenAnimation(GameObject contentObject)
	{
		contentObject.transform.localScale = new Vector3(0f, 0f, 1f);
		contentObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InSine);
	}
}
