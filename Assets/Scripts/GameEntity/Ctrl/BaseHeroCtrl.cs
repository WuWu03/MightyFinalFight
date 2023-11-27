using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class BaseHeroCtrl : BaseRoleCtrl
{
    public override void SetData(BaseRoleSkillData data)
    {
        BaseHeroSkillData heroSkillData = data as BaseHeroSkillData;
        m_CatchAttackID = heroSkillData.catchAttackID;
        m_ThrowAttackID = heroSkillData.throwAttackID;
        m_WeaponAttackID = heroSkillData.weaponAttackID;
        m_ThrowWeaponID = heroSkillData.throwWeaponID;
        m_JumpAttackID = heroSkillData.jumpAttackIds[1];

        base.SetData(data);
    }

    protected override void NormalAttack(Vector2 dir)
    {
        BaseHero hero = m_Owner as BaseHero;

        if (hero.isCatch)
        {
            bool isThrowing = IsCurrSkill(m_ThrowAttackID);
            bool isThrowingComplete = IsSkillComplete(m_ThrowAttackID);
            bool isCatchAttack = IsCurrSkill(m_CatchAttackID);
            bool isCatchAttackComplete = IsSkillComplete(m_CatchAttackID);

            if (isThrowing && !isThrowingComplete)//正在扔出敌人
            {
                m_CatchAttackTimer = 0;
                return;
            }

            if (isCatchAttack && (!isCatchAttackComplete || Time.time - m_CatchAttackTimer < CATCH_ATTACK_TIME))//正在捕捉攻击
            {
                return;
            }

            if(m_Owner.isFloat && dir.y < 0)
            {
                m_CatchAttackTimer = 0;
                DeploySkill(m_JumpAttackID);
                return;
            }

            if (Mathf.Abs(dir.x) != 0)
            {
                m_CatchAttackTimer = 0;
                m_Owner.SetDir(dir.x);

                hero.OnHitStart()[0].SetThrow(true);
                DeploySkill(m_ThrowAttackID);
                return;
            }

            if(m_CatchAttackTimer == 0 || Time.time - m_CatchAttackTimer >= CATCH_ATTACK_TIME)
            {
                m_CatchAttackTimer = Time.time;
                DeploySkill(m_CatchAttackID);
            }

            return;
        }

        m_CatchAttackTimer = 0f;
        BaseSceneItem item = IsNearSceneItem();

        if (item != null && item.canPickUp)
        {
            hero.PickUpSceneItemMsg(item);
            return;
        }
        else if (hero.weapon != null)
        {
            bool isWeaponAttack = IsCurrSkill(m_WeaponAttackID);
            if (hero.weapon.entityAttribute.health <= 1)
            {
                if (!isWeaponAttack && m_Owner.IsPlayComplete())
                {
                    hero.UseWeaponMsg();
                    DeploySkill(m_ThrowWeaponID);
                }
            }
            else
            {
                if (!isWeaponAttack && m_Owner.IsPlayComplete())
                {
                    hero.UseWeaponMsg();
                    DeploySkill(m_WeaponAttackID);
                }
            }

            return;
        }

        if (!m_Owner.IsAnimation(AnimName.ThrowWeapon) || m_Owner.IsPlayComplete())
        {
            base.NormalAttack(dir);
        }
    }

    private BaseSceneItem IsNearSceneItem()
    {
        List<BaseSceneItem> list = SceneEntityMgr.instance.GetSceneItems();

        if(list == null || list.Count < 1)
        {
            return null;
        }

        for (int i = 0; i < list.Count; i++)
        {
            BaseSceneItem item = list[i];

            bool isXNear = Mathf.Abs(item.pos.x - m_Owner.pos.x) <= item.bound.width / 2;
            bool isYNear = Mathf.Abs(item.bound.yMin - m_Owner.bound.yMin) <= 0.2f;

            if (isXNear && isYNear)
            {
                return item;
            }
        }

        return null;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_CatchAttackID = 0;
        m_ThrowAttackID = 0;
        m_ThrowWeaponID = 0;
        m_WeaponAttackID = 0;
    }

    private int m_CatchAttackID;
    private int m_ThrowAttackID;
    private int m_ThrowWeaponID;
    private int m_WeaponAttackID;
    private int m_JumpAttackID;
    private float m_CatchAttackTimer = 0f;

    private const float CATCH_ATTACK_TIME = 0.3f;
}