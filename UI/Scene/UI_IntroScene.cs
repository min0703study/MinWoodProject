using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UI_IntroScene : UI_SceneBase
{	
	[SerializeField]
	Slider loadingSlider;
	
	[SerializeField]
	TextMeshProUGUI loadingValueText;
	
	protected override void Init()
	{
		base.Init();
	}
	
	private void Start() {
		ResourceManager.Instance.LoadAllAsync("PreLoad", (key, count, totalCount) =>
		{
			loadingSlider.value = (float)count/totalCount;
			loadingValueText.text = $"{loadingSlider.value * 100}%";
			if (count == totalCount)
			{
				CompleteLoading();
			}
		});
	}
	
	private void CompleteLoading() 
	{
		SoundManager.Instance.Play(SoundManager.SoundType.Effect, "Sound_IntroOkButton");
		
		TableLoader.Instance.Load();
		
		GameManager.Instance.GeneratePlayer(new Vector3(0,0,0));
		
		SceneChangeManager.Instance.LoadScene(Define.Scene.LobbyScene);
	}
}
