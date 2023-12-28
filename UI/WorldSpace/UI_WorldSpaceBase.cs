using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System;

public class UI_WordSpaceBase : UI_Base
{
	protected Transform worldSpaceTransform;
	
	bool isActive = true;
	
	protected override void Init()
	{
		base.Init();
		UIManager.Instance.SetCanvas(gameObject, true);
	}
	
	private void Update() {
		if(isActive) 
		{
			Vector3 screenPos = CinemachineManager.Instance.MainCamera.WorldToScreenPoint(worldSpaceTransform.position);
			transform.position = screenPos;
		}
	}
}
