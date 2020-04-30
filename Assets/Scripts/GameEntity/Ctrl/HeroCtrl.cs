using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HeroCtrl : AvatarCtrl
{
    protected override void NormalAttack()
    {
        if ((m_Owner as BaseHero).IsCatch)
        {
            if(m_CatchAttackTimer == 0 || Time.time - m_CatchAttackTimer >= CATCH_ATTACK_STAMP)
            {
                m_CatchAttackTimer = Time.time;
                m_SkillManager.DeploySkill(1009);
            }
            return;
        }
        base.NormalAttack();
    }

    private const float CATCH_ATTACK_STAMP = 0.3f;
    private float m_CatchAttackTimer = 0f;
}

