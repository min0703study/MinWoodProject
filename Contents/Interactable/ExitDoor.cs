using System;
using UnityEngine;

public class ExitDoor : MapEntity, IInteractable
{
	public Bounds ColliderBounds
	{
		get
		{
			var bounds = Collider2DBounds;
			bounds.Expand(1.0f);
			return bounds;
		}
	}
	
	public float InteractableRange => 1.0f;
	public float TargetingRange => 3.0f;
	public bool InteractionLocked => false;
	public void Interact() 
	{
		UIManager.Instance.ShowPopupUI<UI_WorldMapPopup>();
	}
}
