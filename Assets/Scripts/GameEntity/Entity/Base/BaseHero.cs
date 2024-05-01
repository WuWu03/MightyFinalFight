using GameFrameWork.Camera;
using GameFrameWork.UI;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseRole
{
    public override bool canMove
    {
        get
        {
            if (m_HurtTimer > 0 && Time.time - m_HurtTimer < 0.5f)
            {
                return false;
            }

            return (base.canMove || IsAnyState(typeof(HeroCatch))) && (!HasCatch() || m_IsCatchControl);
        }
    }

    public override bool canSkill
    {
        get
        {
            return base.canSkill || IsAnyState(typeof(HeroCatch));
        }
    }

    public override bool canAttack
    {
        get
        {
            if (m_HurtTimer > 0 && Time.time - m_HurtTimer < 0.5f)
            {
                return false;
            }

            return base.canAttack || HasCatch();
        }
    }

    public override bool canJump
    {
        get
        {
            return base.canJump || (HasCatch() && !IsAnimation(AnimName.Throw));
        }
    }

    public override bool canChangeDefaultState
    {
        get
        {
            bool condition = base.canChangeDefaultState || m_Weapon != null || IsAnimation(AnimName.ThrowWeapon);
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

    public override bool canBeHit
    {
        get
        {
            return base.canBeHit && !m_IsRebirthState && !IsAnyState(typeof(HeroPickUp)) && !IsAnimation(AnimName.Throw);
        }
    }

    public override bool isCatching
    {
        get
        {
            return HasCatch();
        }
    }

    public bool isRebirthState
    {
        get
        {
            return m_IsRebirthState;
        }
    }

    public Weapon weapon
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

        if (m_DicAttacker == null)
        {
            m_DicAttacker = new Dictionary<int, int>();
        }

        if (m_ListCatchTarget == null)
        {
            m_ListCatchTarget = new List<ICanBeHit>();
        }
    }

    public override void Release()
    {
        m_DicAttacker.Clear();
        m_ListCatchTarget.Clear();
        m_RebirthStateTimer = 0;
        m_RebirthStateTime = 3.0f;
        m_RebirthLightTimer = 0;
        m_RebirthLightTime = 1f / 30f;
        m_CatchStamp = 0f;
        m_HitTime = -1f;
        m_HurtTimer = 0f;
        m_IsDropInGround = false;
        m_CatchAttackCount = 0;
        m_Slot1Renderer = null;
        m_Slot2Renderer = null;
        m_IsRebirthState = false;
        m_Weapon = null;

        base.Release();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        CheckCatch();
        CheckRebirthState();

        if (m_Rigidbody2D.bodyType == RigidbodyType2D.Dynamic && Mathf.Abs(m_Rigidbody2D.velocity.x) > 0)
        {
            float x = m_Rigidbody2D.velocity.x > 0 ? bound.xMax : bound.xMin;

            if (IsOutVersionX(x))
            {
                SetVelocityX(0);
            }
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

    protected override void OnBeforeDestroy()
    {
        m_ListCatchTarget = null;
        m_DicAttacker = null;

        base.OnBeforeDestroy();
    }

    public override List<ICanBeHit> OnHitStart()
    {
        if (m_ListCatchTarget.Count < 1)
        {
            return null;
        }

        return m_ListCatchTarget;
    }

    public override void OnHitEnd(SkillConfigData skillData, bool isHurtTarget)
    {
        base.OnHitEnd(skillData, HasCatch() ? false : isHurtTarget);

        if (m_ListCatchTarget.Count < 1 || !isHurtTarget || m_CatchAttackCount >= 3)
        {
            return;
        }

        if (skillData.Type == SkillConfigData.SkillType.Skill)//捕捉状态下技能攻击不进行次数累积
        {
            if (!m_IsCatchControl)
            {
                ResetCatch(false);
            }

            return;
        }

        m_CatchAttackCount++;

        if (m_CatchAttackCount >= 3)
        {
            HurtStateData hurtStateData = HurtStateData.Create();
            hurtStateData.attackerDir = m_Dir;
            hurtStateData.attackValue = 0;
            hurtStateData.isSwoon = true;
            hurtStateData.attackForce = SkillUtil.GetSmoonForce(m_Dir);
            hurtStateData.isNotPlayHurtSound = true;
            m_ListCatchTarget[0].OnHurtMsg(hurtStateData);
        }
    }

    public override void OnAttackMsg(AttackStateData data,bool isForceJumpAttack = false)
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

    public override void OnMoveMsg(MoveStateData data)
    {
        data.isCatch = HasCatch() && m_IsCatchControl;
        base.OnMoveMsg(data);
    }

    public override void OnJumpMsg(JumpStateData data)
    {
        data.isCatch = false;

        if (HasCatch())
        {
            if (!m_IsCatchControl)
            {
                ResetCatch();
            }
            else
            {
                m_CatchStamp = Time.time;
                data.isCatch = true;
            }
        }
           
        base.OnJumpMsg(data);
    }

    public override void OnHurtMsg(HurtStateData data)
    {
        if (HasCatch())
        {
            ResetCatch(false);
        }

        if (data.isSwoon)
        {
            data.hurtSound = "Sound/OnBlow";
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
        else
        {
            int hitTime = 0;
            if (!isDrop)
            {
                if (!m_DicAttacker.TryGetValue(data.attackerId, out hitTime))
                {
                    m_DicAttacker.Add(data.attackerId, hitTime);
                }

                hitTime++;
                m_DicAttacker[data.attackerId] = hitTime;
            }
            else
            {
                hitTime = data.isBoss ? 6 : 3;
            }

            if (hitTime >= (data.isBoss ? 6 : 3) || m_EntityAttribute.health - data.attackValue <= 0)
            {
                data.attackForce = SkillUtil.GetSmoonForce(data.attackerDir);
                data.isSwoon = true;
                data.hurtSound = "Sound/OnBlow";
                data.isGroundHurt = false;
                m_DicAttacker.Clear();
            }
            m_HitTime = Time.time;
        }

        DropWeaponMsg(data.attackerDir);
        base.OnHurtMsg(data);
        m_HurtTimer = Time.time;
    }

    public override void OnDropTragMsg(DropTrapStateData data)
    {
        base.OnDropTragMsg(data);
        CameraMgr.instance.EndFollow();
    }

    public override void SetPos(Vector2 pos, float posZ, bool caculateZ = false)
    {
        if (!isAutoMove && IsAnyState(typeof(RoleMove), typeof(RoleSkill)))
        {
            if (!canMove)
            {
                return;
            }

            float border = m_MoveDir.x > 0 ? bound.xMax : bound.xMin;
            bool isMapXCanMove = StageMgr.instance.CanMovePosX(border) && !IsOutVersionX(border);
            bool isMapYCanMove = StageMgr.instance.CanMovePosY(pos.y);

            if (!isMapXCanMove && !isMapYCanMove)
            {
                CameraMgr.instance.EndFollow();
            }
            else
            {
                CameraMgr.instance.StartFollow();
            }

            pos.x = !isMapXCanMove ? m_Pos.x : pos.x;
            pos.y = !isMapYCanMove ? m_Pos.y : pos.y;
        }

        base.SetPos(pos, posZ, caculateZ);
    }

    public virtual void OnRebirthMsg(Vector2 rebirthPos)
    {
        ChangeState<HeroRebirth>();
        UIMgr.instance.Get<MainPanel>().SetPlayerHP(m_EntityAttribute.health, m_EntityAttribute.maxHealth);
    }

    public void SetRebirthState()
    {
        m_Slot1Renderer.enabled = true;
        m_Slot2Renderer.enabled = true;
        m_IsRebirthState = true;
        m_RebirthStateTimer = Time.time;
        m_RebirthLightTimer = Time.time;
    }

    public virtual void PickUpSceneItemMsg(BaseSceneItem item)
    {
        if (item == null)
        {
            return;
        }

        ChangeState<HeroPickUp>();

        if (item.objectType == ObjectType.Weapon)
        {
            Weapon weapon = item as Weapon;

            if (m_Weapon == null)
            {
                weapon.SetOwner(this);
                m_Weapon = weapon;
            }
            else
            {
                m_Weapon.entityAttribute.AddHealth(weapon.entityAttribute.health);
                weapon.Release();
            }
        }
        else item.SetOwner(this);
    }

    public virtual void DropWeaponMsg(float dir)
    {
        if (m_Weapon != null)
        {
            m_Weapon.entityAttribute.SubHealth(1);
            m_Weapon.Drop(dir);
            m_Weapon = null;
        }
    }

    public virtual void UseWeaponMsg()
    {
        m_Weapon.entityAttribute.SubHealth(1);

        if (m_Weapon.entityAttribute.health <= 0)
        {
            m_Weapon.Release();
            m_Weapon = null;
        }
    }

    protected override void OnGroundHurtMsg(HurtStateData data)
    {
        base.OnGroundHurtMsg(data);
        UIMgr.instance.Get<MainPanel>().SetPlayerHP(m_EntityAttribute.health, m_EntityAttribute.maxHealth);
    }

    protected override void OnGround()
    {
        if (HasCatch())
        {
            if (IsCurrState<RoleIdle>())
            {
                ResetCatch();
            }
            else
            {
                m_ListCatchTarget[0].SetCatch(false);
            }
        }
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
            List<BaseEnemy> enemyTargets = SceneEntityMgr.instance.GetEnemies();

            if (!IsAnyState(typeof(RoleMove)) || enemyTargets.Count < 1)
            {
                return;
            }

            for (int i = 0; i < enemyTargets.Count; i++)
            {
                ICanBeHit temp = enemyTargets[i].GetComponent<ICanBeHit>();

                if (temp == null || !temp.canBeHit || !(temp is BaseRole))
                {
                    continue;
                }

                BaseRole tempEnemy = temp as BaseRole;

                if (!tempEnemy.isInGround)
                {
                    continue;
                }

                float distance = GetCatchDistance(tempEnemy);
                float yOffest = Mathf.Abs(tempEnemy.pos.y - m_Pos.y);
                float xOffest = Mathf.Abs(tempEnemy.pos.x - m_Pos.x);
                float dirOffest = (tempEnemy.pos.x - m_Pos.x) * m_Dir;
                bool isInRange = yOffest <= 0.03f && xOffest <= distance && dirOffest > 0;

                if (isInRange)
                {
                    tempEnemy.SetDir(m_Dir * -1);
                    tempEnemy.SetPosXY(m_Pos.x + distance * m_Dir, m_Pos.y);
                    tempEnemy.SetDepth(m_Pos.y + 0.05f);
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

        bool isOutCatchTime = Time.time - m_CatchStamp >= ConstField.CatchTime;

        if ((isOutCatchTime && isInGround) || (m_ListCatchTarget[0].isDead && IsPlayComplete()))
        {
            ResetCatch();
            return;
        }

        if (m_ListCatchTarget.Count > 0 && m_IsCatchControl && !m_IsDropInGround)
        {
            if (IsCurrState<RoleSkill>() && !isFloat && !isDrop)
            {
                return;
            }

            BaseAvatar target = m_ListCatchTarget[0] as BaseAvatar;

            if(target.IsAnyState(typeof(RoleSwoon), typeof(RoleAwaken)))
            {
                return;
            }

            if (IsAnyState(typeof(RoleMove), typeof(RoleJump), typeof(RoleSkill)))
            {
                float distance = GetCatchDistance(target);
                float offest = target.transform.localScale.y < 0 ? target.GetAnimTriggerSize(AnimName.Idle).y : 0;
                target.SetPosXY(m_Pos.x + distance * m_Dir, m_Pos.y);
                target.SetPosZ(currPosZ + offest);
                target.SetDepth(m_Pos.y + 0.01f);
            }
        }
    }

    private float GetCatchDistance(BaseAvatar target)
    {
        Vector2 targetSize = target.GetAnimTriggerSize(AnimName.Idle);
        Vector2 selfSize = GetAnimTriggerSize(AnimName.Catch);
        float distance = targetSize.x / 2 + selfSize.x / 2 - 0.05f;

        return distance;
    }

    private void CheckRebirthState()
    {
        if (!m_IsRebirthState) 
        {
            return;
        }

        if (Time.time - m_RebirthStateTimer >= m_RebirthStateTime)
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
        target.SetDepth(target.pos.y);
        target.SetScale2(target.dir, 1);
        target.UpdatePosZ(0);

        if (m_IsDropInGround)
        {
            target.SetPosXY(target.pos.x, m_Pos.y, true);
        }

        if (isDrop)
        {
            target.UpdatePosX(target.pos.x);
            target.SetBodyType(RigidbodyType2D.Dynamic);
        }

        m_ListCatchTarget.Clear();
        m_CatchStamp = 0f;
        m_CatchAttackCount = 0;
        m_IsDropInGround = false;
        SetDefaultState<RoleIdle>();

        if (changeState && !m_IsAddGroundForce && isInGround)
        {
            ChangeDefaultState();
        }
    }

    private bool HasCatch()
    {
        return m_ListCatchTarget != null && m_ListCatchTarget.Count > 0 && m_CatchAttackCount < 3;
    }

    protected bool m_IsRebirthState = false;
    protected Weapon m_Weapon = null;

    private float m_RebirthStateTimer = 0;
    private float m_RebirthStateTime = 3.0f;
    private float m_RebirthLightTimer = 0;
    private float m_RebirthLightTime = 1f/30f;
    private float m_CatchStamp = 0f;
    private float m_HitTime = -1f;
    private float m_HurtTimer = 0f;
    private bool m_IsDropInGround = false;
    private int m_CatchAttackCount = 0;
    private List<ICanBeHit> m_ListCatchTarget = null;
    private Dictionary<int, int> m_DicAttacker = null;

    private Renderer m_Slot1Renderer = null;
    private Renderer m_Slot2Renderer = null;
}