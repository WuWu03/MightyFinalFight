using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BaseListItem
    {
        private int m_Index;
        private bool m_IsActive;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private RectTransform m_RectTransform;

        public int id
        {
            get { return m_Index + 1; }
        }

        public int index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public bool isActive
        {
            get { return m_IsActive; }
        }

        public GameObject gameObject
        {
            get { return m_GameObject; }
        }

        public Transform transform
        {
            get { return m_Transform; }
        }

        public RectTransform rectTransform
        {
            get { return m_RectTransform; }
        }

        public void Create(GameObject go)
        {
            if (go is null)
            {
                throw new GameFrameWorkException("列表格子为空");
            }
            
            m_GameObject = go;
            m_Transform = go.transform;
            m_RectTransform = go.GetComponent<RectTransform>();
            m_IsActive = go.activeSelf;
            SetActive(true);
            OnCreate(go);
        }

        public void Select(bool isSelected)
        {
            OnSelect(isSelected);
        }

        public void SetActive(bool isActive)
        {
            m_IsActive = isActive;
            m_GameObject.SetActiveSelf(isActive);
        }
        
        protected virtual void OnCreate(GameObject go)
        {
        }

        protected virtual void OnSelect(bool isSelected)
        {
        }
    }
}