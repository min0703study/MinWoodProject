using System;
using UnityEngine;

using Object = UnityEngine.Object;

public class BaseScene : MonoBehaviour
{
	public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	void Awake()
	{	
		Init();
	}
	
	protected virtual void Init()
	{
	}
	
}
