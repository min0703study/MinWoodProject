using UnityEngine;

public class ToNextFloorDoor : MapEntity,  IInteractable
{
	[SerializeField]
	GameObject openDoorGO;
	
	[SerializeField]
	GameObject closeDoorGO;
	
	public bool IsOpen { get; private set; }
	
	public float InteractableRange => 1.0f;
	public float TargetingRange => 3.0f;
	public bool InteractionLocked => false;
	public void SetOpenState(bool isOpenValue) 
	{
		IsOpen = isOpenValue;
		
		if(IsOpen == true) 
		{
			openDoorGO.SetActive(true);
			closeDoorGO.SetActive(false);
		} else 
		{
			openDoorGO.SetActive(false);
			closeDoorGO.SetActive(true);
		}
	}
	
	public Bounds ColliderBounds
	{
		get
		{
			var bounds = Collider2DBounds;
			bounds.Expand(1.0f);
			return bounds;
		}
	}
	
	public void Interact() 
	{
		if(IsOpen) 
		{
		}
	}
	
}
