 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonsterController
{
	[SerializeField] 
	private GameObject bulletPrefab;
	
	[SerializeField] 
	private float bulletMoveSpeed = 6;
	
	private bool isShooting = false;
	
	[SerializeField] 
	private int burstCount = 5;
	
	[SerializeField] 
	private float timeBetweenBursts = 0.3f;
	
	[SerializeField] 
	private float restTime = 0.5f;
	
	
	public override void PerformAttack(UnitController target)
	{ 
		if (!isShooting) {
			StartCoroutine(ShootRoutine());
		}
	}

	private IEnumerator ShootRoutine()
	{
		isShooting = true;
		
		for (int i = 0; i < burstCount; i++)
		{
			Vector2 targetDirection = GameManager.Instance.Player.transform.position - transform.position;
		
			GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
			newBullet.transform.right = targetDirection;
			
			if(newBullet.TryGetComponent(out Projectile projectile)) 
			{
				//projectile.Update
			}
			
			yield return new WaitForSeconds(timeBetweenBursts);
		
		}
		
		yield return new WaitForSeconds(restTime);
		isShooting = false;
	}
}
