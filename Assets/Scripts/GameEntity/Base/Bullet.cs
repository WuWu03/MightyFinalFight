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
        if (!m_IsResComplete || (!m_BulletData.IsPenatrate && m_IsHit) || collision.gameObject.Equals(m_Owner.gameObject)) return;

        ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();
        BaseSceneObject targetObj = collision.gameObject.GetComponent<BaseSceneObject>();

        bool canBeHit = hit != null && hit.CanBeHit;
        bool isInRange = Mathf.Abs(targetObj.Pos.y - m_Pos.y) < m_BulletData.HitRange;

        if (isInRange && canBeHit)
        {
            int defenseValue = 0;
            bool isCritical = false;

            if(hit is BaseRole)
            {
                defenseValue = (hit as BaseRole).DefenseValue;
            }

            HurtData hurtData = HurtData.Create();
            hurtData.Id = m_BulletData.Id;
            hurtData.IsSwoon = m_BulletData.IsSmoon;
            hurtData.AttackerDir = m_Owner.Dir;
            hurtData.AttackerPos = m_Owner.Pos;
            hurtData.AttackForce = new Vector2(m_BulletData.AddTargetForce.x * m_Owner.Dir, m_BulletData.AddTargetForce.y);
            hurtData.AttackValue = SkillFactory.CacDamage(m_Owner.AttackValue, defenseValue, m_Owner.CriticalValue, m_BulletData.DamageMulity, out isCritical);
            hurtData.CanBeDefense = false;
            hurtData.IsCritical = isCritical;
            hurtData.SkillExp = m_BulletData.SkillExp;
            hit.OnHurtMsg(hurtData);

            if (!m_BulletData.IsPenatrate)
            {
                SetVelocity(Vector2.zero);
            }

            if(!string.IsNullOrEmpty(m_BulletData.HitAnim))
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
        base.Release();
        m_Owner = null;
        m_BulletData = null;
        m_IsHit = false;
    }

    private bool m_IsHit = false;
    private UnityArmatureComponent m_Animator = null;
    private BulletData m_BulletData = null;
}