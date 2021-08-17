using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.UI;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseRole
{
    public override bool CanMove
    {
        get
        {
            return (base.CanMove || IsAnyState(typeof(HeroCatch))) && (!HasCatch() || m_IsCatchControl);
        }
    }

    public override bool CanSkill
    {
        get
        {
            return base.CanSkill || IsAnyState(typeof(HeroCatch));
        }
    }

    public override bool CanAttack
    {
        get
        {
            return base.CanAttack || HasCatch();
        }
    }


    public override bool CanJump
    {
        get
        {
            return base.CanJump || (HasCatch() && !IsAnim(AnimName.Throw));
        }
    }

    public override bool CanChangeDefaultState
    {
        get
        {
            bool condition = base.CanChangeDefaultState || m_Weapon != null || IsAnim(AnimName.ThrowWeapon);
            if(HasCatch())
            {
                if (IsAnyState(typeof(RoleAttack)))
                    condition = true;
                else
                    condition = condition && !m_IsCatchControl;
            }
            return condition;
        }
    }

    public override bool CanBeHit
    {
        get
        {
            return base.CanBeHit && !m_IsRebirthState && !IsFloat && !IsAnyState(typeof(HeroPickUp)) && !IsAnim(AnimName.Throw);
        }
    }

    public bool IsCatch
    {
        get
        {
            return HasCatch();
        }
    }

    public Weapon Weapon
    {
        get
        {
            return m_Weapon;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<HeroRebirth>();
        AddState<HeroCatch>();
        AddState<HeroPickUp>();
        AddState<HeroAttackEnd>();
        m_DicAttacker = new Dictionary<int, int>();
        m_ListCatchTarget = new List<ICanBeHit>();
    }

    public override void Release()
    {
        base.Release();
        m_DicAttacker.Clear();
        m_ListCatchTarget.Clear();
        m_ListCatchTarget = null;
        m_DicAttacker = null;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        CheckCatch();
        CheckRebirthState();

        if (m_Rigidbody.bodyType == RigidbodyType2D.Dynamic && Mathf.Abs(m_Rigidbody.velocity.x) > 0)
        {
            Rect bound = GetBound(transform.localPosition);
            float x = m_Rigidbody.velocity.x > 0 ? bound.xMax : bound.xMin;

            if (IsOutVersionX(x)) SetVelocityX(0);
        }

        if (m_HitTime < 0) return;

        if (Time.time - m_HitTime > 1.5f)
        {
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_Slot1Renderer = go.transform.Find("slot1").GetComponent<Renderer>();
        m_Slot2Renderer = go.transform.Find("slot2").GetComponent<Renderer>();
        m_Slot1Renderer.enabled = true;
        m_Slot2Renderer.enabled = false;
    }

    public override List<ICanBeHit> OnHitStart()
    {
        if (m_ListCatchTarget.Count < 1) return null;
        return m_ListCatchTarget;
    }

    public override void OnHitEnd(SkillConfigData skillData, bool isHurtTarget)
    {
        base.OnHitEnd(skillData, isHurtTarget);

        if (m_ListCatchTarget.Count < 1 || !isHurtTarget || m_CatchAttackCount >= 3) return;

        if (skillData.Type == SkillConfigData.SkillType.Skill && !m_IsCatchControl)//捕捉状态下技能攻击不进行次数累积
        {
            ResetCatch(false);
            return;
        }

        m_CatchAttackCount++;

        if (m_CatchAttackCount >= 3)
        {
            HurtData hurtData = HurtData.Create();
            hurtData.AttackerDir = m_Dir;
            hurtData.AttackValue = 0;
            hurtData.IsSwoon = true;
            hurtData.AttackForce = new Vector2(40f * m_Dir, 150f);

            m_ListCatchTarget[0].OnHurtMsg(hurtData);
        }
    }

    public override void OnAttackMsg(AttackData data,bool isForceJumpAttack = false)
    {
        if(m_ListCatchTarget != null)
        {
            m_CatchStamp = Time.time;
        }

        if (m_CatchAttackCount >= 3)
        {
            return;
        }

        base.OnAttackMsg(data, isForceJumpAttack);
    }

    public override void OnMoveMsg(MoveData data)
    {
        data.IsCatch = HasCatch() && m_IsCatchControl;
        base.OnMoveMsg(data);
    }

    public override void OnJumpMsg(JumpData data)
    {
        data.IsCatch = false;
        if (HasCatch())
        {
            if (!m_IsCatchControl)
                ResetCatch();
            else
            {
                m_CatchStamp = Time.time;
                data.IsCatch = true;
            }
        }
           
        base.OnJumpMsg(data);
    }

    public override void OnHurtMsg(HurtData data)
    {
        if (HasCatch())
            ResetCatch(false);
        if (data.IsSwoon)
        {
            data.HurtSound = "Sound/OnBlow";
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
        else
        {
            int hitTime = 0;
            if (!IsDrop)
            {
                if (!m_DicAttacker.TryGetValue(data.AttackerId, out hitTime))
                {
                    m_DicAttacker.Add(data.AttackerId, hitTime);
                }

                hitTime++;
                m_DicAttacker[data.AttackerId] = hitTime;
            }
            else
                hitTime = 3;

            if (hitTime >= 3)
            {
                data.AttackForce = new Vector2(40 * data.AttackerDir, 120);
                data.IsSwoon = true;
                data.HurtSound = "Sound/OnBlow";
                data.IsGroundHurt = false;
                m_DicAttacker.Clear();
            }
            m_HitTime = Time.time;
        }

        DropWeaponMsg(data.AttackerDir);
        base.OnHurtMsg(data);
    }

    public override void OnDropTragMsg(TrapData data)
    {
        base.OnDropTragMsg(data);
        CameraMgr.Ins.EndFollow();
    }

    public override void AddHealth(int value)
    {
        base.AddHealth(value);
        UIMgr.Ins.GetPanel<MainPanel>().SetPlayerHP(m_Health, m_MaxHealth);
    }

    public override void SubHealth(int value)
    {
        base.SubHealth(value);
        UIMgr.Ins.GetPanel<MainPanel>().SetPlayerHP(m_Health, m_MaxHealth);
    }

    public override void SetPos(Vector2 pos)
    {
        if (IsAnyState(typeof(RoleMove), typeof(RoleSkill)))
        {
            if (!CanMove) return;

            Rect bound = GetBound(pos);
            bool isMapXCanMove = StageMgr.Ins.CanMovePosX(m_MoveDir.x > 0 ? bound.xMax : bound.xMin) && !IsOutVersionX(m_MoveDir.x > 0 ? bound.xMax : bound.xMin);
            bool isMapYCanMove = StageMgr.Ins.CanMovePosY(pos.y);

            if (!isMapXCanMove && !isMapYCanMove)
            {
                CameraMgr.Ins.EndFollow();
            }
            else
            {
                CameraMgr.Ins.StartFollow();
            }

            if (!isMapXCanMove)
                pos.x = m_Pos.x;
            if (!isMapYCanMove) pos.y = m_Pos.y;
        }

        base.SetPos(pos);
    }

    public virtual void OnRebirthMsg(Vector2 rebirthPos)
    {
        ChangeState<HeroRebirth>();
        m_Slot1Renderer.enabled = true;
        m_Slot2Renderer.enabled = true;
        m_IsRebirthState = true;
        m_RebirthStateTimer = Time.time;
        m_RebirthLightTimer = Time.time;
        UIMgr.Ins.GetPanel<MainPanel>().SetPlayerHP(m_Health, m_MaxHealth);
    }

    public virtual void PickUpSceneItemMsg(BaseSceneItem item)
    {
        if (item == null) return;
        ChangeState<HeroPickUp>();

        if (item.ObjectType == ObjectType.Weapon)
        {
            Weapon weapon = item as Weapon;
            if (m_Weapon == null)
            {
                weapon.SetOwner(this);
                m_Weapon = weapon;
            }
            else
            {
                m_Weapon.AddHealth(weapon.Health);
                weapon.Release();
            }
        }
        else item.SetOwner(this);
    }

    public virtual void DropWeaponMsg(float dir)
    {
        if (m_Weapon != null)
        {
            m_Weapon.SubHealth(1);
            m_Weapon.Drop(dir);
            m_Weapon = null;
        }
    }

    public virtual void UseWeaponMsg()
    {
        m_Weapon.SubHealth(1);
        if (m_Weapon.Health <= 0)
        {
            m_Weapon.Release();
            m_Weapon = null;
            if (IsAnyState(typeof(RoleIdle)))
            {
                PlayAnimation(AnimName.Idle, 0, 1);
            }
        }
    }

    protected override void OnGround()
    {
        if (HasCatch() && IsCurrState<RoleSkill>())
        { 
            ResetCatch();
        }
        m_IsOnGround = !IsAnyState(typeof(RoleHurt), typeof(RoleSwoon));
    }

    protected virtual void CheckCatch()
    {
        if (m_CatchAttackCount >= 3 && IsPlayComplete())
        {
            ResetCatch(true);
            return;
        }

        if (m_ListCatchTarget.Count < 1)
        {
            bool isCheck = false;
            if (IsAnyState(typeof(RoleIdle))) isCheck = m_IsOnGround;
            isCheck = isCheck || IsAnyState(typeof(RoleMove));
            m_IsOnGround = false;

            if (!isCheck || m_ListTargets.Count < 1) return;

            for (int i = 0; i < m_ListTargets.Count; i++)
            {
                ICanBeHit temp = m_ListTargets[i].GetComponent<ICanBeHit>();
                if (temp == null || !temp.CanBeHit || !(temp is BaseAvatar)) continue;

                BaseAvatar tempAvatar = temp as BaseAvatar;

                float distance = GetCatchDistance(tempAvatar);
                bool isInRange = Mathf.Abs(tempAvatar.Pos.y - m_Pos.y) <= 0.03f &&
                                 Mathf.Abs(tempAvatar.Pos.x - m_Pos.x) < distance &&
                                    (tempAvatar.Pos.x - m_Pos.x) * m_Dir > 0;
                if (isInRange)
                {
                    tempAvatar.SetDir(m_Dir * -1);
                    tempAvatar.SetPos2(m_Pos.x + distance * m_Dir, m_Pos.y);
                    tempAvatar.SetDepth(m_Pos.y + 0.05f);
                    temp.SetCatch(true);
                    ChangeState<HeroCatch>();
                    SetDefaultState<HeroCatch>();
                    m_ListCatchTarget.Add(temp);
                    break;
                }
            }

            if (m_ListCatchTarget.Count > 0)
            {
                m_CatchStamp = Time.time;
            }

            return;
        }

        if ((Time.time - m_CatchStamp >= m_CatchTime && IsInGround) || m_ListCatchTarget[0].IsDead)
        {
            ResetCatch();
            return;
        }

        if (IsAnyState(typeof(RoleMove), typeof(RoleJump), typeof(RoleSkill)))
        {
            if (IsCurrState<RoleSkill>() && !IsFloat && !IsDrop) return;

            if (m_ListCatchTarget.Count > 0 && m_IsCatchControl)
            {
                BaseAvatar target = m_ListCatchTarget[0] as BaseAvatar;
                float distance = GetCatchDistance(target);
                float y = target.Pos.y;
                float offest = target.transform.localScale.y < 0 ? target.GetAnimTriggerSize(AnimName.Idle).y : 0;
                target.SetPos2(m_Pos.x + distance * m_Dir, transform.localPosition.y + offest);
                if (IsFloat || IsDrop)
                    target.UpdatePos2(target.Pos.x, y);
                target.SetDepth(m_Pos.y + 0.01f);
            }
        }
    }

    private float GetCatchDistance(BaseAvatar target)
    {
        Vector2 targetSize = target.GetAnimTriggerSize(AnimName.Idle);
        Vector2 selfSize = GetAnimTriggerSize(AnimName.Catch);
        float distance = targetSize.x / 2 + selfSize.x / 2 - 0.01f;

        return distance;
    }

    private void CheckRebirthState()
    {
        if (!m_IsRebirthState) return;

        if(Time.time - m_RebirthStateTimer >= m_RebirthStateTime)
        {
            m_RebirthStateTimer = 0;
            m_RebirthLightTimer = 0;
            m_IsRebirthState = false;
            m_Slot1Renderer.enabled = true;
            m_Slot2Renderer.enabled = false;
            return;
        }

        if(Time.time - m_RebirthLightTimer >= m_RebirthLightTime)
        {
            m_RebirthLightTimer = Time.time;
            m_Slot2Renderer.enabled = !m_Slot2Renderer.enabled;
        }
    }

    public void ResetCatch(bool changeState = true)
    {
        for (int i = 0; i < m_ListCatchTarget.Count; i++)
        {
            m_ListCatchTarget[i].SetCatch(false);
        }

        BaseAvatar target = m_ListCatchTarget[0] as BaseAvatar;
        target.SetDepth(target.Pos.y);
        target.SetScale2(target.Dir, 1);

        if (IsDrop)
        {
            target.UpdatePos2(target.Pos.x, m_Pos.y);
            target.SetBodyType(RigidbodyType2D.Dynamic);
        }

        m_ListCatchTarget.Clear();
        m_CatchStamp = 0f;
        m_CatchAttackCount = 0;
        m_IsOnGround = false;
        SetDefaultState<RoleIdle>();

        if (changeState && !m_IsAddGroundForce)
        {
            ChangeDefaultState();
        }
    }

    private bool HasCatch()
    {
        return m_ListCatchTarget != null && m_ListCatchTarget.Count > 0 && m_CatchAttackCount < 3;
    }

    protected float m_CatchTime = 2;
    protected bool m_IsRebirthState = false;
    protected Weapon m_Weapon = null;

    private float m_RebirthStateTimer = 0;
    private float m_RebirthStateTime = 3.0f;
    private float m_RebirthLightTimer = 0;
    private float m_RebirthLightTime = 1f/30f;
    private float m_CatchStamp = 0f;
    private float m_HitTime = -1f;
    private bool m_IsOnGround = false;
    private int m_CatchAttackCount = 0;
    private List<ICanBeHit> m_ListCatchTarget = null;
    private Dictionary<int, int> m_DicAttacker = null;

    private Renderer m_Slot1Renderer = null;
    private Renderer m_Slot2Renderer = null;
}