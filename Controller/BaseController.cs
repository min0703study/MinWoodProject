using UnityEngine;

public class BaseController : MonoBehaviour
{
	protected bool isInitOver = false;
	
	private void Awake()
	{
		Init();
	}
	
	protected virtual void Init()
	{
		if (isInitOver) 
			return;
		
		isInitOver = true;
	}
}
