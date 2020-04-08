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
            Debug.Log("adddskilllll");
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(m_SkillData);
        }

        public override bool IsAllComplete()
        {
            bool isComplete = base.IsAllComplete();
            if (isComplete)
            {
                Debug.Log("removeskilllll");
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            }

            return isComplete;
        }

        private void SkillEvent(string type, DragonBones.EventObject eventObject)
        {
            Debug.Log("skilllll");
            base.DeploySkill();
        }

        private void SoundEvent(string type, DragonBones.EventObject eventObject)
        {
            SoundMgr.Ins.PlaySound(eventObject.name);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }
    }
}

