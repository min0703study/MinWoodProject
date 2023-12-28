using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_IntroScene : UI_SceneBase
{	
	[SerializeField]
	private Slider loadingSlider;
	
	[SerializeField]
	private Button startGameButton;
	
	public bool IsReadyGame { get; private set;} = false;
	
	protected override void Init()
	{
		base.Init();
		
		startGameButton.onClick.AddListener(OnClickStartGameButton);
	}
	
	private void Start() {
		ResourceManager.Instance.LoadAllAsync("PreLoad", (key, count, totalCount) =>
		{
			loadingSlider.value = (float)count/totalCount;
			if (count == totalCount)
			{
				IsReadyGame = true;
				startGameButton.gameObject.SetActive(true);
			}
		});
	}
	
	private void OnClickStartGameButton() 
	{
		SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_IntroOkButton");
		
		TableLoader.Instance.Load();
		GameManager.Instance.GeneratePlayer(new Vector3(0,0,0));
		
		SceneChangeManager.Instance.LoadScene(Define.Scene.LobbyScene);
	}
}
