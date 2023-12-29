using System.Collections;
using UnityEngine;

using EnvEntityState = Define.EnvEntityState;

public class EnvEntity : MapEntity, IInteractable
{
	public TableData.EnvEntity EnvEntityTableData;
	
	public EnvEntityState CurrentState { get; set; } = EnvEntityState.Idle;
	
	protected float curHP = 100;
			
	[SerializeField]
	protected ProgressBar progressBar;
	
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
	public float TargetingRange => 5.0f;
	
	protected bool interactionLocked = false;
	public bool InteractionLocked => interactionLocked;
	
	protected override void Init()
	{
		base.Init();
	}
	
	public virtual void SetInfo(int envObjectId) {}
		
	public virtual void Interact() {}
	
	public virtual void Hit(float power) {}
	
	protected void OnDrawGizmos() 
	{
		// if(UnityEditor.Selection.activeObject == gameObject)
		// {
		// 	Gizmos.color = Color.red;
		// 	Gizmos.DrawWireSphere(transform.position, TargetingRange);
			
		// 	Gizmos.color = Color.yellow;
		// 	Gizmos.DrawWireSphere(transform.position, InteractableRange);
		// }    
	}
}
