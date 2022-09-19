using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseRole : BaseAvatar, ICanBeHit
{
    public BaseRoleCtrl currCtrl
    {
        get
        {
            return m_CurrCtrl;
        }
    }

    public Vector2 moveToPos
    {
        get
        {
            return m_MoveToPos;
        }
    }

    public Vector2 moveDir
    {
        get
        {
            return m_MoveDir;
        }
    }

    public virtual bool canBeHit
    {
        get
        {
            return !IsAnyState(typeof(RoleSwoon), typeof(RoleDead), typeof(RoleAwaken)) && !m_EntityAttribute.IsDie() && m_IsResComplete && !isFloat;
        }
    }

    public bool isBeCatch
    {
        get
        {
            return m_IsBeCatch;
        }
    }

    public bool isBeThrow
    {
        get
        {
            return m_IsBeThrow;
        }
    }

    public bool isDead
    {
        get
        {
            return m_EntityAttribute.IsDie();
        }
    }

    public virtual bool canMove
    {
        get
        {
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleAttack), typeof(RoleSkill), typeof(RoleJump));
        }
    }

    public virtual bool canAttack
    {
        get
        {
            return !m_IsDropTrag && !m_IsJumpAttack && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump), typeof(RoleAttack));
        }
    }

    public virtual bool canJump
    {
        get
        {
            return !m_IsDropTrag && !m_IsBeCatch && isInGround && IsAnyState(typeof(RoleIdle), typeof(RoleMove));
        }
    }

    public virtual bool canSkill
    {
        get
        {
            return !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump), typeof(RoleAttack));
        }
    }

    public bool isDropTrag
    {
        get
        {
            return m_IsDropTrag;
        }
    }

    public bool isDropGround
    {
        get
        {
            return m_IsDropGround;
        }
    }

    public bool isAutoMove
    {
        get
        {
            return m_IsAutoMove;
        }
    }

    public virtual bool canChangeDefaultState
    {
        get
        {
            return !IsAnyState(typeof(RoleAttack)) && isInGround;
        }
    }

    public bool isHitSuccess
    {
        get
        {
            return m_CurrCtrl != null && m_CurrCtrl.isAttackSuccess;
        }
    }

    public event GameFrameWorkBooleanAction<HurtData> onHurtEvent
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
        m_IsCatchControl = (data as BaseRoleData).isCatchControl;
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
        roleAttack.canChangeDir = data.canChangeDir;
        roleAttack.dir = data.dir;
        ChangeState<RoleAttack>();
        PlayAnimation(data.animName, data.animTime, data.animSpeed * m_AttackSpeed);
    }

    public virtual void OnSkillMsg(SkillConfigData data)
    {
        if (data == null)
        {
            return;
        }
        RoleSkill skillState = GetState<RoleSkill>();
        skillState.canChangeDir = data.CanChangeDir;
        skillState.canMove = data.CanMove;
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
            GetState<RoleJump>().dir = data.dir.x;
            return;
        }

        if (IsAnyState(typeof(RoleAttack)))
        {
            GetState<RoleAttack>().dir = data.dir.x;
            return;
        }

        if (IsAnyState(typeof(RoleSkill)))
        {
            GetState<RoleSkill>().dir = data.dir;
            return;
        }

        if (data.dir == Vector2.zero)
        {
            if (data.isCatch)
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

        m_MoveDir = data.dir;
        RoleMove roleMove = GetState<RoleMove>();
        roleMove.canChangeDir = data.canChangeDir;
        roleMove.isCatch = data.isCatch;
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
        roleJump.canChangeDir = !data.isCatch && data.canChangeDir;
        roleJump.dir = data.dir.x;
        roleJump.isCatch = data.isCatch;

        ChangeState<RoleJump>();
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, SoundName.DefaultJump);
    }

    public virtual void OnHurtMsg(HurtData data)
    {
        if (data == null || !canBeHit) 
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

        if (m_EntityAttribute.health - data.attackValue <= 0 && !data.isSwoon)
        {
            data.isSwoon = true;
            data.attackForce = SkillFactory.GetSmoonForce(data.attackerDir);
        }

        m_IsSmoon = data.isSwoon;

        if (string.IsNullOrEmpty(data.hurtAnim))
        {
            data.hurtAnim = AnimName.Hurt;
        }

        if (m_IsSmoon)
        {
            GetState<RoleSwoon>().force = data.attackForce;
            ChangeState<RoleSwoon>();
        }
        else
        {
            GetState<RoleHurt>().hurtAnim = data.hurtAnim;
            ChangeState<RoleHurt>();
        }

        if (data.isGroundHurt && data.attackForce.y > 0)
        {
            m_OnGroundHurtData = data;
        }
        else if (data.attackValue > 0)
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
        string hurtSound = string.IsNullOrEmpty(data.hurtSound) ? SoundName.DefaultHurt : data.hurtSound;
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, hurtSound);
        m_EntityAttribute.SubHealth(data.attackValue);

        if (m_EntityAttribute.IsDie() && !m_IsSmoon)
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

        if (m_FsmMachine == null || !m_FsmMachine.isRunning)
        {
            return;
        }

        if (m_Rigidbody2D.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        UpdatePosX(transform.localPosition.x);

        if (isFloat)
        {
            return;
        }

        onDropEvent.Invoke();
        onDropEvent.RemoveAllListeners();

        CheckDropTrag();

        if (!isInGround || m_IsDropTrag)
        {
            return;
        }

 
        m_IsDropGround = true;
        m_DropGourndTime = Time.time;
        onGroundEvent.Invoke();
        onGroundEvent.RemoveAllListeners();
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
                onGroundEvent.AddListener(OnGroundCheck);
                return;
            }

            if (!m_EntityAttribute.IsDie())
            {
                ChangeDefaultState();
                SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, SoundName.DefaultDrop);
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

        Rect visionRect = CameraMgr.instance.GetVision();

        if ((transform.localPosition + Vector3.up * 0.6f).y + 0.1f < visionRect.yMin)
        {
            if (m_ObjectType == ObjectType.Player)
            {
                m_EntityAttribute.SubHealth(m_TrapData.attackValue);
                if (m_EntityAttribute.IsDie())
                {
                    GetState<RoleDead>().rebirthPos = m_TrapData.rebirthPos;
                    ChangeState<RoleDead>();
                }
                else
                {
                    SetPos2(m_TrapData.rebirthPos);
                    ChangeState<RoleIdle>();
                    CameraMgr.instance.StartFollow();
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
            if (m_EntityAttribute.health - m_OnGroundHurtData.attackValue <= 0)
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

        if(m_EntityAttribute.health <= 0)
        {
            m_IsSmoon = false;
            ChangeState<RoleDead>();
            return;
        }

        Timer.Register(1f, ()=> 
        {
            m_IsSmoon = false;

            if (m_EntityAttribute.health > 0)
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
        if (!m_IsResComplete || !isInGround || !m_IsAutoMove)
        {
            return;
        }

        if (!m_YArrived)
        {
            float yOffest = m_MoveToPos.y - m_Pos.y;
            m_YArrived = Mathf.Abs(yOffest)*1000f <= 10f;
            MoveData data = MoveData.Create();
            data.dir = (Vector2.up * yOffest).normalized;
            OnMoveMsg(data);
            ReferencePool.Release(data);
            return;
        }

        if (!m_XArrived)
        {
            float xOffest = m_MoveToPos.x - m_Pos.x;
            m_XArrived = Mathf.Abs(xOffest) <= 0.05f;
            MoveData data = MoveData.Create();
            data.dir = (Vector2.right * xOffest).normalized;
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