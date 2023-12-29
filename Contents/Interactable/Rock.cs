using System.Collections;
using DG.Tweening;
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
		
		float totalDamage = power;
		bool isCritical = false;
		if (Random.Range(0, 1000) <= (UserServerData.Instance.CriRate * 10))
		{
			totalDamage  = power * UserServerData.Instance.CriDamage;
			isCritical = true;
		}
		
		curHP -= totalDamage;
		
		progressBar.SetActive(true);
		progressBar.SetProgressBarValue(EnvEntityTableData.MaxHp, curHP);

		MapManager.Instance.Map.ShowDamageFont(CenterPos, totalDamage, 0, transform, isCritical);

		if (curHP <= 0) {
			MapManager.Instance.Map.ShowBrokenEffect(CenterPos, transform);
			CurrentState = EnvEntityState.Harvested;
			MapManager.Instance.Map.GenerateDropItem(gameObject.transform.position, EnvEntityTableData.DropItemId);
			MapManager.Instance.Map.DestroyMapEntity(this);
		}
		else 
		{			
			CoOnHitting();
			CurrentState = EnvEntityState.Idle;
		}
	}

	public void CoOnHitting() {
		Sequence dotweenSequence = DOTween.Sequence();
		dotweenSequence.Append(spriteRenderer.material.DOFloat(1.0f, "_Transparency", 0.1f));
		dotweenSequence.Insert(0, transform.DOScale(transform.localScale * 0.9f, 0.1f));
		dotweenSequence.SetEase(Ease.InOutBounce);
		dotweenSequence.SetLoops(2, LoopType.Yoyo);
	}
	
}
