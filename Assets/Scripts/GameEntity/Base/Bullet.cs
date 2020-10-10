using DragonBones;
using FrameWork;
using FrameWork.GameEntity;
using UnityEngine;

public class Bullet : BaseSceneItem
{
    public override void InitInfo(BaseSceneObjectInfo data)
    {
        base.InitInfo(data);
        m_BulletData = data as BulletInfo;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        SetDir(owner.Dir);
        SetPos(owner.Pos + new Vector2(m_BulletData.Pos.x * owner.Dir, m_BulletData.Pos.y));
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!m_ResComplete) return;

        if (m_BulletData.IsPenatrate)
        {
            bool isOutVersion = IsOutVersionX(transform.localPosition.x) || IsOutVersionY(transform.localPosition.y);
            if (isOutVersion || m_Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1) 
                Release();
        }
        else
        {
            if (m_IsHit && m_Animator.animation.isCompleted) Release();
            if (!m_IsHit && m_Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1) Release();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckHit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckHit(collision);
    }

    private void CheckHit(Collider2D collision)
    {
        if (!m_ResComplete || (!m_BulletData.IsPenatrate && m_IsHit) || collision.gameObject.Equals(m_Owner.gameObject)) return;

        ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();
        BaseSceneObject targetObj = collision.gameObject.GetComponent<BaseSceneObject>();

        bool canBeHit = hit != null && hit.CanBeHit;
        bool isInRange = true;//Mathf.Abs(targetObj.Pos.y - m_Owner.Pos.y) < m_BulletData.HitRange;

        if (isInRange && canBeHit)
        {
            hit.OnHurtMsg(new HurtData()
            {
                ID = m_BulletData.ID,
                IsSwoon = m_BulletData.IsSmoon,
                AttackerDir = m_Owner.Dir,
                AttackerPos = m_Owner.Pos,
                AttackForce = new Vector2(m_BulletData.AddTargetForce.x * m_Owner.Dir, m_BulletData.AddTargetForce.y),
                AttackValue = 1,
                SkillExp = m_BulletData.SkillExp,
            });

            if (!m_BulletData.IsPenatrate)
                m_Rigidbody.velocity = Vector2.zero;
           
            if(!string.IsNullOrEmpty(m_BulletData.HitAnim))
            {
                m_Animator.animation.timeScale = m_BulletData.HitAnimSpeed;
                m_Animator.animation.Play(m_BulletData.HitAnim, 1);
            }
            
            m_IsHit = true;
        }
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        SetCollider(m_BulletData.TriggerOffest, m_BulletData.TriggerSize);
        m_Rigidbody.drag = m_BulletData.Drag;    
        m_Collider.enabled = true;
        m_Collider.isTrigger = true;
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_Animator.animation.timeScale = m_BulletData.NormalAnimSpeed;
        m_Animator.animation.Play(m_BulletData.NormalAnim, 0);
        m_Rigidbody.gravityScale = 0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody.velocity = new Vector2(m_BulletData.Velocity.x * m_Owner.Dir, m_BulletData.Velocity.y);
    }

    public override void Release()
    {
        base.Release();
        m_Owner = null;
        m_BulletData = null;
        m_IsHit = false;
    }

    private bool m_IsHit = false;
    private UnityArmatureComponent m_Animator = null;
    private BulletInfo m_BulletData = null;
}