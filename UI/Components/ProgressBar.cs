using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : BaseController
{
	[SerializeField]
	GameObject valueGO;
	
	protected override void Init()
	{
		base.Init();
	}
	
	public void SetActive(bool value) 
	{
		gameObject.SetActive(value);
	}
	
	public void SetProgressBarValue(float maxValue, float curValue) 
	{
		float changeValue = curValue / maxValue;
		
		var originalScale = valueGO.transform.localScale;
		originalScale.x = 1 - changeValue;
		valueGO.transform.localScale = originalScale;
	}
}
