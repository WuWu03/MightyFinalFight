using UnityEngine;
using GameFrameWork.GameEntity;
using GameFrameWork.Pool;
using GameFrameWork;
using GameFrameWork.Camera;

public class BaseSceneObject : BaseObject
{
    public ObjectType ObjectType
    {
        get
        {
            return m_ObjectType;
        }
    }

    public GameObject ResGO
    {
        get
        {
            return m_ResGO;
        }
    }

    public BoxCollider2D Collider
    {
        get
        {
            return m_Collider;
        }
    }

    public Vector2 Pos
    {
        get
        {
            return m_Pos;
        }
    }

    public Vector2Int MapPos
    {
        get
        {
            return m_MapPos;
        }
    }

    public Rect Bound
    {
        get
        {
            return GetBound(m_Pos);
        }
    }

    public float Dir//物体朝向 1右 -1左 不能为0
    {
        get
        {
            return m_Dir;
        }
    }

    public bool IsInGround
    {
        get
        {
            return transform.localPosition.y <= m_Pos.y;
        }
    }

    public int Health
    {
        get { return m_Health; }
        set { m_Health = value; }
    }

    public int MaxHealth
    {
        get { return m_MaxHealth; }
        set { m_MaxHealth = value; }
    }

    public bool ResComplete
    {
        get
        {
            return m_ResComplete;
        }
    }

    public int EntityID
    {
        get
        {
            return m_EntityID;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_Pos = transform.localPosition;
        m_Collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_Collider.isTrigger = true;
    }

    public virtual void InitInfo(BaseSceneObjectInfo info)
    {
        m_Health = info.Health;
        m_MaxHealth = info.MaxHealth;
        m_EntityID = info.ID;
        if (m_MaxHealth < m_Health)
            m_MaxHealth = m_Health;
    }

    public override void Release()
    {
        base.Release();
        GameObjectPool.Ins.Put(m_ResPath, m_ResGO);
        SceneObjectPool.Ins.Put(this);
        m_ResPath = null;
        m_ResComplete = false;
    }

    public void SetObjectType(ObjectType type)
    {
        m_ObjectType = type;
    }

    public void UpdatePos2(float x, float y)
    {
        UpdatePos(new Vector2(x, y));
    }

    public virtual void UpdatePos(Vector2 pos)
    {
        m_Pos = pos;
        m_MapPos.x = Mathf.CeilToInt(m_Pos.x * 100);
        m_MapPos.y = Mathf.CeilToInt(m_Pos.y * 100);
    }

    public void SetPos2(float x, float y)
    {
        SetPos(new Vector2(x, y));
    }

    public virtual void SetMapPos(Vector2Int pos)
    {
        SetPos(new Vector2(pos.x / 100f, pos.y / 100f));
        m_MapPos = pos;
    }

    public virtual void SetPos(Vector2 pos)
    {
        m_Pos = pos;
        m_MapPos.x = Mathf.CeilToInt(m_Pos.x * 100);
        m_MapPos.y = Mathf.CeilToInt(m_Pos.y * 100);
        transform.localPosition = new Vector3(pos.x, pos.y, pos.y);
    }

    public void SetDir(float dir)
    {
        if (dir == 0) return;
        m_Dir = dir;
        if (m_Dir > 0) m_Dir = 1;
        if (m_Dir < 0) m_Dir = -1;

        float angleY = transform.localRotation.eulerAngles.y;

        if (m_Dir > 0) angleY = 0;
        else if (m_Dir < 0) angleY = 180;
        transform.localRotation = Quaternion.Euler(0, angleY, 0);
    }

    public bool IsInRange2(Vector2 pos)
    {
        return Vector2.Distance(pos, m_Pos) < 0.03f;
    }

    public bool IsInRange2(float x, float y)
    {
        return Vector2.Distance(new Vector2(x, y), m_Pos) < 0.03f;
    }

    public void SetRes(string resPath)
    {
        if (!string.IsNullOrEmpty(m_ResPath) && m_ResPath.Equals(resPath)) return;
        m_ResPath = resPath;
        GameObjectPool.Ins.Get(resPath, OnResComplete);
    }

    public virtual void AddHealth(int value)
    {
        m_Health += value;
    }

    public virtual void AddMaxHealth(int value)
    {
        m_MaxHealth += value;
    }

    public  virtual void SubHealth(int value)
    {
        m_Health -= value;
        if (m_Health < 0) m_Health = 0;
    }

    public virtual void SubMaxHealth(int value)
    {
        m_MaxHealth -= value;
        if (m_MaxHealth < 0) m_MaxHealth = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (!m_ResComplete) return;
        OnUpdate();
    }

    protected virtual void OnResComplete(GameObject go)
    {
        m_ResGO = go;
        m_ResGO.transform.SetParent(this.transform, false);
        m_ResGO.transform.localPosition = Vector3.zero;
        m_ResGO.SetActive(true);
        m_ResComplete = true;
        SetLayer(m_Layer);
    }

    protected Rect GetBound(Vector2 pos)
    {
        m_Bound.width = m_Collider.size.x;
        m_Bound.height = m_Collider.size.y;
        m_Bound.center = pos + Vector2.up * (m_Collider.offset.y + m_Collider.size.y / 2);
        m_Bound.xMin = pos.x + m_Collider.offset.x - m_Collider.size.x / 2;
        m_Bound.xMax = pos.x + m_Collider.offset.x + m_Collider.size.x / 2;
        m_Bound.yMin = pos.y + m_Collider.offset.y - m_Collider.size.y / 2;
        m_Bound.yMax = pos.y + m_Collider.offset.y + m_Collider.size.y / 2;
        return m_Bound;
    }

    protected void SetCollider(Vector2 offest, Vector2 size)
    {
        m_Collider.offset = offest;
        m_Collider.size = size;
    }

    protected bool IsOutVersionX(float posX)
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        return posX <= visionRect.xMin || posX >= visionRect.xMax;
    }

    protected bool IsOutVersionXRight(float posX)
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        return posX >= visionRect.xMax;
    }

    protected bool IsOutVersionXLeft(float posX)
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        return posX <= visionRect.xMin;
    }

    protected bool IsOutVersionY(float posY)
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        return posY <= visionRect.yMin || posY >= visionRect.yMax;
    }

    protected virtual void OnUpdate() { }

    protected bool m_ResComplete = false;
    protected float m_Dir = 1f;
    protected int m_EntityID = 0;
    protected int m_Health = 0;
    protected int m_MaxHealth = 0;
    protected string m_ResPath = string.Empty;
    protected BoxCollider2D m_Collider = null;
    protected GameObject m_ResGO;
    protected Rect m_Bound = Rect.zero;
    protected Vector2 m_Pos = Vector2.zero;
    protected Vector2Int m_MapPos = Vector2Int.zero;
    protected ObjectType m_ObjectType = ObjectType.NONE;
}
