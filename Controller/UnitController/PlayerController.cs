using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

using PlayerState = Define.PlayerState;

public class PlayerController : UnitController
{
	
	[SerializeField]
	[Range(4.0f, 7.0f)]
	private float dashSpeed = 4.0f;
	
	#region info
	public float InteractableRange { get; set; } = 2f;	
	private bool IsInBattleMode = false;
	[SerializeField]
	float dropItemRange = 3.0f;
	#endregion
	
	public PlayerState CurrentState { get; set; } = PlayerState.Idle;
	
	[SerializeField]
	public PlayerAvatar avatar;
	
	#region AnimationName
	private int isIdleToHash = Animator.StringToHash("isIdle");
	private int isMovingToHash = Animator.StringToHash("isMoving");
	private string miningAniName = "Mining";
	private string idleAniName = "Idle";
	#endregion
	
	[SerializeField]
	private GameObject toolsRootGO, weaponRootGO;

	[SerializeField]
	private Sword sword;
	
	[SerializeField]
	private Riffle riffle;

	private Dictionary<Define.WeaponMode, WeaponController> weaponDict = new Dictionary<Define.WeaponMode, WeaponController>();

	private Dictionary<Define.ToolType, ToolController> toolDict = new Dictionary<Define.ToolType, ToolController>();
	
	private Coroutine activeCoroutine;
	
	private bool tryingChangeStateManually;

	public void SetPosition(Vector3 changePosition) => transform.position = changePosition;

	private Define.WeaponMode weaponMode;
	
	protected override void Init()
	{
		base.Init();

		GameObject pickGO = ResourceManager.Instance.Instantiate("NormalPick", toolsRootGO.transform);
		toolDict.Add(Define.ToolType.Pick, pickGO.GetComponent<ToolController>());
		
		GameObject axeGO = ResourceManager.Instance.Instantiate("NormalAxe", toolsRootGO.transform);
		toolDict.Add(Define.ToolType.Axe, axeGO.GetComponent<ToolController>());	
		
		GameObject swordGO = ResourceManager.Instance.Instantiate("Sword", weaponRootGO.transform);
		weaponDict.Add(Define.WeaponMode.Sword , swordGO.GetComponent<Sword>());
		
		GameObject riffleGO = ResourceManager.Instance.Instantiate("Riffle", weaponRootGO.transform);
		weaponDict.Add(Define.WeaponMode.Riffle , riffleGO.GetComponent<Riffle>());
	}
	
	private void Start() {
		weaponMode = Define.WeaponMode.None;
		ChangeWeaponMode(weaponMode);
	}

	protected override void UpdateSpriteDir()
	{
		if(moveDir == Vector2.zero)
			return;

		Vector3 spriteDir = Vector3.Cross(Vector2.up, moveDir);
		
		Vector3 currentScale = avatar.transform.localScale;
		IsFlipX = spriteDir.z > 0;
		currentScale.x = 1 * (IsFlipX ? -1 : 1);	
		avatar.transform.localScale = currentScale;
	}

	protected void Update()
	{	
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if(weaponMode == Define.WeaponMode.Riffle) 
			{
				riffle.Attack();
			} else 
			{
				sword.Attack();
			}
		}

		base.UpdatePosition();
		UpdateSpriteDir();
		
		CheckBeingChasedByMonster();
		CheckNeareastInteractiveObject();
		CheckCollisionWithDropItem();
		
		if(tryingChangeStateManually == false) 
		{
			updateTest();
		}
		
		switch(CurrentState)
		{
			case PlayerState.Idle:
				if(animator.GetBool("ToIdleTrigger") == false) 
				{
					animator.ResetTrigger("ToMoveTrigger");
					animator.SetTrigger("ToIdleTrigger");
				};
			break;
			case PlayerState.Moving:
			case PlayerState.GoToTarget:
				if(animator.GetBool("ToMoveTrigger") == false) 
				{
					animator.ResetTrigger("ToIdleTrigger");
					animator.SetTrigger("ToMoveTrigger");
				};
				break;
			default:
				break;
		}
	}

	public void updateTest() 
	{
		if(CurrentState == PlayerState.Mining)
			return;
			
		var target = GameManager.Instance.TargetMarker.Target;
		if(target == null)
			return;
			
		if(IsInBattleMode) 
		{
			var targetMonster = target as MonsterController;
			if(targetMonster == null)
				return;
				
			var dirVector = targetMonster.transform.position - transform.position;
			if(dirVector.magnitude < weaponDict[weaponMode].AttackRange) 
			{
				CurrentState = PlayerState.Idle;
				this.moveDir = Vector3.zero;
				
				weaponDict[weaponMode].Attack();
			}
			else 
			{
				CurrentState = PlayerState.Moving;
				this.moveDir = dirVector.normalized;	
			} 
		} else 
		{
			List<Vector3> pathToTarget = MapManager.Instance.Map.FindPathToPosition(CenterPos, target.CenterPos);
			//var dirVector = pathToTarget.First() - CenterPos;
			var dirVector = target.transform.position - transform.position;
			
			if(target is IInteractable) 
			{
				if(dirVector.magnitude < ((IInteractable)target).InteractableRange) 
				{
					CurrentState = PlayerState.Idle;
					this.moveDir = Vector3.zero;
					
					((IInteractable)target).Interact();
				} 
				else 
				{
					CurrentState = PlayerState.Moving;
					this.moveDir = dirVector.normalized;	
				}
			}
		}
	}

	private void CheckBeingChasedByMonster()
	{			
		if(MapManager.Instance.Map == null || MapManager.Instance.Map.Monsters == null)
			return;
			
		if(MapManager.Instance.Map.ChasingMonsters.Count > 0) 
		{
			if (IsInBattleMode == false) 
			{
				IsInBattleMode = true;
			}

			MapManager.Instance.Map.SetTargetNeareastMonster();	
		} else 
		{
			IsInBattleMode = false;
		}
	}

	private void CheckCollisionWithDropItem()
	{
		if(MapManager.Instance.Map == null || MapManager.Instance.Map.Monsters == null)
			return;
			
		var dropItemsOnMap = MapManager.Instance.Map.DropItems;
		foreach (DropItemController item in dropItemsOnMap)
		{
			Vector3 dir = item.transform.position - transform.position;
			if (dir.magnitude <= dropItemRange)
			{
				item.GetItem();
			}
		}
	}
	
	private void CheckNeareastInteractiveObject()
	{
		if(IsInBattleMode) 
			return;
		
		if(MapManager.Instance.Map == null || MapManager.Instance.Map.Monsters == null)
			return;
		
		MapManager.Instance.Map.SetTargetNeareastInteractableEntity();	
	}

	public override void OnDamaged(BaseController attacker, float damage = 1, bool knockBack = false)
	{
		if(CurrentState == PlayerState.OnDamaged)
			return;
			
		base.OnDamaged(attacker, damage);
		CurrentState = PlayerState.OnDamaged;

		StartCoroutine(CoOnDamaged(attacker, damage));
	}
	
	public void InteractionNeareastObject()
	{
		var interactableEntity = MapManager.Instance.Map.GetNeareastInteractableEntity();
		if(interactableEntity == null)
			return;
			
		CurrentState = PlayerState.GoToTarget;
	}
	
	public void ChangeWeaponMode(Define.WeaponMode changeWeapon)  
	{
		if(weaponMode == Define.WeaponMode.None) 
		{
			foreach(var weapon in weaponDict.Values) 
			{
				weapon.EndWeaponUsage();
			}
		} else 
		{
			weaponDict[changeWeapon].BeginWeaponUsage();
			weaponDict[weaponMode].EndWeaponUsage();
		}

		weaponMode = changeWeapon;
	}

	public void MineNodes(EnvEntity envEntity) 
	{
		if(IsInBattleMode)
			return;
			
		if(CurrentState == PlayerState.Mining)
			return;
	
		CurrentState = PlayerState.Mining;		
		activeCoroutine = StartCoroutine(CoHarvesting(Define.ToolType.Pick, envEntity));
	}
	
	public void ChopTree(EnvEntity envEntity) 
	{	
		if(CurrentState == PlayerState.InBattle)
			return;
			
		if(CurrentState == PlayerState.Mining)
			return;
			
		CurrentState = PlayerState.Mining;		
		activeCoroutine = StartCoroutine(CoHarvesting(Define.ToolType.Axe, envEntity));
	}
	
	public void SwingSword() 
	{		
		activeCoroutine = StartCoroutine(CoSwingSword());
	}
	
	public void SwingSwordSkill() 
	{	
		activeCoroutine = StartCoroutine(CoSwingSwordSkill());
	}
	
	public void Dash() 
	{	
		CurrentState = PlayerState.Dashing;		
		activeCoroutine = StartCoroutine(CoDashing());
	}
	
	public void AddCoin(int coin)
	{
		UserServerData.Instance.AddCoin(coin);
	}

	public void AddItem(int itemId, int itemCount = 1)
	{
		if(itemId == 103010020) 
		{
			AddCoin(itemCount);
			return;
		}
		InventoryServerData.Instance.AddItem(itemId, itemCount);
		QuestServerData.Instance.UpdateQuestCheck("collect_item", itemId, itemCount);
		
		var item = ItemTable.Instance.GetItemDataByItemId(itemId);
		var itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
		var toastMessage = UIManager.Instance.MakeSceneUISubItem<UI_CollectedItemToast>();
		toastMessage.ShowToastMessage(itemSprite, item.Name, itemCount);
	}
	
	public void AddEXP(int exp)
	{
		var levelUpExp = LevelExpTable.Instance.GetRequiredExpForLevel(UserServerData.Instance.Level + 1);
		var expNeededForLevelUp = levelUpExp - (UserServerData.Instance.EXP + exp);
		
		if(expNeededForLevelUp <= 0) 
		{
			UserServerData.Instance.LevelUp(math.abs(exp));
		} else 
		{
			UserServerData.Instance.AddEXP(exp);
		}
	}	

	public void ManualSetMoveDir(Vector3 moveDir)
	{
		tryingChangeStateManually = moveDir != Vector3.zero;

		if(activeCoroutine != null)
			return;
		
		if(moveDir == Vector3.zero) 
		{
			CurrentState = PlayerState.Idle;
		} else 
		{
			CurrentState = PlayerState.Moving;
		}

		this.moveDir = moveDir;
	}
	
	private IEnumerator CoInteractToTarget(IInteractable target)
	{	
		CurrentState = PlayerState.GoToTarget;
		List<Vector3> pathToTarget = MapManager.Instance.Map.FindPathToPosition(CenterPos, target.ColliderBounds.center);
		foreach (var position in pathToTarget) 
		{	
			var dirVector = position - CenterPos;
			while (dirVector.magnitude > 0.5f) 
			{
				if(dirVector.magnitude < ((IInteractable)target).InteractableRange) 
				{
					break;
				}
				this.moveDir = dirVector.normalized;	
				dirVector = position - CenterPos;
				yield return new WaitForNextFrameUnit();
			}
			
			if(tryingChangeStateManually) 
			{
				this.moveDir = Vector3.zero;
				CurrentState = PlayerState.Idle;
				activeCoroutine = null;
				yield break;
			}
		}
		
		this.moveDir = Vector3.zero;
		CurrentState = PlayerState.Idle;
		yield return new WaitForNextFrameUnit();
		
		activeCoroutine = null;
		target.Interact();
	
		yield break;
	}
	
	IEnumerator CoOnDamaged(BaseController attacker, float damage = 1)
	{		
		//StartCoroutine(CoFlash());
		//yield return StartCoroutine(CoKnockBack(attacker.transform, damage));
		CurrentState = PlayerState.InBattle;
		yield return null;
	}
	
	private IEnumerator CoSwingSword()
	{
		animator.Play("SwingSword");
	
		var miningAniClip = animationClips.Where(clip => clip.name == "Player_SwingSword").First();
		yield return new WaitForSeconds(miningAniClip.length);
		
		CurrentState = PlayerState.Idle;
		animator.Play(idleAniName);
		
		activeCoroutine = null;
	}
	
	private IEnumerator CoSwingSwordSkill()
	{
		animator.Play("SwingSwordSkill");
	
		MapManager.Instance.Map.ShowSkillEffect(CenterPos, null);
		CinemachineManager.Instance.ShakeCamera();
		var miningAniClip = animationClips.Where(clip => clip.name == "Player_SwingSwordSkill").First();
		
		yield return new WaitForSeconds(miningAniClip.length);
		
		CurrentState = PlayerState.Idle;
		animator.Play(idleAniName);
		
		activeCoroutine = null;
	}
	
	private IEnumerator CoDashing()
	{
		animator.Play("Dashing");
	
		var miningAniClip = animationClips.Where(clip => clip.name == "Player_Dashing").First();
		var originalMoveSpeed = moveSpeed;
		moveSpeed = dashSpeed;
		
		yield return new WaitForSeconds(miningAniClip.length);
		
		moveSpeed = originalMoveSpeed;
		CurrentState = PlayerState.Idle;
		animator.Play(idleAniName);
		
		activeCoroutine = null;
	}
	int i = 0;
	private IEnumerator CoHarvesting(Define.ToolType useToolType, EnvEntity envEntity)
	{

		ToolController useTool = toolDict[useToolType];
		
		while (envEntity.CurrentState != Define.EnvEntityState.Harvested) 
		{			
			useTool.BeginToolUsage();	
			animator.Play(miningAniName);
		
			var miningAniClip = animationClips.Where(clip => clip.name == "Player_Mining").First();
			yield return new WaitForSeconds(miningAniClip.length);
			
			envEntity.Hit(useTool.Power);
			
			while(envEntity.CurrentState == Define.EnvEntityState.OnDamaged) 
			{
				yield return new WaitForSeconds(0.1f);
			}
			
			useTool.EndToolUsage();	
			
			if(tryingChangeStateManually) 
			{
				break;
			}
		
			animator.Play(idleAniName);
			yield return new WaitForSeconds(0.3f);
		}
		
		CurrentState = PlayerState.Idle;
		animator.Play(idleAniName);
		AddEXP(5);
		
		activeCoroutine = null;
	}
	
	public void PlayPickaxeSound() 
	{
		CinemachineManager.Instance.ShakeCamera();
		if(toolDict[Define.ToolType.Pick].IsUsingTool) 
		{
			SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_Pickaxe");	
		} else if(toolDict[Define.ToolType.Axe].IsUsingTool)
		{
			SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_Axe");	
		}
	}
	
	public void PlayFootStepSound() 
	{
		SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_FootStep");
	}
}
