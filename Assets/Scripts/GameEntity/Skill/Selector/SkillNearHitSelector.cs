using FrameWork.GameEntity;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class SkillNearHitSelector : ISkillSelector
    {
        public SkillNearHitSelector()
        {
            m_ListTargets = new List<GameObject>();
        }

        public List<GameObject> GetTargets(BaseRole owner, SkillData skillData)
        {
            m_ListTargets.Clear();
            TriggerTargets trigger = owner.GetComponent<TriggerTargets>();
            if (trigger == null) return m_ListTargets;

            for (int i = 0; i < trigger.Targets.Count; i++)
            {
                ICanBeHit hit = trigger.GetComponent<ICanBeHit>();
                BaseObject targetObj = trigger.Targets[i].GetComponent<BaseObject>();

                bool canBeHit = hit != null && hit.CanBeHit;
                bool isInRange = false;

                if (Mathf.Abs(targetObj.Pos.y - owner.Pos.y) < 0.03f)
                {
                    isInRange = owner.Dir > 0 ? targetObj.Pos.x >= owner.Pos.x : targetObj.Pos.x <= owner.Pos.x;
                }

                if (isInRange && canBeHit)
                {
                    m_ListTargets.Add(trigger.Targets[i]);
                }
            }

            return m_ListTargets;
        }

        private List<GameObject> m_ListTargets = null;
    }
}

