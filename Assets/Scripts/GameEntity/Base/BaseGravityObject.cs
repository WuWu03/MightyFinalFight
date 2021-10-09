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

    public virtual bool IsInGround
    {
        get
        {
            return CurrPosZ <= m_PosZ;
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

    public bool IsAddGroundForce
    {
        get
        {
            return m_IsAddGroundForce;
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
        if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        UpdatePosX(transform.localPosition.x);

        if (IsFloat)
        {
            return;
        }

        m_OnDropEvent.Invoke();
        m_OnDropEvent.RemoveAllListeners();
        OnDrop();

        if (!IsInGround)
        {
            return;
        }

        m_OnGroundEvent.Invoke();
        m_OnGroundEvent.RemoveAllListeners();
        ResetRigidbody();
        OnGround();
        m_IsAddGroundForce = false;
    }

    public void AddForce(float x, float y, bool isGroundForce = false)
    {
        AddForce(new Vector2(x, y), isGroundForce);
    }

    public void AddForce(Vector2 force, bool isGroundForce = false)
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody.AddForce(force);
        m_IsAddGroundForce = isGroundForce;
    }

    public void SetBodyType(RigidbodyType2D bodyType)
    {
        m_Rigidbody.bodyType = bodyType;
    }

    public void SetVelocityX(float x, bool isGroundForce = false)
    {
        SetVelocity(x, m_Rigidbody.velocity.y, isGroundForce);
    }

    public void SetVelocityY(float y, bool isGroundForce = false)
    {
        SetVelocity(m_Rigidbody.velocity.x, y, isGroundForce);
    }

    public void SetVelocity(float x, float y, bool isGroundForce = false)
    {
        SetVelocity(new Vector2(x, y), isGroundForce);
    }

    public void SetVelocity(Vector2 velocity, bool isGroundForce = false)
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody.velocity = velocity;
        m_IsAddGroundForce = isGroundForce;
    }

    public void SetGravityScale(float gravity)
    {
        m_Rigidbody.gravityScale = Mathf.Clamp(gravity, 0, 1);
    }

    public void SetDrag(float drag)
    {
        m_Rigidbody.drag = drag;
    }

    public void SetAngularDrag(float angularDrag)
    {
        m_Rigidbody.angularDrag = angularDrag;
    }

    public void ResetRigidbody(bool changBodyType = true)
    {
        m_Rigidbody.gravityScale = 1;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.drag = 0;
        m_Rigidbody.angularDrag = 0;
        m_Rigidbody.angularVelocity = 0;

        if (changBodyType)
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public override void Release()
    {
        ResetRigidbody();
        base.Release();
    }

    protected virtual void OnDrop() { }
    protected virtual void OnGround() { }

    private UnityEvent m_OnDropEvent = new UnityEvent();
    private UnityEvent m_OnGroundEvent = new UnityEvent();

    protected bool m_IsAddGroundForce = false;
    protected Rigidbody2D m_Rigidbody = null;
}