using System.Collections;
using UnityEngine;

public class SkillBase : MonoBehaviour
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
	float degree = 0.0f;
	
	// Start is called before the first frame update

	private void Awake()
	{
	}

	// Update is called once per frame
	void Update()
	{
		transform.localPosition = Quaternion.Euler(0f, 0f, degree) * Vector3.up * 10.0f;
		float x = (GameManager.Instance.Player.CenterPos.x + (Mathf.Cos(degree) * 1.2f));
		float y = (GameManager.Instance.Player.CenterPos.y + (Mathf.Sin(degree) * 1.2f));

		transform.position = new Vector3(x, y, 0);

		degree = Mathf.Repeat(degree + Time.deltaTime, 360.0f);
	}

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
			unit?.OnDamaged(null, UserServerData.Instance.AtkValue);
		}
	}
}
