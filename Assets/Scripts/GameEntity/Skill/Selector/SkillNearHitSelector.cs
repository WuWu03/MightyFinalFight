using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SkillNearHitSelector : SkillBaseSelector
{
    public SkillNearHitSelector(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override List<ICanBeHit> GetTargets()
    {
        m_ListTargets.Clear();

        if(m_Owner.objectType == ObjectType.Player)
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

    private void CheckTarget(BaseAvatar ba)
    {
        if(ba == null)
        {
            return;
        }

        ICanBeHit hit = ba.GetComponent<ICanBeHit>();

        if (hit == null)
        {
            return;
        }

        bool isInRange = false;

        if(SkillUtil.IsRectangleCollide(ba.bound, m_Owner.bound))
        {
            Vector2 target = (ba.pos - m_Owner.pos).normalized;
            Vector2 normal = m_Owner.dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
            float angle = Vector2.Angle(target, normal);

            if (angle <= m_SkillEffect.SelectorAngle / 2)
            {
                //计算是否在椭圆范围内
                //float w = m_Owner.bound.width / 2;
                float h = m_Owner.bound.height / 2;
                //float x = w * Mathf.Cos(angle * Mathf.Rad2Deg);
                float y = h * Mathf.Sin(angle * Mathf.Rad2Deg);
                //float distance = Vector2.Distance(m_Owner.pos, ba.pos);

                isInRange = Mathf.Abs(m_Owner.pos.y - ba.pos.y) <= Mathf.Abs(y);//x坐标满足条件只需检测y坐标

                //!(ba.bound.xMin > m_Owner.bound.xMax || ba.bound.yMin > m_Owner.bound.yMax || m_Owner.bound.xMin > ba.bound.xMax || m_Owner.bound.yMin > ba.bound.yMax);
            }
        }
      
        if (isInRange && hit.canBeHit)
        {
            m_ListTargets.Add(hit);
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