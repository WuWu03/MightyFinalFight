using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseGravityObject : BaseSceneObject
{
    public bool IsFloat
    {
        get
        {
            return m_Rigidbody.velocity.y >= 0 && m_Rigidbody.bodyType == RigidbodyType2D.Dynamic;
        }
    }

    public bool IsDrop
    {
        get
        {
            return m_Rigidbody.velocity.y < 0 && m_Rigidbody.bodyType == RigidbodyType2D.Dynamic;
        }
    }

    public Rigidbody2D Rigidbody
    {
        get
        {
            return m_Rigidbody;
        }
    }

    public UnityEvent OnDropEvent = new UnityEvent();
    public UnityEvent OnGroundEvent = new UnityEvent();

    public override void Init(int id, string name)
    {
        base.Init(id, name);

        m_Rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
        m_Rigidbody.gravityScale = 1.0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        m_Rigidbody.freezeRotation = true;
    }

    protected override void Update()
    {
        base.Update();
        CheckGround();
    }

    protected virtual void CheckGround()
    {
        if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic) return;

        UpdatePos2(transform.localPosition.x, Pos.y);

        if (IsFloat)
        {
            return;
        }

        OnDropEvent.Invoke();
        OnDropEvent.RemoveAllListeners();

        if (!IsInGround) return;

        OnGroundEvent.Invoke();
        OnGroundEvent.RemoveAllListeners();
    }

    protected Rigidbody2D m_Rigidbody = null;
}
