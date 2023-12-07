using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillBulletSelector : SkillBaseSelector
{
    public SkillBulletSelector(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) 
    {

    }

    public override List<ICanBeHit> GetTargets()
    {
        m_ListTargets.Clear();

        if (m_Owner.objectType == ObjectType.Player)
        {
            List<BaseEnemy> enemyTargets = SceneEntityMgr.instance.GetEnemies();
            List<BaseSceneItem> sceneItemTargets = SceneEntityMgr.instance.GetSceneItems();

            if (enemyTargets != null)
            {
                for (int i = 0; i < enemyTargets.Count; i++)
                {
                    CheckTarget(enemyTargets[i]);
                }
            }

            if (sceneItemTargets != null)
            {
                for (int i = 0; i < sceneItemTargets.Count; i++)
                {
                    CheckTarget(sceneItemTargets[i]);
                }
            }
        }
        else
        {
            CheckTarget(PlayerMgr.instance.player);
        }

        return m_ListTargets;
    }

    private void CheckTarget(BaseSceneObject bso)
    {
        if (bso == null)
        {
            return;
        }

        ICanBeHit hit = bso.GetComponent<ICanBeHit>();

        if (hit == null)
        {
            return;
        }

        for (int i = 0; i < m_Owner.bullets.Count; i++)
        {
            bool isInRange = false;

            if( Mathf.Abs(bso.pos.y - m_Owner.bullets[i].pos.y) < m_SkillEffect.Bullets[i].HitRange)
            {
                isInRange = SkillUtil.IsRectangleCollide(bso.bound, m_Owner.bound);
            }

            if (isInRange && hit.canBeHit)
            {
                m_ListTargets.Add(hit);
            }
        }
    }

    public override void Reset()
    {
        m_ListTargets.Clear();
    }

    public override void Exit()
    {
        m_ListTargets.Clear();
    }
}
