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
        SetDir(owner.Dir);
        SetPos(owner.Pos + new Vector2(m_BulletData.Pos.x * owner.Dir, m_BulletData.Pos.y));
    }

    public void SetSkillEffect(SkillBulletEffect skillBulletEffect)
    {
        m_SkillBulletEffect = skillBulletEffect;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_BulletData.IsPenatrate)
        {
            bool isOutVersion = IsOutVersionX(transform.localPosition.x) || IsOutVersionY(transform.localPosition.y);
            if (isOutVersion || m_Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1)
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
                if(m_Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1) Release();
            }
        }

        UpdatePos(transform.localPosition);
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
        if (!m_IsResComplete || (!m_BulletData.IsPenatrate && m_IsHit) || collision.gameObject.Equals(m_Owner.gameObject))
        {
            return;
        }

        BaseSceneObject targetObj = collision.gameObject.GetComponent<BaseSceneObject>();
        bool isInRange = Mathf.Abs(targetObj.Pos.y - m_Pos.y) < m_BulletData.HitRange;

        if (isInRange)
        {
            ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();

            if (m_SkillBulletEffect != null)
            {
                m_SkillBulletEffect.BulletEffect(hit);
            }

            if (!m_BulletData.IsPenatrate)
            {
                SetVelocity(Vector2.zero);
            }

            if (!string.IsNullOrEmpty(m_BulletData.HitAnim))
            {
                m_Animator.animation.timeScale = m_BulletData.HitAnimSpeed;
                m_Animator.animation.Play(m_BulletData.HitAnim, 1);
                SetTrigger(m_BulletData.HitAnim);
            }

            m_IsHit = true;
        }
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        go.SetActive(true);
        
        m_Collider.enabled = true;
        m_Collider.isTrigger = true;
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_Animator.animation.timeScale = m_BulletData.NormalAnimSpeed;
        m_Animator.animation.Play(m_BulletData.NormalAnim, 0);
        SetTrigger(m_BulletData.NormalAnim);
        SetDrag(m_BulletData.Drag);
        SetGravityScale(0f);
        SetVelocity(m_BulletData.Velocity.x * m_Owner.Dir, m_BulletData.Velocity.y);
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