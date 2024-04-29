using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public class BaseEntity : MonoBehaviour
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

        public virtual void Init(int id, string name)
        {
            m_Id = id;
            m_EntityName = name;
            gameObject.name = name;
        }

        public virtual void Release() 
        {
            EntityMgr.instance.PutEntity(this);
        }

        public void SetName(string name)
        {
            m_EntityName = name;
            gameObject.name = name;
        }

        public void SetID(int id)
        {
            m_Id = id;
        }

        public void SetParent(Transform parent, bool worldPossitionStays = false)
        {
            transform.SetParent(parent, worldPossitionStays);
        }

        public void SetLayer(string layer, bool isChild = true)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                m_Layer = layer;
            }
       
            gameObject.SetLayer(m_Layer, isChild);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void BeforeDestroy()
        {
            OnBeforeDestroy();
        }

        protected void SetLayer(bool isChild = true)
        {
            SetLayer(m_Layer, isChild);
        }

        protected virtual void Awake() { }
        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
        protected virtual void FixedUpdate() { }
        protected virtual void OnBeforeDestroy() 
        {
            m_EntityName = string.Empty;
            m_Layer = string.Empty;
        }
       
        private int m_Id = 0;
        private string m_EntityName = string.Empty;
        private string m_Layer = string.Empty;   
    }
}
