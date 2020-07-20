using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneItem : BaseSceneObject
{
    public bool IsFloat
    {
        get
        {
            return m_Rigidbody.velocity.y >= 0 && m_Rigidbody.bodyType == RigidbodyType2D.Dynamic;
        }
    }

    public Rect Bound
    {
        get
        {
            m_Bound.width = m_Collider.size.x;
            m_Bound.height = m_Collider.size.x;
            m_Bound.xMin = m_Pos.x + m_Collider.offset.x - m_Collider.size.x / 2;
            m_Bound.xMax = m_Pos.x + m_Collider.offset.x + m_Collider.size.x / 2;
            m_Bound.yMin = m_Pos.y + m_Collider.offset.y - m_Collider.size.y / 2;
            m_Bound.yMax = m_Pos.y + m_Collider.offset.y + m_Collider.size.y / 2;
            return m_Bound;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_Rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
        m_Rigidbody.gravityScale = 1.0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        m_Rigidbody.freezeRotation = true;
        m_Collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_Collider.enabled = false;
        m_Collider.isTrigger = false;
    }

    public override void SetPos(Vector2 pos)
    {
        m_Pos = pos;
        transform.localPosition = new Vector3(pos.x, pos.y, Bound.yMin);
    }

    public virtual void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
    }

    public override void Release()
    {
        base.Release();
        m_Owner = null;
    }

    protected void SetCollider(Vector2 offest,Vector2 size)
    {
        m_Collider.offset = offest;
        m_Collider.size = size;
        SetPos(m_Pos);
    }

    private Rect m_Bound = Rect.zero;
    protected BaseSceneObject m_Owner = null;
    protected Rigidbody2D m_Rigidbody = null;
    protected BoxCollider2D m_Collider = null;
}
