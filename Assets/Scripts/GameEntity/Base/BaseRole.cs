using FrameWork.Camera;
using Runtime.Config;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    public class BaseRole : BaseAvatar, ICanBeHit
    {
        public float AttackSpeed
        {
            get
            {
                return m_AttackSpeed;
            }
            set
            {
                m_AttackSpeed = value;
            }
        }

        public Vector2 JumpForce
        {
            get
            {
                return m_JumpForce;
            }
            set
            {
                m_JumpForce = value;
            }
        }

        public float AttackValue
        {
            get;
            set;
        }

        public float AttackRange
        {
            get
            {
                return m_AttackRange;
            }
        }

        public virtual bool CanBeHit
        {
            get
            {
                return m_FsmMachine.CurrStateType != typeof(RoleSwoon) &&
                       m_FsmMachine.CurrStateType != typeof(RoleDead) &&
                       m_FsmMachine.CurrStateType != typeof(RoleAwaken) &&
                       m_Health > 0;
            }
        }

        public virtual bool CanMove
        {
            get
            {
                return !m_IsDropTrag && IsAnyState(typeof(RoleIdle),
                                  typeof(RoleMove),
                                  typeof(RoleAttack),
                                  typeof(RoleJump));
            }
        }

        public virtual bool CanAttack
        {
            get
            {
                return !m_IsDropTrag && !m_IsJumpAttack && IsAnyState(typeof(RoleIdle),
                                  typeof(RoleMove),
                                  typeof(RoleJump),
                                  typeof(RoleAttack));
            }
        }

        public virtual bool CanJump
        {
            get
            {
                return !m_IsDropTrag && IsAnyState(typeof(RoleIdle), typeof(RoleMove));
            }
        }

        public bool CanSkill
        {
            get
            {
                return !m_IsDropTrag && IsAnyState(typeof(RoleIdle), typeof(RoleMove));
            }
        }

        public bool IsDropTrag
        {
            get
            {
                return m_IsDropTrag;
            }
        }

        public UnityEvent OnDropEvent = new UnityEvent();
        public UnityEvent OnGroundEvent = new UnityEvent();

        public override void Init(int id, string name)
        {
            base.Init(id, name);
            AddState<RoleIdle>();
            AddState<RoleMove>();
            AddState<RoleJump>();
            AddState<RoleDropTrag>();
            AddState<RoleAttack>();
            AddState<RoleJumpAttack>();
            AddState<RoleHurt>();
            AddState<RoleSwoon>();
            AddState<RoleDead>();
            AddState<RoleAwaken>();
            AddState<RoleSkill>();
        }

        public override void Release()
        {
            base.Release();
        }

        protected override void OnResComplete(GameObject go)
        {
            base.OnResComplete(go);
            m_MoveDir = Vector2.right;
            m_FsmMachine.Start<RoleIdle>();
        }

        protected override void Update()
        {
            base.Update();
            if (m_FsmMachine == null || !m_FsmMachine.IsRunning) return;
            if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic) return;

            UpdatePos2(transform.localPosition.x, Pos.y);

            if (IsFloat)
            {
                return;
            }

            if(!m_IsJumpAttack)
            {
                OnDropEvent.Invoke();
            }

            OnDropEvent.RemoveAllListeners();

            CheckDropTrag();
            CheckGround();
        }

        public virtual void OnAttackMsg(AttackData data)
        {
            if (data == null) return;

            m_IsJumpAttack = IsAnyState(typeof(RoleJump));
            GetState<RoleAttack>().StateParam = data;
            ChangeState<RoleAttack>();

            SetTrigger(data.AnimationName);
            PlayAnimation(data.AnimationName, 1, m_AttackSpeed);
        }

        public virtual void OnSkillMsg(SkillData data)
        {
            if (data == null) return;
            ChangeState<RoleSkill>();
            SetTrigger(data.AnimationName);
            PlayAnimation(data.AnimationName, 1, 0.4f);
        }

        public virtual void OnMoveMsg(MoveData data)
        {
            if (data == null) return;
            m_MoveDir = data.Dir;

            if (data.Dir.x != 0)
            {
                m_Dir = data.Dir.x > 0 ? 1 : -1;
            }

            if (IsAnyState(typeof(RoleJump)))
            {
                GetState<RoleJump>().StateParam.Dir = data.Dir;
                return;
            }

            if (IsAnyState(typeof(RoleAttack)))
            {
                GetState<RoleAttack>().StateParam.Dir = m_Dir;
                return;
            }

            if (data.Dir.Equals(Vector2.zero))
            {
                ChangeState<RoleIdle>();
                return;
            }

            ChangeState<RoleMove>();
        }

        public virtual void OnJumpMsg(JumpData data)
        {
            if (data == null) return;

            GetState<RoleJump>().StateParam = data;
            ChangeState<RoleJump>();
        }

        public virtual void OnHurtMsg(HurtData data)
        {
            if (data == null) return;
            if (!CanBeHit) return;
 
            m_Health -= data.AttackValue;
            m_IsSmoon = data.IsSwoon;

            if (m_IsSmoon)
            {
                GetState<RoleSwoon>().Force = data.AttackForce;
                ChangeState<RoleSwoon>();
            }
            else
            {
                GetState<RoleHurt>().StateParam = data;
                ChangeState<RoleHurt>();
            }
        }

        public void OnDropTragMsg(DropTragData data)
        {
            if (data == null) return;
            if (!IsAnyState(typeof(RoleMove), typeof(RoleIdle), typeof(RoleJump))&&!m_IsJumpAttack) return;

            if (IsAnyState(typeof(RoleMove), typeof(RoleIdle)))
            {
                PlayAnimation(AnimName.JumpDown);
            }

            m_DropTragData = data;
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_IsDropTrag = true;

            if (ObjectType == ObjectType.Player)
                CameraMgr.Ins.EndFollow();
        }

        private void CheckDropTrag()
        {
            if (!m_IsDropTrag) return;

            Vector2[] vision = CameraMgr.Ins.GetVision();

            if ((transform.localPosition + Vector3.up * 0.6f).y + 0.1f < vision[0].y)
            {   
                if (m_ObjectType == ObjectType.Player)
                {
                    SetPos(m_DropTragData.InitPos);
                    ChangeState<RoleIdle>();
                    CameraMgr.Ins.StartFollow();
                }
                else
                {
                    Release();
                }

                m_DropTragData = null;
                m_IsDropTrag = false;
            }
        }

        private void CheckGround()
        {
            if (!IsInGround || m_IsDropTrag) return;

            OnGroundEvent.Invoke();
            OnGroundEvent.RemoveAllListeners();
            m_IsJumpAttack = false;

            if (IsAnyState(typeof(RoleSwoon)))
            {
                if (m_Animator.animation.isCompleted)
                {
                    m_Rigidbody.velocity = Vector2.zero;
                    if (m_Health > 0) ChangeState<RoleAwaken>();
                    else ChangeState<RoleDead>();
                }
            }
            else
            {
                ChangeState<RoleIdle>();
            }
        }

        protected bool m_IsSmoon = false;
        protected float m_Attack = 100;
        protected float m_AttackSpeed = 0.8f;
        protected float m_AttackRange = 0.25f;
        protected bool m_IsJumpAttack = false;
        protected bool m_IsDropTrag = false;
        protected DropTragData m_DropTragData = null;
        protected Vector2 m_JumpForce = new Vector2(40f, 150f);
    }
}