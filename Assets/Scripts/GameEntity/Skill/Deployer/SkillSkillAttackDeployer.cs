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
            if(!CheckStatus(m_SkillData.Status))
            {
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
                return;
            }

            if(m_SkillData.DeployeType == Config.SkillData.SkillDeployeType.Just)
            {
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
                m_Owner.OnSkillMsg(m_SkillData);
                base.DeploySkill();
                return;
            }

            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(m_SkillData);
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

        private bool CheckStatus(Config.SkillData.SkillStatus status)
        {
            bool ret = false;
            switch (status)
            {
                case Config.SkillData.SkillStatus.None:
                    ret = true;
                    break;
                case Config.SkillData.SkillStatus.Float:
                    ret = m_Owner.IsFloat;
                    break;
                case Config.SkillData.SkillStatus.Ground:
                    ret = m_Owner.IsInGround;
                    break;
            }

            return ret;
        }
    }
}

