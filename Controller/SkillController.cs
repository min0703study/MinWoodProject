using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SkillController : MonoBehaviour
{
    private List<SkillBase> skills = new List<SkillBase>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (SkillBase skill in skills)
        {
            if (skill.IsActive == false)
            {
                skill.IsActive = true;
                StartCoroutine(CoActiveSkill());
            }
        }
    }

    public void AddSkill(SkillBase skill)
    {
        skills.Add(skill);
        skill.transform.SetParent(gameObject.transform);
    }

    public void ActiveSkill(int slotIndex)
    {
        skills[slotIndex].ActiveSkill();
    }

    private IEnumerator CoActiveSkill()
    {
        yield return new WaitForSeconds(3.0f);
        skills[0].ActiveSkill();
    }
}
