using FrameWork.Camera;
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
                if (m_SkillData != null && m_SkillData.DeployeType == SkillData.SkillDeployeType.Animtion)
                {
                    m_Complete = m_Owner.IsPlayComplete();
                }

                return m_Complete;
            }
        }

        public void Effect(BaseRole owner, SkillData skillData, ISkillSelector skillSelector)
        {
            m_Owner = owner;
            m_SkillData = skillData;

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
                        AttackerDir = owner.Dir,
                        AttackForce = new Vector2(skillData.AddTargetForce.x * owner.Dir, skillData.AddTargetForce.y),
                        IsSwoon = skillData.IsSmoon,
                        AttackerID = owner.ID,
                        AttackValue = 1,
                    });

                    if(skillData.IsShakeCamera)
                    {
                        CameraMgr.Ins.Shake();
                    }
                }
            }

            m_Complete = true;

            if (skillData.Type != SkillData.SkillType.SkillAttack)
            {
                owner.GetComponent<AvatarCtrl>().AttackSuccess = hurtTarget;
            }
        }

        public void Reset()
        {
            m_Complete = false;
            m_Owner = null;
            m_SkillData = null;
        }

        private BaseRole m_Owner = null;
        private SkillData m_SkillData = null;
        private bool m_Complete = false;
    }
}

