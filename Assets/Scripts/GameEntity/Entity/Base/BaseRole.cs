using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Camera;
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
            return !IsAnyState(typeof(RoleSwoon), typeof(RoleDead), typeof(RoleAwaken)) && !m_EntityAttribute.IsDie() && m_IsAssetLoadComplete && !isFloat;
        }
    }

    public bool isBeCatch
    {
        get
        {
            return m_IsBeCatch;
        }
    }

    public virtual bool isCatching
    {
        get
        {
            return false;
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
            return !m_IsAutoMove && !m_IsDropTrag && !m_IsBeCatch && IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleAttack), typeof(RoleSkill), typeof(RoleJump));
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

    public bool isPause
    {
        get
        {
            return m_IsPause;
        }
    }

    public virtual bool canChangeDefaultState
    {
        get
        {
            return !IsAnyState(typeof(RoleAttack)) && isInGround;
        }
    }

    public event GameFrameWorkAction<HurtStateData> onHurtEvent
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

        if (m_AutoMoveComplete == null)
        {
            m_AutoMoveComplete = new UnityEvent();
        }
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


        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.Release();
        }

        if(m_OnGroundHurtStateData != null)
        {
            ReferencePool.ReleaseReference(m_OnGroundHurtStateData);
        }

        if(m_DropTrapStateData != null)
        {
            ReferencePool.ReleaseReference(m_DropTrapStateData);
        }

        m_IsSmoon = false;
        m_IsJumpAttack = false;
        m_IsDropTrag = false;
        m_IsBeCatch = false;
        m_IsBeThrow = false;
        m_IsCatchControl = false;
        m_IsAutoMove = false;
        m_XArrived = false;
        m_YArrived = false;
        m_IsDropGround = false;
        m_DropGourndTime = 0f;
        m_OnGroundHurtStateData = null;
        m_DropTrapStateData = null;
        m_CurrCtrl = null;
        m_OnHurtEvent = null;

        base.Release();
    }

    protected override void OnLoadAssetComplete(GameObject go, object[] param)
    {
        base.OnLoadAssetComplete(go, param);
        m_MoveDir = Vector2.right;
        m_FSM.Start<RoleIdle>();

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.Start();
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.Update();
        }
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.LateUpdate();
        }
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.FixedUpdate();
        }

        CheckAutoMove();
    }

    protected override void OnBeforeDestroy()
    {
        m_AutoMoveComplete = null;
        base.OnBeforeDestroy();
    }

    public virtual void OnAttackMsg(AttackStateData data, bool forceJumpAttack = false)
    {
        if (data == null)
        {
            return;
        }

        m_IsJumpAttack = IsAnyState(typeof(RoleJump)) || forceJumpAttack;
        ChangeState<RoleAttack>(data);
        PlayAnimation(data.animName, data.animTime, data.animSpeed * (1 + m_EntityAttribute.attackSpeed));
    }

    public virtual void OnSkillMsg(SkillStateData data)
    {
        if (data == null)
        {
            return;
        }

        ChangeState<RoleSkill>(data);
        PlayAnimation(data.animName, data.animTime, data.animSpeed * (1 + m_EntityAttribute.attackSpeed));
    }

    public virtual void OnMoveMsg(MoveStateData data)
    {
        if (this.gameObject.name == "TwoP")
        {

        }

        if (data == null)
        {
            return;
        }

        if (IsAnyState(typeof(RoleJump)))
        {
            SetStateData<RoleJump>(data);
            return;
        }

        if (IsAnyState(typeof(RoleAttack)))
        {
            SetStateData<RoleAttack>(data);
            return;
        }

        if (IsAnyState(typeof(RoleSkill)))
        {
            SetStateData<RoleSkill>(data);
            return;
        }

        if (data.dir == Vector2.zero)
        {
            if (data.isCatch)
            {
                if (this is BaseHero)
                {
                    ChangeState<HeroCatch>();
                }
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
        SetStateData<RoleMove>(data);
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

    public virtual void OnJumpMsg(JumpStateData jumpData)
    {
        if (jumpData == null)
        {
            return;
        }

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        SetStateData<RoleJump>(jumpData);
        ChangeState<RoleJump>();
        AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, SoundName.DefaultJump);
    }

    public virtual bool IsHurtWillDie(int attackValue)
    {
        return m_EntityAttribute.health - attackValue <= 0;
    }

    public virtual void OnHurtMsg(HurtStateData hurtStateData)
    {
        if (hurtStateData == null || !canBeHit)
        {
            return;
        }

        m_OnHurtEvent?.Invoke(hurtStateData);

        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.ExitSkill();
        }

        if (m_EntityAttribute.health - hurtStateData.attackValue <= 0 && !hurtStateData.isSwoon)
        {
            hurtStateData.isSwoon = true;
            hurtStateData.attackForce = SkillUtil.GetSmoonForce(hurtStateData.attackerDir);
        }

        m_IsSmoon = hurtStateData.isSwoon;

        if (string.IsNullOrEmpty(hurtStateData.hurtAnim))
        {
            hurtStateData.hurtAnim = AnimName.Hurt;
        }

        if (!hurtStateData.isDefense)
        {
            if (m_IsSmoon)
            {
                SetStateData<RoleSwoon>(hurtStateData);
                ChangeState<RoleSwoon>();
            }
            else
            {
                SetStateData<RoleHurt>(hurtStateData);
                ChangeState<RoleHurt>();
            }
        }

        if (hurtStateData.isGroundHurt && hurtStateData.attackForce.y > 0)
        {
            m_OnGroundHurtStateData = hurtStateData;
        }
        else
        {
            OnGroundHurtMsg(hurtStateData);
        }
    }

    public virtual void OnDefenseMsg(float attackerDir)
    {
        SetDir(-attackerDir);
        SetPosX(m_Pos.x + attackerDir * 0.04f);
        ChangeState<RoleDefense>();
    }

    public virtual void OnDropTragMsg(DropTrapStateData dropTrapStateData)
    {
        if (dropTrapStateData == null)
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

        m_DropTrapStateData = dropTrapStateData;
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
        if (m_CurrCtrl != null)
        {
            m_CurrCtrl.SetHitState(isHurtTarget);
        }
    }

    public virtual void Resume()
    {
        if (m_MoveToPos != Vector2.zero)
        {
            m_IsAutoMove = true;
        }

        m_IsPause = false;
        m_Rigidbody2D.bodyType = m_PrevBodyType;
        m_Rigidbody2D.gravityScale = m_PrevGravityScale;
        m_Rigidbody2D.linearDamping = m_PrevLinearDamping;
        m_Rigidbody2D.angularDamping = m_PrevAngularDamping;
        m_Rigidbody2D.linearVelocity = m_PrevVelocity;

        ResumeAnimation();
    }

    public virtual void Pause()
    {
        m_IsPause = true;
        m_IsAutoMove = false;
        m_PrevBodyType = rigidbody2D.bodyType;
        m_PrevGravityScale = m_Rigidbody2D.gravityScale;
        m_PrevLinearDamping =  m_Rigidbody2D.linearDamping;
        m_PrevAngularDamping = m_Rigidbody2D.angularDamping;
        m_PrevVelocity = m_Rigidbody2D.linearVelocity;
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

        PauseAnimation();
    }

    protected virtual void OnGroundHurtMsg(HurtStateData hurtStatedata)
    {
        if (!hurtStatedata.isNotPlayHurtSound)
        {
            string hurtSound = string.IsNullOrEmpty(hurtStatedata.hurtSound) ? SoundName.DefaultHurt : hurtStatedata.hurtSound;
            AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, hurtSound);
        }

        m_EntityAttribute.SubHealth(hurtStatedata.attackValue);

        if (m_EntityAttribute.IsDie() && !m_IsSmoon)
        {
            ChangeState<RoleDead>();
        }

        ReferencePool.ReleaseReference(hurtStatedata);
        m_OnGroundHurtStateData = null;
    }

    protected override void CheckGround()
    {
        if (m_IsDropGround && m_DropGourndTime > 0 && Time.time - m_DropGourndTime > 0.05f)
        {
            m_DropGourndTime = 0f;
            m_IsDropGround = false;
        }

        if (m_FSM == null || !m_FSM.isRunning)
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

        m_Rigidbody2D.linearDamping = 0;
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
                AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, SoundName.DefaultDrop);
            }
            else
            {
                ChangeState<RoleDead>();
            }
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
                m_EntityAttribute.SubHealth(m_DropTrapStateData.attackValue);

                if (m_EntityAttribute.IsDie())
                {
                    SetStateData<RoleDead>(m_DropTrapStateData);
                    ChangeState<RoleDead>();
                }
                else
                {
                    SetPos2(m_DropTrapStateData.rebirthPos);
                    ChangeState<RoleIdle>();
                    CameraMgr.instance.StartFollow();
                }
            }
            else
            {
                Release();
            }

            ReferencePool.ReleaseReference(m_DropTrapStateData);
            m_IsDropTrag = false;
            m_DropTrapStateData = null;
        }
    }

    private void CheckGroundHurt()
    {
        if (m_OnGroundHurtStateData != null)
        {
            if (m_EntityAttribute.health - m_OnGroundHurtStateData.attackValue <= 0)
            {
                m_IsSmoon = false;
                OnGroundHurtMsg(m_OnGroundHurtStateData);
                return;
            }
            else
            {
                Timer.Register(0.1f, () => { OnGroundHurtMsg(m_OnGroundHurtStateData); });
            }
        }

        if (m_EntityAttribute.health <= 0)
        {
            m_IsSmoon = false;
            ChangeState<RoleDead>();
            return;
        }

        Timer.Register(1f, () =>
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
        if(this.gameObject.name == "TwoP")
        {

        }

        if (!m_IsAssetLoadComplete || !isInGround || !m_IsAutoMove)
        {
            return;
        }

        if (!m_YArrived)
        {
            float yOffset = Mathf.Abs(m_MoveToPos.y - m_Pos.y);
            m_YArrived = yOffset <= 0.02f;
            MoveStateData data = MoveStateData.Create();
            data.dir = (Vector2.up * yOffset).normalized;
            OnMoveMsg(data);
            ReferencePool.ReleaseReference(data);
            return;
        }

        if (!m_XArrived)
        {
            float xOffset = Mathf.Abs(m_MoveToPos.x - m_Pos.x);
            m_XArrived = xOffset <= 0.02f;
            MoveStateData data = MoveStateData.Create();
            data.dir = (Vector2.right * xOffset).normalized;
            OnMoveMsg(data);
            ReferencePool.ReleaseReference(data);
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

    protected Vector2 m_MoveToPos = Vector2.zero;
    protected Vector2 m_MoveDir = Vector2.zero;

    protected bool m_IsSmoon = false;
    protected bool m_IsJumpAttack = false;
    protected bool m_IsDropTrag = false;
    protected bool m_IsBeCatch = false;
    protected bool m_IsBeThrow = false;
    protected bool m_IsCatchControl = false;
    protected bool m_IsAutoMove = false;
    protected BaseRoleCtrl m_CurrCtrl = null;
    protected event GameFrameWorkAction<HurtStateData> m_OnHurtEvent = null;

    private bool m_XArrived = false;
    private bool m_YArrived = false;
    private bool m_IsDropGround = false;
    private bool m_IsPause = false;
    private float m_DropGourndTime = 0f;

    private float m_PrevGravityScale = 0f;
    private float m_PrevLinearDamping = 0f;
    private float m_PrevAngularDamping = 0f;
    private Vector2 m_PrevVelocity = Vector2.zero;
    private RigidbodyType2D m_PrevBodyType = RigidbodyType2D.Static;
    private HurtStateData m_OnGroundHurtStateData = null;
    private DropTrapStateData m_DropTrapStateData = null;
    private UnityEvent m_AutoMoveComplete = null;
}