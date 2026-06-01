using WuWuFramework;
using WuWuFramework.Event;
using UnityEngine;

public class BaseGravityObject : BaseBoundObject
{
    private event WuWuFrameworkAction m_OnDropEvent;
    private event WuWuFrameworkAction m_OnGroundEvent;
    private bool m_IsAddGroundForce;
    private Rigidbody2D m_Rigidbody2D;
    
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
            if (mapPosZ <= 0)
            {
                return currPosZ <= 0f;
            }

            return currPosZ * 100f <= mapPosZ;
        }
    }

    public Rigidbody2D rigidbody2D
    {
        get
        {
            return m_Rigidbody2D;
        }
    }

    public event WuWuFrameworkAction onDropEvent
    {
        add
        {
            m_OnDropEvent += value;
        }
        remove
        {
            m_OnDropEvent -= value;
        }
    }

    public event WuWuFrameworkAction onGroundEvent
    {
        add
        {
            m_OnGroundEvent += value;
        }
        remove
        {
            m_OnGroundEvent -= value;
        }
    }

    public bool isAddGroundForce
    {
        get
        {
            return m_IsAddGroundForce;
        }
        protected set
        {
            m_IsAddGroundForce = value;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        m_Rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
        m_Rigidbody2D.gravityScale = 0.8f;
        m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        m_Rigidbody2D.linearVelocity = Vector2.zero;
        m_Rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
        m_Rigidbody2D.freezeRotation = true;
    }

    public void AddForceX(float x, bool isAddGroundForce = false)
    {
        AddForce(new Vector2(x, 0), isAddGroundForce);
    }

    public void AddForceY(float y, bool isAddGroundForce = false)
    {
        AddForce(new Vector2(0, y), isAddGroundForce);
    }

    public void AddForce(float x, float y, bool isAddGroundForce = false)
    {
        AddForce(new Vector2(x, y), isAddGroundForce);
    }

    public void AddForce(Vector2 force, bool isAddGroundForce = false)
    {
        m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody2D.AddForce(force);
        m_IsAddGroundForce = isAddGroundForce;
    }

    public void SetVelocityX(float x, bool isAddGroundForce = false)
    {
        SetVelocity(x, m_Rigidbody2D.linearVelocity.y, isAddGroundForce);
    }

    public void SetVelocityY(float y, bool isAddGroundForce = false)
    {
        SetVelocity(m_Rigidbody2D.linearVelocity.x, y, isAddGroundForce);
    }

    public void SetVelocity(float x, float y, bool isAddGroundForce = false)
    {
        SetVelocity(new Vector2(x, y), isAddGroundForce);
    }

    public void SetVelocity(Vector2 velocity, bool isAddGroundForce = false)
    {
        m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody2D.linearVelocity = velocity;
        m_IsAddGroundForce = isAddGroundForce;
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
            if (m_Rigidbody2D.bodyType != RigidbodyType2D.Static)
            {
                m_Rigidbody2D.angularVelocity = 0;
                m_Rigidbody2D.linearVelocity = Vector2.zero;
            }

            m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    protected override void OnRelease()
    {
        ResetRigidbody();
        m_OnDropEvent = null;
        m_OnGroundEvent = null;
        m_IsAddGroundForce = false;
        m_Rigidbody2D = null;
        base.OnRelease();
    }

    protected virtual void OnDrop()
    {
        m_OnDropEvent?.Invoke();
        m_OnDropEvent = null;
    }

    protected virtual void OnGround()
    {
        m_OnGroundEvent?.Invoke();
        m_OnGroundEvent = null;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        CheckGround();
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

        OnDrop();

        if (!isInGround)
        {
            return;
        }

        ResetRigidbody();
        OnGround();
        m_IsAddGroundForce = false;
    }
}