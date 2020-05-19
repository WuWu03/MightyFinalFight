using FrameWork.GameEntity;
using System.Collections.Generic;
using UnityEngine;


public class SkillNearHitSelector : ISkillSelector
{
    public SkillNearHitSelector()
    {
        m_ListTargets = new List<ICanBeHit>();
    }

    public int Index { get; set; }

    public List<ICanBeHit> GetTargets(BaseRole owner, SkillData skillData)
    {
        m_ListTargets.Clear();
        TriggerTargets trigger = owner.GetComponent<TriggerTargets>();
        if (trigger == null) return m_ListTargets;

        for (int i = 0; i < trigger.Targets.Count; i++)
        {
            ICanBeHit hit = trigger.Targets[i].GetComponent<ICanBeHit>();
            BaseObject targetObj = trigger.Targets[i].GetComponent<BaseObject>();

            bool canBeHit = hit != null && hit.CanBeHit;
            bool isInRange = false;

            if (Mathf.Abs(targetObj.Pos.y - owner.Pos.y) < 0.03f)
            {
                isInRange = owner.Dir > 0 ? targetObj.Pos.x >= owner.Pos.x : targetObj.Pos.x <= owner.Pos.x;
            }

            if (isInRange && canBeHit)
            {
                m_ListTargets.Add(hit);
            }
        }

        return m_ListTargets;
    }

    public List<GameObject> GetTargetsObj(BaseRole owner, SkillData skillData)
    {
        return owner.GetComponent<TriggerTargets>().Targets;
    }

    private List<ICanBeHit> m_ListTargets = null;
}