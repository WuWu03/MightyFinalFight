using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using FrameWork.Pool;

namespace FrameWork.GameEntity
{
    public class BaseObject : MonoBehaviour
    {
        public ObjectType ObjectType
        {
            get
            {
                return m_ObjectType;
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }
        }
        public int ID
        {
            get
            {
                return m_ID;
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

        public string Layer 
        {
            get 
            {
                return m_Layer;
            }
        }

        public bool IsInGround
        {
            get
            {
                return transform.localPosition.y <= m_Pos.y;
            }
        }

        public float Health
        {
            get { return m_Health; }
            set { m_Health = value; }
        }

        public virtual void Init(int id, string name)
        {
            m_ID = id;
            m_Name = name;
            gameObject.name = name;
            m_Pos = transform.localPosition;
        }

        public virtual void Release()
        {
            GameObjectPool.Ins.Put(m_ResPath, m_ResGO);
            SceneObjectPool.Ins.Put(this);
            m_ResPath = null;
        }

        public void SetName(string name)
        {
            m_Name = name;
            gameObject.name = name;
        }

        public void SetID(int id)
        {
            m_ID = id;
        }

        public void SetObjectType(ObjectType type)
        {
            m_ObjectType = type;
        }

        public void UpdatePos2(float x, float y)
        {
            m_Pos = new Vector2(x, y);
        }

        public void UpdatePos(Vector2 pos)
        {
            m_Pos = pos;
        }

        public void SetPos2(float x, float y)
        {
            m_Pos = new Vector2(x, y);
            transform.localPosition = new Vector3(x, y, y);
        }

        public void SetPos(Vector2Int pos)
        {
            SetPos(new Vector2((float)pos.x / 100, (float)pos.y / 100));
        }

        public void SetPos(Vector2 pos)
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

        public bool IsInRange2(float x,float y)
        {
            return Vector2.Distance(new Vector2(x, y), m_Pos) < 0.03f;
        }

        public void SetRes(string resPath)
        {
            if (!string.IsNullOrEmpty(m_ResPath) && m_ResPath.Equals(resPath)) return;
            m_ResPath = resPath;
            GameObjectPool.Ins.Get(resPath, OnResComplete);
        }

        public void SetParent(Transform parent, bool worldPossitionStays = false)
        {
            transform.SetParent(parent, worldPossitionStays);
        }

        public void SetLayer(string layer, bool isChild = true)
        {
            m_Layer = layer;
            this.gameObject.SetLayer(layer, isChild);
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnResComplete(GameObject go)
        {
            m_ResGO = go;
            m_ResGO.transform.SetParent(this.transform, false);
            m_ResGO.transform.localPosition = Vector3.zero;
            m_ResGO.SetActive(true);
            SetLayer(m_Layer);
        }

        protected int m_ID = 0;
        protected string m_Name = string.Empty;
        protected string m_Layer = "Unit";
        protected string m_ResPath = string.Empty;
        protected GameObject m_ResGO;
        protected Vector2 m_Pos = Vector2.zero;
        protected ObjectType m_ObjectType = ObjectType.NONE;
        protected float m_Dir = 1;
        protected float m_Health = 0;
    }
}
