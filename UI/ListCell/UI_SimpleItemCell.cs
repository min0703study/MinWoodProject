using System;

using UnityEngine;
using UnityEngine.UI;

public class UI_SimpleItemCell : UI_Base
{
	public int ItemId { get; set; }

	private bool isSelected;
	public bool IsSelected
	{
		get { return isSelected; }
		set
		{
			isSelected = value;
			selectedPanel.SetActive(value);
		}
	}
	
	public bool IsLocked { get; private set; }

	[SerializeField]
	Image ItemIconImage;

	[SerializeField]
	GameObject selectedPanel, lockPanel, normalPanel;
	public Action<UI_SimpleItemCell> CellButtonClicked;

	private Button cellButton;

	protected override void Init()
	{
		cellButton = GetComponent<Button>();
		cellButton.onClick.AddListener(OnCellButtonClick);
	}

	public void SetInfo(TableData.Item itemTableData)
	{
		ItemId = itemTableData.Id;
		ItemIconImage.sprite = ResourceManager.Instance.Load<Sprite>(itemTableData.SpriteName);
	}

	public void OnCellButtonClick()
	{
		CellButtonClicked?.Invoke(this);
	}
}
