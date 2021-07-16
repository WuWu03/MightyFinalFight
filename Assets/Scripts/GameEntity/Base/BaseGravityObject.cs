using GameFrameWork;
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

    public UnityEvent OnDropEvent
    {
        get
        {
            return m_OnDropEvent;
        }
    }

    public UnityEvent OnGroundEvent
    {
        get
        {
            return m_OnGroundEvent;
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
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

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

        m_OnDropEvent.Invoke();
        m_OnDropEvent.RemoveAllListeners();
        OnDrop();

        if (!IsInGround) return;

        m_OnGroundEvent.Invoke();
        m_OnGroundEvent.RemoveAllListeners();
        OnGround();
    }

    public override void Release()
    {
        base.Release();
        m_Rigidbody.gravityScale = 0f;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    protected virtual void OnDrop() { }
    protected virtual void OnGround() { }

    private UnityEvent m_OnDropEvent = new UnityEvent();
    private UnityEvent m_OnGroundEvent = new UnityEvent();
    protected Rigidbody2D m_Rigidbody = null;
}
