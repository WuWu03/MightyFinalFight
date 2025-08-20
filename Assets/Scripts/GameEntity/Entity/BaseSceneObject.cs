using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Pool;
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

    public GameObject asset
    {
        get
        {
            return m_Asset;
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

    public int mapPosZ
    {
        get
        {
            return m_MapPosZ;
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

    public bool isAssetLoadComplete
    {
        get
        {
            return m_IsAssetLoadComplete;
        }
    }

    public int entityId
    {
        get
        {
            return m_EntityId;
        }
    }

    public event GameFrameWorkAction<int> onReleaseEvent
    {
        add
        {
            m_OnReleaseEventHandler += value;
        }
        remove
        {
            m_OnReleaseEventHandler -= value;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_IsAssetLoadComplete = false;
        m_Pos = transform.localPosition;
    }

    protected override void OnRelease()
    {
        m_OnReleaseEventHandler?.Invoke(m_EntityId);

        if (m_Asset != null)
        {
            GameObjectPoolMgr.instance.Put(m_AssetPath, m_Asset);
        }

        m_Data?.Release();
        m_EntityAttribute?.Release();

        m_IsAssetLoadComplete = false;
        m_OnReleaseEventHandler = null;
        m_AssetPath = null;
        m_Data = null;
        m_EntityAttribute = null;
        m_Asset = null;
    }

    public virtual void SetData(BaseSceneObjectData data)
    {
        m_EntityId = data.entityId;
        m_Data = data;
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
        if (caculateZ)
        {
            pos.y += posZ;
            posZ = 0;
        }

        UpdatePos(pos, posZ);
        m_Depth = pos.y;

        transform.localPosition = new Vector3(pos.x, pos.y, pos.y);
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
        UpdatePos(new Vector2(x, m_Pos.y), m_PosZ);
    }

    public void UpdatePosY(float y)
    {
        UpdatePos(new Vector2(m_Pos.x, y), m_PosZ);
    }

    public void UpdatePosZ(float z)
    {
        UpdatePos(new Vector2(m_Pos.x, m_Pos.y), z);
    }

    public void UpdatePosXY(float x, float y)
    {
        UpdatePos(new Vector2(x, y), m_PosZ);
    }

    public void UpdatePosXYZ(float x, float y, float z)
    {
        UpdatePos(new Vector2(x, y), z);
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

    public void SetAsset(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }

        if (m_AssetPath == assetPath)
        {
            return;
        }

        m_AssetPath = assetPath;
        m_IsAssetLoadComplete = false;
        GameObjectPoolMgr.instance.GetFromAsset(assetPath, LoadAssetComplete);
    }

    public bool IsOutVersionX(float posX)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posX <= visionRect.xMin || posX >= visionRect.xMax;
    }

    public bool IsOutVersionXRight(float posX)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posX >= visionRect.xMax;
    }

    public bool IsOutVersionXLeft(float posX)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posX <= visionRect.xMin;
    }

    public bool IsOutVersionY(float posY)
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        return posY <= visionRect.yMin || posY >= visionRect.yMax;
    }

    protected override void Update()
    {
        base.Update();

        if (!m_IsAssetLoadComplete)
        {
            return;
        }

        OnUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (!m_IsAssetLoadComplete)
        {
            return;
        }

        OnLateUpdate();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!m_IsAssetLoadComplete)
        {
            return;
        }

        OnFixedUpdate();
    }

    private void LoadAssetComplete(string assetPath, UnityEngine.Object obj, object arg)
    {
        if (obj == null)
        {
            Release();
            return;
        }

        m_Asset = obj as GameObject;
        m_Asset.transform.SetParent(transform, false);
        m_Asset.transform.localPosition = Vector3.zero;
        m_Asset.SetActiveSelf(true);
        SetLayer();
        OnLoadAssetComplete(m_Asset, arg);
        m_IsAssetLoadComplete = true;
    }

    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnLoadAssetComplete(GameObject go, object arg) { }
    protected virtual void OnTriggerEnter2D(Collider2D collision) { }
    protected virtual void OnTriggerStay2D(Collider2D collision) { }
    protected virtual void OnTriggerExit2D(Collider2D collision) { }

    private bool m_IsAssetLoadComplete = false;
    private float m_Dir = 1f;
    private float m_Depth = 0f;
    private float m_PosZ = 0f;
    private int m_MapPosZ = 0;
    private int m_EntityId = 0;
    private Vector2 m_Pos = Vector2.zero;
    private Vector2Int m_MapPos = Vector2Int.zero;
    private ObjectType m_ObjectType = ObjectType.NONE;
    private GameObject m_Asset;
    private EntityAttribute m_EntityAttribute = null;
    private GameFrameWorkAction<int> m_OnReleaseEventHandler = null;
    private IReference m_Data = null;
    private string m_AssetPath = string.Empty;
}