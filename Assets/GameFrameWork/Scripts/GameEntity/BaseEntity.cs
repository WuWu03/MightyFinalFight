using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public abstract class BaseEntity
    {
        public string entityName
        {
            get
            {
                return m_EntityName;
            }
        }

        public int id
        {
            get
            {
                return m_Id;
            }
        }

        public string layer 
        {
            get 
            {
                return m_Layer;
            }
        }

        public GameObject gameObject
        {
            get
            {
                return m_GameObject;
            }
        }

        public Transform transform
        {
            get
            {
                return m_GameObject.transform;
            }
        }

        public virtual void Init(int id, string name)
        {
            m_Id = id;
            m_EntityName = name;
            m_GameObject.name = name;
        }

        public virtual void Update(float deltaTime, float unscaledDeltaTime) { }
        public virtual void LateUpdate(float deltaTime, float unscaledDeltaTime) { }
        public virtual void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime) { }

        public virtual void Release()
        {
            EntityMgr.instance.PutEntity(this);
        }

        public void BeforeDestroy()
        {
            OnBeforeDestroy();
        }

        public void SetName(string name)
        {
            m_EntityName = name;
            m_GameObject.name = name;
        }

        public void SetID(int id)
        {
            m_Id = id;
        }

        public void SetGameObject(GameObject gameObject)
        {
            m_GameObject = gameObject;
        }

        public void SetParent(Transform parent, bool worldPossitionStays = false)
        {
            m_GameObject.transform.SetParent(parent, worldPossitionStays);
            m_GameObject.transform.localPosition = Vector3.zero;
        }

        public void SetLayer(string layer, bool isChild = true)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                m_Layer = layer;
            }

            m_GameObject.SetLayer(m_Layer, isChild);
        }

        public void SetActive(bool active)
        {
            if (m_GameObject.activeSelf != active)
            {
                m_GameObject.SetActive(active);
            }
        }

        protected void SetLayer(bool isChild = true)
        {
            SetLayer(m_Layer, isChild);
        }

        protected virtual void OnBeforeDestroy() 
        {
            m_Id = 0;
            m_EntityName = string.Empty;
            m_Layer = string.Empty;
        }

        protected int m_Id = 0;
        protected string m_EntityName = string.Empty;
        protected string m_Layer = string.Empty;
        protected GameObject m_GameObject = null;
    }
}
