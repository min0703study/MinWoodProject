using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TableData
{
	public class NPC
	{
		public int NPCId { get; set; }
		public string NPCTextId { get; set; }
		public string Name { get; set;}
		
		public string PrefabAddress { get; set; }
		public string IconSpriteAddress { get; set; }
		public string PortraitSpriteAddress { get; set; }
	}
}

public class NPCTable: BaseTable<NPCTable>
{
	public List<TableData.NPC> NPCs {get; private set;} = new List<TableData.NPC>();
	public Dictionary<int, TableData.NPC> NPCDict { get; private set; } = new Dictionary<int, TableData.NPC>();
	protected override void init()
	{
		base.init();
	}
	
	public void Load(string assetLabel)
	{
		TextAsset textAsset = ResourceManager.Instance.Load<TextAsset>(assetLabel);
		NPCs = JsonConvert.DeserializeObject<List<TableData.NPC>>(textAsset.text);
		
		foreach (var npc in NPCs)
			NPCDict.Add(npc.NPCId, npc);
	}

	public TableData.NPC GetNPCTableDataById(int npcId) 
	{
		Dictionary<int, TableData.NPC> dict = NPCDict;
		return dict[npcId];
	}
}
