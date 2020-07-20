using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BaseHeroCtrl : BaseRoleCtrl
{
    public override void InitData(BaseRoleSkillData data)
    {     
        BaseHeroSkillData heroSkillData = data as BaseHeroSkillData;
        m_CatchAttackID = heroSkillData.CatchAttackID;
        m_ThrowAttackID = heroSkillData.ThrowAttackID;
        m_WeaponAttackID = heroSkillData.WeaponAttackID;

        base.InitData(data);
    }

    protected override void NormalAttack(Vector2 dir)
    {
        BaseHero hero = m_Owner as BaseHero;
        if (hero.IsCatch)
        {
            if (m_Owner.IsAnim(AnimName.Throw))//正在扔出敌人
            {
                m_CatchAttackTimer = 0;
                return;
            }

            if (Mathf.Abs(dir.x) != 0)
            {
                m_CatchAttackTimer = 0;
                m_Owner.SetDir(dir.x);
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

        if (hero.Weapon != null)
        {
            hero.Weapon.SubHealth(1);
            if (hero.Weapon.Health <= 0)
                m_SkillManager.DeploySkill(1013);
            else
                m_SkillManager.DeploySkill(1012);
            return;
        }
        else
        {
            Weapon weapon = IsNearWeapon();
            if (weapon != null)
            {
                hero.PickUpWeaponMsg(weapon);
                return;
            }
        }
        base.NormalAttack(dir);
    }

    private Weapon IsNearWeapon()
    {
        List<GameObject> list = m_Owner.TriggerTargets.Targets;
        for (int i = 0; i < list.Count; i++)
        {
            Weapon weapon = list[i].GetComponent<Weapon>();
            if (weapon == null) continue;

            bool isInRange = Mathf.Abs(weapon.Bound.yMin - m_Owner.Bound.yMin) <= weapon.Bound.height/2 &&
                             Mathf.Abs(weapon.Pos.x - m_Owner.Pos.x) <= weapon.Bound.width / 2;
            if (isInRange)
                return weapon;
        }

        return null;
    }

    private int m_CatchAttackID;
    private int m_ThrowAttackID;
    private int m_WeaponAttackID;
    private const float CATCH_ATTACK_STAMP = 0.3f;
    private float m_CatchAttackTimer = 0f;
}

