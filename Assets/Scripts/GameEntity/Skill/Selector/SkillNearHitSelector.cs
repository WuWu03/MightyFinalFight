using System.Collections.Generic;
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
            List<Barrel> barrelTargets = SceneEntityMgr.instance.GetBarrels();

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

            if (barrelTargets != null)
            {
                for (int i = 0; i < barrelTargets.Count; i++)
                {
                    CheckTarget(barrelTargets[i]);
                }
            }
        }
        else
        {
            CheckTarget(PlayerMgr.instance.player);
        }
     
        return m_ListTargets;
    }

    private void CheckTarget(BaseBoundObject bbo)
    {
        if (bbo == null || bbo is not ICanBeHit)
        {
            return;
        }

        bool isInRange = false;

        if (SkillUtil.IsRectangleCollide(bbo.bound, m_Owner.bound))
        {
            if (bbo is Barrel)
            {
                Vector2 bsoLeftTop = new Vector2(bbo.bound.xMin, bbo.bound.yMax) - bbo.bound.center;
                float selectorAngle = m_SkillEffect.SelectorAngle;

                if (m_Owner.pos.y > bbo.pos.y)
                {
                    selectorAngle = Vector2.Angle(Vector2.left, bsoLeftTop.normalized);
                }

                Vector2 target = (bbo.pos - m_Owner.pos).normalized;
                Vector2 normal = m_Owner.dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
                float angle = Vector2.Angle(target, normal);

                if (angle <= selectorAngle / 2)
                {
                    isInRange = true;
                }
            }
            else
            {
                Vector2 target = (bbo.pos - m_Owner.pos).normalized;
                Vector2 normal = m_Owner.dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
                float angle = Vector2.Angle(target, normal);

                if (angle <= m_SkillEffect.SelectorAngle / 2)
                {
                    //计算是否在椭圆范围内
                    //float w = m_Owner.bound.width / 2;
                    float h = m_Owner.bound.height / 2;
                    //float x = w * Mathf.Cos(angle * Mathf.Rad2Deg);
                    float y = Mathf.Max(h * Mathf.Sin(angle * Mathf.Rad2Deg), 0.01f);
                    //float distance = Vector2.Distance(m_Owner.pos, ba.pos);

                    isInRange = Mathf.Abs(m_Owner.pos.y - bbo.pos.y) <= Mathf.Abs(y);//x坐标满足条件只需检测y坐标

                    //!(ba.bound.xMin > m_Owner.bound.xMax || ba.bound.yMin > m_Owner.bound.yMax || m_Owner.bound.xMin > ba.bound.xMax || m_Owner.bound.yMin > ba.bound.yMax);
                }
            }
        }

        ICanBeHit hit = bbo as ICanBeHit;

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