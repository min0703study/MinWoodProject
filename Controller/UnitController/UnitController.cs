using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MapEntity
{		
	#region Component
	protected Rigidbody2D rBody2D;
	protected Animator animator;
	protected AnimationClip[] animationClips;
	protected SpriteRenderer spriteRenderer;
	#endregion
	protected SpeechBalloonController speechBalloon;
	protected Vector2 moveDir = Vector2.zero;
	
	public bool IsFlipX { get; protected set;} = false;

	[SerializeField]
	[Range(1.0f, 5.0f)]
	protected float moveSpeed = 1.0f;
	
	public virtual float Hp { get; set; }
	
	protected override void Init()
	{
		base.Init();
		
		rBody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		
		if(animator != null) 
		{
			animationClips = animator.runtimeAnimatorController.animationClips;
		}
		
		//말풍선
		var speechBalloonGO = ResourceManager.Instance.Instantiate("SpeechBalloon", gameObject.transform);
		speechBalloon = Util.GetOrAddComponent<SpeechBalloonController>(speechBalloonGO);
		speechBalloon.HideSpeechBallon();
	}

	protected void UpdatePosition()
	{
		if(moveDir == Vector2.zero)
			return;
			
		// 이동
		Vector3 addedDir = moveDir * moveSpeed * Time.deltaTime;
		transform.position += addedDir;
	}
	
	protected virtual void UpdateSpriteDir()
	{
		if(moveDir == Vector2.zero)
			return;

		//회전 - transfom 추출 방식
		Vector3 spriteDir = Vector3.Cross(Vector2.up, moveDir);
		
		Vector3 currentScale = gameObject.transform.localScale;
		IsFlipX = spriteDir.z > 0;
		currentScale.x = 1 * (IsFlipX ? -1 : 1);
		gameObject.transform.localScale = currentScale;
	}

	public virtual void OnDamaged(BaseController attacker, float damage = 0, bool knockBack = false)
	{
		Hp -= damage;
	}

	public virtual void OnDead()
	{

	}
	
	protected IEnumerator CoGoToTarget(MapEntity targetEntity)
	{		
		List<Vector3> pathToTarget = MapManager.Instance.Map.FindPathToPosition(CenterPos, targetEntity.CenterPos);
		foreach (var position in pathToTarget) 
		{	
			var dirVector = position - CenterPos;
			while (dirVector.magnitude > 0.5f) 
			{
				this.moveDir = dirVector.normalized;	
				dirVector = position - CenterPos;
				yield return new WaitForNextFrameUnit();
			}
		}
		
		this.moveDir = Vector3.zero;
	}
	
	protected IEnumerator CoKnockBack(Transform attackTransform, float nockBackThrust = 1f)
	{
		Vector2 difference = (transform.position - attackTransform.position).normalized * nockBackThrust;
		rBody2D.AddForce(difference, ForceMode2D.Impulse);		
		yield return new WaitForSeconds(0.2f);
		rBody2D.velocity = Vector2.zero;
		yield return new WaitForSeconds(0.3f);
	}
	
	protected IEnumerator CoFlash()
	{
		spriteRenderer.material.SetInt("_SolidColorMode", 1);
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.material.SetInt("_SolidColorMode", 0);
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.material.SetInt("_SolidColorMode", 1);
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.material.SetInt("_SolidColorMode", 0);
	}
}
