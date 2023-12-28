using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

public class Define
{
	public enum Scene
	{
		Unknown,
		IntroScene,
		LobbyScene,
		DungeonScene,
	}
	
	public enum Map
	{
		Unknown,
		Home,
		Guild,
		Dungeon,
		Forest,
		Backyard
	}
	
	public enum EnvObjectType
	{
		Tree,
		Nodes,
	}
	
	public enum ToolType 
	{
		None,
		Pick,
		Axe
	}
	
	public enum CustomerState 
	{
		None,
		EnteredShop, 
		MoveToCounter, 
		MoveToExitDoor,
		WaitToOrder
	}
	
	public enum DayCycleType
	{
		Morning,
		Afternoon,
		Evening,
		Night,
	}
	
	#region PlayerState
	public enum PlayerState
	{
		Idle,
		Moving,
		GoToTarget,
		//Mining
		Mining,
		Dashing,
		//Battle
		InBattle,
		OnDamaged,
		Dead
	}
	#endregion
	
	#region CatState
	public enum CatState
	{
		Idle,
		Moving,
		GoToTarget,
		Sleeping,
		//Mining
		WakeUp
	}
	
	public enum OrderState
	{
		DeliveryNotReady,
		DeliveryReady,
		Delivering,
		Completed
	}
	
	public enum EnvEntityState
	{
		Idle,
		OnDamaged,
		Harvested,		
		Broken,
	}
	#endregion
	
	#region MonsterState
	public enum MonsterState
	{
		Idle,
		Chase,
		Moving,
		Attack,
		OnDamaged,
		Dead,
	}
	#endregion

	
	#region SpeechBalloonState
	public enum SpeechBalloonState
	{
		Hello,
		Happy,
		WaitOrder,
		Sad
	}
	#endregion
	
	public enum WeaponMode 
	{
		None,
		Sword,
		Riffle
	}
}
