
using UnityEngine;

public class MapEntity : BaseController
{	
	[SerializeField]
	protected GameObject centerPosGO;
	
	protected Collider2D collider2D;

	public Vector3 CenterPos { get { return centerPosGO.transform.position; } }
	public Bounds Collider2DBounds { get { return collider2D.bounds; } }	
		
	protected override void Init()
	{
		base.Init();		
		
		collider2D = GetComponent<Collider2D>();
	}
}
