using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrderManagerPopup : UI_PopupBase
{
	[SerializeField]
	Button backgroundButton;
	
	[SerializeField]
	GameObject deliveryListGO;
	
	[SerializeField]
	GameObject contentPanel;
	
	[SerializeField]
	public Transform coinTransform;
	
	protected override void Init() 
	{
		backgroundButton.onClick.AddListener(()=>ClosePopupUI());
		Refresh();
	}
	
	private void Start() {
		PopupOpenAnimation(contentPanel);
	}
	
	public void Refresh() {
		Util.DestroyChilds(deliveryListGO);
		
		if(OrderListServerData.Instance.Orders.Count <= 0) 
		{
			var cell = UIManager.Instance.MakeSubItem<UI_OrderEmptyCell>(deliveryListGO.transform);
			cell.gameObject.transform.SetParent(deliveryListGO.transform, false);
		}
		
		foreach (var order in OrderListServerData.Instance.Orders) 
		{				
			var deliveryCell = UIManager.Instance.MakeSubItem<UI_OrderCell>();
			deliveryCell.gameObject.transform.SetParent(deliveryListGO.transform, false);
			deliveryCell.SetInfo(order.OrderId);
			deliveryCell.OnCompletedOrder += Refresh;
			deliveryCell.StartRewardAnimation += StartRewardAnimation;
		}
	}
	
	private void Update() {

	}
	
	void StartRewardAnimation(Transform transform)
	{
		var item = ItemTable.Instance.GetItemDataByItemId(103010020);
		Sprite itemSprite = ResourceManager.Instance.Load<Sprite>(item.SpriteName);
		List<UI_ItemIcon> itemIcons = new List<UI_ItemIcon>(); 
		
		for(int i = 0; i < 20; i++) 
		{			
			var itemIcon = UIManager.Instance.MakeSubItem<UI_ItemIcon>(gameObject.transform);
			itemIcon.SetInfo(itemSprite);
			itemIcon.transform.position = transform.position;
			itemIcons.Add(itemIcon);
		}
		
		// 각 Sprite에 대해 랜덤한 방향과 거리로 이동 애니메이션 적용
		foreach (var itemIcon in itemIcons)
		{
			// 랜덤한 방향과 거리 계산
			Vector2 randomDirection = Random.insideUnitCircle.normalized;
			float randomDistance = Random.Range(100f, 280f);

			// 현재 위치에서 랜덤한 방향으로 랜덤한 거리만큼 이동하는 Tweener 생성
			 var spreadTweener = itemIcon.transform.DOMove(itemIcon.transform.position + (Vector3)randomDirection * randomDistance, 1f)
				.SetEase(Ease.OutQuad);
				
			// 퍼진 후에 지정한 위치로 바로 이동하는 Tweener 생성
			var moveToTargetTweener = itemIcon.transform.DOMove(coinTransform.position, 0.3f)
				.SetEase(Ease.InQuad)
				.Pause(); // 초기에는 일시정지 상태로 시작
			
			// spreadTweener가 끝날 때 moveToTargetTweener 실행
			spreadTweener.OnComplete(() => moveToTargetTweener.Play());

			moveToTargetTweener.OnComplete(() => { 
				ResourceManager.Instance.Destroy(itemIcon.gameObject); });
			
		}
	}

}
