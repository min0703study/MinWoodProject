using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangeManager : BaseManager<SceneChangeManager>
{
	public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
	public void LoadScene(Define.Scene loadScene, Transform parent = null)
	{
		CinemachineManager.Instance.Clear();
			
		var sceneChangePopupGO = ResourceManager.Instance.Instantiate("SceneChangePopupGroup");	
		var sceneChangePopup = Util.GetOrAddComponent<SceneChangePopupGroup>(sceneChangePopupGO);
		
		DontDestroyOnLoad(sceneChangePopup);

		
		sceneChangePopup.SetInfo(() =>
		{
	
			if(CurrentScene.SceneType != Define.Scene.IntroScene)
			{
				MapManager.Instance.ChangeMap();
			}
			
		
			PoolManager.Instance.Clear();
			SceneManager.LoadScene(System.Enum.GetName(typeof(Define.Scene), loadScene));
			sceneChangePopup.SceneChangedSuccess();
		});
   	}
}
