using GameFrameWork;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseAvatar
{
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
            if (isOutVersion || m_Rigidbody2D.velocity.sqrMagnitude <= 0.1 * 0.1)
            {
                Release();
                return;
            }
        }
        else
        {
            if (m_IsHit)
            {
                if (m_Animator.animation.isCompleted)
                {
                    Release();
                    return;
                }
            }
            else
            {
                if (m_Rigidbody2D.velocity.sqrMagnitude <= 0.1 * 0.1)
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
        if (!m_IsAssetLoadComplete)
        {
            return;
        }

        if (!m_BulletData.isPenatrate && m_IsHit)
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

    private void CheckTarget(BaseGravityObject bgo)
    {
        if (bgo == null)
        {
            return;
        }

        ICanBeHit hit = bgo.GetComponent<ICanBeHit>();

        if (hit == null || !hit.canBeHit)
        {
            return;
        }

        bool isInRange = false;

        if (SkillUtil.IsRectangleCollide(bgo.bound, bound))
        {
            isInRange = Mathf.Abs(bgo.pos.y - m_Pos.y) < m_BulletData.hitRange;
        }

        if(!isInRange)
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

        if (m_SkillBulletEffect != null)
        {
            m_SkillBulletEffect.BulletEffect(hit);
        }

        m_IsHit = true;
    }

    protected override void OnLoadAssetComplete(GameObject go, object[] param)
    {
        base.OnLoadAssetComplete(go, param);

        PlayAnimation(m_BulletData.normalAnim, 0, m_BulletData.normalAnimSpeed);
        SetTrigger(m_BulletData.normalAnim);
        SetDrag(m_BulletData.drag);
        SetGravityScale(0f);
        SetVelocity(m_BulletData.velocity.x * m_Owner.dir, m_BulletData.velocity.y);
    }

    public override void Release()
    {
        m_BulletData = null;
        m_IsHit = false;
        m_SkillBulletEffect = null;
        base.Release();
    }

    private bool m_IsHit = false;
    private SkillBulletEffect m_SkillBulletEffect = null;
    private BaseRole m_Owner = null;
    private BulletData m_BulletData = null;
}