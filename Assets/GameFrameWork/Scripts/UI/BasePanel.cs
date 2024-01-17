using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BasePanel
    {
        public abstract string panelName { get; }

        public abstract float panelUnLoadTime { get; }

        public abstract UIMgr.Type panelType { get; }

        public abstract UIMgr.Layer panelLayer { get; }

        public abstract UIMgr.CloseMode panelCloseMode { get; }

        public GameObject gameObject { get; private set; }

        public Transform transform { get; private set; }

        public string resPath { get; private set; }

        public bool isOpen
        {
            get
            {
                return m_IsOpen;
            }
        }

        public bool isInit
        {
            get
            {
                return m_IsInit;
            }
        }

        public bool isDelayTimeOut
        {
            get
            {
                return panelCloseMode == UIMgr.CloseMode.DelayDestroy && m_DelayTime > 0f && Time.time - m_DelayTime >= 5f;
            }
        }

        protected UIRefRoot m_UIRefRoot { get; private set; }

        public void Init(GameObject go, string resPath, object[] param)
        {
            gameObject = go;
            transform = go.transform;
            m_UIRefRoot = go.GetComponent<UIRefRoot>();
            this.resPath = resPath;

            if (m_UIRefRoot == null)
            {
                Log.LogError("UIRefRoot is null!");
                return;
            }

            transform.SetParent(UIMgr.instance.GetUILayer(panelLayer), false);
            OnInit(param);
            m_IsInit = true;
            Open();

            if (m_IsHide)
            {
                gameObject.SetActive(false);
            }
        }
        
        public void Open()
        {
            m_IsOpen = true;
            gameObject.SetActive(true);
            m_DelayTime = 0;
            OnOpen();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Close()
        {
            m_IsOpen = false;
            gameObject.SetActive(false);
            m_DelayTime = Time.unscaledTime;
            OnClose();
        }

        public void Show()
        {
            if(!m_IsHide)
            {
                return;
            }

            m_IsHide = false;

            if (gameObject != null)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (m_IsHide)
            {
                return;
            }

            m_IsHide = true;

            if(gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }

        public T GetComponent<T>()
        {
            if (gameObject != null)
            {
                return gameObject.GetComponent<T>();
            }

            return default;
        }

        public void Destroy()
        {
            m_IsInit = false;
            m_DelayTime = 0f;
            OnDestroy();
        }

        protected abstract void OnInit(object[] param);
        protected abstract void OnOpen();
        protected abstract void OnUpdate();
        protected abstract void OnClose();
        protected abstract void OnDestroy();

        protected void InnerClose()
        {
            UIMgr.instance.Close(panelName);
        }

        private bool m_IsOpen = false;
        private bool m_IsInit = false;
        private bool m_IsHide = false;
        private float m_DelayTime = 0f;
    }
}