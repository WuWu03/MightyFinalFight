using WuWuFramework;
using WuWuFramework.Event;
using WuWuFramework.Utils;
using System.Collections.Generic;
using UnityEngine;

public class BaseRole : BaseAvatar, ICanBeHit
{
    private event WuWuFrameworkAction<HurtStateArg> m_OnHurtEvent;
    private event WuWuFrameworkAction m_AutoMoveComplete;
    private RoleStateParam m_RoleStateParam;

    private bool m_IsAttack;
    private bool m_IsJumpAttack;
    private bool m_IsDropTrap;
    private bool m_IsBeCatch;
    private bool m_IsBeThrow;
    private bool m_IsCatchControl;
    private bool m_IsAutoMove;
    private int m_CurrSkillID;
    private int m_AttackIndex;
    private float m_AttackTimer = -1;
    private float m_AwakenTimer = -1f;
    private bool m_IsHitSuccess;
    private bool m_CanCombo = true;
    private bool m_XArrived;
    private bool m_YArrived;
    private bool m_IsDropGround;
    private bool m_IsPause;
    private float m_DropGroundTime;
    private float m_PrevGravityScale;
    private float m_PrevLinearDamping;
    private float m_PrevAngularDamping;
    private Vector2 m_PrevVelocity = Vector2.zero;
    private Vector2 m_MoveToPos = Vector2.zero;
    private Vector2 m_MoveDir = Vector2.zero;
    private RigidbodyType2D m_PrevBodyType = RigidbodyType2D.Static;
    private SkillMgr m_SkillMgr;
    private HurtStateArg m_OnGroundHurtStateArg;
    private DropTrapStateArg m_DropTrapStateArg;
    private BaseRoleSkillData m_SkillData;
    private List<Bullet> m_Bullets;

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

    public bool isAttack
    {
        get
        {
            return m_IsAttack;
        }
    }

    public bool isJumpAttack
    {
        get
        {
            return m_IsJumpAttack;
        }
    }

    public bool isBeCatch
    {
        get
        {
            return m_IsBeCatch;
        }
    }

    public bool isCatchControl
    {
        get
        {
            return m_IsCatchControl;
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

    public bool isSwoon
    {
        get
        {
            return IsAnyState(typeof(RoleSwoon));
        }
    }

    public bool isDead
    {
        get
        {
            return entityAttribute.IsDead();
        }
    }

    public bool isDropTrap
    {
        get
        {
            return m_IsDropTrap;
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

    public bool isHitSuccess
    {
        get
        {
            return m_IsHitSuccess;
        }
    }

    public virtual bool canBeHit
    {
        get
        {
            return !entityAttribute.IsDead() && isAssetLoadComplete && m_RoleStateParam.canBeHit;
        }
    }

    public virtual bool canMove
    {
        get
        {
            return !m_IsAutoMove && !m_IsDropTrap && !m_IsBeCatch && m_RoleStateParam.canMove;
        }
    }

    public virtual bool canAttack
    {
        get
        {
            return !m_IsDropTrap && !m_IsJumpAttack && !m_IsBeCatch && m_RoleStateParam.canAttack;
        }
    }

    public virtual bool canJump
    {
        get
        {
            return !m_IsDropTrap && !m_IsBeCatch && isInGround && m_RoleStateParam.canJump;
        }
    }

    public virtual bool canSkill
    {
        get
        {
            return !m_IsDropTrap && !m_IsBeCatch && m_RoleStateParam.canSkill;
        }
    }

    public bool canBeCatch
    {
        get
        {
            return m_RoleStateParam.canBeCatch;
        }
    }

    public virtual bool canChangeDefaultState
    {
        get
        {
            return isInGround && !m_IsAttack && !isAddGroundForce;
        }
    }


    public event WuWuFrameworkAction<HurtStateArg> onHurtEvent
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

    public List<Bullet> bullets
    {
        get
        {
            return m_Bullets;
        }
    }

    public SkillMgr skillMgr
    {
        get
        {
            return m_SkillMgr;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();

        AddState<RoleIdle>();
        AddState<RoleMove>();
        AddState<RoleJump>();
        AddState<RoleHurt>();
        AddState<RoleSwoon>();
        AddState<RoleDead>();
        AddState<RoleAwaken>();
        AddState<RoleSkill>();
        AddState<RoleDefense>();
        SetDefaultState<RoleIdle>();
        m_Bullets ??= new();
        m_RoleStateParam ??= GetRoleStateParam();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);

        if (data is BaseRoleData baseRoleData)
        {
            m_IsCatchControl = baseRoleData.isCatchControl;
        }
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_AutoMoveComplete = null;
        m_OnGroundHurtStateArg?.Release();
        m_DropTrapStateArg?.Release();
        m_SkillData?.Release();
        m_SkillMgr?.Release();
        m_RoleStateParam?.Release();
        m_SkillData = null;
        m_SkillMgr = null;
        m_IsJumpAttack = false;
        m_IsDropTrap = false;
        m_IsBeCatch = false;
        m_IsBeThrow = false;
        m_IsCatchControl = false;
        m_IsAutoMove = false;
        m_XArrived = false;
        m_YArrived = false;
        m_IsDropGround = false;
        m_IsPause = false;
        m_DropGroundTime = 0f;
        m_OnGroundHurtStateArg = null;
        m_DropTrapStateArg = null;
        m_RoleStateParam = null;
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
        CheckAwaken();
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        m_SkillMgr?.Update();

        if (isAssetLoadComplete && m_IsPause && rigidbody2D.bodyType != RigidbodyType2D.Static)
        {
            m_IsAutoMove = false;
            m_PrevBodyType = rigidbody2D.bodyType;
            m_PrevGravityScale = rigidbody2D.gravityScale;
            m_PrevLinearDamping = rigidbody2D.linearDamping;
            m_PrevAngularDamping = rigidbody2D.angularDamping;
            m_PrevVelocity = rigidbody2D.linearVelocity;
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            PauseAnimation();
        }
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        CheckAutoMove();
    }

    public virtual void SetSkillData(BaseRoleSkillData skillData)
    {
        m_SkillData = skillData;
        m_SkillMgr = new SkillMgr(this, skillData.skillIds);
    }

    public void Move(Vector2 dir, bool canChangeDir = true)
    {
        if (!canMove)
        {
            return;
        }

        MoveStateArg moveArg = MoveStateArg.Create();
        moveArg.dir = dir;
        moveArg.canChangeDir = canChangeDir;
        MoveState(moveArg);
        ReferencePool.Release(moveArg);
    }

    public virtual void AutoMove(Vector2 pos, WuWuFrameworkAction moveComplete = null)
    {
        m_MoveToPos = pos;
        m_IsAutoMove = true;

        ExitSkill();

        if (moveComplete != null)
        {
            m_AutoMoveComplete += moveComplete;
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

    public void Jump(Vector2 jumpDir, bool canChangeDir, bool isForceJump = false)
    {
        if (!isForceJump && !canJump)
        {
            return;
        }

        JumpStateArg jumpArg = JumpStateArg.Create();
        jumpArg.dir = jumpDir;
        jumpArg.canChangeDir = canChangeDir;
        JumpState(jumpArg);
        jumpArg.Release();
    }

    public void DeploySkill(int skillID)
    {
        if (!canSkill)
        {
            return;
        }

        if (m_SkillMgr.IsCurrSkill(skillID) && !m_SkillMgr.IsSkillComplete(skillID))
        {
            return;
        }

        ExitSkill();

        m_CurrSkillID = skillID;
        m_SkillMgr.DeploySkill(m_CurrSkillID);
    }

    public virtual void ExitSkill()
    {
        if (m_SkillMgr == null)
        {
            return;
        }

        foreach (var attackId in m_SkillData.attackIds)
        {
            if (m_CurrSkillID == attackId)
            {
                m_IsHitSuccess = false;
                m_IsAttack = false;
                m_AttackTimer = -1;
                m_CanCombo = true;
                m_AttackIndex = 0;
                break;
            }
        }

        m_IsHitSuccess = false;
        m_SkillMgr.ExitSkill();
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

        if (isAssetLoadComplete)
        {
            if (!isInGround)
            {
                m_PrevBodyType = RigidbodyType2D.Dynamic;
            }

            rigidbody2D.bodyType = m_PrevBodyType;
            rigidbody2D.gravityScale = m_PrevGravityScale;
            rigidbody2D.linearDamping = m_PrevLinearDamping;
            rigidbody2D.angularDamping = m_PrevAngularDamping;
            rigidbody2D.linearVelocity = m_PrevVelocity;
            ResumeAnimation();
        }
    }

    public virtual void Pause()
    {
        if (m_IsPause)
        {
            return;
        }

        m_IsPause = true;
    }

    public void SetHitSuccess(bool isHitSuccess)
    {
        m_IsHitSuccess = isHitSuccess;
    }

    public virtual void SetStateParam(RoleStateParam roleStateParam)
    {
        roleStateParam.CopyTo(this.m_RoleStateParam);
    }

    public virtual void SetIsBeCatch(bool isBeCatch)
    {
        m_IsBeCatch = isBeCatch;

        if (isBeCatch)
        {
            ExitSkill();
        }
    }

    public virtual void SetIsBeThrow(bool isBeThrow)
    {
        m_IsBeThrow = isBeThrow;

        if (isBeThrow)
        {
            ExitSkill();
        }
    }

    public virtual bool IsHurtWillDie(int attackValue)
    {
        return entityAttribute.health - attackValue <= 0;
    }

    public virtual List<ICanBeHit> OnHitStart()
    {
        return null;
    }

    public virtual void OnHitEnd(SkillConfigData skillData, bool isHitSuccess)
    {
        SetHitSuccess(isHitSuccess);
    }

    public virtual void AttackState(SkillStateArg skillStateArg, bool forceJumpAttack = false)
    {
        if (skillStateArg == null)
        {
            return;
        }

        m_IsJumpAttack = IsAnyState(typeof(RoleJump)) || forceJumpAttack;
        ChangeState<RoleSkill>(skillStateArg);
        PlayAnimation(skillStateArg.animName, skillStateArg.animTime, skillStateArg.animSpeed * (1 + entityAttribute.attackSpeed));
    }

    public virtual void SkillState(SkillStateArg arg)
    {
        if (arg == null)
        {
            return;
        }

        ChangeState<RoleSkill>(arg);
        PlayAnimation(arg.animName, arg.animTime, arg.animSpeed * (1 + entityAttribute.attackSpeed));
    }

    public virtual void MoveState(MoveStateArg arg)
    {
        if (arg == null)
        {
            return;
        }

        if (IsAnyState(typeof(RoleJump)))
        {
            SetStateData<RoleJump>(arg);
            return;
        }

        if (IsAnyState(typeof(RoleSkill)))
        {
            SetStateData<RoleSkill>(arg);
            return;
        }

        if (arg.dir == Vector2.zero)
        {
            ChangeDefaultState();
            return;
        }

        ExitSkill();
        m_MoveDir = arg.dir;
        SetStateData<RoleMove>(arg);
        ChangeState<RoleMove>();
    }

    public virtual void JumpState(JumpStateArg jumpArg)
    {
        if (jumpArg == null)
        {
            return;
        }

        ExitSkill();
        SetStateData<RoleJump>(jumpArg);
        ChangeState<RoleJump>();
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.DefaultJump));
    }

    public virtual void HurtState(HurtStateArg hurtStateArg)
    {
        if (hurtStateArg == null || !canBeHit)
        {
            return;
        }

        m_OnHurtEvent?.Invoke(hurtStateArg);
        ExitSkill();
        bool isSwoon = hurtStateArg.isSwoon;

        if (isFloat || isDrop)
        {
            hurtStateArg.isChangeVelocity = true;
            hurtStateArg.changeVelocity = Vector2.zero;
        }

        if (!isSwoon)
        {
            if (IsHurtWillDie(hurtStateArg.attackValue))
            {
                isSwoon = true;

                if (this.isSwoon && isInGround)
                {
                    hurtStateArg.attackForce = SkillUtil.GetGroundSmoonForce(-dir, hurtStateArg.attackForce);
                }
                else
                {
                    hurtStateArg.attackForce = SkillUtil.GetSmoonForce(hurtStateArg.attackerDir);
                }
            }
            else if (isFloat || isDrop)
            {
                isSwoon = true;
                hurtStateArg.isChangeVelocity = true;
                hurtStateArg.changeVelocity = Vector2.zero;
                hurtStateArg.attackForce = SkillUtil.GetFloatSmoonForce();
            }
            else if (this.isSwoon && isInGround)
            {
                isSwoon = true;
                hurtStateArg.attackForce = SkillUtil.GetGroundSmoonForce(-dir, hurtStateArg.attackForce);
            }
        }

        if (hurtStateArg.attackForce.y > 0)
        {
            m_AwakenTimer = -1;
        }

        if (string.IsNullOrEmpty(hurtStateArg.hurtAnim))
        {
            hurtStateArg.hurtAnim = AnimName.Hurt;
        }

        if (!hurtStateArg.isDefense)
        {
            if (isSwoon)
            {
                ChangeState<RoleSwoon>(hurtStateArg);
            }
            else
            {
                ChangeState<RoleHurt>(hurtStateArg);
            }
        }

        if (hurtStateArg.isGroundHurt && hurtStateArg.attackForce.y > 0)
        {
            m_OnGroundHurtStateArg = hurtStateArg;
        }
        else
        {
            OnGroundHurtMsg(hurtStateArg);
        }
    }

    public virtual void DefenseState(float attackerDir)
    {
        SetDir(-attackerDir);
        SetPosX(pos.x + attackerDir * 0.04f);
        ChangeState<RoleDefense>();
    }

    public virtual void DropTrapState(DropTrapStateArg dropTrapStateArg)
    {
        if (dropTrapStateArg == null)
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

        m_DropTrapStateArg = dropTrapStateArg;
        m_IsDropTrap = true;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    protected virtual void NormalAttack(Vector2 dir)
    {
        if (!m_CanCombo)
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
        m_CanCombo = false;
        m_IsHitSuccess = false;
        m_AttackTimer = -1;
        m_CurrSkillID = m_SkillData.attackIds[m_AttackIndex];
        m_SkillMgr.DeploySkill(m_CurrSkillID);
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

        m_SkillMgr.DeploySkill(m_CurrSkillID);
    }

    protected override void CheckGround()
    {
        if (m_IsDropGround && m_DropGroundTime > 0 && Time.time - m_DropGroundTime > 0.05f)
        {
            m_DropGroundTime = 0f;
            m_IsDropGround = false;
        }

        if (fsm == null || !fsm.isRunning)
        {
            return;
        }

        if (rigidbody2D.bodyType != RigidbodyType2D.Dynamic)
        {
            if (rigidbody2D.bodyType == RigidbodyType2D.Kinematic && isDead)
            {
                m_AwakenTimer = -1f;
                ChangeState<RoleDead>();
            }

            return;
        }

        UpdatePosX(transform.localPosition.x);

        if (isFloat)
        {
            return;
        }

        rigidbody2D.linearDamping = 0;
        OnDrop();
        CheckDropTrap();

        if (!isInGround || m_IsDropTrap)
        {
            return;
        }

        m_IsDropGround = true;
        m_DropGroundTime = Time.time;
        m_IsJumpAttack = false;
        m_IsBeThrow = false;//被扔出落地
        OnGround();

        if (isAddGroundForce)
        {
            onGroundEvent += OnGroundCheck;
            return;
        }

        ResetRigidbody();
        ExitSkill();
        CheckGroundHurt();
    }

    protected virtual void CheckAttack()
    {
        if (m_IsAttack)
        {
            if (IsCurrAnimationComplete() && m_AttackTimer < 0)
            {
                m_AttackTimer = Time.time;
            }
        }

        if (m_AttackTimer > 0)
        {
            if (Time.time - m_AttackTimer > (m_IsHitSuccess ? 0.05f : 0f))
            {
                if (m_AttackIndex < m_SkillData.attackIds.Length - 1)
                {
                    m_CanCombo = true;
                }
            }

            if (Time.time - m_AttackTimer > 0.2f && isInGround)
            {
                m_IsHitSuccess = false;
                m_IsAttack = false;
                m_AttackTimer = -1;
                m_CanCombo = true;
                m_AttackIndex = 0;
                ChangeDefaultState();
            }
        }
    }

    protected virtual void CheckDropTrap()
    {
        if (!m_IsDropTrap)
        {
            return;
        }

        Rect visionRect = CameraFollowMgr.instance.cameraFollow.GetVision();

        if ((transform.localPosition + Vector3.up * 0.6f).y + 0.1f < visionRect.yMin)
        {
            if (objectType == ObjectType.Player)
            {
                entityAttribute.SubHealth(m_DropTrapStateArg.attackValue);

                if (entityAttribute.IsDead())
                {
                    SetStateData<RoleDead>(m_DropTrapStateArg);
                    ChangeState<RoleDead>();
                }
                else
                {
                    SetPos2(m_DropTrapStateArg.rebirthPos);
                    ChangeState<RoleIdle>();
                    CameraFollowMgr.instance.cameraFollow.StartFollow();
                }
            }
            else
            {
                Release();
            }

            m_DropTrapStateArg.Release();
            m_IsDropTrap = false;
            m_DropTrapStateArg = null;
        }
    }

    protected virtual void OnGroundHurtMsg(HurtStateArg hurtStateArg)
    {
        if (!hurtStateArg.isNotPlayHurtSound)
        {
            string hurtSound = string.IsNullOrEmpty(hurtStateArg.hurtSound) ? SoundName.DefaultHurt : hurtStateArg.hurtSound;
            GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, hurtSound));
        }

        entityAttribute.SubHealth(hurtStateArg.attackValue);
        hurtStateArg.Release();
        m_OnGroundHurtStateArg = null;
    }

    private void CheckGroundHurt()
    {
        if (m_OnGroundHurtStateArg != null)
        {
            if (IsHurtWillDie(m_OnGroundHurtStateArg.attackValue))
            {
                OnGroundHurtMsg(m_OnGroundHurtStateArg);
                return;
            }

            GameEntry.timerMgr.Register(0.1f, () =>
            {
                if (m_OnGroundHurtStateArg != null)
                {
                    OnGroundHurtMsg(m_OnGroundHurtStateArg);
                }
            });

            m_AwakenTimer = Time.time;
        }
        else if (!isDead)
        {
            if (isSwoon)
            {
                m_AwakenTimer = Time.time;
            }
            else
            {
                m_AwakenTimer = -1f;
                ChangeDefaultState();
                GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.DefaultDrop));
            }
        }
    }

    private void CheckAwaken()
    {
        if (m_AwakenTimer > 0 && Time.time - m_AwakenTimer >= 1f)
        {
            m_AwakenTimer = -1f;

            if (entityAttribute.health > 0)
            {
                ChangeState<RoleAwaken>();
            }
            else
            {
                ChangeState<RoleDead>();
            }
        }
    }

    private void OnGroundCheck()
    {
        onGroundEvent -= OnGroundCheck;
        isAddGroundForce = false;
    }

    private void CheckAutoMove()
    {
        if (!isAssetLoadComplete || !isInGround || !m_IsAutoMove || m_IsDropTrap || m_IsBeCatch)
        {
            return;
        }

        if (!IsAnyState(typeof(RoleIdle), typeof(RoleMove), typeof(RoleJump)))
        {
            return;
        }

        if (!m_YArrived)
        {
            float yOffset = Mathf.Abs(m_MoveToPos.y - pos.y);
            m_YArrived = yOffset <= 0.02f;
            MoveStateArg arg = MoveStateArg.Create();
            arg.dir = (Vector2.up * yOffset).normalized;
            MoveState(arg);
            arg.Release();
            return;
        }

        if (!m_XArrived)
        {
            float xOffset = Mathf.Abs(m_MoveToPos.x - pos.x);
            m_XArrived = xOffset <= 0.02f;
            MoveStateArg arg = MoveStateArg.Create();
            arg.dir = (Vector2.right * xOffset).normalized;
            MoveState(arg);
            arg.Release();
            return;
        }

        SetDefaultState<RoleIdle>();
        ChangeDefaultState();

        m_AutoMoveComplete?.Invoke();
        m_AutoMoveComplete = null;
        m_IsAutoMove = false;
        m_XArrived = false;
        m_YArrived = false;
        m_MoveToPos = Vector2.zero;
    }

    protected virtual RoleStateParam GetRoleStateParam()
    {
        return ReferencePool.Acquire<RoleStateParam>();
    }
}