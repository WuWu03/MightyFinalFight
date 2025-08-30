using GameFrameWork.Camera;
using GameFrameWork.UI;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseRole
{
    public override bool canBeHit
    {
        get
        {
            return base.canBeHit && !m_IsRebirthState && !skillMgr.IsCurrSkill(m_BaseHeroSkillData.throwAttackID);
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

    protected override void OnInit()
    {
        base.OnInit();
        AddState<HeroRebirth>();
        AddState<HeroCatch>();
        AddState<HeroPickUp>();
        AddState<HeroAttackEnd>();

        m_DicAttacker ??= new();
        m_ListCatchTarget ??= new();
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        m_Slot1Renderer = go.transform.Find("slot1").GetComponent<Renderer>();
        m_Slot2Renderer = go.transform.Find("slot2").GetComponent<Renderer>();
        m_Slot1Renderer.enabled = true;
        m_Slot2Renderer.enabled = false;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        CheckCatch();
        CheckRebirthState();

        if (skillMgr.IsCurrSkill(m_BaseHeroSkillData.throwAttackID) && skillMgr.IsSkillComplete(m_BaseHeroSkillData.throwAttackID))
        {
            ResetCatch(true, true);
        }

        if (rigidbody2D.bodyType == RigidbodyType2D.Dynamic && Mathf.Abs(rigidbody2D.linearVelocity.x) > 0)
        {
            float x = rigidbody2D.linearVelocity.x > 0 ? bound.xMax : bound.xMin;

            if (IsOutVersionX(x))
            {
                SetVelocityX(0);
            }
        }

        if (m_HitTime < 0)
        {
            return;
        }

        if (Time.time - m_HitTime > 1.5f)
        {
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_DicAttacker.Clear();
        m_ListCatchTarget.Clear();
        m_RebirthStateTimer = 0;
        m_RebirthStateTime = 3.0f;
        m_RebirthLightTimer = 0;
        m_RebirthLightTime = 1f / 30f;
        m_CatchStamp = 0f;
        m_HitTime = -1f;
        m_CatchAttackCount = 0;
        m_Slot1Renderer = null;
        m_Slot2Renderer = null;
        m_IsRebirthState = false;
        m_Weapon = null;
        m_ListCatchTarget = null;
        m_DicAttacker = null;
        m_BaseHeroSkillData = null;
    }

    public override void SetSkillData(BaseRoleSkillData skillData)
    {
        base.SetSkillData(skillData);
        m_BaseHeroSkillData = skillData as BaseHeroSkillData;
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
        base.OnHitEnd(skillData, !HasCatch() && isHurtTarget);

        if (m_ListCatchTarget.Count < 1 || !isHurtTarget || m_CatchAttackCount >= 3)
        {
            return;
        }

        if (skillData.Type == SkillConfigData.SkillType.Skill)//捕捉状态下技能攻击不进行次数累积
        {
            ResetCatch(false, false);
            return;
        }

        m_CatchAttackCount++;

        if (m_CatchAttackCount >= 3)
        {
            ICanBeHit hit = m_ListCatchTarget[0];
            ResetCatch(false, false);
            HurtStateData hurtStateData = HurtStateData.Create();
            hurtStateData.attackerDir = dir;
            hurtStateData.attackValue = 0;
            hurtStateData.isSwoon = true;
            hurtStateData.attackForce = SkillUtil.GetSmoonForce(dir);
            hurtStateData.isNotPlayHurtSound = true;
            hit.OnHurtMsg(hurtStateData);
        }
    }

    public override void OnAttackMsg(SkillStateData data, bool isForceJumpAttack = false)
    {
        if (m_ListCatchTarget != null && m_ListCatchTarget.Count > 0)
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
        data.isCatch = HasCatch() && isCatchControl;
        base.OnMoveMsg(data);
    }

    public override void OnJumpMsg(JumpStateData data)
    {
        data.isCatch = false;

        if (HasCatch())
        {
            if (!isCatchControl)
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
            ResetCatch();
        }

        if (data.isSwoon)
        {
            data.hurtSound = SoundName.OnBlow;
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

            if (hitTime >= (data.isBoss ? 6 : 3) || IsHurtWillDie(data.attackValue))
            {
                data.attackForce = SkillUtil.GetSmoonForce(data.attackerDir);
                data.isSwoon = true;
                data.hurtSound = SoundName.OnBlow;
                data.isGroundHurt = false;
                m_DicAttacker.Clear();
            }
            m_HitTime = Time.time;
        }

        DropWeaponMsg(data.attackerDir);
        base.OnHurtMsg(data);
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

            float border = moveDir.x > 0 ? bound.xMax : bound.xMin;
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

            pos.x = isMapXCanMove ? pos.x : this.pos.x;
            pos.y = isMapYCanMove ? pos.y : this.pos.y;
        }

        base.SetPos(pos, posZ, caculateZ);
    }

    public virtual void OnRebirthMsg(Vector2 rebirthPos)
    {
        ChangeState<HeroRebirth>();
        MainPanel mainPanel = UIMgr.instance.Get(UINames.MainPanel) as MainPanel;
        mainPanel.SetPlayerHP(entityAttribute.health, entityAttribute.maxHealth);
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
        else
        {
            item.SetOwner(this);
        }
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
        Vector3 damagePos = transform.position + Vector3.up * boxCollider2D.size.y / 2f + Vector3.right * boxCollider2D.size.x / 2 * data.attackerDir;

        if (data.attackValue > 0)
        {
            HudMgr.instance.ShowEnemyDamage(data.attackValue, damagePos);
            base.OnGroundHurtMsg(data);
            MainPanel mainPanel = UIMgr.instance.Get(UINames.MainPanel) as MainPanel;
            mainPanel.SetPlayerHP(entityAttribute.health, entityAttribute.maxHealth);
        }
        else
        {
            base.OnGroundHurtMsg(data);
        }
    }

    protected override void NormalAttack(Vector2 dir)
    {
        if (isCatching)
        {
            bool isThrowing = skillMgr.IsCurrSkill(m_BaseHeroSkillData.throwAttackID);
            bool isThrowingComplete = skillMgr.IsSkillComplete(m_BaseHeroSkillData.throwAttackID);
            bool isCatchAttack = skillMgr.IsCurrSkill(m_BaseHeroSkillData.catchAttackID);
            bool isCatchAttackComplete = skillMgr.IsSkillComplete(m_BaseHeroSkillData.catchAttackID);

            if (isThrowing && !isThrowingComplete)//正在扔出敌人
            {
                m_CatchAttackTimer = 0;
                return;
            }

            if (isCatchAttack && (!isCatchAttackComplete || Time.time - m_CatchAttackTimer < ConstField.CatchAttackTime))//正在捕捉攻击
            {
                return;
            }

            if (isFloat && dir.y < 0)
            {
                m_CatchAttackTimer = 0;
                DeploySkill(m_BaseHeroSkillData.jumpAttackIds[1]);
                return;
            }

            if (Mathf.Abs(dir.x) != 0)
            {
                m_CatchAttackTimer = 0;
                SetDir(dir.x);
                m_ListCatchTarget[0].SetIsBeThrow(true);
                SetTargetPos();
                DeploySkill(m_BaseHeroSkillData.throwAttackID);
                return;
            }

            if (m_CatchAttackTimer == 0 || Time.time - m_CatchAttackTimer >= ConstField.CatchAttackTime)
            {
                m_CatchAttackTimer = Time.time;
                DeploySkill(m_BaseHeroSkillData.catchAttackID);
            }

            return;
        }

        m_CatchAttackTimer = 0f;
        BaseSceneItem item = IsNearSceneItem();

        if (item != null && item.canPickUp)
        {
            PickUpSceneItemMsg(item);
            return;
        }
        else if (m_Weapon != null)
        {
            if (m_Weapon.entityAttribute.health <= 1)
            {
                DeploySkill(m_BaseHeroSkillData.throwWeaponID);
                UseWeaponMsg();
            }
            else
            {
                DeploySkill(m_BaseHeroSkillData.weaponAttackID);
                UseWeaponMsg();
            }

            return;
        }

        if (!IsAnimation(AnimName.ThrowWeapon) || IsCurrAnimationComplete())
        {
            base.NormalAttack(dir);
        }
    }

    private BaseSceneItem IsNearSceneItem()
    {
        List<BaseSceneItem> list = SceneEntityMgr.instance.GetSceneItems();

        if (list == null || list.Count < 1)
        {
            return null;
        }

        for (int i = 0; i < list.Count; i++)
        {
            BaseSceneItem item = list[i];

            bool isXNear = Mathf.Abs(item.pos.x - pos.x) <= item.bound.width / 2;
            bool isYNear = Mathf.Abs(item.bound.yMin - bound.yMin) <= 0.05f;

            if (isXNear && isYNear)
            {
                return item;
            }
        }

        return null;
    }

    protected override void OnGround()
    {
        if (HasCatch())
        {
            if (isCatchControl)
            {
                SetTargetPos();
            }
            else
            {
                ResetCatch();
                m_ListCatchTarget[0].SetIsBeCatch(false);
            }
        }
    }

    protected virtual void CheckCatch()
    {
        if (m_ListCatchTarget.Count < 1)
        {
            List<BaseEnemy> enemyTargets = SceneEntityMgr.instance.GetEnemies();

            if (!IsAnyState(typeof(RoleMove)) || enemyTargets.Count < 1)
            {
                return;
            }

            for (int i = 0; i < enemyTargets.Count; i++)
            {
                BaseEnemy target = enemyTargets[i];

                if (target == null || !target.canBeHit || !enemyTargets[i].isInGround || !target.canBeCatch)
                {
                    continue;
                }

                float distance = GetCatchDistance(target);
                float yOffest = Mathf.Abs(target.pos.y - pos.y);
                float xOffest = Mathf.Abs(target.pos.x - pos.x);
                float dirOffest = (target.pos.x - pos.x) * dir;
                bool isInRange = yOffest <= 0.03f && xOffest <= distance && dirOffest > 0;

                if (isInRange)
                {
                    target.SetDir(dir * -1);
                    target.SetPosXY(pos.x + distance * dir, pos.y);
                    target.SetDepth(pos.y + 0.01f);
                    target.SetIsBeCatch(true);
                    SetDefaultState<HeroCatch>();
                    ChangeState<HeroCatch>();
                    m_ListCatchTarget.Add(target);
                    break;
                }
            }

            if (m_ListCatchTarget.Count > 0)
            {
                m_CatchStamp = Time.time;
            }

            return;
        }

        float catchTime = m_CatchAttackCount >= 3 ? 0.3f : ConstField.CatchTime;
        bool isOutCatchTime = Time.time - m_CatchStamp >= catchTime;

        if ((isOutCatchTime && isInGround) || (m_ListCatchTarget[0].isDead && IsAllAnimationComplete()))
        {
            ResetCatch();
            return;
        }

        if (m_ListCatchTarget.Count > 0 && isCatchControl)
        {
            if (IsCurrState<RoleSkill>() && !isFloat && !isDrop)
            {
                return;
            }

            BaseAvatar target = m_ListCatchTarget[0] as BaseAvatar;

            if (target.IsAnyState(typeof(RoleSwoon), typeof(RoleAwaken)))
            {
                return;
            }

            if (IsAnyState(typeof(RoleMove), typeof(RoleJump), typeof(RoleSkill)))
            {
                float distance = GetCatchDistance(target);
                float offest = target.transform.localScale.y < 0 ? target.GetAnimTriggerSize(AnimName.Idle).y : 0;
                target.SetPosXYZ(pos.x + distance * dir, pos.y, currPosZ + offest);
                target.SetDepth(pos.y + 0.01f);
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

        if (Time.time - m_RebirthLightTimer >= m_RebirthLightTime)
        {
            m_RebirthLightTimer = Time.time;
            m_Slot2Renderer.enabled = !m_Slot2Renderer.enabled;
        }
    }

    public void ResetCatch(bool changeState = true, bool exitSkill = true)
    {
        SetDefaultState<RoleIdle>();

        for (int i = 0; i < m_ListCatchTarget.Count; i++)
        {
            m_ListCatchTarget[i].SetIsBeCatch(false);
        }

        SetTargetPos();

        m_ListCatchTarget.Clear();
        m_CatchStamp = 0f;
        m_CatchAttackCount = 0;

        if (exitSkill)
        {
            ExitSkill();
        }

        if (changeState && !isAddGroundForce && isInGround)
        {
            ChangeDefaultState();
        }
    }

    private void SetTargetPos()
    {
        if (m_ListCatchTarget == null || m_ListCatchTarget.Count < 1)
        {
            return;
        }

        BaseAvatar target = m_ListCatchTarget[0] as BaseAvatar;
        target.SetDepth(pos.y + 0.01f);
        target.SetScale2(target.dir, 1);

        if (target.isInGround)
        {
            target.SetPosXYZ(target.pos.x + dir * 0.01f, pos.y, 0);
        }
    }

    private bool HasCatch()
    {
        return m_ListCatchTarget != null && m_ListCatchTarget.Count > 0 && m_CatchAttackCount < 3;
    }

    private int m_CatchAttackCount = 0;
    private bool m_IsRebirthState = false;
    private float m_CatchAttackTimer = 0f;
    private float m_RebirthStateTimer = 0;
    private float m_RebirthStateTime = 3.0f;
    private float m_RebirthLightTimer = 0;
    private float m_RebirthLightTime = 1f / 30f;
    private float m_CatchStamp = 0f;
    private float m_HitTime = -1f;
    private List<ICanBeHit> m_ListCatchTarget = null;
    private Dictionary<int, int> m_DicAttacker = null;
    private Weapon m_Weapon = null;
    private BaseHeroSkillData m_BaseHeroSkillData = null;
    private Renderer m_Slot1Renderer = null;
    private Renderer m_Slot2Renderer = null;
}