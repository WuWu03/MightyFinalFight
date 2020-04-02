using DragonBones;
using FrameWork.Sound;
using UnityEngine;


namespace Runtime
{
    public class AvatarCtrl : BaseCtrl
    {
        public bool AttackSuccess
        {
            get;
            set;
        }

        protected override void Awake()
        {
            base.Awake();
            m_Owner = base.m_Owner as BaseRole;
        }

        public void Init(float[] attackWaitTime, int[] skillIDs)
        {
            m_AttackWaitTime = attackWaitTime;
            m_SkillManager = new SkillManager(m_Owner, skillIDs);
            m_Owner = GetComponent<BaseRole>();
            m_AttackMsgData = new AttackData();
        }

        public void Move(Vector2 dir)
        {
            if (!m_Owner.CanMove)
            {
                return;
            }

            MoveData moveMsgData = new MoveData()
            {
                Dir = dir,
            };

            m_Owner.OnMoveMsg(moveMsgData);
        }

        public void Attack(Vector2 dir)
        {
            if (!m_Owner.CanAttack)
            {
                return;
            }

            bool isJump = m_Owner.IsAnyState(typeof(RoleJump), typeof(RoleJumpDown));

            if (isJump)
            {
                JumpAttack(dir);
            }
            else
            {
                NormalAttack();
            }
        }

        public void Skill(int skillID)
        {
            if (!m_Owner.CanSkill)
            {
                return;
            }

            m_CurrSkillID = skillID;
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(m_CurrSkillID);
        }

        public void Jump(Vector2 jumpDir)
        {
            if (!m_Owner.CanJump) return;

            JumpData jumpMsgData = new JumpData()
            {
                Dir = jumpDir,
            };

            m_Owner.OnJumpMsg(jumpMsgData);
        }

        protected override void Update()
        {
            m_SkillManager.Update();
            if (m_Owner.ResGO == null) return;
            if (m_Owner.IsAnyState(typeof(RoleJumpAttack)))
            {
                if (!AttackSuccess)
                {
                    m_SkillManager.ExcuteSkill(m_CurrSkillID);
                }

                return;
            }

            if (m_AttackTimer > 0)
            {
                float currWait = m_AttackWaitTime[m_AttackIndex - 1 <= 0 ? 1 : m_AttackIndex - 1];
                if (currWait < 0)
                {
                    if (m_Owner.ActorAnimator.animation.isCompleted)
                    {
                        m_AttackIndex = 0;
                        m_AttackTimer = 0;
                        AttackSuccess = false;
                        m_Owner.FsmMachine.ChangeDefaultState();
                    }
                }
                else
                {
                    float attckStamp = Time.time - m_AttackTimer;
                    if (attckStamp > currWait)
                    {
                        m_AttackIndex = 0;
                        m_AttackTimer = 0;
                        AttackSuccess = false;
                        m_Owner.FsmMachine.ChangeDefaultState();
                    }
                }
            }
        }

        private void NormalAttack()
        {
            if (m_AttackWaitTime == null || m_AttackWaitTime.Length < 1)
            {
                return;
            }

            if (m_AttackIndex >= m_AttackWaitTime.Length)
            {
                return;
            }

            if (m_AttackTimer > 0 && Time.time - m_AttackTimer < 0.15f)
            {
                return;
            }

            if (m_AttackIndex == 0)
                AttackSuccess = true;

            if (AttackSuccess)
                m_AttackIndex++;
            else m_AttackIndex = 1;

            m_CurrSkillID = 1000 + m_AttackIndex;
            m_AttackMsgData.AttackType = AttackType.Attack;
            m_AttackMsgData.Dir = m_Owner.Dir;
            m_AttackMsgData.CanChangeDir = true;
            m_AttackMsgData.AnimationName = StaticConfig.SkillConfig.GetData(m_CurrSkillID).AnimationName;
            m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.OnAttackMsg(m_AttackMsgData);
            m_AttackTimer = Time.time;
        }

        private void JumpAttack(Vector2 dir)
        {
            AttackSuccess = false;
            m_CurrSkillID = 1004 + (dir.y < 0 ? 2 : 1);
            m_AttackMsgData.AttackType = AttackType.JumpAttack;
            m_AttackMsgData.Dir = m_Owner.Dir;
            m_AttackMsgData.AnimationName = StaticConfig.SkillConfig.GetData(m_CurrSkillID).AnimationName;
            m_Owner.OnAttackMsg(m_AttackMsgData);
        }

        private void SkillEvent(string type, EventObject eventObject)
        {
            m_SkillManager.ExcuteSkill(m_CurrSkillID);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        }

        private void SoundEvent(string type, EventObject eventObject)
        {
            Debug.Log(eventObject.name);
            SoundMgr.Ins.PlaySound(eventObject.name);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }

        private SkillManager m_SkillManager = null;
        private float[] m_AttackWaitTime = null;
        private AttackData m_AttackMsgData = null;
        protected new BaseRole m_Owner = null;
        private float m_AttackTimer = 0;
        private int m_AttackIndex = 0;
        private int m_CurrSkillID = 0;
    }
}