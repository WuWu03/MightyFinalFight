using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class LayoutGroupViewItem
    {
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

        public ButtonEx selectButton
        {
            get
            {
                return m_SelectButton;
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

        public void SetActiveSelf(bool isAcitve)
        {
            m_IsActive = isAcitve;

            if (m_GameObject != null)
            {
                m_GameObject.SetActiveSelf(isAcitve);
            }
        }

        public virtual void ReleaseItem()
        {
            m_GameObject = null;
            m_Transform = null;
            m_RectTransform = null;
            m_ItemIndex = 0;
            m_IsActive = false;

            if (m_SelectButton != null)
            {
                m_SelectButton.onClick.RemoveAllListeners();
                m_SelectButton = null;
            }
        }

        protected abstract void OnCreate(GameObject go);

        private int m_ItemIndex = 0;
        private bool m_IsActive = false;

        private GameObject m_GameObject;
        private Transform m_Transform;
        private RectTransform m_RectTransform;
        protected ButtonEx m_SelectButton = null;
    }
}