using System.Collections.Generic;
using UnityEngine;

public class BaseHeroCtrl : BaseRoleCtrl
{
    public override void SetData(BaseRoleSkillData data)
    {
        base.SetData(data);
        BaseHeroSkillData heroSkillData = data as BaseHeroSkillData;
        m_CatchAttackID = heroSkillData.CatchAttackID;
        m_ThrowAttackID = heroSkillData.ThrowAttackID;
        m_WeaponAttackID = heroSkillData.WeaponAttackID;
        m_ThrowWeaponID = heroSkillData.ThrowWeaponID;
        m_JumpAttackID = heroSkillData.JumpAttackIds[1];
    }

    protected override void NormalAttack(Vector2 dir)
    {
        BaseHero hero = m_Owner as BaseHero;
        if (hero.IsCatch)
        {
            m_AttackIndex = 0;
            bool isThrowing = m_SkillManager.IsCurrSkill(m_ThrowAttackID);
            bool isThrowingComplete = m_SkillManager.IsSkillComplete(m_ThrowAttackID);
            bool isCatchAttack = m_SkillManager.IsCurrSkill(m_CatchAttackID);
            bool isCatchAttackComplete = m_SkillManager.IsSkillComplete(m_CatchAttackID);

            if (isThrowing && !isThrowingComplete)//正在扔出敌人
            {
                m_CatchAttackTimer = 0;
                return;
            }

            if (isCatchAttack && (!isCatchAttackComplete || Time.time - m_CatchAttackTimer < CATCH_ATTACK_STAMP))//正在捕捉攻击
            {
                return;
            }

            if(m_Owner.IsFloat && dir.y < 0)
            {
                m_CatchAttackTimer = 0;
                m_SkillManager.DeploySkill(m_JumpAttackID);
                return;
            }

            if (Mathf.Abs(dir.x) != 0)
            {
                m_CatchAttackTimer = 0;
                m_Owner.SetDir(dir.x);
                hero.OnHitStart()[0].SetThrow(true);
                m_SkillManager.DeploySkill(m_ThrowAttackID);
                return;
            }

            if(m_CatchAttackTimer == 0 || Time.time - m_CatchAttackTimer >= CATCH_ATTACK_STAMP)
            {
                m_CatchAttackTimer = Time.time;
                m_SkillManager.DeploySkill(m_CatchAttackID);
            }

            return;
        }

        m_CatchAttackTimer = 0f;
        BaseSceneItem item = IsNearSceneItem();

        if (item != null && item.CanPickUp)
        {
            hero.PickUpSceneItemMsg(item);
            return;
        }
        else if (hero.Weapon != null)
        {
            bool isWeaponAttack = m_SkillManager.IsCurrSkill(m_WeaponAttackID);
            if (hero.Weapon.Health <= 1)
            {
                if (!isWeaponAttack && m_Owner.IsPlayComplete())
                {
                    hero.UseWeaponMsg();
                    m_SkillManager.DeploySkill(m_ThrowWeaponID);
                }
            }
            else
            {
                if (!isWeaponAttack && m_Owner.IsPlayComplete())
                {
                    hero.UseWeaponMsg();
                    m_SkillManager.DeploySkill(m_WeaponAttackID);
                }
            }

            return;
        }

        if (!m_Owner.IsAnim(AnimName.ThrowWeapon) || m_Owner.IsPlayComplete())
        {
            base.NormalAttack(dir);
        }
    }

    private BaseSceneItem IsNearSceneItem()
    {
        List<GameObject> list = m_Owner.Targets;
        for (int i = 0; i < list.Count; i++)
        {
            BaseSceneItem item = list[i].GetComponent<BaseSceneItem>();
            if (item == null) continue;

            bool isInRange = Mathf.Abs(item.Bound.yMin - m_Owner.Bound.yMin) <= 0.2f &&
                             Mathf.Abs(item.Pos.x - m_Owner.Pos.x) <= item.Bound.width / 2;
            if (isInRange)
                return item;
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

    private const float CATCH_ATTACK_STAMP = 0.2f;
}

