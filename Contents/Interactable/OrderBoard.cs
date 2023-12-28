using System;
using UnityEngine;
using UnityEngine.UI;

public class OrderBoard : Building
{	
	protected override void Init()
	{
		base.Init();
	}
	
	public override void OpenControlPopup() 
	{
		SoundManager.Instance.PlayButtonPressSound();
		UIManager.Instance.ShowPopupUI<UI_OrderManagerPopup>();
	}

	public override void BuildComplete()
	{
		base.BuildComplete();
		
		MyShopServerData.Instance.OnVisitCustomer();
	}
	
}
