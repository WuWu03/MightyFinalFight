using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseRole : BaseAvatar, ICanBeHit
{
    public int AttackValue
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

    public int DefenseValue
    {
        get
        {
            return m_DefenseValue;
        }
        set
        {
            m_DefenseValue = value;
        }
    }

    public int CriticalValue
    {
        get
        {
            return m_CriticalValue;
        }
        set
        {
            m_CriticalValue = value;
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
            m_AttackSpeed = value;
        }
    }

    public float MoveSpeed
    {
        get
        {
            return m_MoveSpeed;
        }
        set
        {
            m_MoveSpeed = value;
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

    public Vector2 MoveToPos
    {
        get
        {
            return m_MoveToPos;
        }
    }

    public Vector2 MoveDir
    {
        get
        {
            return m_MoveDir;
        }
    }

    public BaseRoleCtrl CurrCtrl
    {
        get
        {
            return m_CurrCtrl;
        }
    }

    public virtual bool CanBeHit
    {
        get
        {
            return !IsAnyState(typeof(RoleSwoon), typeof(RoleDead), typeof(RoleAwaken)) && m_Health > 0 && m_IsResComplete && !IsFloat;
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
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleAttack), typeof(RoleSkill), typeof(RoleJump));
        }
    }

    public virtual bool CanAttack
    {
        get
        {
            return !m_IsDropTrag && !m_IsJumpAttack && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump), typeof(RoleAttack));
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
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump), typeof(RoleAttack));
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

    public bool IsAutoMove
    {
        get
        {
            return m_IsAutoMove;
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

    public event GameFrameWorkBooleanAction<HurtData> OnHurtEvent
    {
        add
        {
            m_OnHurtEvent += value;
        }
        remove
        {
            m_OnHurtEvent -= value;
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
        AddState<RoleDefense>();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        BaseRoleData baseRoleData = data as BaseRoleData;
        m_AttackValue = baseRoleData.AttackValue;
        m_DefenseValue = baseRoleData.DefenseValue;
        m_CriticalValue = baseRoleData.CriticalValue;
        m_AttackSpeed = baseRoleData.AttackSpeed;   
        m_JumpForce = baseRoleData.JumpForce;
        m_MoveSpeed = baseRoleData.MoveSpeed;
        m_IsCatchControl = baseRoleData.CatchControl;
    }

    public T AddCtrl<T>() where T : BaseRoleCtrl, new()
    {
        if (m_CurrCtrl == null)
        {
            m_CurrCtrl = new T();
            m_CurrCtrl.SetOwner(this);
        }

        return m_CurrCtrl as T;
    }

    public override void Release()
    {
        m_AutoMoveComplete.RemoveAllListeners();
        m_OnHurtEvent = null;

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.Release();
            m_CurrCtrl = null;
        }

        base.Release();
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_MoveDir = Vector2.right;
        m_FsmMachine.Start<RoleIdle>();

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.Start();
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurrCtrl != null)
            m_CurrCtrl.Update();

        CheckAutoMove();
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();

        if (m_CurrCtrl != null)
            m_CurrCtrl.LateUpdate();
    }

    public virtual void OnAttackMsg(AttackData data, bool forceJumpAttack = false)
    {
        if (data == null)
        {
            return;
        }
        m_IsJumpAttack = IsAnyState(typeof(RoleJump)) || forceJumpAttack;
        RoleAttack roleAttack = GetState<RoleAttack>();
        roleAttack.CanChangeDir = data.CanChangeDir;
        roleAttack.Dir = data.Dir;
        ChangeState<RoleAttack>();
        PlayAnimation(data.AnimName, data.AnimTime, data.AnimSpeed * m_AttackSpeed);
    }

    public virtual void OnSkillMsg(SkillConfigData data)
    {
        if (data == null)
        {
            return;
        }
        RoleSkill skillState = GetState<RoleSkill>();
        skillState.CanChangeDir = data.CanChangeDir;
        skillState.CanMove = data.CanMove;
        ChangeState<RoleSkill>();
        PlayAnimation(data.AnimationName, data.AnimTime, data.AnimSpeed);
    }

    public virtual void OnMoveMsg(MoveData data)
    {
        if (data == null)
        {
            return;
        } 

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
            GetState<RoleSkill>().Dir = data.Dir;
            return;
        }

        if (data.Dir == Vector2.zero)
        {
            if (data.IsCatch)
            {
                if (this is BaseHero)
                    ChangeState<HeroCatch>();
            }
            else
            {
                ChangeState<RoleIdle>();
            }
            return;
        }

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        m_MoveDir = data.Dir;
        RoleMove roleMove = GetState<RoleMove>();
        roleMove.CanChangeDir = data.CanChangeDir;
        roleMove.IsCatch = data.IsCatch;
        ChangeState<RoleMove>();
    }

    public virtual void AutoMoveToPos(Vector2 pos, UnityAction moveComplete = null)
    {
        m_MoveToPos = pos;
        m_IsAutoMove = true;

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        if (moveComplete != null)
        {
            m_AutoMoveComplete.AddListener(moveComplete);
        }
    }

    public virtual void OnJumpMsg(JumpData data)
    {
        if (data == null)
        {
            return;
        }

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        RoleJump roleJump = GetState<RoleJump>();
        roleJump.CanChangeDir = !data.IsCatch && data.CanChangeDir;
        roleJump.Dir = data.Dir.x;
        roleJump.IsCatch = data.IsCatch;

        ChangeState<RoleJump>();
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, SoundName.DefaultJump);
    }

    public virtual void OnHurtMsg(HurtData data)
    {
        if (data == null || !CanBeHit) 
        {
            return;
        }

        if (m_OnHurtEvent != null && !m_OnHurtEvent.Invoke(data))
        {
            return;
        }

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        if (m_Health - data.AttackValue <= 0 && !data.IsSwoon)
        {
            data.IsSwoon = true;
            data.AttackForce = SkillFactory.GetSmoonForce(data.AttackerDir);
        }

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
        {
            m_OnGroundHurtData = data;
        }
        else if (data.AttackValue > 0)
        {
            OnGroundHurtMsg(data);
        }
    }

    public virtual void OnDefenseMsg(float attackerDir)
    {
        SetDir(-attackerDir);
        SetPosX(m_Pos.x + attackerDir * 0.07f);
        ChangeState<RoleDefense>(true);
    }

    public virtual void OnDropTragMsg(DropTrapData data)
    {
        if (data == null)
        {
            return;
        }

        if (!IsAnyState(typeof(RoleMove), typeof(RoleIdle), typeof(RoleJump)) && !m_IsJumpAttack)
        {
            return;
        }

        if (IsAnyState(typeof(RoleMove), typeof(RoleIdle)))
        {
            PlayAnimation(AnimName.JumpDown);
        }

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        m_TrapData = data;
        m_IsDropTrag = true;
        SetBodyType(RigidbodyType2D.Dynamic);
    }

    public virtual void SetCatch(bool value)
    {
        m_IsBeCatch = value;
        if (value && m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }
    }

    public virtual void SetThrow(bool value)
    {
        m_IsBeThrow = value;

        if (value && m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }
    }

    public virtual List<ICanBeHit> OnHitStart()
    {
        return null;
    }

    public virtual void OnHitEnd(SkillConfigData skillData, bool isHurtTarget)
    {
        if (skillData.Type != SkillConfigData.SkillType.Skill && m_CurrCtrl != null)
        {
            m_CurrCtrl.OnAttackSuccess(isHurtTarget);
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

        if (m_FsmMachine == null || !m_FsmMachine.IsRunning)
        {
            return;
        }

        if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        UpdatePosX(transform.localPosition.x);

        if (IsFloat)
        {
            return;
        }

        OnDropEvent.Invoke();
        OnDropEvent.RemoveAllListeners();

        CheckDropTrag();

        if (!IsInGround || m_IsDropTrag)
        {
            return;
        }

 
        m_IsDropGround = true;
        m_DropGourndTime = Time.time;
        OnGroundEvent.Invoke();
        OnGroundEvent.RemoveAllListeners();
        OnGround();
        m_IsJumpAttack = false;

        if (!m_IsAddGroundForce)
        {
            if (m_CurrCtrl != null)
            {
                m_CurrCtrl.ExitSkill();
            }
        }

        if (IsAnyState(typeof(RoleSwoon)))
        {
            if (!IsPlayComplete())
            {
                return;
            }
            CheckGroundHurt();
            ResetRigidbody();
        }
        else
        {
            if (m_IsAddGroundForce)
            {
                OnGroundEvent.AddListener(OnGroundCheck);
                return;
            }

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
        if (!m_IsDropTrag)
        {
            return;
        }

        Rect visionRect = CameraMgr.Ins.GetVision();

        if ((transform.localPosition + Vector3.up * 0.6f).y + 0.1f < visionRect.yMin)
        {
            if (m_ObjectType == ObjectType.Player)
            {
                SubHealth(m_TrapData.AttackValue);
                if (m_Health <= 0)
                {
                    GetState<RoleDead>().ReBirthPos = m_TrapData.RebirthPos;
                    ChangeState<RoleDead>();
                }
                else
                {
                    SetPos(m_TrapData.RebirthPos);
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

    private void OnGroundCheck()
    {
        m_IsAddGroundForce = false;
    }

    private void CheckAutoMove()
    {
        if (!m_IsResComplete || !IsInGround || !m_IsAutoMove)
        {
            return;
        }

        if (!m_YArrived)
        {
            float yOffest = m_MoveToPos.y - m_Pos.y;
            m_YArrived = Mathf.Abs(yOffest) <= 0.02f;
            MoveData data = MoveData.Create();
            data.Dir = (Vector2.up * yOffest).normalized;
            OnMoveMsg(data);
            ReferencePool.Release(data);
            return;
        }

        if (!m_XArrived)
        {
            float xOffest = m_MoveToPos.x - m_Pos.x;
            m_XArrived = Mathf.Abs(xOffest) <= 0.02f;
            MoveData data = MoveData.Create();
            data.Dir = (Vector2.right * xOffest).normalized;
            OnMoveMsg(data);
            ReferencePool.Release(data);
            return;
        }

        SetDefaultState<RoleIdle>();
        ChangeDefaultState();
        m_AutoMoveComplete.Invoke();
        m_AutoMoveComplete.RemoveAllListeners();
        m_IsAutoMove = false;
        m_XArrived = false;
        m_YArrived = false;
        m_MoveToPos = Vector2.zero;
    }

    protected int m_AttackValue = 0;
    protected int m_DefenseValue = 0;
    protected int m_CriticalValue = 0;
    protected float m_AttackSpeed = 0.8f;
    protected float m_MoveSpeed = 0.8f;
    protected Vector2 m_JumpForce = Vector2.zero;
    protected Vector2 m_MoveToPos = Vector2.zero;
    protected Vector2 m_MoveDir = Vector2.zero;

    protected bool m_IsSmoon = false;
    protected bool m_IsJumpAttack = false;
    protected bool m_IsDropTrag = false;
    protected bool m_IsBeCatch = false;
    protected bool m_IsBeThrow = false;
    protected bool m_IsCatchControl = false;
    protected BaseRoleCtrl m_CurrCtrl = null;
    protected event GameFrameWorkBooleanAction<HurtData> m_OnHurtEvent = null;

    private bool m_IsAutoMove = false;
    private bool m_XArrived = false;
    private bool m_YArrived = false;
    private bool m_IsDropGround = false;
    private float m_DropGourndTime = 0f;
    private HurtData m_OnGroundHurtData = null;
    private DropTrapData m_TrapData = null;
    private UnityEvent m_AutoMoveComplete = new UnityEvent();
}