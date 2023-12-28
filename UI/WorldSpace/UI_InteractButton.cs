using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractButton : UI_WordSpaceBase
{
	Building building;
	
	CustomerController customer;
	
	[SerializeField]
	public Button button;
	
	protected override void Init()
	{
		base.Init();
		
		button.onClick.AddListener(OnClickInteractButton);
	}
	
	public void SetInfo(Transform worldSpaceTransform, Building building) 
	{
		this.building = building;
		this.worldSpaceTransform = worldSpaceTransform;
	}
	
	public void SetInfo(Transform worldSpaceTransform, CustomerController customer) 
	{
		this.customer = customer;
		this.worldSpaceTransform = worldSpaceTransform;
	}
	
	
	public void SetActive(bool value) 
	{
		gameObject.SetActive(value);
	}
	
	public void OnClickInteractButton() 
	{
		if(building != null) 
		{
			building.OpenControlPopup();
		} else 
		{
			customer.OpenControlPopup();
		}
	}
	
}
