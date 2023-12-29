using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponController
{
	private Animator animator;
	private Collider2D col2D;

	protected override void Init()
	{
		base.Init();
		
		weaponCooldown = 1.2f;
		
		animator = GetComponent<Animator>();	
		col2D = GetComponent<Collider2D>();
		
		col2D.enabled = false;
		isAttacking = false;
	}

	// Update is called once per frame
	void Update()
	{
		if(isActivating) 
		{
			Vector3 currentScale = transform.localScale;
			currentScale.x = 1 * (GameManager.Instance.Player.IsFlipX ? -1 : 1);
			transform.localScale = currentScale;
		}
	}
	
	public override void Attack() 
	{
		if(isAttacking)
			return;
		
		GameManager.Instance.Player.SwingSword();
			
		isAttacking = true;
	
		StartCoroutine(CoOnCooldown());
	}
	
	public override void UseSkill() 
	{
		if(isAttacking)
			return;
		
		GameManager.Instance.Player.SwingSwordSkill();
			
		isAttacking = true;
		col2D.enabled = true;

		StartCoroutine(CoOnCooldown());
	}
	
	private void OnTriggerEnter2D(Collider2D collisionData)
	{
		UnitController unit = collisionData.GetComponent<UnitController>();

		if (collisionData.gameObject.layer == LayerMask.NameToLayer("Monster"))
		{
			if(unit)
			unit?.OnDamaged(GameManager.Instance.Player, UserServerData.Instance.AtkValue, 1);
		}
	}
	
	private void CompletedAttackAnimaton()
	{
		animator.ResetTrigger("ToAttackTrigger");
		animator.SetTrigger("ToEntryTrigger");
		col2D.enabled = false;
	}
	
	private void PlaySwingSound()
	{
		SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_SwordSwing");
	}
		
	public IEnumerator CoOnCooldown() 
	{
		yield return new WaitForSeconds(startAttackTime);
		col2D.enabled = true;
		yield return new WaitForSeconds(weaponCooldown);
		col2D.enabled = false;
		yield return new WaitForSeconds(0.1f);
		isAttacking = false; 
		
	}
}
