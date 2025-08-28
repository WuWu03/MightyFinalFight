using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseAvatar
{
    protected override void OnInit()
    {
        base.OnInit();
        m_TaretHits ??= new();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_BulletData = data as BulletData;
    }

    public void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
        SetDir(owner.dir);
        SetPos2(owner.pos + new Vector2(m_BulletData.pos.x * owner.dir, m_BulletData.pos.y));
    }

    public void SetSkillEffect(SkillBulletEffect skillBulletEffect)
    {
        m_SkillBulletEffect = skillBulletEffect;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (m_BulletData.isPenatrate)
        {
            bool isOutVersion = IsOutVersionX(transform.localPosition.x) || IsOutVersionY(transform.localPosition.y);
            if (isOutVersion || rigidbody2D.linearVelocity.sqrMagnitude <= 0.1 * 0.1)
            {
                Release();
                return;
            }
        }
        else
        {
            if (m_IsHit)
            {
                if (IsPlayComplete())
                {
                    Release();
                    return;
                }
            }
            else
            {
                if (rigidbody2D.linearVelocity.sqrMagnitude <= 0.1 * 0.1)
                {
                    Release();
                    return;
                }
            }
        }

        UpdatePos2(transform.localPosition);
        CheckHit();
    }

    private void CheckHit()
    {
        if (!isAssetLoadComplete)
        {
            return;
        }

        if (m_Owner.objectType == ObjectType.Player)
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
    }

    private void CheckTarget(BaseBoundObject bgo)
    {
        if (bgo == null)
        {
            return;
        }

        if (bgo is not ICanBeHit)
        {
            return;
        }

        ICanBeHit hit = bgo as ICanBeHit;
        bool isInRange = false;

        if (SkillUtil.IsRectangleCollide(bgo.bound, bound))
        {
            isInRange = Mathf.Abs(bgo.bound.center.y - bound.center.y) < m_BulletData.hitRange;
        }

        if (!isInRange)
        {
            return;
        }

        if (!m_BulletData.isPenatrate)
        {
            SetVelocity(Vector2.zero);
        }

        if (!string.IsNullOrEmpty(m_BulletData.hitAnim))
        {
            PlayAnimation(m_BulletData.hitAnim, 1, m_BulletData.hitAnimSpeed);
        }

        if (!m_TaretHits.Contains(hit) && m_SkillBulletEffect.BulletEffect(hit)) 
        {
            m_TaretHits.Add(hit);
        }

        m_IsHit = true;
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);

        PlayAnimation(m_BulletData.normalAnim, 0, m_BulletData.normalAnimSpeed);
        SetTrigger(m_BulletData.normalAnim);
        SetDrag(m_BulletData.drag);
        SetGravityScale(0f);
        SetVelocity(m_BulletData.velocity.x * m_Owner.dir, m_BulletData.velocity.y);
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BulletData = null;
        m_IsHit = false;
        m_SkillBulletEffect = null;
        m_TaretHits.Clear();
    }

    private bool m_IsHit = false;
    private List<ICanBeHit> m_TaretHits = null;
    private SkillBulletEffect m_SkillBulletEffect = null;
    private BaseRole m_Owner = null;
    private BulletData m_BulletData = null;
}