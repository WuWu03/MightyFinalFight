using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BasePanel
    {
        public abstract string PanelName { get; }

        public abstract float PanelUnLoadTime { get; }

        public abstract UIMgr.Type PanelType { get; }

        public abstract UIMgr.Layer PanelLayer { get; }

        public abstract UIMgr.CloseMode PanelCloseMode { get; }

        public GameObject gameObject { get; private set; }

        public Transform transform { get; private set; }

        public string ResPath { get; private set; }

        public bool IsOpen
        {
            get
            {
                return m_IsOpen;
            }
        }

        public bool IsInit
        {
            get
            {
                return m_IsInit;
            }
        }

        public bool IsDelayTimeOut
        {
            get
            {
                return PanelCloseMode == UIMgr.CloseMode.DelayDestroy &&
                       m_DelayTime > 0f && Time.time - m_DelayTime >= 5f;
            }
        }

        protected UIRefRoot UIRefRoot { get; private set; }

        public void Init(GameObject go, object[] param)
        {
            gameObject = go;
            transform = go.transform;
            UIRefRoot = go.GetComponent<UIRefRoot>();
            ResPath = UITools.GetUIResPath(PanelName);

            if (UIRefRoot == null)
            {
                Log.Debugger.LogError("UIRefRoot is null!");
                return;
            }

            transform.SetParent(UIMgr.Ins.GetUILayer(PanelLayer), false);
            OnInit(param);
            m_IsInit = true;
            Open();
        }
        
        public void Open()
        {
            m_IsOpen = true;
            gameObject.SetActive(true);
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
            UIMgr.Ins.Close(PanelName);
        }

        private bool m_IsOpen = false;
        private bool m_IsInit = false;
        private float m_DelayTime = 0f;
    }
}