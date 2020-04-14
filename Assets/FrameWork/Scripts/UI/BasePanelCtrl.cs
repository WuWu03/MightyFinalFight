using FrameWork.Pool;
using FrameWork.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public abstract class BasePanelCtrl
    {
        public BasePanel Panel { get; private set; }
        
        private enum ResState
        {
            UNLOAD = 0,
            COMPLETE = 1,
            LOADING = 2,
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
                string resPath = string.Format("{0}/{1}", ResDefine.UI_PATH, Panel.PanelName);
                ResPool.Ins.Get(resPath, LoadViewCallback);
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
            m_ResState = ResState.UNLOAD;
            m_IsInit = true;
            Panel = GetPanel();
            OnInit(param);
        }

        private void LoadViewCallback(GameObject go, string resPath)
        {
            Panel.Init(go, this, resPath);
            ShowPanel(true);
        }

        private void Destroy()
        {
            if (Panel.PanelCloseMode == UIMgr.CloseMode.Always) return;

            if (Panel.PanelCloseMode == UIMgr.CloseMode.Destroy)
            {
                OnDestroy();

                if (Panel == null)
                {
                    Debug.LogError("Panel is null!");            
                }

                GameObject.Destroy(Panel.gameObject);
                ResMgr.Ins.UnloadAssetBundle(Panel.ResPath, true);
                Panel = null;
                m_IsInit = false;
            }

            m_ResState = ResState.UNLOAD;
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
                Destroy();
            }
        }

        protected virtual void PlayOpenAnim(){ }
        protected virtual void PlayCloseAnim() { }
        protected abstract void OnInit(object[] param);
        protected abstract void OnOpen();
        protected abstract void OnUpdate();
        protected abstract void OnClose();
        protected abstract void OnDestroy();
        protected abstract BasePanel GetPanel();

        private bool m_IsShow = false;
        private bool m_IsInit = false;
        private List<VoidNotPar> m_ListOpenCallback = null;
        private List<VoidNotPar> m_ListCloseCallback = null;
        private ResState m_ResState = ResState.UNLOAD;
    }
}