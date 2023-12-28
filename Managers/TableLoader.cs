using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class TableLoader : BaseManager<TableLoader>
{
	protected override void init()
	{
		TableParsing();
	}
	
	public void Load() {
		ItemTable.Instance.Load("Table_Item");
		EnvEntityTable.Instance.Load("Table_EnvEntity");
		MapTable.Instance.Load("Table_Map", "Table_MapMonsterSpawn");
		MonsterTable.Instance.Load("Table_Monster");	
		CraftableItemTable.Instance.Load("Table_CraftableItem", "Table_CraftingIngredient");	
		CustomerTable.Instance.Load("Table_Customer");	
		NPCTable.Instance.Load("Table_NPC");	
		StoryQuestTable.Instance.Load("Table_StoryQuest");
		LevelExpTable.Instance.Load("Table_LevelEXP");
		
		StoryQuestTable.Instance.LoadAfter();
	}
	
	private void TableParsing() 
	{
		Debug.Log("Table CSV To Json Parsing Start");
		
		Process psi = new Process();
		psi.StartInfo.FileName = @"python";
		psi.StartInfo.Arguments = @"D:\MInProject_JF\Assets\_Resources\Tables\paresr.py";
		psi.StartInfo.CreateNoWindow = false;
		psi.StartInfo.UseShellExecute = false;
		psi.Start();

		Debug.Log("Table CSV To Json Parsing End");
	}
}
