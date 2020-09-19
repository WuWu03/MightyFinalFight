using System.Collections.Generic;
using UnityEngine;

public class BaseHeroCtrl : BaseRoleCtrl
{
    public override void InitData(BaseRoleSkillInfo data)
    {     
        BaseHeroSkillInfo heroSkillData = data as BaseHeroSkillInfo;
        m_CatchAttackID = heroSkillData.CatchAttackID;
        m_ThrowAttackID = heroSkillData.ThrowAttackID;
        m_WeaponAttackID = heroSkillData.WeaponAttackID;
        m_ThrowWeaponID = heroSkillData.ThrowWeaponID;
        base.InitData(data);
    }

    protected override void NormalAttack(Vector2 dir)
    {
        BaseHero hero = m_Owner as BaseHero;
        if (hero.IsCatch)
        {
            m_AttackIndex = 0;
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

        BaseSceneItem item = IsNearSceneItem();
        if (item != null && item.CanPickUp)
        {
            hero.PickUpSceneItemMsg(item);
            return;
        }
        else if (hero.Weapon != null)
        {
            if (hero.Weapon.Health <= 1 && m_ThrowWeaponID != 0)
                m_SkillManager.DeploySkill(m_ThrowWeaponID);
            else
                m_SkillManager.DeploySkill(m_WeaponAttackID);
            hero.UseWeaponMsg();
            return;
        }
        base.NormalAttack(dir);
    }

    private BaseSceneItem IsNearSceneItem()
    {
        List<GameObject> list = m_Owner.TriggerTargets.Targets;
        for (int i = 0; i < list.Count; i++)
        {
            BaseSceneItem item = list[i].GetComponent<BaseSceneItem>();
            if (item == null) continue;

            bool isInRange = Mathf.Abs(item.Bound.yMin - m_Owner.Bound.yMin) <= item.Bound.height/2 &&
                             Mathf.Abs(item.Pos.x - m_Owner.Pos.x) <= item.Bound.width / 2;
            if (isInRange)
                return item;
        }

        return null;
    }

    private int m_CatchAttackID;
    private int m_ThrowAttackID;
    private int m_ThrowWeaponID;
    private int m_WeaponAttackID;
    private const float CATCH_ATTACK_STAMP = 0.3f;
    private float m_CatchAttackTimer = 0f;
}

