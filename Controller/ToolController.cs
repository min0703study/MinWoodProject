
using UnityEngine;

public class ToolController : BaseController
{
	public Define.ToolType ToolType { get; set; }
	public int Power = 10;
	
	public bool IsUsingTool { get; set; } = false;

	protected override void Init()
	{
		base.Init();
		gameObject.SetActive(false);
	}

	public void BeginToolUsage()
	{
		// 툴 사용을 시작할 때 실행할 코드를 여기에 작성
		IsUsingTool = true;
		gameObject.SetActive(true);
	}

	public void EndToolUsage()
	{
		// 툴 사용을 종료할 때 실행할 코드를 여기에 작성
		IsUsingTool = false;
		gameObject.SetActive(false);
	}
}
