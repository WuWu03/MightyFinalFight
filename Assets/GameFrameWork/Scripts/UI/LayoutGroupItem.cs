using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class LayoutGroupViewItem
    {
        public int Index 
        {
            get
            {
                return m_Index + 1;
            }
        }

        public int RealIndex
        {
            get
            {
                return m_Index;
            }
        }

        public virtual MyButton SelectButton
        {
            get
            {
                return m_MyButton;
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
        public void Create(GameObject go,int index)
        {
            m_GameObject = go;
            m_Transform = go.transform;
            m_RectTransform = go.GetComponent<RectTransform>();
            m_Index = index;
            OnCreate(go);
        }

        public void UpdateIndex(int index)
        {
            m_Index = index;
        }

        protected abstract void OnCreate(GameObject go);

        private int m_Index = 0;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private RectTransform m_RectTransform;

        protected MyButton m_MyButton = null;
    }
}