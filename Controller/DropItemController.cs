using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DropItemController : MonoBehaviour
{
	#region DropItemState
	public enum DropItemState
	{
		Dropping,
		Idle,
		MoveToPlayer,
		Destroying,
	}
	#endregion
	
	public TableData.Item ItemData { get; private set; }
	DropItemState curState = DropItemState.Idle;
	
	[SerializeField]
	private SpriteRenderer itemSpriteRenderer;
	
	[SerializeField]
	private SpriteMask itemSpriteMask;
	
	private Rigidbody2D rBody2D;
	
	private float moveToPlayerSpeed = 3.0f;
	
	public Action<DropItemController> DestroyAction { get; set; }
	
	#region BoundAnimation
	public Vector3 startPosition;
	public float gravity = -9.8f;        // 중력 가속도
	private int bounceCount = 0;
	#endregion
	
	private void Awake() {
		rBody2D = GetComponent<Rigidbody2D>();
	}
	
	void Update()
	{
		if(curState == DropItemState.MoveToPlayer) 
		{
			var direction = GameManager.Instance.Player.CenterPos - transform.position;
			transform.position += (direction.normalized * moveToPlayerSpeed) * Time.deltaTime;
			moveToPlayerSpeed += 0.01f;

			// To Attack State
			float distanceFromPlayer = Vector3.Distance(GameManager.Instance.Player.CenterPos, transform.position);
			if (distanceFromPlayer <= 0.3f)
			{
				CompleteGetItem();
			}
		} 
	}
	

	// Update is called once per frame
	void FixedUpdate()
	{
		if(curState == DropItemState.Dropping) 
		{
			rBody2D.AddForce(new Vector2(0, gravity * 2.0f));
			if(startPosition.y > gameObject.transform.position.y) 
			{
				curState = DropItemState.Idle;
				rBody2D.velocity = Vector3.zero;
				if(bounceCount < 2) 
				{
					rBody2D.velocity = Vector2.zero;
					rBody2D.AddForce(Vector2.up * 200.0f);
					bounceCount++;
				} else 
				{
					curState = DropItemState.Idle;
					rBody2D.velocity = Vector3.zero;
				}
			}
		} else 
		{
			rBody2D.velocity = Vector3.zero;
		}
	}
	
	public void SetInfo(int itemId) 
	{
		ItemData = ItemTable.Instance.GetItemDataByItemId(itemId);
		
		Sprite infoSprite = ResourceManager.Instance.Load<Sprite>($"{ItemData.SpriteName}");
		if(infoSprite != null)
			itemSpriteRenderer.sprite = infoSprite;
			itemSpriteMask.sprite = infoSprite;
	}
	
	public void StartDropping() 
	{
		if(curState == DropItemState.Dropping)
			return;
			
			
		curState = DropItemState.Dropping;
		startPosition = gameObject.transform.position;
		rBody2D.AddForce(Vector2.up * 300.0f);
	}
	
	public void GetItem() 
	{
		if(curState == DropItemState.Idle) 
		{
			curState = DropItemState.MoveToPlayer;
			//StartCoroutine(CoCheckDistance());
		}
	}
	
	public IEnumerator CoCheckDistance()
	{
		float dist = Vector3.Distance(gameObject.transform.position, GameManager.Instance.Player.CenterPos);
		transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.CenterPos, Time.deltaTime * 15.0f);
		if (dist < 1f)
		{
			CompleteGetItem();
			yield break;
		}

		yield return new WaitForFixedUpdate();
	}
	
	public void CompleteGetItem() 
	{
		curState = DropItemState.Destroying;
		GameManager.Instance.Player.AddItem(ItemData.Id);
		DestroyMySelf();
		SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_InspectItem");
	}
	
	public void DestroyMySelf() 
	{
		if(DestroyAction == null) 
		{
			GameObject.Destroy(this);	
		}
		
		DestroyAction?.Invoke(this);
	}
}
