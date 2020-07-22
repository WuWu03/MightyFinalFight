using UnityEngine;
using FrameWork.GameEntity;
using FrameWork.Pool;

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

    public Vector2 Pos
    {
        get
        {
            return m_Pos;
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
    }

    public void SetPos2(float x, float y)
    {
        SetPos(new Vector2(x, y));
    }

    public void SetMapPos(Vector2Int pos)
    {
        SetPos(new Vector2(pos.x / 100f, pos.y / 100f));
    }

    public virtual void SetPos(Vector2 pos)
    {
        m_Pos = pos;
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

    protected virtual void OnResComplete(GameObject go)
    {
        m_ResGO = go;
        m_ResGO.transform.SetParent(this.transform, false);
        m_ResGO.transform.localPosition = Vector3.zero;
        m_ResGO.SetActive(true);
        m_ResComplete = true;
        SetLayer(m_Layer);
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

    protected bool m_ResComplete = false;
    protected int m_Health = 0;
    protected int m_MaxHealth = 0;
    protected string m_ResPath = string.Empty;
    protected GameObject m_ResGO;
    protected Vector2 m_Pos = Vector2.zero;
    protected ObjectType m_ObjectType = ObjectType.NONE;
    protected float m_Dir = 1;
    protected int m_EntityID = 0;
}
