using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using SpeechBalloonState = Define.SpeechBalloonState;
public class SpeechBalloonController : BaseController
{
	private SpriteResolver spriteResolver;
	
	protected override void Init()
	{
		base.Init();
		
		spriteResolver = GetComponentInChildren<SpriteResolver>();
	}
	
	public void ShowSpeechBallon(SpeechBalloonState speechBalloonState)
	{
		spriteResolver.SetCategoryAndLabel("Customer", Enum.GetName(typeof(SpeechBalloonState), speechBalloonState));
		gameObject.SetActive(true);
	}
	
	public void HideSpeechBallon()
	{
		gameObject.SetActive(false);
	}
}
