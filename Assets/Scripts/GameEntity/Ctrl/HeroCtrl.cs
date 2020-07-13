using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HeroCtrl : AvatarCtrl
{
    public override void Init(BaseRoleSkillData data)
    {     
        BaseHeroSkillData heroSkillData = data as BaseHeroSkillData;
        m_CatchAttackID = heroSkillData.CatchAttackID;
        m_ThrowAttackID = heroSkillData.ThrowAttackID;
        base.Init(data);
    }

    protected override void NormalAttack(Vector2 dir)
    {
        if ((m_Owner as BaseHero).IsCatch)
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
        base.NormalAttack(dir);
    }

    private int m_CatchAttackID;
    private int m_ThrowAttackID;
    private const float CATCH_ATTACK_STAMP = 0.3f;
    private float m_CatchAttackTimer = 0f;
}

