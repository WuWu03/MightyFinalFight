using FrameWork;
using FrameWork.GameEntity;
using UnityEngine;

public class Bullet : BaseObject
{
    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_Rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
        m_Rigidbody.gravityScale = 0;
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        m_BoxCollider = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_BoxCollider.enabled = true;
        m_BoxCollider.isTrigger = true;
        m_IsHit = false;

    }
    public void SetBulletInfo(BaseRole owner, SkillData.SkillEffect skillEffect, SkillData.Bullet bulletData)
    {
        m_Rigidbody.drag = bulletData.Drag;
        m_Rigidbody.velocity = new Vector2(bulletData.Velocity.x * owner.Dir, bulletData.Velocity.y);

        m_BoxCollider.offset = bulletData.TriggerOffest;
        m_BoxCollider.size = bulletData.TriggerSize;

        m_Owner = owner;
        m_SkillEffect = skillEffect;
        m_BulletData = bulletData;

        if (owner.Dir > 0) transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (owner.Dir < 0) transform.localRotation = Quaternion.Euler(0, 180, 0);

        SetPos(owner.Pos + new Vector2(bulletData.Pos.x * owner.Dir, bulletData.Pos.y));
        SetRes(string.Format("{0}/{1}", ResDefine.EFFECT_PATH, bulletData.Name));
    }

    protected override void Update()
    {
        base.Update();
        if (m_Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1)
        {
            Release();
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
        if (m_ResGO == null || m_IsHit || collision.gameObject.Equals(m_Owner.gameObject)) return;

        ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();
        BaseObject targetObj = collision.gameObject.GetComponent<BaseObject>();

        bool canBeHit = hit != null && hit.CanBeHit;
        bool isInRange = Mathf.Abs(targetObj.Pos.y - m_Owner.Pos.y) < m_BulletData.HitRange;

        if (isInRange && canBeHit)
        {
            hit.OnHurtMsg(new HurtData()
            {
                IsSwoon = m_SkillEffect.IsSmoon,
                AttackerDir = m_Owner.Dir,
                AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * m_Owner.Dir, m_SkillEffect.AddTargetForce.y),
                AttackValue = 1,
            });

            m_IsHit = true;
            Release();
        }
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        m_Animator = go.GetComponent<DragonBones.UnityArmatureComponent>();
        m_Animator.animation.Play(m_BulletData.Name, 1);
    }

    public override void Release()
    {
        base.Release();
        m_Owner = null;
        m_SkillEffect = null;
        m_BulletData = null;
    }

    private bool m_IsHit = false;
    private DragonBones.UnityArmatureComponent m_Animator = null;
    private BaseObject m_Owner = null;
    private SkillData.SkillEffect m_SkillEffect = null;
    private SkillData.Bullet m_BulletData = null;
    private Rigidbody2D m_Rigidbody = null;
    private BoxCollider2D m_BoxCollider = null;
}