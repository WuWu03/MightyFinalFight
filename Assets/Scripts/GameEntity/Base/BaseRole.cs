using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using System.Collections.Generic;
using UnityEngine;


public class BaseRole : BaseAvatar, ICanBeHit
{
    public float AttackValue
    {
        get
        {
            return m_AttackValue;
        }
        set
        {
            m_AttackValue = value;
        }
    }

    public float AttackSpeed
    {
        get
        {
            return m_AttackSpeed;
        }
        set
        {
            m_AttackValue = value;
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

    public float Defense
    {
        get
        {
            return m_Defense;
        }
        set
        {
            m_Defense = value;
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

    public bool IsBeCatch
    {
        get
        {
            return m_IsBeCatch;
        }
    }

    public bool IsBeThrow
    {
        get
        {
            return m_IsBeThrow;
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
                typeof(RoleSkill),
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
            return !m_IsDropTrag && !m_IsBeCatch && IsInGround && IsAnyState(typeof(RoleIdle), typeof(RoleMove));
        }
    }

    public virtual bool CanSkill
    {
        get
        {
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump),typeof(RoleAttack));
        }
    }

    public bool IsDropTrag
    {
        get
        {
            return m_IsDropTrag;
        }
    }

    public bool IsDropGround
    {
        get
        {
            return m_IsDropGround;
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

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<RoleIdle>();
        AddState<RoleMove>();
        AddState<RoleJump>();
        AddState<RoleAttack>();
        AddState<RoleHurt>();
        AddState<RoleSwoon>();
        AddState<RoleDead>();
        AddState<RoleAwaken>();
        AddState<RoleSkill>();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
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

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_MoveDir = Vector2.right;
        m_FsmMachine.Start<RoleIdle>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurrCtrl != null)
            m_CurrCtrl.Update();
    }

    public virtual void OnAttackMsg(AttackData data, bool forceJumpAttack = false)
    {
        if (data == null) return;
        m_IsJumpAttack = IsAnyState(typeof(RoleJump)) || forceJumpAttack;

        if (m_IsJumpAttack && data.AddSelfForce != Vector2.zero)
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_Rigidbody.velocity = Vector2.zero;
            m_Rigidbody.AddForce(new Vector2(data.AddSelfForce.x * m_Dir, data.AddSelfForce.y));
        }

        GetState<RoleAttack>().CanChangeDir = data.CanChangeDir;
        ChangeState<RoleAttack>();
        PlayAnimation(data.AnimName, data.AnimTime, data.AnimSpeed * m_AttackSpeed);
    }

    public virtual void OnSkillMsg(SkillConfigData data)
    {
        if (data == null) return;
        GetState<RoleSkill>().CanChangeDir = data.CanChangeDir;
        ChangeState<RoleSkill>();
        PlayAnimation(data.AnimationName, data.AnimTime, data.AnimSpeed);
    }

    public virtual void OnMoveMsg(MoveData data)
    {
        if (data == null) return;

        if (IsAnyState(typeof(RoleJump)))
        {
            GetState<RoleJump>().Dir = data.Dir.x;
            return;
        }

        if (IsAnyState(typeof(RoleAttack)))
        {
            GetState<RoleAttack>().Dir = data.Dir.x;
            return;
        }

        if (IsAnyState(typeof(RoleSkill)))
        {
            GetState<RoleSkill>().Dir = data.Dir.x;
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
        RoleJump roleJump = GetState<RoleJump>();
        roleJump.CanChangeDir = data.CanChangeDir;
        roleJump.Dir = data.Dir.x;
        ChangeState<RoleJump>();
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, SoundName.DefaultJump);
    }

    public virtual void OnHurtMsg(HurtData data)
    {
        if (data == null) return;
        if (!CanBeHit) return;
        m_CurrCtrl.ExitSkill();
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
            GetState<RoleHurt>().HurtAnim = data.HurtAnim;
            ChangeState<RoleHurt>();
        }

        if (data.IsGroundHurt && data.AttackForce.y > 0)
            m_OnGroundHurtData = data;
        else if (data.AttackValue > 0)
        {
            OnGroundHurtMsg(data);
        }
    }

    public virtual void OnDefenseMsg(HurtData data)
    {

    }

    public virtual void OnDropTragMsg(TrapData data)
    {
        if (data == null) return;
        if (!IsAnyState(typeof(RoleMove), typeof(RoleIdle), typeof(RoleJump)) && !m_IsJumpAttack) return;

        if (IsAnyState(typeof(RoleMove), typeof(RoleIdle)))
        {
            PlayAnimation(AnimName.JumpDown);
        }

        m_TrapData = data;
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_IsDropTrag = true;
        m_CurrCtrl.ExitSkill();
    }

    public virtual void SetCatch(bool value)
    {
        m_IsBeCatch = value;
        if (value)
        {
            m_CurrCtrl.ExitSkill();
        }
    }

    public virtual void SetThrow(bool value)
    {
        m_IsBeThrow = value;
        if (value)
        {
            m_CurrCtrl.ExitSkill();
        }
    }

    public virtual List<ICanBeHit> OnHitStart()
    {
        return null;
    }

    public virtual void OnHitEnd(SkillConfigData skillData,bool isHurtTarget) 
    {
        if (skillData.Type != SkillConfigData.SkillType.Skill && m_CurrCtrl != null)
        {
            m_CurrCtrl.AttackSuccess = isHurtTarget;
        }
    }

    protected virtual void OnGroundHurtMsg(HurtData data)
    {
        string hurtSound = string.IsNullOrEmpty(data.HurtSound) ? SoundName.DefaultHurt : data.HurtSound;
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, hurtSound);
        SubHealth(data.AttackValue);

        if (m_Health <= 0 && !m_IsSmoon)
        {
            ChangeState<RoleDead>();
        }

        ReferencePool.Release(data);
        m_OnGroundHurtData = null;
    }

    protected override void CheckGround()
    {
        if (m_IsDropGround && m_DropGourndTime > 0 && Time.time - m_DropGourndTime > 0.05f)
        {
            m_DropGourndTime = 0f;
            m_IsDropGround = false;
        }

        if (m_FsmMachine == null || !m_FsmMachine.IsRunning) return;
        if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic) return;

        UpdatePos2(transform.localPosition.x, Pos.y);

        if (IsFloat)
        {
            return;
        }

        OnDropEvent.Invoke();
        OnDropEvent.RemoveAllListeners();

        CheckDropTrag();

        if (!IsInGround || m_IsDropTrag) return;

        m_IsDropGround = true;
        m_DropGourndTime = Time.time;
        OnGroundEvent.Invoke();
        OnGroundEvent.RemoveAllListeners();
        m_IsJumpAttack = false;
        m_CurrCtrl.ExitSkill();

        if (IsAnyState(typeof(RoleSwoon)))
        {
            if (!IsPlayComplete()) return;
            CheckGroundHurt();
            m_Rigidbody.velocity = Vector2.zero;
            m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            if (m_Health > 0)
            {
                ChangeDefaultState();
                SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, SoundName.DefaultDrop);
            }
            else ChangeState<RoleDead>();
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
                SubHealth(m_TrapData.AttackValue);
                if (m_Health <= 0)
                {
                    GetState<RoleDead>().ReBirthPos = m_TrapData.Pos;
                    ChangeState<RoleDead>();
                }
                else
                {
                    SetPos(m_TrapData.Pos);
                    ChangeState<RoleIdle>();
                    CameraMgr.Ins.StartFollow();
                }
            }
            else
            {
                Release();
            }

            ReferencePool.Release(m_TrapData);
            m_IsDropTrag = false;
            m_TrapData = null;
        }
    }

    private void CheckGroundHurt()
    {
        if (m_OnGroundHurtData != null)
        {
            if (m_Health - m_OnGroundHurtData.AttackValue <= 0)
            {
                m_IsSmoon = false;
                OnGroundHurtMsg(m_OnGroundHurtData);
                return;
            }
            else
            {
                Timer.Register(0.1f, () => { OnGroundHurtMsg(m_OnGroundHurtData); });
            }
        }

        if(m_Health <= 0)
        {
            m_IsSmoon = false;
            ChangeState<RoleDead>();
            return;
        }

        Timer.Register(1f, ()=> 
        {
            m_IsSmoon = false;

            if (m_Health > 0)
                ChangeState<RoleAwaken>();
            else
                ChangeState<RoleDead>();
        });
    }

    protected bool m_IsSmoon = false;
    protected float m_AttackSpeed = 0.8f;
    protected float m_AttackValue = 0;
    protected float m_Defense = 0;
    protected bool m_IsJumpAttack = false;
    protected bool m_IsDropTrag = false;
    protected bool m_IsBeCatch = false;
    protected bool m_IsBeThrow = false;
    protected BaseRoleCtrl m_CurrCtrl = null;
    protected Vector2 m_JumpForce = Vector2.zero;

    private float m_DropGourndTime = 0f;
    private bool m_IsDropGround = false;
    private HurtData m_OnGroundHurtData = null;
    private TrapData m_TrapData = null;
}