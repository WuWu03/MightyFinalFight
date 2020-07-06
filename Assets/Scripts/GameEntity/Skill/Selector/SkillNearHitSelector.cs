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

            Vector2 target = (targetObj.Pos - owner.Pos).normalized;
            Vector2 normal = owner.Dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;

            if (Vector2.Angle(target, normal) <= skillData.SkillEffects[Index].SelectorAngle / 2)
            {
                isInRange = Mathf.Abs(targetObj.Pos.x - owner.Pos.x) <= skillData.SkillEffects[Index].SelectorRadius; //owner.Dir > 0 ? targetObj.Pos.x >= owner.Pos.x : targetObj.Pos.x <= owner.Pos.x;
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