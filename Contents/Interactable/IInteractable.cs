

using UnityEngine;

public interface IInteractable
{
	public Bounds ColliderBounds { get; }	
	public float InteractableRange { get; }
	public float TargetingRange { get; }
	
	public bool InteractionLocked { get; }
	
	public void Interact();
}
