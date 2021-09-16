using UnityEngine;
using GameFrameWork.GameEntity;
using GameFrameWork.Pool;
using GameFrameWork;
using GameFrameWork.Camera;
using System.Collections.Generic;

public class BaseSceneObject : BaseEntity
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

    public float CurrPosZ
    {
        get
        {
            return transform.localPosition.y - m_Pos.y;
        }
    }

    public float PosZ
    {
        get
        {
            return m_PosZ;
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

    public float Depth
    {
        get
        {
            return m_Depth;
        }
    }

    public int Health
    {
        get
        {
            return m_Health;
        }
    }

    public int MaxHealth
    {
        get 
        { 
            return m_MaxHealth; 
        }
    }

    public bool IsResComplete
    {
        get
        {
            return m_IsResComplete;
        }
    }

    public int EntityID
    {
        get
        {
            return m_EntityID;
        }
    }

    public List<GameObject> Targets
    {
        get
        {
            return m_ListTargets;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_Pos = transform.localPosition;
        m_Collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_Collider.isTrigger = true;
        m_Collider.enabled = false;
        m_ListTargets = new List<GameObject>();
    }

    public virtual void SetData(BaseSceneObjectData data)
    {
        m_EntityID = data.Id;
        m_Health = data.Health;
        m_MaxHealth = data.MaxHealth;
        m_Data = data;
    }

    public override void Release()
    {
        base.Release();
        m_ListTargets.Clear();
        m_ListTargets = null;
        m_IsResComplete = false;
       
        if (m_ResGO != null)
        {
            GameObjectPool.Ins.Put(m_ResPath, m_ResGO);
            EntityMgr.Ins.PutEntity(this);
            m_ResPath = null;
        }

        if (m_Data != null)
        {
            ReferencePool.Release(m_Data);
        }
    }

    public void SetObjectType(ObjectType type)
    {
        m_ObjectType = type;
    }

    public void SetDepth(float depth)
    {
        float x = transform.localPosition.x;
        float y = transform.localPosition.y;
        m_Depth = depth;
        transform.localPosition = new Vector3(x, y, depth);
    }

    public void UpdatePosX(float x)
    {
        UpdatePos(new Vector2(x, m_Pos.y));
    }

    public void UpdatePosY(float y)
    {
        UpdatePos(new Vector2(m_Pos.x, y));
    }

    public void UpdatePosZ(float z)
    {
        m_PosZ = z;
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

    public void SetPosX(float x)
    {
        SetPos(new Vector2(x, m_Pos.y));
    }

    public void SetPosY(float y)
    {
        SetPos(new Vector2(m_Pos.x, y));
    }

    public void SetPos2(float x, float y)
    {
        SetPos(new Vector2(x, y));
    }
    public void SetPosZ(float z)
    {
        m_PosZ = z;
        transform.localPosition = new Vector3(m_Pos.x, m_Pos.y + m_PosZ, m_Pos.y);
    }

    public virtual void SetPos(Vector2 pos)
    {
        m_Pos = pos;
        m_MapPos.x = Mathf.CeilToInt(m_Pos.x * 100);
        m_MapPos.y = Mathf.CeilToInt(m_Pos.y * 100);
        m_Depth = pos.y;
        transform.localPosition = new Vector3(pos.x, pos.y + m_PosZ, pos.y);
    }

    public virtual void SetMapPos(Vector2Int pos)
    {
        SetPos(new Vector2(pos.x / 100f, pos.y / 100f));
        m_MapPos = pos;
    }

    public void SetScale2(float x,float y)
    {
        SetScale(new Vector2(x, y));
    }

    public virtual void SetScale(Vector2 scale)
    {
        SetDir(scale.x);
        transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, 1);
    }

    public void SetDir(float dir)
    {
        if (dir == 0)
        {
            return;
        }

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
        GameObjectPool.Ins.Get(resPath, ResComplete);
    }

    public virtual void AddHealth(int value)
    {
         m_Health += value;
    }

    public virtual void AddMaxHealth(int value)
    {
        m_MaxHealth += value;
    }

    public virtual void SubHealth(int value)
    {
        m_Health = Mathf.Max(m_Health - value, 0);
    }

    public virtual void SubMaxHealth(int value)
    {
        m_MaxHealth = Mathf.Max(m_MaxHealth - value, 0);
    }

    public virtual void SetHealth(int value)
    {
        m_Health = Mathf.Min(m_MaxHealth, value);
    }

    public virtual void SetMaxHealth(int value)
    {
        m_MaxHealth = Mathf.Max(value, 1);
    }

    private void ResComplete(GameObject go, object[] param)
    {
        m_ResGO = go;
        m_ResGO.transform.SetParent(transform, false);
        m_ResGO.transform.localPosition = Vector3.zero;
        m_ResGO.SetActive(true);
        m_Collider.enabled = true;
        SetLayer();
        OnResComplete(go, param);
        m_IsResComplete = true;
    }

    protected override void Update()
    {
        base.Update();
        if (!m_IsResComplete)
        {
            return;
        }

        if (m_ResGO == null)
        {
            Release();
            return;
        }

        OnUpdate();
    }

    protected override void LateUpdate()
    {
        OnLateUpdate();
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_ListTargets == null || collision.gameObject.Equals(gameObject))
        {
            return;
        }

        BaseSceneObject bso = collision.gameObject.GetComponent<BaseSceneObject>();

        if (bso == null || bso.ObjectType == ObjectType.CantBreakItem || bso.ObjectType == m_ObjectType)
        {
            return;
        }

        if (!m_ListTargets.Contains(collision.gameObject))
        {
            m_ListTargets.Add(collision.gameObject);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision) { }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (m_ListTargets != null && m_ListTargets.Contains(collision.gameObject))
            m_ListTargets.Remove(collision.gameObject);
    }

    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnResComplete(GameObject go, object[] param) { }


    protected bool m_IsResComplete = false;
    protected float m_Dir = 1f;
    protected float m_Depth = 0f;
    protected float m_PosZ = 0f;
    protected int m_EntityID = 0;
    protected string m_ResPath = string.Empty;
    protected int m_Health = 0;
    protected int m_MaxHealth = 0;
    protected BoxCollider2D m_Collider = null;
    protected GameObject m_ResGO;
    protected Rect m_Bound = Rect.zero;
    protected Vector2 m_Pos = Vector2.zero;
    protected Vector2Int m_MapPos = Vector2Int.zero;
    protected ObjectType m_ObjectType = ObjectType.NONE;
    protected List<GameObject> m_ListTargets = null;

    private BaseSceneObjectData m_Data = null;
}