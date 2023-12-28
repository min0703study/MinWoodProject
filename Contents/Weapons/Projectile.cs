using UnityEngine;

public class Projectile : BaseController
{
	protected Rigidbody2D rBody2D;
	
	[SerializeField]
	[Range(0.1f, 5.0f)]
	private float moveSpeed = 2.0f;
	
	[SerializeField]
	private string targetLayerName = "Player";
	
	private Vector3 startPosition;
	
	private float projectileRange = 10f;

	protected override void Init()
	{
		base.Init();
		rBody2D = GetComponent<Rigidbody2D>();
		startPosition = transform.position;
	}
	
	private void Update()
	{
		DetectFireDistance();
	}

	private void FixedUpdate()
	{
		rBody2D.MovePosition(transform.position + moveSpeed * transform.right * Time.fixedDeltaTime);
	}

	private void DetectFireDistance() {
		if (Vector3.Distance(transform.position, startPosition) > projectileRange) {
			Destroy(gameObject);
		}
	}
	
	private void OnTriggerEnter2D(Collider2D collisionData) {
		if (collisionData.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
		{
			Destroy(gameObject);
			
			UnitController unit = collisionData.GetComponent<UnitController>();
			unit?.OnDamaged(this, 10);
		}
	}
}
