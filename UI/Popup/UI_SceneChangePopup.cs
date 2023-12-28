using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SceneChangePopup : UI_PopupBase
{
	Animator animator;
	Action callback;
	Define.Scene nextScene;

	protected override void Init() 
	{
		base.Init();
		
		animator = GetComponent<Animator>();   
	}
	
	public void SetInfo(Define.Scene nextScene, Action callback)
	{
		transform.localScale = Vector3.one;
		this.callback = callback;
		this.nextScene = nextScene;
		
		StartCoroutine(OnAnimationComplete());
	}
	
	IEnumerator OnAnimationComplete()
	{
		yield return new WaitForSeconds(1f);
		callback.Invoke();
	}
}
