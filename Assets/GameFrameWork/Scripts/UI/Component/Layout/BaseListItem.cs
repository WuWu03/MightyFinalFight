using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BaseListItem
    {
        private int m_ItemIndex;
        private bool m_IsActive;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private RectTransform m_RectTransform;
        
        public virtual int id
        {
            get
            {
                return m_ItemIndex + 1;
            }
        }

        public int itemIndex
        {
            get
            {
                return m_ItemIndex;
            }
            set
            {
                m_ItemIndex = value;
            }
        }

        public bool isActive
        {
            get
            {
                return m_IsActive;
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
                return m_Transform;
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                return m_RectTransform;
            }
        }

        public void Create(GameObject go)
        {
            m_GameObject = go;
            m_Transform = go.transform;
            m_RectTransform = go.GetComponent<RectTransform>();
            m_GameObject.SetActiveSelf(m_IsActive);
            OnCreate(go);
        }

        public void Select(bool isSelected)
        {
            OnSelect(isSelected);
        }

        public void SetActiveSelf(bool isActive)
        {
            m_IsActive = isActive;

            if (m_GameObject is not null)
            {
                m_GameObject.SetActiveSelf(isActive);
            }
        }

        public void Release()
        {
            m_GameObject = null;
            m_Transform = null;
            m_RectTransform = null;
            m_ItemIndex = 0;
            m_IsActive = false;
            OnReleaseItem();
        }

        protected virtual void OnCreate(GameObject go) { }
        protected virtual void OnSelect(bool isSelected) { }
        protected virtual void OnReleaseItem() { }
    }
}