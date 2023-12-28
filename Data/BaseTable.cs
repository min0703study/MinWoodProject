using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

public abstract class BaseTable<T> where T : BaseTable<T>, new()
{
	private static T instance;

	public static T Instance { 
	get {
		if (instance == null) 
		{
			instance = new T();
			instance.init();
		}
		
		return instance;
	}}
	
	protected static bool isInit = false;

	protected virtual void init()
	{
		if(isInit == true)
			return;
			
		isInit = true;
	}
}
