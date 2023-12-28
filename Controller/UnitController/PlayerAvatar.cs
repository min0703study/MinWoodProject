using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerAvatar : BaseController
{
	[SerializeField]
	SpriteLibrary clothSpriteLibrary;
	
	[SerializeField]
	SpriteRenderer frontHairSpriteRenderer, backHairSpriteRenderer;

	public int costumeId;
	
	protected override void Init()
	{
		base.Init();
		
		RefreshCostume();
	}
	
	private void RefreshCostume() 
	{
		if(this.costumeId != UserServerData.Instance.CostumeId) 
		{
			this.costumeId = UserServerData.Instance.CostumeId;
			//var costume = CostumeTable.Instance.GetCostumeById(UserServerData.Instance.CostumeId);
			
			// if(costume.HairBackSpriteName != "") 
			// {
			// 	backHairSpriteRenderer.gameObject.SetActive(true);
			// 	backHairSpriteRenderer.sprite = ResourceManager.Instance.Load<Sprite>(costume.HairBackSpriteName);
			// } else 
			// {
			// 	backHairSpriteRenderer.gameObject.SetActive(false);
			// }

			// frontHairSpriteRenderer.sprite = ResourceManager.Instance.Load<Sprite>(costume.HairFrontSpriteName);
			// clothSpriteLibrary.spriteLibraryAsset = ResourceManager.Instance.Load<SpriteLibraryAsset>(costume.ClothSpriteLibraryName);
		}
	}
}
