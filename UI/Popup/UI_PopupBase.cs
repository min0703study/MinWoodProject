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
		contentObject.transform.localScale = new Vector3(0.8f,0.8f,1);
		contentObject.transform.DOScale(10f, 0.1f).SetEase(Ease.InOutBack).SetUpdate(true);
	}

}
