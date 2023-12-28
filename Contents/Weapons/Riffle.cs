using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

using Random = UnityEngine.Random;


public class Riffle : WeaponController
{
	[SerializeField]
	protected GameObject muzzlePos;

	[SerializeField]
	private Light2D shootEffectLight;
	
	protected float desiredAngle;

	float spreadAngle = 5;

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

	public void Update()
	{
		var target = GameManager.Instance.TargetMarker.Target;
		
		if(target is MonsterController) 
		{
			Vector3 pointsVector = target.CenterPos - transform.position;
			SetAngle(pointsVector.normalized);
		}
	}
	
	public override void Attack()
	{
		if (!isShooting) {
			StartCoroutine(NormalShootRoutine());
		}
	}

	IEnumerator CoOnShootEffectLight()
	{
		shootEffectLight.enabled = true;
		yield return new WaitForSeconds(0.1f);
		shootEffectLight.enabled = false;
		yield break;
	}
	
	private void SetAngle(Vector2 pointsVector)
	{
		// 방향 벡터 unitVector의 (라디안에서 도(degree)도 변환)
		// Mathf.Atan2 (두 숫자의 몫이 나오는 각도를 반환)
		float desiredAngle = Mathf.Atan2(pointsVector.y, pointsVector.x) * Mathf.Rad2Deg;
		
		Quaternion angle = Quaternion.AngleAxis(desiredAngle, Vector3.forward);
		//총의 회전을 설정
		transform.rotation = Quaternion.AngleAxis(desiredAngle, Vector3.forward);
	
		bool isFlipY = desiredAngle > 90.0f || desiredAngle < -90.0f;
		Vector3 changeScale = transform.localScale;
		changeScale.y = 1 * (isFlipY ? -1 : 1);
		transform.localScale = changeScale;
	}
	
	private IEnumerator NormalShootRoutine()
	{
		isShooting = true;
		
		for (int i = 0; i < 3; i++)
		{			
			SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_GunShot");
	
			float spread = Random.Range(-spreadAngle, spreadAngle);
			Quaternion bulletSpreadRotation = Quaternion.Euler(new Vector3(0, 0, spread));
			Quaternion QSpreadAngle = muzzlePos.transform.rotation * bulletSpreadRotation;

			Instantiate(bulletPrefab, muzzlePos.transform.position, QSpreadAngle);
			CinemachineManager.Instance.ShakeCamera();
			
			StartCoroutine(CoOnShootEffectLight());
			
			yield return new WaitForSeconds(0.1f);
		}
		
		yield return new WaitForSeconds(restTime);
		isShooting = false;
	}

	public IEnumerator CoOnCooldown() 
	{
		yield return new WaitForSeconds(weaponCooldown);
		isAttacking = false;
	}
}
