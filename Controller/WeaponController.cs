
using System.Collections;
using UnityEngine;

public class WeaponController : BaseController
{	
	public void SetActive (bool isActive) => gameObject.SetActive(isActive);
	
	[SerializeField]
	protected float weaponCooldown = 1.3f;
	
	[SerializeField]
	protected float startAttackTime = 0.2f;
	
	[SerializeField]
	protected float attackRange = 3.0f;
	public float AttackRange { get { return attackRange; } }
	
	public bool isAttacking = false;
	public bool isActivating = false;

	protected override void Init()
	{
		base.Init();
	}
	
	public void BeginWeaponUsage()
	{
		isActivating = true;
		gameObject.SetActive(true);
	}
			
	public void EndWeaponUsage()
	{
		isActivating = false;
		gameObject.SetActive(false);
	}

	public virtual void Attack() {}
	
	public virtual void UseSkill() {}
	
	protected void OnDrawGizmos() 
	{
		// if(UnityEditor.Selection.activeObject == gameObject)
		// {
		// 	Gizmos.color = Color.yellow;
		// 	Gizmos.DrawWireSphere(transform.position, attackRange);
		// }    
	}
}
