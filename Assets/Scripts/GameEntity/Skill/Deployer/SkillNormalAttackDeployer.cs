using FrameWork.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class SkillNormalAttackDeployer : SkillDeployer
    {
        public SkillNormalAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
        {
            m_AttackMsgData = new AttackData();
        }

        public override void DeploySkill()
        {
            m_AttackMsgData.AttackType = AttackType.Attack;
            m_AttackMsgData.Dir = m_Owner.Dir;
            m_AttackMsgData.CanChangeDir = true;
            m_AttackMsgData.AnimationName = m_SkillData.AnimationName;
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnAttackMsg(m_AttackMsgData);
        }


        private void SkillEvent(string type, DragonBones.EventObject eventObject)
        {
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            base.DeploySkill();
        }

        private void SoundEvent(string type, DragonBones.EventObject eventObject)
        {
            SoundMgr.Ins.PlaySound(eventObject.name);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }

        private AttackData m_AttackMsgData = null;
    }
}
