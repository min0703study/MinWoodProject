using System;
using ServerData;
using UnityEngine;

public class CatCushion : MapEntity, IInteractableWithCat
{
	[SerializeField]
	Transform CatSleepPosTransform;
	
	protected override void Init()
	{
		base.Init();
	}

	public Bounds InteractiveBounds
	{
		get
		{
			var bounds = Collider2DBounds;
			bounds.Expand(1.0f);
			return bounds;
		}
	}

	public void Interact(CatController cat)
	{
		cat.transform.position = CatSleepPosTransform.position;
		cat.Sleep();
	}
}
