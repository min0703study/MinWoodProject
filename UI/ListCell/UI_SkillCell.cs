using System.Collections;
using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillCell : MonoBehaviour
{
	[SerializeField]
	private Slider cooltimeSlider;

	[SerializeField]
	private TextMeshProUGUI cooltimeText;
	
	[SerializeField]
	private bool isLock = false;

	private Button button;

	public Action<int> OnButtonClick { get; set; }
	
	public float Cooltime { get; set; } = 1.0f;
	private float coolTimeCheck;
	public int Index { get; set; }
	
	private void Awake()
	{
		button = GetComponent<Button>();
		if(button != null) 
		{
			button.onClick.AddListener(OnClickCellButton);	
			button.interactable = true;
		}
	}

	public void StartCooltime()
	{
		StartCoroutine(CoStartCooltime());
	}
	
	public void OnClickCellButton() 
	{
		if(isLock)
			return;
			
		GameManager.Instance.Player.SwingSwordSkill();
		StartCoroutine(CoStartCooltime());
	}

	IEnumerator CoStartCooltime()
	{
		button.interactable = false;
		cooltimeSlider.gameObject.SetActive(true);
		cooltimeText.gameObject.SetActive(true);

		coolTimeCheck = Cooltime;

		while (coolTimeCheck >= 0)
		{
			coolTimeCheck -= 0.01f;
			cooltimeText.text = (Mathf.Round(coolTimeCheck * 10) / 10).ToString();
			cooltimeSlider.value = coolTimeCheck;
			yield return new WaitForSeconds(0.1f);
		}

		button.interactable = true;
		cooltimeSlider.gameObject.SetActive(false);
		cooltimeText.gameObject.SetActive(false);

	}
}
