using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularBullet : Projectile
{
	Vector3 lastVelocity;
	float _speed = 3f;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
	}

	private void FixedUpdate()
	{
		if (rBody2D != null)
		{
			rBody2D.MovePosition(transform.position + _speed * transform.right * Time.fixedDeltaTime);
		}
	}

	private void OnTriggerEnter2D(Collider2D collisionData)
	{
		UnitController unit = collisionData.GetComponent<UnitController>();

		if (collisionData.gameObject.layer == LayerMask.NameToLayer("Monster"))
		{	
			unit?.OnDamaged(GameManager.Instance.Player, UserServerData.Instance.AtkValue);
			Destroy(gameObject);
		}
		
		if (collisionData.gameObject.CompareTag("CollisionTilemap"))
		{
			Vector2 wallNormal = collisionData.ClosestPoint(transform.position) - (Vector2)transform.position;
			wallNormal.Normalize();

			// 현재 방향을 기준으로 벽에 대한 반사 방향을 계산
			Vector2 reflection = Vector2.Reflect(transform.position, wallNormal);

			// 총알의 방향도 반사 방향으로 설정
			transform.up = reflection;
		}
	}
}
