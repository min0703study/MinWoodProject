using System.Collections;
using UnityEngine;

using EnvEntityState = Define.EnvEntityState;

public class Rock : EnvEntity
{	
	#region Component
	SpriteRenderer spriteRenderer;
	#endregion
	
	[SerializeField]
	int EnvEntityId;

	protected override void Init()
	{
		base.Init();
		spriteRenderer = GetComponent<SpriteRenderer>();
		
		SetInfo(EnvEntityId);
	}

	public override void SetInfo(int envObjectId)
	{
		base.SetInfo(envObjectId);
		
		EnvEntityTableData = EnvEntityTable.Instance.GetEnvEntityById(envObjectId);
		curHP = EnvEntityTableData.MaxHp;
		Sprite infoSprite = ResourceManager.Instance.Load<Sprite>($"{EnvEntityTableData.SpriteName}");
		if(infoSprite != null)
			spriteRenderer.sprite = infoSprite;
	}

	public override void Interact()
	{
		base.Interact();
		GameManager.Instance.Player.MineNodes(this);
	}
	
	public override void Hit(float power) {		
		if(CurrentState == EnvEntityState.OnDamaged)
			return;
			
			
		CurrentState = EnvEntityState.OnDamaged;
		curHP -= power;
		
		progressBar.SetActive(true);
		progressBar.SetProgressBarValue(EnvEntityTableData.MaxHp, curHP);

		if (curHP <= 0) {
			MapManager.Instance.Map.ShowBrokenEffect(CenterPos, transform);
			CurrentState = EnvEntityState.Harvested;
			MapManager.Instance.Map.GenerateDropItem(gameObject.transform.position, EnvEntityTableData.DropItemId);
			MapManager.Instance.Map.DestroyMapEntity(this);
		}
		else 
		{			
			StartCoroutine(CoFlash());		
		}
	}
	
	protected IEnumerator CoFlash()
	{
		spriteRenderer.material.SetInt("_SolidColorMode", 1);
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.material.SetInt("_SolidColorMode", 0);
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.material.SetInt("_SolidColorMode", 1);
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.material.SetInt("_SolidColorMode", 0);
		CurrentState = EnvEntityState.Idle;
	}
}
