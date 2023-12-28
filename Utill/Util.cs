using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;

public static class Util
{
	public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
	{
		T component = go.GetComponent<T>();
		if (component == null)
			component = go.AddComponent<T>();
		return component;
	}
	
	public static void DestroyChilds(GameObject go)
	{
		Transform[] children = new Transform[go.transform.childCount];
		for (int i = 0; i < go.transform.childCount; i++)
		{
			children[i] = go.transform.GetChild(i);
		}

		// 모든 자식 오브젝트 삭제
		foreach (Transform child in children)
		{
			child.SetParent(null);
			ResourceManager.Instance.Destroy(child.gameObject);
		}
	}
	
	public static Color HexToColor(string color)
	{
		Color parsedColor;
		ColorUtility.TryParseHtmlString("#"+color, out parsedColor);

		return parsedColor;
	}
	
	public static string MyMethod()
	{
		// 현재 메서드의 호출 스택 추적
		StackTrace stackTrace = new StackTrace();
		
		// 첫 번째 프레임(현재 메서드를 호출한 위치)의 메서드 정보 가져오기
		StackFrame stackFrame = stackTrace.GetFrame(1);
		MethodBase method = stackFrame.GetMethod();

		// 메서드명 출력
		string methodName = method.Name;
		return methodName;
	}
}