using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserServerData : BaseData<UserServerData>
{
	public int CostumeId { get; private set; }
	public int AtkLevel { get; private set; }
	public float AtkValue { get; private set; }
	public int HpLevel { get; private set; }
	public float HpValue { get; private set; }
	public int EXP { get; private set; }
	public float CriRate { get; private set; }
	public float CriDamage { get; private set; }
	public int Level {get; private set; }
	public int Coin { get; private set; }
	public int Dia { get; private set; }
	
	public event Action OnPlayerChanged;

	protected override void init() 
	{
		base.init();
		
		AtkLevel = 1;
		Level = 1;
		AtkValue = 30;
		HpLevel = 1;
		HpValue = 100;   
		CriRate = 20f;
		CriDamage = 2f;
		EXP = 0;
		Coin = 0;
		Dia = 0;
	}

	public void AtkLevelUp()
	{
		AtkLevel++;
		AtkValue += 10;
	}

	public void HpLevelUp()
	{
		HpLevel++;
		HpValue += 10;
	}

	public void AddCoin(int addValue)
	{
		Coin += addValue;
		OnPlayerChanged?.Invoke();
	}
	
	public void AddEXP(int addValue)
	{
		EXP += addValue;
		OnPlayerChanged?.Invoke();
	}

	public void LevelUp(int exp)
	{
		Level++;
		EXP = exp;
		
		OnPlayerChanged?.Invoke();
	}
}
