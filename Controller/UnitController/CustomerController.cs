using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CustomerState = Define.CustomerState;

public class CustomerController : UnitController, IInteractable
{
	public TableData.NPC NPCTableData { get; private set; }
	public CustomerState CurrentState { get; set; } = CustomerState.None;
	
	[SerializeField]
	public Transform topPosTransform;
	
	public float InteractableRange => 1.0f;
	public float TargetingRange => 3.0f;
	public bool InteractionLocked => false;
	
	public Bounds ColliderBounds
	{
		get
		{
			var bounds = Collider2DBounds;
			bounds.Expand(1.0f);
			return bounds;
		}
	}
		
	public Action OnEnterShop;
	public Action OnExitShop;

	UI_InteractButton uiInteractButton;
	
	[SerializeField]
	Transform worldSpaceTransform;
	
	protected override void Init()
	{
		base.Init();

		moveSpeed = 1.0f;

		CurrentState = CustomerState.None;
		
		speechBalloon.transform.position = topPosTransform.position;
		
		uiInteractButton = UIManager.Instance.MakeWorldSpaceUI<UI_InteractButton>();
		uiInteractButton.SetInfo(worldSpaceTransform, this);
		uiInteractButton.SetActive(false);
		
		gameObject.SetActive(false);
	}
	
	public void SetInfo(int NPCId) 
	{
		NPCTableData = NPCTable.Instance.GetNPCTableDataById(102010001);
		MyShopServerData.Instance.EnterNPC(NPCTableData.NPCId);
	}
	
	private void Update() {
		base.UpdatePosition();
		base.UpdateSpriteDir();
		
		if(CurrentState == CustomerState.MoveToCounter) 
		{			
			animator.SetTrigger("ToMoveTrigger");
		} else if(CurrentState == CustomerState.WaitToOrder) 
		{
			animator.SetTrigger("ToIdleTrigger");
		}
		else if(CurrentState == CustomerState.MoveToExitDoor) 
		{
			MapEntity orderPos = MapManager.Instance.Map.GetMapEntityGO<ExitDoor>();
			List<Vector3> pathPositions = MapManager.Instance.Map.FindPathToPosition(CenterPos, orderPos.CenterPos);		
			if(pathPositions.Count == 1) 
			{
				ExitShop();
				return;
			}
			
			moveDir = pathPositions[1] - CenterPos;
			
			animator.SetTrigger("ToMoveTrigger");
		}
	}
	
	public void EnterShop()
	{
		gameObject.SetActive(true);
		CurrentState = CustomerState.EnteredShop;
		MyShopServerData.Instance.RealEnterNPC(NPCTableData.NPCId);
		StartCoroutine(CoEnteredShop());
		OnEnterShop?.Invoke();
	}
	
	public void PlayerAcceptedOrder()
	{
		StartCoroutine(CoPlayerAcceptOrder());
	}
	
	public void PlayerRefuseOrder()
	{
		StartCoroutine(CoPlayerRefuseOrder());
	}
		
	public void ExitShop()
	{
		CurrentState = CustomerState.None;
		MyShopServerData.Instance.ExitNPC(NPCTableData.NPCId);
		MapManager.Instance.Map.DestroyNPC(this);
		OnExitShop?.Invoke();
	}
	
	public void Interact() 
	{
	}
	
	public void OpenControlPopup()
	{
		speechBalloon.HideSpeechBallon();
		var popup = UIManager.Instance.ShowPopupUI<UI_NPCDialoguePopup>();
		popup.SetInfo(this);
	}

	private IEnumerator CoEnteredShop()
	{
		yield return new WaitForSeconds(1.0f);
		speechBalloon.ShowSpeechBallon(Define.SpeechBalloonState.Hello);
		yield return new WaitForSeconds(4.0f);
		speechBalloon.HideSpeechBallon();
		CurrentState = CustomerState.MoveToCounter;
		StartCoroutine(CoMoveToCounter());
	}
	
	private IEnumerator CoMoveToCounter()
	{		
		MapEntity counter = MapManager.Instance.Map.GetMapEntityGO<CustomerOrderPos>();
		
		// Target 까지 이동
		List<Vector3> toTargetPositions = MapManager.Instance.Map.FindPathToPosition(CenterPos, counter.CenterPos);		
		foreach (var position in toTargetPositions) 
		{	
			var dirVector = position - CenterPos;
			while (dirVector.magnitude > 0.5f) 
			{
				this.moveDir = dirVector.normalized;	
				dirVector = position - CenterPos;
				yield return new WaitForSeconds(0.1f);
			}
		}
		
		// Target에게 도착하면 정지 후 상호 작용
		this.moveDir = Vector3.zero;
		CurrentState = CustomerState.WaitToOrder;
		yield return new WaitForSeconds(1.0f);
		speechBalloon.ShowSpeechBallon(Define.SpeechBalloonState.WaitOrder);
		uiInteractButton.SetActive(true);
	}
	
	private IEnumerator CoPlayerAcceptOrder()
	{
		uiInteractButton.SetActive(false);
		yield return new WaitForSeconds(1.0f);
		speechBalloon.ShowSpeechBallon(Define.SpeechBalloonState.Happy);
		yield return new WaitForSeconds(3.0f);
		speechBalloon.HideSpeechBallon();
		CurrentState = CustomerState.MoveToExitDoor;
	}
	
	private IEnumerator CoPlayerRefuseOrder()
	{
		yield return new WaitForSeconds(1.0f);
		speechBalloon.ShowSpeechBallon(Define.SpeechBalloonState.Sad);
		yield return new WaitForSeconds(3.0f);
		speechBalloon.HideSpeechBallon();
		CurrentState = CustomerState.MoveToExitDoor;
	}
}
