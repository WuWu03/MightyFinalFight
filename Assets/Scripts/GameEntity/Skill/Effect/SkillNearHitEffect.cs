using Runtime.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class SkillNearHitEffect : ISkillEffect
    {
        public bool IsCompleted
        {
            get
            {
                return m_Complete;
            }
        }

        public void Effect(BaseRole owner, SkillData skillData, ISkillSelector skillSelector)
        {
            List<GameObject> targets = skillSelector.GetTargets(owner, skillData);
            
            bool hurtTarget = false;
            m_Complete = false;
            for (int i = 0; i < targets.Count; i++)
            {
                ICanBeHit hit = targets[i].GetComponent<ICanBeHit>();
                if (hit != null && hit.CanBeHit)
                {
                    hurtTarget = true;
                    hit.OnHurtMsg(new HurtData()
                    {
                        AttackForce = new Vector2(skillData.AddTargetForce.x * owner.Dir, skillData.AddTargetForce.y),
                        IsSwoon = skillData.IsSmoon,
                        AttackerID = owner.ID,
                        AttackValue = 1,
                    });
                }
            }

            m_Complete = true;

            if(owner.ObjectType == ObjectType.Player)
            {
                owner.GetComponent<AvatarCtrl>().AttackSuccess = hurtTarget;
            }
        }

        private bool m_Complete = false;
    }
}

