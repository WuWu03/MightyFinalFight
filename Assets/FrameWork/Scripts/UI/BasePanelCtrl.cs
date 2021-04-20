using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public abstract class BasePanelCtrl
    {
        public BasePanel Panel { get; private set; }
        
        private enum ResState
        {
            UNLOAD = 0,
            LOADED = 1,
            LOADING = 2,
        }

        public bool IsOpen 
        { 
            get 
            {
                return m_IsInit && m_IsShow;
            } 
        }

        public bool IsDelayTimeOut
        {
            get
            {
                return Panel != null && Panel.PanelCloseMode == UIMgr.CloseMode.DelayDestroy &&
                       m_DelayTime > 0f && Time.time - m_DelayTime >= 5f;
            }
        }

        public void Open(VoidNotPar callback = null,object[] param = null)
        {
            if (!m_IsInit)
            {
                Init(param);
            }

            if (callback != null && !m_ListOpenCallback.Contains(callback))
            {
                m_ListOpenCallback.Add(callback);
            }

            if (m_ResState == ResState.UNLOAD)
            {
                m_ResState = ResState.LOADING;
                //UITools.LoadUI(Panel.PanelName, LoadViewCallback);
            }
            else
            {
                ShowPanel(true);
            }
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Close(VoidNotPar callback)
        {
            if (!m_IsInit)
            {
                return;
            }

            if (callback != null && !m_ListCloseCallback.Contains(callback))
            {
                m_ListCloseCallback.Add(callback);
            }

            ShowPanel(false);
        }

        protected void InnerClose()
        {
            UIMgr.Ins.Close(Panel.PanelName);
        }

        private void Init(object[] param)
        {
            if (Panel != null)
            {
                Debug.LogError("Panel is already init!");
                return;
            }

            m_ListOpenCallback = new List<VoidNotPar>();
            m_ListCloseCallback = new List<VoidNotPar>();

            m_IsInit = true;
            m_ResState = ResState.UNLOAD;
            Panel = GetPanel();
            OnInit(param);
        }

        private void LoadViewCallback(GameObject go)
        {
            m_ResState = ResState.LOADED;     
          //  Panel.Init(go, this);
            OnLoaded();
            ShowPanel(true);
        }

        private void ShowPanel(bool isShow)
        {
            if (Panel == null || m_IsShow == isShow)
            {
                return;
            }

            m_IsShow = isShow;
            Panel.gameObject.SetActive(isShow);

            if (m_IsShow)
            {
                for (int i = 0; i < m_ListOpenCallback.Count; i++)
                {
                    m_ListOpenCallback[i]?.Invoke();
                }
                m_ListOpenCallback.Clear();
                PlayOpenAnim();
                OnOpen();
            }
            else
            {
                for (int i = 0; i < m_ListCloseCallback.Count; i++)
                {
                    m_ListCloseCallback[i]?.Invoke();
                }
                m_ListCloseCallback.Clear();
                PlayCloseAnim();
                OnClose();
                Destroy(false);
            }
        }

        public void Destroy(bool isForce)
        {   
            if (Panel.PanelCloseMode == UIMgr.CloseMode.Always) return;

            if(!isForce && Panel.PanelCloseMode == UIMgr.CloseMode.DelayDestroy)
            {
                m_DelayTime = Time.time;
                return;
            }

            if (isForce || Panel.PanelCloseMode == UIMgr.CloseMode.Destroy)
            {
                OnDestroy();

                if (Panel == null)
                {
                    Debug.LogError("Panel is null!");
                }

                GameObject.Destroy(Panel.gameObject);
                ResMgr.Ins.UnloadAssetBundle(Panel.ResPath, true);
                Panel = null;
                m_DelayTime = 0f;
                m_IsInit = false;
                m_ResState = ResState.UNLOAD;
            }
        }

        protected virtual void PlayOpenAnim(){ }
        protected virtual void PlayCloseAnim() { }
        protected abstract void OnInit(object[] param);
        protected abstract void OnLoaded();
        protected abstract void OnOpen();
        protected abstract void OnUpdate();
        protected abstract void OnClose();
        protected abstract void OnDestroy();
        protected abstract BasePanel GetPanel();

        private float m_DelayTime = 0f;
        private bool m_IsShow = false;
        private bool m_IsInit = false;
        private List<VoidNotPar> m_ListOpenCallback = null;
        private List<VoidNotPar> m_ListCloseCallback = null;
        private ResState m_ResState = ResState.UNLOAD;
    }
}