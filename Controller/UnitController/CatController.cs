using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using CatState = Define.CatState;

public class CatController : UnitController
{
	#region info
	private CatState catState = CatState.Idle;
	
	#endregion
	
	protected override void Init()
	{
		base.Init();

	}
	
	private void Start() {
				
		if(catState == CatState.Idle) 
		{
			catState = CatState.GoToTarget;
			StartCoroutine(CoInitTest());
		}
	}

	private void Update()
	{
		moveSpeed = 1.0f;
		UpdatePosition();
		UpdateSpriteDir();
		
		switch(catState)
		{
			case CatState.Idle:
				if(animator.GetBool("ToIdleTrigger") == false) 
				{
					animator.ResetTrigger("ToSleepTrigger");
					animator.ResetTrigger("ToWakeUpTrigger");
					animator.ResetTrigger("ToWalkTrigger");
					animator.SetTrigger("ToIdleTrigger");
				};
				break;
			case CatState.Moving:
			case CatState.GoToTarget:
				if(animator.GetBool("ToWalkTrigger") == false) 
				{
					animator.ResetTrigger("ToSleepTrigger");
					animator.ResetTrigger("ToWakeUpTrigger");
					animator.ResetTrigger("ToIdleTrigger");
					animator.SetTrigger("ToWalkTrigger");
				};
				break;
			case CatState.Sleeping:
				if(animator.GetBool("ToSleepTrigger") == false) 
				{
					animator.ResetTrigger("ToIdleTrigger");
					animator.ResetTrigger("ToWakeUpTrigger");
					animator.ResetTrigger("ToWalkTrigger");
					animator.SetTrigger("ToSleepTrigger");
				};
				break;
			case CatState.WakeUp:
				if(animator.GetBool("ToWakeUpTrigger") == false) 
				{
					animator.ResetTrigger("ToIdleTrigger");
					animator.ResetTrigger("ToSleepTrigger");
					animator.ResetTrigger("ToWalkTrigger");
					animator.SetTrigger("ToWakeUpTrigger");
				};
				break;
		}
	}

	public void Deliver(int bojagiIndex)
	{
		// List<Bojagi> bozagis = MapManager.Instance.Map.GetMapEntityGOList<Bojagi>();
		// var targetBojagi = bozagis.Find(bozagi => bozagi.BojagiIndex == bojagiIndex);
		// if(targetBojagi != null) 
		// {
		// 	catState = CatState.GoToTarget;	
		// 	StartCoroutine(CoDeliver(targetBojagi));
		// }
	}
	
	public void Sleep()
	{
		StartCoroutine(CoSleeping());
	}
	
	private IEnumerator CoInitTest()
	{
		MapEntity orderPos = MapManager.Instance.Map.GetMapEntityGO<CatCushion>();
		yield return StartCoroutine(CoGoToTarget(orderPos));
		catState = CatState.Idle;
		yield return new WaitForNextFrameUnit();
		
		if(orderPos is IInteractableWithCat) 
		{
			(orderPos as IInteractableWithCat).Interact(this);
		}
		
	}
	
	// private IEnumerator CoDeliver(Bojagi bojagi)
	// {
	// 	yield return StartCoroutine(CoGoToTarget(bojagi));	
	// 	yield return new WaitForSeconds(1.0f);
		
	// 	bojagi.Wrapping();
	// 	yield return new WaitForSeconds(1.0f);
		
	// 	bojagi.Refresh();
	// 	MapEntity exitDoor = MapManager.Instance.Map.GetMapEntityGO<ExitDoor>();
	// 	yield return StartCoroutine(CoGoToTarget(exitDoor));
	// 	speechBalloon.ShowSpeechBallon(Define.SpeechBalloonState.Hello);
	// 	yield return new WaitForSeconds(1.0f);
		
	// 	MapManager.Instance.Map.DestroyMapEntity(this);
	// }
	
	private IEnumerator CoSleeping()
	{
		catState = CatState.Sleeping;
		yield return new WaitForSeconds(10.0f);
		//catState = CatState.WakeUp;
		//yield return new WaitForSeconds(2.0f);
		//catState = CatState.Idle;
	}
}
