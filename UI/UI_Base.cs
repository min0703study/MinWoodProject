using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Base : MonoBehaviour
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
	
	protected virtual void UpdateUI()
	{
	}

}
