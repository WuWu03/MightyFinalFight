using System.Collections.Generic;
using UnityEngine;


public class SkillNearHitSelector : SkillBaseSelector
{
    public SkillNearHitSelector(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

 

    public override List<ICanBeHit> GetTargets()
    {
        m_ListTargets.Clear();

        for (int i = 0; i < m_Owner.Targets.Count; i++)
        {
            ICanBeHit hit = m_Owner.Targets[i].GetComponent<ICanBeHit>();
            if (hit == null) continue;

            bool isInRange = false;
            BaseSceneObject hitObj = hit as BaseSceneObject;

            Vector2 target = (hitObj.Pos - m_Owner.Pos).normalized;
            Vector2 normal = m_Owner.Dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
            Vector2 pos = m_Owner.Pos + m_SkillEffect.SelectorOffest;

            if (Vector2.Angle(target, normal) <= m_SkillEffect.SelectorAngle / 2)
            {
                isInRange = Vector2.Distance(hitObj.Pos, pos) <= m_SkillEffect.SelectorRadius;
            }

            if (isInRange && hit.CanBeHit)
            {
                m_ListTargets.Add(hit);
            }
        }

        return m_ListTargets;
    }

    public override List<GameObject> GetTargetsObj()
    {
        return m_Owner.Targets;
    }

    public override void Reset()
    {
        m_ListTargets.Clear();
    }

    public override void Exit()
    {
        m_ListTargets.Clear();
    }
}