using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public abstract class BaseEntity : MonoBehaviour
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
            SetID(id);
            SetName(name);
        }

        public void SetID(int id)
        {
            m_Id = id;
        }

        public void SetName(string name)
        {
            m_EntityName = name;
            gameObject.name = name;
        }

        public void SetParent(Transform parent, bool worldPossitionStays = false)
        {
            gameObject.transform.SetParent(parent, worldPossitionStays);
            gameObject.transform.localPosition = Vector3.zero;
        }

        public void SetLayer(string layer, bool isChild = true)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                m_Layer = layer;
            }

            gameObject.SetLayer(m_Layer, isChild);
        }

        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
        protected virtual void FixedUpdate() { }

        public void Release()
        {
            m_Id = -1;
            m_EntityName = string.Empty;
            m_Layer = string.Empty;
            gameObject.SetActiveSelf(false);
            OnRelease();
            EntityMgr.instance.PutEntity(this);
        }

        protected virtual void OnRelease() { }

        protected void SetLayer(bool isChild = true)
        {
            SetLayer(m_Layer, isChild);
        }

        private int m_Id = 0;
        private string m_EntityName = string.Empty;
        private string m_Layer = string.Empty;
    }
}
