using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UI_ToastBase : UI_Base
{
	[SerializeField]
	protected float dulation = 2.0f;
	
	protected CanvasGroup canvasGroup;
	
	protected void Awake()
	{		
		canvasGroup = GetComponent<CanvasGroup>();
		UIManager.Instance.SetCanvas(gameObject, true);
	}
}
