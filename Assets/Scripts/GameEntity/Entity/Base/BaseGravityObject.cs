using GameFrameWork;
using UnityEngine;
using UnityEngine.Events;

public class BaseGravityObject : BaseBoundObject
{
    public bool isFloat
    {
        get
        {
            return m_Rigidbody2D.linearVelocity.y >= 0 && m_Rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
        }
    }

    public bool isDrop
    {
        get
        {
            return m_Rigidbody2D.linearVelocity.y < 0 && m_Rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
        }
    }

    public virtual bool isInGround
    {
        get
        {
            if(m_MapPosZ <= 0)
            {
                return currPosZ <= 0f;
            }

            return currPosZ * 100f <= m_MapPosZ;
        }
    }

    public Rigidbody2D rigidbody2D
    {
        get
        {
            return m_Rigidbody2D;
        }
    }

    public UnityEvent onDropEvent
    {
        get
        {
            return m_OnDropEvent;
        }
    }

    public UnityEvent onGroundEvent
    {
        get
        {
            return m_OnGroundEvent;
        }
    }

    public bool isAddGroundForce
    {
        get
        {
            return m_IsAddGroundForce;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);

        m_Rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
        m_Rigidbody2D.gravityScale = 0.8f;
        m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        m_Rigidbody2D.linearVelocity = Vector2.zero;
        m_Rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
        m_Rigidbody2D.freezeRotation = true;

        if (m_OnDropEvent == null)
        {
            m_OnDropEvent = new UnityEvent();
        }

        if (m_OnGroundEvent == null)
        {
            m_OnGroundEvent = new UnityEvent();
        }
    }

    public void AddForce(float x, float y, bool isGroundForce = false)
    {
        AddForce(new Vector2(x, y), isGroundForce);
    }

    public void AddForce(Vector2 force, bool isGroundForce = false)
    {
        m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody2D.AddForce(force);
        m_IsAddGroundForce = isGroundForce;
    }

    public void SetBodyType(RigidbodyType2D bodyType)
    {
        m_Rigidbody2D.bodyType = bodyType;
    }

    public void SetVelocityX(float x, bool isGroundForce = false)
    {
        SetVelocity(x, m_Rigidbody2D.linearVelocity.y, isGroundForce);
    }

    public void SetVelocityY(float y, bool isGroundForce = false)
    {
        SetVelocity(m_Rigidbody2D.linearVelocity.x, y, isGroundForce);
    }

    public void SetVelocity(float x, float y, bool isGroundForce = false)
    {
        SetVelocity(new Vector2(x, y), isGroundForce);
    }

    public void SetVelocity(Vector2 velocity, bool isGroundForce = false)
    {
        m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody2D.linearVelocity = velocity;
        m_IsAddGroundForce = isGroundForce;
    }

    public void SetGravityScale(float gravity)
    {
        m_Rigidbody2D.gravityScale = Mathf.Clamp(gravity, 0, 1);
    }

    public void SetDrag(float drag)
    {
        m_Rigidbody2D.linearDamping = drag;
    }

    public void SetAngularDrag(float angularDrag)
    {
        m_Rigidbody2D.angularDamping = angularDrag;
    }

    public void ResetRigidbody(bool changBodyType = true)
    {
        m_Rigidbody2D.gravityScale = 0.8f;
        m_Rigidbody2D.linearDamping = 0;
        m_Rigidbody2D.angularDamping = 0;
      
        if (changBodyType)
        {
            if(m_Rigidbody2D.bodyType != RigidbodyType2D.Static)
            {
                m_Rigidbody2D.angularVelocity = 0;
                m_Rigidbody2D.linearVelocity = Vector2.zero;
            }

            m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public override void Release()
    {
        ResetRigidbody();

        m_OnDropEvent.RemoveAllListeners();
        m_OnGroundEvent.RemoveAllListeners();

        m_IsAddGroundForce = false;
        m_Rigidbody2D = null;

        base.Release();
    }

    protected virtual void OnDrop() { }
    protected virtual void OnGround() { }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        CheckGround();
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
    }

    protected virtual void CheckGround()
    {
        if (m_Rigidbody2D.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        UpdatePosX(transform.localPosition.x);

        if (isFloat)
        {
            return;
        }

        m_Rigidbody2D.linearDamping = 0;
        m_OnDropEvent.Invoke();
        m_OnDropEvent.RemoveAllListeners();
        OnDrop();

        if (!isInGround)
        {
            return;
        }

        m_OnGroundEvent.Invoke();
        m_OnGroundEvent.RemoveAllListeners();
        ResetRigidbody();
        OnGround();
        m_IsAddGroundForce = false;
    }

    protected override void OnBeforeDestroy()
    {
        m_OnDropEvent = null;
        m_OnGroundEvent = null;

        base.OnBeforeDestroy();
    }

    private UnityEvent m_OnDropEvent = null;
    private UnityEvent m_OnGroundEvent = null;

    protected bool m_IsAddGroundForce = false;
    protected Rigidbody2D m_Rigidbody2D = null;
}