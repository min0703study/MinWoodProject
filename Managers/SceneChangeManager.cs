using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangeManager : BaseManager<SceneChangeManager>
{
	public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
	public void LoadScene(Define.Scene loadScene, Transform parent = null)
	{
		CinemachineManager.Instance.Clear();
		if(CurrentScene.SceneType != Define.Scene.IntroScene)
		{
			MapManager.Instance.ChangeMap();
		}
		
		var sceneChangePopup = UIManager.Instance.ShowPopupUI<UI_SceneChangePopup>();	
		sceneChangePopup.SetInfo(loadScene, () =>
		{
			//ResourceManager.Instance.Destroy(UIManager.Instance.CurSceneUI);
			PoolManager.Instance.Clear();
			SceneManager.LoadScene(System.Enum.GetName(typeof(Define.Scene), loadScene));
		});
   	}
}
