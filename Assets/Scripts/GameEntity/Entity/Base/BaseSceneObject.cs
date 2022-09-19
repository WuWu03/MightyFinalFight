using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Resources;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneObject : BaseEntity
{
    public ObjectType objectType
    {
        get
        {
            return m_ObjectType;
        }
    }

    public GameObject resGO
    {
        get
        {
            return m_ResGO;
        }
    }

    public BoxCollider2D boxCollider2D
    {
        get
        {
            return m_BoxCollider2D;
        }
    }

    public Vector2 pos
    {
        get
        {
            return m_Pos;
        }
    }

    public float posZ
    {
        get
        {
            return m_PosZ;
        }
    }

    public float currPosZ
    {
        get
        {
            return transform.localPosition.y - m_Pos.y;
        }
    }

    public Vector2Int mapPos
    {
        get
        {
            return m_MapPos;
        }
    }

    public Rect bound
    {
        get
        {
            return GetBound(m_Pos);
        }
    }

    public float dir//物体朝向 1右 -1左 不能为0
    {
        get
        {
            return m_Dir;
        }
    }

    public float depth
    {
        get
        {
            return m_Depth;
        }
    }

    public EntityAttribute entityAttribute
    {
        get
        {
            return m_EntityAttribute;
        }
    }

    public bool isResComplete
    {
        get
        {
            return m_IsResComplete;
        }
    }

    public int entityId
    {
        get
        {
            return m_EntityId;
        }
    }

    public List<GameObject> targets
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
        m_BoxCollider2D = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_BoxCollider2D.isTrigger = true;
        m_BoxCollider2D.enabled = false;
        m_ListTargets = new List<GameObject>();
    }

    public override void Release()
    {
        base.Release();
        m_ListTargets.Clear();
        m_ListTargets = null;
        m_IsResComplete = false;

        if (m_ResGO != null)
        {
            GameObjectPool.instance.Put(m_ResPath, m_ResGO);
            EntityMgr.instance.PutEntity(this);
            m_ResPath = null;
        }

        if (m_EntityAttribute != null)
        {
            ReferencePool.Release(m_EntityAttribute);
        }
    }

    public virtual void SetData(BaseSceneObjectData data)
    {
        m_EntityId = data.entityId;
    }

    public void SetAttribute(EntityAttribute attribute)
    {
        m_EntityAttribute = attribute;
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

    public void SetPosX(float x)
    {
        SetPos(new Vector3(x, m_Pos.y), m_PosZ);
    }

    public void SetPosY(float y, bool caculateZ = false)
    {
        SetPos(new Vector3(m_Pos.x, y), m_PosZ, caculateZ);
    }

    public void SetPosZ(float z)
    {
        SetPos(new Vector3(m_Pos.x, m_Pos.y), z, true);
    }

    public void SetPosXY(float x, float y, bool caculateZ = false)
    {
        SetPos(new Vector3(x, y), m_PosZ, caculateZ);
    }

    public void SetPosXYZ(float x, float y, float z)
    {
        SetPos(new Vector3(x, y), z, true);
    }

    public void SetPos2(Vector2 pos, bool caculateZ = false)
    {
        SetPos(pos, m_PosZ, caculateZ);
    }

    public virtual void SetPos(Vector2 pos, float posZ, bool caculateZ = false)
    {
        UpdatePos(pos, posZ);
        m_Depth = pos.y;
        transform.localPosition = new Vector3(pos.x, caculateZ ? pos.y + m_PosZ : pos.y, pos.y);
    }

    public virtual void SetMapPos(Vector2Int pos, int z = 0)
    {
        SetPos(new Vector3(pos.x / 100f, pos.y / 100f), z / 100f, true);
    }

    public void SetScale2(float x, float y)
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

        m_Dir = dir > 0 ? 1 : -1;
        transform.localRotation = Quaternion.Euler(0, dir > 0 ? 0f : 180f, 0);
    }

    public void UpdatePosX(float x)
    {
        UpdatePos(new Vector3(x, m_Pos.y), m_PosZ);
    }

    public void UpdatePosY(float y)
    {
        UpdatePos(new Vector3(m_Pos.x, y), m_PosZ);
    }

    public void UpdatePosZ(float z)
    {
        UpdatePos(new Vector3(m_Pos.x, m_Pos.y), z);
    }

    public void UpdatePosXY(float x, float y)
    {
        UpdatePos(new Vector3(x, y), m_PosZ);
    }

    public void UpdatePosXYZ(float x, float y, float z)
    {
        UpdatePos(new Vector3(x, y), z);
    }

    public void UpdatePos2(Vector2 pos)
    {
        UpdatePos(pos, m_PosZ);
    }

    public virtual void UpdatePos(Vector2 pos, float posZ)
    {
        m_Pos.x = pos.x;
        m_Pos.y = pos.y;
        m_PosZ = posZ;
        m_MapPos.x = Mathf.CeilToInt(m_Pos.x * 100);
        m_MapPos.y = Mathf.CeilToInt(m_Pos.y * 100);
        m_MapPosZ = Mathf.CeilToInt(m_PosZ * 100);
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
        if (!string.IsNullOrEmpty(m_ResPath) && m_ResPath.Equals(resPath))
        {
            return;
        }

        m_ResPath = resPath;
        GameObjectPool.instance.Get(resPath, ResComplete);
    }

    private void ResComplete(GameObject go, object[] param)
    {
        m_ResGO = go;
        m_ResGO.transform.SetParent(transform, false);
        m_ResGO.transform.localPosition = Vector3.zero;
        m_ResGO.SetActive(true);
        m_BoxCollider2D.enabled = true;
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
        m_Bound.width = m_BoxCollider2D.size.x;
        m_Bound.height = m_BoxCollider2D.size.y;
        m_Bound.center = pos + Vector2.up * (m_BoxCollider2D.offset.y + m_BoxCollider2D.size.y / 2);
        m_Bound.xMin = pos.x + m_BoxCollider2D.offset.x - m_BoxCollider2D.size.x / 2;
        m_Bound.xMax = pos.x + m_BoxCollider2D.offset.x + m_BoxCollider2D.size.x / 2;
        m_Bound.yMin = pos.y + m_BoxCollider2D.offset.y - m_BoxCollider2D.size.y / 2;
        m_Bound.yMax = pos.y + m_BoxCollider2D.offset.y + m_BoxCollider2D.size.y / 2;
        return m_Bound;
    }

    protected void SetCollider(Vector2 offest, Vector2 size)
    {
        m_BoxCollider2D.offset = offest;
        m_BoxCollider2D.size = size;
    }

    protected bool IsOutVersionX(float posX)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posX <= visionRect.xMin || posX >= visionRect.xMax;
    }

    protected bool IsOutVersionXRight(float posX)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posX >= visionRect.xMax;
    }

    protected bool IsOutVersionXLeft(float posX)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posX <= visionRect.xMin;
    }

    protected bool IsOutVersionY(float posY)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posY <= visionRect.yMin || posY >= visionRect.yMax;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_ListTargets == null || collision.gameObject.Equals(gameObject))
        {
            return;
        }

        BaseSceneObject bso = collision.gameObject.GetComponent<BaseSceneObject>();

        if (bso == null || bso.objectType == ObjectType.CantBreakItem || bso.objectType == m_ObjectType)
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
        {
            m_ListTargets.Remove(collision.gameObject);
        }
    }

    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnResComplete(GameObject go, object[] param) { }

    protected bool m_IsResComplete = false;
    protected float m_Dir = 1f;
    protected float m_Depth = 0f;
    protected float m_PosZ = 0f;
    protected int m_MapPosZ = 0;
    protected int m_EntityId = 0;
    protected string m_ResPath = string.Empty;
    protected BoxCollider2D m_BoxCollider2D = null;
    protected GameObject m_ResGO;
    protected Rect m_Bound = Rect.zero;
    protected Vector2 m_Pos = Vector2.zero;
    protected Vector2Int m_MapPos = Vector2Int.zero;
    protected ObjectType m_ObjectType = ObjectType.NONE;
    protected List<GameObject> m_ListTargets = null;
    protected EntityAttribute m_EntityAttribute = null;
}