using DG.Tweening;
using UnityEngine;

public class TargetMarker : BaseController
{
	[SerializeField]
	private GameObject leftTopCornerGO, rightTopCornerGO, leftButtonCornerGO, rightButtonCornerGO;
	
	public MapEntity Target { get; private set; }
	
	[SerializeField]
	public float expendRange = 0.3f;
	
	protected override void Init()
	{
		base.Init();
	}
	
	private void Update() {
		if(this.Target != null) 
		{
			gameObject.transform.position = Target.Collider2DBounds.center;
		}
	}

	
	public void SetTargetObject(MapEntity newTarget) 
	{
		if(newTarget == null) 
		{
			OffTargetMarker();
			return;
		}
		
		if(this.Target != newTarget) 
		{
			gameObject.SetActive(true);
			this.Target = newTarget;
			AnimateBoundsCorners(Target.Collider2DBounds);
		}
	}
	
	public void OffTargetMarker() 
	{
		Target = null;
		gameObject.SetActive(false);
	}
	
	public void AnimateBoundsCorners(Bounds bounds)
	{
		gameObject.transform.position = bounds.center;
		
		// 네모서리 점의 좌표를 배열로 저장
		Vector3[] corners = new Vector3[4];
		corners[0] = new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
		corners[1] = new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
		corners[2] = new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
		corners[3] = new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
		
		GameObject[] cornerGOs = new GameObject[4];
		cornerGOs[0] = leftButtonCornerGO;
		cornerGOs[1] = rightButtonCornerGO;
		cornerGOs[2] = leftTopCornerGO;
		cornerGOs[3] = rightTopCornerGO;

		
		for (int i = 0; i < 4; i++) 
		{
			cornerGOs[i].transform.localPosition = corners[i];
			
			var directionVector = corners[i] - Vector3.zero;
			Vector3 toPosition = corners[i] + directionVector.normalized * 0.4f;

			Sequence dotweenSequence = DOTween.Sequence();
			dotweenSequence.Append(cornerGOs[i].transform.DOLocalMove(toPosition, 0.8f).SetEase(Ease.InOutQuad));
			dotweenSequence.SetLoops(-1, LoopType.Yoyo);
		}
	}
}