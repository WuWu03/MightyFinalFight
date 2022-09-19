using System.Collections.Generic;
using UnityEngine;


public class SkillNearHitSelector : SkillBaseSelector
{
    public SkillNearHitSelector(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

 

    public override List<ICanBeHit> GetTargets()
    {
        m_ListTargets.Clear();

        for (int i = 0; i < m_Owner.targets.Count; i++)
        {
            ICanBeHit hit = m_Owner.targets[i].GetComponent<ICanBeHit>();
            if (hit == null) continue;

            bool isInRange = false;
            BaseSceneObject hitObj = hit as BaseSceneObject;

            Vector2 target = (hitObj.pos - m_Owner.pos).normalized;
            Vector2 normal = m_Owner.dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
            Vector2 pos = m_Owner.pos + m_SkillEffect.SelectorOffest;

            if (Vector2.Angle(target, normal) <= m_SkillEffect.SelectorAngle / 2)
            {
                isInRange = Vector2.Distance(hitObj.pos, pos) <= m_SkillEffect.SelectorRadius;
            }

            if (isInRange && hit.canBeHit)
            {
                m_ListTargets.Add(hit);
            }
        }

        return m_ListTargets;
    }

    public override List<GameObject> GetTargetsObj()
    {
        return m_Owner.targets;
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