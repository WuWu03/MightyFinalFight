using DragonBones;
using GameFrameWork;
using GameFrameWork.GameEntity;
using System.Collections.Generic;
using UnityEngine;
using static SkillConfigData;

public class Bullet : BaseSceneItem
{
    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_BulletData = data as BulletData;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        SetDir(owner.dir);
        SetPos2(owner.pos + new Vector2(m_BulletData.pos.x * owner.dir, m_BulletData.pos.y));
    }

    public void SetSkillEffect(SkillBulletEffect skillBulletEffect)
    {
        m_SkillBulletEffect = skillBulletEffect;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_BulletData.isPenatrate)
        {
            bool isOutVersion = IsOutVersionX(transform.localPosition.x) || IsOutVersionY(transform.localPosition.y);
            if (isOutVersion || m_Rigidbody2D.velocity.sqrMagnitude <= 0.1 * 0.1)
            {
                Release();
            }
        }
        else
        {
            if (m_IsHit)
            {
                if (m_Animator.animation.isCompleted) Release();
            }
            else
            {
                if(m_Rigidbody2D.velocity.sqrMagnitude <= 0.1 * 0.1) Release();
            }
        }

        UpdatePos2(transform.localPosition);
        CheckHit();
    }

    private void CheckHit()
    {
        if (!m_IsResComplete)
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
    }

    private void CheckTarget(BaseAvatar ba)
    {
        if (ba == null)
        {
            return;
        }

        ICanBeHit hit = ba.GetComponent<ICanBeHit>();

        if (hit == null || !hit.canBeHit)
        {
            return;
        }

        bool isInRange = false;

        if (Mathf.Abs(ba.pos.y - m_Pos.y) < m_BulletData.hitRange)
        {
            isInRange = SkillUtil.IsRectangleCollide(ba.bound, m_Bound);
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

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);

        PlayAnimation(m_BulletData.normalAnim, 1, m_BulletData.normalAnimSpeed);
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
    private BulletData m_BulletData = null;
}