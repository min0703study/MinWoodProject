using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public struct VirtualCamera
{
	public CinemachineVirtualCamera Camera;
	public CinemachineBasicMultiChannelPerlin Noise;
	public CinemachineConfiner2D Confiner2D;
}

public class CinemachineManager : BaseManager<CinemachineManager>
{
	[SerializeField]
	public Camera MainCamera;
	public Dictionary<string, VirtualCamera> virtualCameraDict = new Dictionary<string, VirtualCamera>();
	
	public VirtualCamera ActiveCameraStruct;

	private float amplitude = 1.2f, intensity = 1;
	private float duration = 0.1f;

	protected override void init() 
	{
		base.init();
	}
	
	public void AddVirtualCamera(string virtualCameraKey, CinemachineVirtualCamera cinemachineVirtualCamera)
	{
		VirtualCamera virtualCamera = new VirtualCamera();
		
		virtualCamera.Camera = cinemachineVirtualCamera;
		virtualCamera.Camera.Priority = 0;
		virtualCamera.Noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		virtualCamera.Confiner2D = cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>();
		virtualCameraDict.Add(virtualCameraKey, virtualCamera);
		
	}
	
	public void ActivateCamera(string virtualCameraKey)
	{
		var virtualCamera = virtualCameraDict[virtualCameraKey];
		ActiveCameraStruct = virtualCamera;
		
		virtualCamera.Camera.Priority = 10;
	}
	
	public void SetTarget(GameObject target)
	{
		ActiveCameraStruct.Camera.m_Follow = target.transform;
	}
	
	public void SwitchCamera(string toCameraKey)
	{
		var toVirtualCamera = virtualCameraDict[toCameraKey];
		ActiveCameraStruct.Camera.Priority = 0;
		toVirtualCamera.Camera.Priority = 10;
		toVirtualCamera.Confiner2D.InvalidateCache();
		ActivateCamera(toCameraKey);
	}
	
	public void SwitchFocusForSeconds(Vector2 otherFocusPosition, float seconds = 1.0f)
	{
		var originalTarget = ActiveCameraStruct.Camera.Follow;
		ActiveCameraStruct.Camera.Follow = null;
		
		// 잠시 대기
		Sequence mySequence = DOTween.Sequence();
				
		mySequence.Append(ActiveCameraStruct.Camera.transform.DOMoveX(otherFocusPosition.x, seconds));
		mySequence.Insert(0, ActiveCameraStruct.Camera.transform.DOMoveY(otherFocusPosition.y, seconds));
		mySequence.Insert(0, DOTween.To(() => ActiveCameraStruct.Camera.m_Lens.OrthographicSize, x => ActiveCameraStruct.Camera.m_Lens.OrthographicSize = x, 4f, seconds)
			.SetEase(Ease.InOutQuad)); // 회전 (0.5초 후 시작)
		mySequence.AppendInterval(0.5f);

		mySequence.SetLoops(2, LoopType.Yoyo);
		mySequence.OnComplete(()=>
		{
			ActiveCameraStruct.Camera.Follow = originalTarget;
		});
	}

	public void ShakeCamera()
	{
		ActiveCameraStruct.Noise.m_AmplitudeGain = amplitude;
		ActiveCameraStruct.Noise.m_FrequencyGain = intensity;

		StartCoroutine(CoShakeCamera());
	}

	IEnumerator CoShakeCamera()
	{
		for (float i = duration; i > 0; i -= Time.deltaTime)
		{
			ActiveCameraStruct.Noise.m_AmplitudeGain = Mathf.Lerp(0, amplitude, i / duration);
			yield return null;
		}

		ActiveCameraStruct.Noise.m_AmplitudeGain = 0;
	}
	
	public void Clear()
	{		
		virtualCameraDict.Clear();
		ActiveCameraStruct.Camera = null;
	}
}
