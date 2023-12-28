using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

using Newtonsoft.Json;
using UnityEngine;

namespace TableData
{
	public class LevelExp
	{
		public int Level { get; set; }
		public int RequiredExp { get; set;}
	}
}

public class LevelExpTable: BaseTable<LevelExpTable>
{
	public List<TableData.LevelExp> LevelExps { get; private set; } = new List<TableData.LevelExp>();
	public Dictionary<int, TableData.LevelExp> LevelExpDict { get; private set; } = new Dictionary<int, TableData.LevelExp>();

	protected override void init()
	{
		base.init();
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		LevelExps = JsonConvert.DeserializeObject<List<TableData.LevelExp>>(textAsset.text);
		
		foreach (var value in LevelExps)
			LevelExpDict.Add(value.Level, value);
			
		Debug.Log($"{Util.MyMethod()} TryGetValue Error");
	}

	public int GetRequiredExpForLevel(int level) 
	{
		if(LevelExpDict.TryGetValue(level, out var exp)) 
		{
			return exp.RequiredExp;
		};
		
		Debug.Log($"{Util.MyMethod()} TryGetValue Error");
		return 0;
	}
}
