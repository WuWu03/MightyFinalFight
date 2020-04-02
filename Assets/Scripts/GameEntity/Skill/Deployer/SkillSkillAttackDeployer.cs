using FrameWork.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class SkillSkillAttackDeployer : SkillDeployer
    {
        public SkillSkillAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner) {}

        public override void DeploySkill()
        {
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(SkillID);
        }

        public override bool IsAllComplete()
        {
            bool isComplete = base.IsAllComplete();
            if (isComplete)
            {
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            }

            return isComplete;
        }

        private void SkillEvent(string type, DragonBones.EventObject eventObject)
        {
            base.DeploySkill();
        }

        private void SoundEvent(string type, DragonBones.EventObject eventObject)
        {
            SoundMgr.Ins.PlaySound(eventObject.name);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }
    }
}

