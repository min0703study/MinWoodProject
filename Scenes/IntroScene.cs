using UnityEngine;

public class IntroScene : BaseScene
{	
	[SerializeField]
	GameObject managersPrefab;
	
	protected override void Init()
	{
		SceneType = Define.Scene.IntroScene;
		
		GameObject managersGO = GameObject.Find("Managers");
		
		if(managersGO == null) 
		{
			managersGO = GameObject.Instantiate(managersPrefab);
		}
		
		DontDestroyOnLoad(managersGO);
	}
	
}
