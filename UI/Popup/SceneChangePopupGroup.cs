using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangePopupGroup : MonoBehaviour
{
	Action callback;
	
	[SerializeField]
	Image stencilImage;
	
	public void SetInfo(Action callback)
	{
		this.callback = callback;
		
		stencilImage.transform.DOScale(new Vector3(1, 1, 0), 0.5f).OnComplete(()=>
		{
			callback?.Invoke();
		});
	}
	
	public void SceneChangedSuccess()
	{
		stencilImage.transform.DOScale(new Vector3(110, 110, 0), 0.5f).OnComplete(()=>
		{
			ResourceManager.Instance.Destroy(this.gameObject);
		});
	}
}
