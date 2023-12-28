using UnityEngine;

public interface IInteractableWithCat
{
	public Bounds InteractiveBounds { get; }
	public void Interact(CatController cat);
}
