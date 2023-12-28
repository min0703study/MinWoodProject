using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_ToastMessage : UI_Base
{
	public TextMeshProUGUI messageText;
	private Stack<string> messageQueue = new Stack<string>();
	private bool isShowingMessage = false;

	protected override void Init()
	{
		base.Init();
		gameObject.SetActive(false);
	}

	// Toast 메시지를 표시할 UI 텍스트(Text 컴포넌트)를 연결합니다.
	public void ShowMessage(string message)
	{
		// 메시지를 큐에 추가
		messageQueue.Push(message);

		// 메시지 표시 중이 아닌 경우에만 표시 시작
		if (!isShowingMessage)
		{
			ShowNextMessage();
		}
	}

	// 다음 메시지를 표시합니다.
	private void ShowNextMessage()
	{
		if (messageQueue.Count > 0)
		{
			gameObject.SetActive(true);
			string message = messageQueue.Pop();
			messageText.text = message;

			// DOTween을 사용하여 Fade In과 Fade Out 애니메이션을 추가
			messageText.DOFade(1f, 1f).SetEase(Ease.Linear)
				.OnComplete(() => messageText.DOFade(0f, 0.5f).SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					// 다음 메시지 표시
					ShowNextMessage();
				}));
		}
		else
		{
			// 큐에 더 이상 메시지가 없으면 메시지 표시 종료
			isShowingMessage = false;
			gameObject.SetActive(false);
		}
	}
}
