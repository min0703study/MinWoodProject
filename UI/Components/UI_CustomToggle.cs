using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_CustomToggle : BaseController
{	
	Toggle toggle;
	
	[SerializeField]
	GameObject onToggleGO, offToggleGO;

	protected override void Init()
	{
		base.Init();
		
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener((bool isOn) => this.SwitchToggle(isOn));
		SwitchToggle(toggle.isOn);
	}

	public void SwitchToggle(bool isOn) 
	{
		onToggleGO.SetActive(isOn);
		offToggleGO.SetActive(!isOn);
	}
	
	public void AddOnValueChangedListener(UnityAction<bool> action) 
	{
		toggle.onValueChanged.AddListener(action);
	}
}
