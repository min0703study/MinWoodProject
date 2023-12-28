using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_SceneBase : UI_Base
{
   protected override void Init()
	{
		base.Init();
	}
	
	private void Start() {
		UIManager.Instance.SetCanvas(gameObject, false);
	}
}
