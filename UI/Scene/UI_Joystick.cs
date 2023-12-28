using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.U2D.IK;

public class UI_Joystick : UI_SceneBase, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private Image backgroundImage, handlerImage;

	float joystickRadius;
	private Vector2 originalPosition;
	private Vector2 touchPosition;

	private Vector2 moveDir;
	private bool onDraging;

	// Start is called before the first frame update
	protected override void Init()
	{
		base.Init();
		backgroundImage.enabled = false;
		handlerImage.enabled = false;
	
		joystickRadius = backgroundImage.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
		originalPosition = backgroundImage.transform.position;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		backgroundImage.enabled = true;
		handlerImage.enabled = true;
		
		onDraging = true;
		// 위치 추출 후 background(바깥원), (handelr)안쪽원 위치를 옮겨준다
		backgroundImage.transform.position = eventData.position;
		handlerImage.transform.position = eventData.position;

		// 클릭 위치를 저장
		touchPosition = eventData.position;
	}

	// 2. 드래그, 가운데 원이 따라와야 한다. 바깥원까지 
	public void OnDrag(PointerEventData eventData)
	{
		// 방향만 추출하기 위해서는 normallize
		// 방향 벡터
		Vector2 touchDir = eventData.position - touchPosition;

		//최대거리 이상은 가지 않겠다!  + 많이 쓰는 코드 느낌 
		//최대 범위 이하이면 인정하고, 최대범위 넘어서면 최대범위로 하겠다!
		float moveDist = Mathf.Min(touchDir.magnitude, joystickRadius);

		// 내가 이동해야하는 방향 touchDir.normalized
		if(moveDir == touchDir.normalized) 
			return;
			
		moveDir = touchDir.normalized;

		Vector2 newPosition = touchPosition + moveDir * moveDist;

		handlerImage.transform.position = newPosition;
		
		GameManager.Instance.Player.ManualSetMoveDir(moveDir);
	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
		if(onDraging)
			return;
			
			
		//GameManager.Instance.Player.InteractionNeareastObject();
	}


	public void OnEndDrag(PointerEventData eventData)
	{	
		backgroundImage.enabled = false;
		handlerImage.enabled = false;
		
		onDraging = false;
		moveDir = Vector2.zero;
		GameManager.Instance.Player.ManualSetMoveDir(moveDir);
		
		backgroundImage.transform.position = originalPosition;
		handlerImage.transform.position = originalPosition;
	}
}
