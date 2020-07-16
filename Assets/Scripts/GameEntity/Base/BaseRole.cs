using FrameWork;
using FrameWork.Camera;
using FrameWork.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BaseRole : BaseAvatar, ICanBeHit
{
    public float AttackValue
    {
        get { return m_AttackValue; }
        set { m_AttackValue = value; }
    }

    public float AttackSpeed
    {
        get { return m_AttackSpeed; }
        set { m_AttackValue = value; }
    }

    public Vector2 JumpForce
    {
        get { return m_JumpForce; }
        set { m_JumpForce = value; }
    }

    public float Defense
    {
        get { return m_Defense; }
        set { m_Defense = value; }
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

    public virtual bool IsBeCatch
    {
        get
        {
            return m_IsBeCatch;
        }
    }

    public bool IsDead
    {
        get
        {
            return m_Health <= 0;
        }
    }

    public virtual bool CanMove
    {
        get
        {
            return !m_IsDropTrag &&
                   !m_IsBeCatch && IsAnyState(typeof(RoleIdle),
                typeof(RoleMove),
                typeof(RoleAttack),
                typeof(RoleJump));
        }
    }

    public virtual bool CanAttack
    {
        get
        {
            return !m_IsDropTrag && !m_IsJumpAttack &&
                   !m_IsBeCatch && IsAnyState(typeof(RoleIdle),
                typeof(RoleMove),
                typeof(RoleJump),
                typeof(RoleAttack));
        }
    }

    public virtual bool CanJump
    {
        get
        {
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove));
        }
    }

    public virtual bool CanSkill
    {
        get
        {
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump));
        }
    }

    public bool IsDropTrag
    {
        get
        {
            return m_IsDropTrag;
        }
    }

    public Vector2 HurtPos
    {
        get
        {
            return m_Pos;
        }
    }

    public virtual bool CanChangeDefaultState
    {
        get
        {
            return !IsAnyState(typeof(RoleAttack)) && IsInGround;
        }
    }

    public bool HitSuccess
    {
        get
        {
            return m_CurrCtrl != null && m_CurrCtrl.AttackSuccess;
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
        AddState<RoleAttack>();
        AddState<RoleJumpAttack>();
        AddState<RoleHurt>();
        AddState<RoleSwoon>();
        AddState<RoleDead>();
        AddState<RoleAwaken>();
        AddState<RoleSkill>();
    }

    //初始化基本数值
    public override void InitData(BaseSceneObjectData data)
    {
        base.InitData(data);
        BaseRoleData baseRoleData = data as BaseRoleData;
        m_AttackSpeed = baseRoleData.AttackSpeed;
        m_AttackValue = baseRoleData.AttackValue;
        m_Defense = baseRoleData.Defense;
        m_JumpForce = baseRoleData.JumpForce;
        m_MoveSpeed = baseRoleData.MoveSpeed;
    }

    public T AddCtrl<T>() where T : BaseRoleCtrl,new()
    {
        if(m_CurrCtrl == null)
        {
            m_CurrCtrl = new T();
            m_CurrCtrl.SetOwner(this);
        }

        return m_CurrCtrl as T;
    }

    public override void Release()
    {
        m_CurrCtrl.Release();
        m_CurrCtrl = null;
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

        if (m_CurrCtrl != null)
            m_CurrCtrl.Update();

        if (m_FsmMachine == null || !m_FsmMachine.IsRunning) return;
        if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic) return;

        UpdatePos2(transform.localPosition.x, Pos.y);

        if (IsFloat)
        {
            return;
        }

        //if (!m_IsJumpAttack)
        //{
            OnDropEvent.Invoke();
        //}

        OnDropEvent.RemoveAllListeners();

        CheckDropTrag();
        CheckGround();
    }

    public virtual void OnAttackMsg(AttackData data,bool forceJumpAttack = false)
    {
        if (data == null) return;
        m_IsJumpAttack = IsAnyState(typeof(RoleJump)) || forceJumpAttack;
        GetState<RoleAttack>().StateParam = data;
        ChangeState<RoleAttack>();
        PlayAnimation(data.AnimationName, data.AnimTime, data.AnimSpeed * m_AttackSpeed);
    }

    public virtual void OnSkillMsg(SkillData data)
    {
        if (data == null) return;
        ChangeState<RoleSkill>();
        PlayAnimation(data.AnimationName, data.AnimTime, data.AnimSpeed);
    }

    public virtual void OnMoveMsg(MoveData data)
    {
        if (data == null) return;
       
        if (IsAnyState(typeof(RoleJump)))
        {
            GetState<RoleJump>().StateParam.Dir = data.Dir;
            return;
        }

        if (IsAnyState(typeof(RoleAttack)))
        {
            GetState<RoleAttack>().StateParam.Dir = data.Dir.x;             
            return;
        }

        if (data.Dir == Vector2.zero)
        {
            ChangeState<RoleIdle>();
            return;
        }

        m_MoveDir = data.Dir;
        m_CurrCtrl.ExitSkill();
        GetState<RoleMove>().CanChangeDir = data.CanChangeDir;
        ChangeState<RoleMove>();
    }

    public virtual void OnJumpMsg(JumpData data)
    {
        if (data == null) return;
        m_CurrCtrl.ExitSkill();
        GetState<RoleJump>().StateParam = data;
        ChangeState<RoleJump>();
        FrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "Jump");
    }

    public virtual void OnHurtMsg(HurtData data)
    {
        if (data == null) return;
        if (!CanBeHit) return;
        m_CurrCtrl.ExitSkill();
        SubHealth(data.AttackValue);
        m_IsSmoon = data.IsSwoon;

        if (string.IsNullOrEmpty(data.HurtAnim))
        {
            data.HurtAnim = AnimName.Hurt;
        }

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

        if (data.AttackValue > 0)
        {
            string hurtSound = string.IsNullOrEmpty(data.HurtSound) ? "OnHit02" : data.HurtSound;
            FrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", hurtSound);
        }
    }

    public virtual void OnDropTragMsg(DropTragData data)
    {
        if (data == null) return;
        if (!IsAnyState(typeof(RoleMove), typeof(RoleIdle), typeof(RoleJump)) && !m_IsJumpAttack) return;

        if (IsAnyState(typeof(RoleMove), typeof(RoleIdle)))
        {
            PlayAnimation(AnimName.JumpDown);
        }

        m_DropTragData = data;
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_IsDropTrag = true;
        m_CurrCtrl.ExitSkill();
        if (ObjectType == ObjectType.Player)
            CameraMgr.Ins.EndFollow();
    }

    public virtual void SetCatch(bool value)
    {
        m_IsBeCatch = value;
        if (value)
        {
            m_CurrCtrl.ExitSkill();
        }
    }

    public virtual List<ICanBeHit> OnHitStart()
    {
        return null;
    }

    public virtual void OnHitEnd(SkillData skillData,bool isHurtTarget) 
    {
        if (skillData.Type != SkillData.SkillType.SkillAttack && m_CurrCtrl != null)
        {
            m_CurrCtrl.AttackSuccess = isHurtTarget;
        }
    }

    private void CheckDropTrag()
    {
        if (!m_IsDropTrag) return;

        Rect visionRect = CameraMgr.Ins.GetVision();

        if ((transform.localPosition + Vector3.up * 0.6f).y + 0.1f < visionRect.yMin)
        {
            if (m_ObjectType == ObjectType.Player)
            {
                SubHealth(m_DropTragData.AttackValue);
                if (m_Health <= 0)
                {
                    GetState<RoleDead>().ReBirthPos = m_DropTragData.InitPos;
                    ChangeState<RoleDead>();
                }
                else
                {
                    SetPos(m_DropTragData.InitPos);
                    ChangeState<RoleIdle>();
                    CameraMgr.Ins.StartFollow();
                }
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
        m_CurrCtrl.ExitSkill();

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
            if (m_Health > 0)
            {
                ChangeState<RoleIdle>();
                SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnDrop");
            }
            else ChangeState<RoleDead>();
        }
    }

    protected bool m_IsSmoon = false;
    protected float m_AttackSpeed = 0.8f;
    protected float m_AttackValue = 0;
    protected float m_Defense = 0;
    protected bool m_IsJumpAttack = false;
    protected bool m_IsDropTrag = false;
    protected bool m_IsBeCatch = false;
    protected BaseRoleCtrl m_CurrCtrl = null;
    protected DropTragData m_DropTragData = null;
    protected Vector2 m_JumpForce = Vector2.zero;
}