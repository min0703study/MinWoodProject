using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

public class UI_ProgressBar : UI_Base
{
	Slider slider;

	protected override void Init()
	{
		base.Init();
		
		slider = GetComponent<Slider>();
		slider.value = 0;
	}

	public void Refresh(float maxValue, float currentValue = 0f)
	{
		float sliderValue = currentValue / maxValue;
		float originalSliderValue = slider.value;

		if (slider.value != currentValue)
		{
			DOTween.To(() => originalSliderValue, x => slider.value = x, sliderValue, 0.5f)
				.SetEase(Ease.Linear);
		}
	}

}
