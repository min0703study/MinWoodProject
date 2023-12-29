using System.Collections;
using UnityEngine;

public class SwordSkill : MonoBehaviour
{
	#region Skill Type
	public enum PlayerSkillType
	{
		None,
	}

	public bool IsActive { get; set; }

	PlayerSkillType skillType;

	public PlayerSkillType SkillType { get; set; }
	#endregion

	public void ActiveSkill()
	{
		this.gameObject.SetActive(true);
		StartCoroutine(CoCoolTime());
	}

	protected virtual IEnumerator CoCoolTime()
	{
		yield return new WaitForSeconds(3.0f);
		this.gameObject.SetActive(false);
		this.IsActive = false;
	}

	private void OnTriggerEnter2D(Collider2D collisionData)
	{
		UnitController unit = collisionData.GetComponent<UnitController>();

		if (collisionData.gameObject.layer == LayerMask.NameToLayer("Monster"))
		{
			unit?.OnDamaged(GameManager.Instance.Player, UserServerData.Instance.AtkValue * 2, 2);
		}
	}
}
