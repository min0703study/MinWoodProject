using System;
using System.Collections;
using System.Linq;
using UnityEngine;

using MonsterState = Define.MonsterState;

using Random = UnityEngine.Random;

public class MonsterController : UnitController
{
	public TableData.Monster MonsterTableData { get; private set; }

	public MonsterState CurrentState { get; private set; } = MonsterState.Idle;
	
	[SerializeField]
	protected ProgressBar progressBar;
	
	[SerializeField]
	[Range(0.1f, 5.0f)]
	private float chaseDistance = 3.0f;

	[SerializeField]
	[Range(0.1f, 5.0f)]
	private float attackDistance = 1.0f;
	
	private string dieAniName = "Die";
	[SerializeField]
	private string dieClipName = "Slime_Die";

	public Action<MonsterController> DestroyAction { get; set; }
	public Action<MonsterController> OnChasePlayer;
	public Action<MonsterController> StoppedChasePlayer;
	
	public Action OnChasged;
	public Action<MonsterController> OnMonsterDead;
	protected override void Init()
	{
		base.Init();
		rBody2D = GetComponent<Rigidbody2D>();
	}

	public void SetInfo(TableData.Monster monsterTableData)
	{
		MonsterTableData = monsterTableData;
		Hp = monsterTableData.MaxHp;
	}

	void Update()
	{
		base.UpdatePosition();
		base.UpdateSpriteDir();

		switch (CurrentState)
		{
			case MonsterState.Idle:
				UpdateIdle();
				if (animator.GetBool("ToIdleTrigger") == false)
				{
					animator.ResetTrigger("ToMoveTrigger");
					animator.SetTrigger("ToIdleTrigger");
				};

				break;
			case MonsterState.Chase:
				UpdateChase();

				if (animator.GetBool("ToMoveTrigger") == false)
				{
					animator.ResetTrigger("ToIdleTrigger");
					animator.SetTrigger("ToMoveTrigger");
				};

				break;
			case MonsterState.Attack:
				UpdateAttack();
				break;
		}
	}

	void UpdateIdle()
	{
		//거리 계산
		float distanceFromPlayer = Vector3.Distance(GameManager.Instance.Player.CenterPos, transform.position);
		if (distanceFromPlayer <= chaseDistance)
		{
			CurrentState = MonsterState.Chase;
			OnChasePlayer.Invoke(this);
		}
	}

	void UpdateChase()
	{
		var dirVector = GameManager.Instance.Player.CenterPos - CenterPos;
		moveDir = dirVector.normalized;

		// To Attack MonsterState
		float distanceFromPlayer = Vector3.Distance(GameManager.Instance.Player.CenterPos, transform.position);
		if (distanceFromPlayer <= attackDistance)
		{
			this.moveDir = Vector3.zero;
			CurrentState = MonsterState.Attack;
		}
		else if (distanceFromPlayer > chaseDistance)
		{
			StoppedChasePlayer?.Invoke(this);
			this.moveDir = Vector3.zero;
			CurrentState = MonsterState.Idle;
		}
	}

	void UpdateAttack()
	{
		float distanceFromPlayer = Vector3.Distance(GameManager.Instance.Player.CenterPos, transform.position);
		if (distanceFromPlayer > attackDistance)
		{
			CurrentState = MonsterState.Chase;
		} else 
		{
			PerformAttack(GameManager.Instance.Player);
		}
	}

	// 공격
	public virtual void PerformAttack(UnitController target) 
	{
		//target?.OnDamaged(this, MonsterTableData.AtkPower);
	}

	//공격 받음
	public override void OnDamaged(BaseController attacker, float power = 1, bool knockBack = false)
	{
		if(CurrentState == MonsterState.OnDamaged) 
			return;
		
		CurrentState = MonsterState.OnDamaged;

		float totalDamage = power;
		bool isCritical = false;
		if (Random.Range(0, 1000) <= (UserServerData.Instance.CriRate * 10))
		{
			totalDamage  = power * UserServerData.Instance.CriDamage;
			isCritical = true;
		}
	
		Hp -= totalDamage;
		
		progressBar.SetActive(true);
		progressBar.SetProgressBarValue(MonsterTableData.MaxHp, Hp);
		MapManager.Instance.Map.ShowDamageFont(CenterPos, totalDamage, 0, transform, isCritical);
		MapManager.Instance.Map.ShowShootEffect(CenterPos, transform);

		if (Hp <= 0)
		{
			progressBar.SetActive(false);
			CurrentState = MonsterState.Dead;
			StartCoroutine(CoDie());
		} else 
		{
			StartCoroutine(CoOnDamaged(attacker, totalDamage, knockBack));
		}
	}

	// 충돌시
	public void OnTriggerEnter2D(Collider2D colllider)
	{
		PlayerController target = colllider.gameObject.GetComponent<PlayerController>();

		if (target == null)
			return;

		this.PerformAttack(target);
	}
	
	IEnumerator CoOnDamaged(BaseController attacker, float damage = 1, bool knockBack = false)
	{
		StartCoroutine(CoFlash());
		if(knockBack) 
		{
			yield return StartCoroutine(CoKnockBack(attacker.transform, 2f));
		}
		
		CurrentState = MonsterState.Chase;
	}

	IEnumerator CoDie()
	{
		animator.Play(dieAniName);
		var dieAniClip = animationClips.Where(clip => clip.name == dieClipName).First();
		yield return new WaitForSeconds(dieAniClip.length);
		
		QuestServerData.Instance.UpdateQuestCheck("kill_monster", MonsterTableData.Id, 1);
		if(MonsterTableData.DropItemId != -1) 
		{
			MapManager.Instance.Map.GenerateDropItem(gameObject.transform.position, MonsterTableData.DropItemId);
		}

		OnMonsterDead?.Invoke(this);
		DestroyMySelf();
		
	}

	public void DestroyMySelf()
	{
		if (DestroyAction == null)
		{
			GameObject.Destroy(this);
		}

		DestroyAction?.Invoke(this);
	}
	
	public void PlayDyingSound() 
	{
		SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_SlimeDie");
	}
	
	protected void OnDrawGizmos() 
	{
		// if(UnityEditor.Selection.activeObject == gameObject)
		// {
		// 	Gizmos.color = Color.yellow;
		// 	Gizmos.DrawWireSphere(transform.position, chaseDistance);

		// 	Gizmos.color = Color.red;
		// 	Gizmos.DrawWireSphere(transform.position, attackDistance);
			
		// 	Gizmos.color = Color.white;
		// }    
	}
}
