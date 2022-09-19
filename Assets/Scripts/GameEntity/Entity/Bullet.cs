using DragonBones;
using GameFrameWork;
using GameFrameWork.GameEntity;
using UnityEngine;

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
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        CheckHit(collision);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        CheckHit(collision);
    }

    private void CheckHit(Collider2D collision)
    {
        if (!m_IsResComplete || (!m_BulletData.isPenatrate && m_IsHit) || collision.gameObject.Equals(m_Owner.gameObject))
        {
            return;
        }

        BaseSceneObject targetObj = collision.gameObject.GetComponent<BaseSceneObject>();

        if(targetObj == null)
        {
            return;
        }

        bool isInRange = Mathf.Abs(targetObj.pos.y - m_Pos.y) < m_BulletData.hitRange;

        if (isInRange)
        {
            ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();

            if (m_SkillBulletEffect != null)
            {
                m_SkillBulletEffect.BulletEffect(hit);
            }

            if (!m_BulletData.isPenatrate)
            {
                SetVelocity(Vector2.zero);
            }

            if (!string.IsNullOrEmpty(m_BulletData.hitAnim))
            {
                m_Animator.animation.timeScale = m_BulletData.hitAnimSpeed;
                m_Animator.animation.Play(m_BulletData.hitAnim, 1);
                SetTrigger(m_BulletData.hitAnim);
            }

            m_IsHit = true;
        }
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        go.SetActive(true);
        
        m_BoxCollider2D.enabled = true;
        m_BoxCollider2D.isTrigger = true;
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_Animator.animation.timeScale = m_BulletData.normalAnimSpeed;
        m_Animator.animation.Play(m_BulletData.normalAnim, 0);
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
    private UnityArmatureComponent m_Animator = null;
    private BulletData m_BulletData = null;
}