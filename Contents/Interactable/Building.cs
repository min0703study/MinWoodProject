
using DG.Tweening;
using UnityEngine;

public class Building : MapEntity, IInteractable
{	
	[SerializeField]
	BuildingInfoSO buildingInfo;
	
	[SerializeField]
	Transform worldSpaceTransform;
	
	public BuildingInfoSO BuildingInfo { get => MyShopServerData.Instance.GetBuildingByBuildingId(buildingInfo.BuildingId); }
	
		
	#region IInteractable
	public Bounds ColliderBounds => Collider2DBounds;
	public float InteractableRange => 1.0f;
	public float TargetingRange => 3.0f;
	public bool InteractionLocked => false;
	#endregion
	
	[SerializeField]
	GameObject buildingGO, builtGO;
	
	#region UI
	protected UI_BuildCondition uiBuildCondition;
	protected UI_InteractButton uiInteractButton;
	#endregion
		
	protected override void Init()
	{
		base.Init();		
				
		if(MyShopServerData.Instance.GetBuildingByBuildingId(buildingInfo.BuildingId) == null) 
		{
			MyShopServerData.Instance.AddBuilding(buildingInfo);
		} else 
		{
			buildingInfo = MyShopServerData.Instance.GetBuildingByBuildingId(buildingInfo.BuildingId);
		}
		
		uiBuildCondition = UIManager.Instance.MakeWorldSpaceUI<UI_BuildCondition>();
		uiBuildCondition.SetInfo(worldSpaceTransform, this);
		
		uiInteractButton = UIManager.Instance.MakeWorldSpaceUI<UI_InteractButton>();
		uiInteractButton.SetInfo(worldSpaceTransform, this);
		uiInteractButton.SetActive(false);	
			
		if(buildingInfo.IsConstructionComplete) 
		{
			builtGO.SetActive(true);
			buildingGO.SetActive(false);
			uiBuildCondition.gameObject.SetActive(false);
			uiInteractButton.SetActive(true);	
		}
	}
	
	public virtual void BuildComplete() 
	{
		builtGO.SetActive(true);
		buildingGO.SetActive(false);
		SoundManager.Instance.PlayQuestComplete();
		QuestServerData.Instance.UpdateQuestCheck("build", buildingInfo.BuildingId, 1);
	}
	
	public void ActiveInteractButton() 
	{
		uiInteractButton.SetActive(true);
	}
	
	public virtual void OpenControlPopup() 
	{
	}
	
	public void Interact() 
	{
	}
	
	
}
