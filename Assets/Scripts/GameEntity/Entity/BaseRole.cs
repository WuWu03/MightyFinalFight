using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Camera;
using GameFrameWork.Timer;
using GameFrameWork.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseRole : BaseAvatar, ICanBeHit
{
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
            return !IsAnyState(typeof(RoleSwoon), typeof(RoleDead), typeof(RoleAwaken)) && !m_EntityAttribute.IsDead() && m_IsAssetLoadComplete && !isFloat;
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
            return m_EntityAttribute.IsDead();
        }
    }

    public bool isHitSuccess
    {
        get
        {
            return m_IsHitSuccess;
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

    public SmallList<Bullet> bullets
    {
        get
        {
            return m_Bullets;
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
        SetDefaultState<RoleIdle>();

        m_AutoMoveComplete ??= new();
        m_Bullets ??= new();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_IsCatchControl = (data as BaseRoleData).isCatchControl;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_AutoMoveComplete.RemoveAllListeners();
        m_OnGroundHurtStateData?.Release();
        m_DropTrapStateData?.Release();
        m_SkillData?.Release();
        m_SkillManager?.Release();

        m_SkillData = null;
        m_SkillManager = null;
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
        m_IsPause = false;
        m_DropGourndTime = 0f;
        m_OnGroundHurtStateData = null;
        m_DropTrapStateData = null;
        m_OnHurtEvent = null;
        m_AutoMoveComplete = null;
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        m_MoveDir = Vector2.right;

        if (m_IsPause)
        {
            Pause();
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        CheckAttack();
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        m_SkillManager?.Update();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        CheckAutoMove();
    }

    public virtual void SetSkillData(BaseRoleSkillData skillData)
    {
        m_SkillData = skillData;
        m_SkillManager = new SkillManager(this, skillData.skillIds);
    }

    public void AddBullet(Bullet bullet)
    {
        m_Bullets.Add(bullet);
    }

    public void Move(Vector2 dir, bool canChangeDir = true)
    {
        if (!canMove)
        {
            return;
        }

        MoveStateData moveData = MoveStateData.Create();
        moveData.dir = dir;
        moveData.canChangeDir = canChangeDir;
        OnMoveMsg(moveData);

        ReferencePool.Release(moveData);
    }

    public virtual void OnMoveMsg(MoveStateData data)
    {
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

        ExitSkill();
        m_MoveDir = data.dir;
        SetStateData<RoleMove>(data);
        ChangeState<RoleMove>();
    }

    public virtual void AutoMove(Vector2 pos, UnityAction moveComplete = null)
    {
        m_MoveToPos = pos;
        m_IsAutoMove = true;

        ExitSkill();

        if (moveComplete != null)
        {
            m_AutoMoveComplete.AddListener(moveComplete);
        }
    }

    public void Attack(Vector2 dir)
    {
        if (!canAttack)
        {
            return;
        }

        if (IsAnyState(typeof(RoleJump)))
        {
            JumpAttack(dir);
        }
        else
        {
            NormalAttack(dir);
        }
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

    public void DeploySkill(int skillID)
    {
        if (!canSkill)
        {
            return;
        }

        if (m_SkillManager.IsCurrSkill(skillID) && !m_SkillManager.IsSkillComplete(skillID))
        {
            return;
        }

        ExitSkill();

        m_CurrSkillID = skillID;
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    public bool IsInSkill()
    {
        return m_SkillManager.IsInSkill();
    }

    public bool IsCurrSkill(int skillId)
    {
        return m_SkillManager.IsCurrSkill(skillId);
    }

    public bool IsSkillComplete(int skillId)
    {
        return m_SkillManager.IsSkillComplete(skillId);
    }

    public virtual void ExitSkill()
    {
        if (m_SkillManager == null)
        {
            return;
        }

        for (int i = 0; i < m_SkillData.attackIds.Length; i++)
        {
            if (m_CurrSkillID == m_SkillData.attackIds[i])
            {
                m_IsHitSuccess = false;
                m_IsAttack = false;
                m_AttackTimer = -1;
                m_CanAttack = true;
                m_AttackIndex = 0;
                break;
            }
        }

        m_IsHitSuccess = false;
        m_SkillManager.ExitSkill();
    }

    public virtual void SetCatch(bool value)
    {
        m_IsBeCatch = value;

        if (value)
        {
            ExitSkill();
        }
    }

    public virtual void SetThrow(bool value)
    {
        m_IsBeThrow = value;

        if (value)
        {
            ExitSkill();
        }
    }

    public virtual List<ICanBeHit> OnHitStart()
    {
        return null;
    }

    public virtual void OnHitEnd(SkillConfigData skillData, bool isHitSuccess)
    {
        SetHitSuccess(isHitSuccess);
    }

    public void SetHitSuccess(bool isHitSuccess)
    {
        m_IsHitSuccess = isHitSuccess;
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

    public void Jump(Vector2 jumpDir, bool canChangeDir, bool isForceJump = false)
    {
        if (!isForceJump && !canJump)
        {
            return;
        }

        JumpStateData jumpData = JumpStateData.Create();
        jumpData.dir = jumpDir;
        jumpData.canChangeDir = canChangeDir;
        OnJumpMsg(jumpData);
        jumpData.Release();
    }

    public virtual void OnJumpMsg(JumpStateData jumpData)
    {
        if (jumpData == null)
        {
            return;
        }

        ExitSkill();

        SetStateData<RoleJump>(jumpData);
        ChangeState<RoleJump>();
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.DefaultJump));
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
        ExitSkill();

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

        ExitSkill();

        m_DropTrapStateData = dropTrapStateData;
        m_IsDropTrag = true;
        SetBodyType(RigidbodyType2D.Dynamic);
    }

    public virtual void Resume()
    {
        if (!m_IsPause)
        {
            return;
        }

        m_IsPause = false;

        if (m_MoveToPos != Vector2.zero)
        {
            m_IsAutoMove = true;
        }

        if (m_IsAssetLoadComplete)
        {
            m_Rigidbody2D.bodyType = m_PrevBodyType;
            m_Rigidbody2D.gravityScale = m_PrevGravityScale;
            m_Rigidbody2D.linearDamping = m_PrevLinearDamping;
            m_Rigidbody2D.angularDamping = m_PrevAngularDamping;
            m_Rigidbody2D.linearVelocity = m_PrevVelocity;

            ResumeAnimation();
        }
    }

    public virtual void Pause()
    {
        if (!m_IsPause)
        {
            return;
        }

        m_IsPause = true;

        if (m_IsAssetLoadComplete)
        {
            m_IsAutoMove = false;
            m_PrevBodyType = rigidbody2D.bodyType;
            m_PrevGravityScale = m_Rigidbody2D.gravityScale;
            m_PrevLinearDamping = m_Rigidbody2D.linearDamping;
            m_PrevAngularDamping = m_Rigidbody2D.angularDamping;
            m_PrevVelocity = m_Rigidbody2D.linearVelocity;
            m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

            PauseAnimation();
        }
    }

    protected virtual void NormalAttack(Vector2 dir)
    {
        if (!m_CanAttack)
        {
            return;
        }

        if (m_IsHitSuccess)
        {
            if (m_AttackIndex < m_SkillData.attackIds.Length - 1)
            {
                m_AttackIndex++;
            }
        }
        else
        {
            m_AttackIndex = 0;
        }

        m_IsAttack = true;
        m_CanAttack = false;
        m_AttackTimer = -1;
        m_CurrSkillID = m_SkillData.attackIds[m_AttackIndex];
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    protected virtual void JumpAttack(Vector2 dir)
    {
        m_IsHitSuccess = false;

        if (dir.y < 0 && m_SkillData.jumpAttackIds.Length > 1)
        {
            m_CurrSkillID = m_SkillData.jumpAttackIds[1];
        }
        else
        {
            m_CurrSkillID = m_SkillData.jumpAttackIds[0];
        }

        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    protected virtual void OnGroundHurtMsg(HurtStateData hurtStatedata)
    {
        if (!hurtStatedata.isNotPlayHurtSound)
        {
            string hurtSound = string.IsNullOrEmpty(hurtStatedata.hurtSound) ? SoundName.DefaultHurt : hurtStatedata.hurtSound;
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, hurtSound));
        }

        m_EntityAttribute.SubHealth(hurtStatedata.attackValue);

        if (m_EntityAttribute.IsDead() && !m_IsSmoon)
        {
            ChangeState<RoleDead>();
        }

        hurtStatedata.Release();
        m_OnGroundHurtStateData = null;
    }

    protected override void CheckGround()
    {
        if (m_IsDropGround && m_DropGourndTime > 0 && Time.time - m_DropGourndTime > 0.05f)
        {
            m_DropGourndTime = 0f;
            m_IsDropGround = false;
        }

        if (m_Fsm == null || !m_Fsm.isRunning)
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
            ExitSkill();
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

            if (!m_EntityAttribute.IsDead())
            {
                ChangeDefaultState();
                AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.DefaultDrop));
            }
            else
            {
                ChangeState<RoleDead>();
            }
        }
    }

    protected virtual void CheckAttack()
    {
        if (m_IsAttack)
        {
            if (IsPlayComplete())
            {
                m_AttackTimer = Time.time;
                m_IsAttack = false;
            }
        }

        if (m_AttackTimer > 0)
        {
            if (Time.time - m_AttackTimer > (m_IsHitSuccess ? 0.05f : 0f))
            {
                if (m_AttackIndex < m_SkillData.attackIds.Length - 1)
                {
                    m_CanAttack = true;
                }
            }

            if (Time.time - m_AttackTimer > 0.2f && isInGround)
            {
                m_IsHitSuccess = false;
                m_IsAttack = false;
                m_AttackTimer = -1;
                m_CanAttack = true;
                m_AttackIndex = 0;
                ChangeDefaultState();
            }
        }
    }

    protected virtual void CheckDropTrag()
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

                if (m_EntityAttribute.IsDead())
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

            m_DropTrapStateData.Release();
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
                TimerMgr.instance.Register(0.1f, () => { OnGroundHurtMsg(m_OnGroundHurtStateData); });
            }
        }

        if (m_EntityAttribute != null)
        {
            if (m_EntityAttribute.health <= 0)
            {
                m_IsSmoon = false;
                ChangeState<RoleDead>();
                return;
            }

            TimerMgr.instance.Register(1f, () =>
            {
                m_IsSmoon = false;

                if (m_EntityAttribute.health > 0)
                    ChangeState<RoleAwaken>();
                else
                    ChangeState<RoleDead>();
            });
        }
    }

    private void OnGroundCheck()
    {
        m_IsAddGroundForce = false;
    }

    private void CheckAutoMove()
    {
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
            data.Release();
            return;
        }

        if (!m_XArrived)
        {
            float xOffset = Mathf.Abs(m_MoveToPos.x - m_Pos.x);
            m_XArrived = xOffset <= 0.02f;
            MoveStateData data = MoveStateData.Create();
            data.dir = (Vector2.right * xOffset).normalized;
            OnMoveMsg(data);
            data.Release();
            return;
        }

        SetDefaultState<RoleIdle>();
        ChangeDefaultState();

        m_AutoMoveComplete?.Invoke();
        m_AutoMoveComplete?.RemoveAllListeners();
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
    protected event GameFrameWorkAction<HurtStateData> m_OnHurtEvent = null;
    protected BaseRoleSkillData m_SkillData = null;
    protected SkillManager m_SkillManager = null;

    private int m_CurrSkillID = 0;
    private int m_AttackIndex = 0;
    private float m_AttackTimer = -1;
    private bool m_IsHitSuccess = false;
    private bool m_IsAttack = false;
    private bool m_CanAttack = true;
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
    private SmallList<Bullet> m_Bullets = null;
}