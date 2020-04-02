using FrameWork.Pool;
using FrameWork.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UI
{
    public abstract class BasePanel : Singleton<BasePanel>
    {
        private enum ResState
        {
            UNLOAD = 0,
            COMPLETE = 1,
            LOADING = 2,
        }

        protected enum CloseMode
        {
            COMMON = 0,//常用界面
            DESTROY = 1,//不常用界面
            CACHEDESTROY = 2,//缓存
        }

        public GameObject gameObject
        {
            get;
            private set;
        }

        public Transform transform
        {
            get;
            private set;
        }

        public abstract UIMgr.UILayer PanelLayer
        {
            get;
        }

        protected abstract string PanelName
        {
            get;
        }

        protected virtual CloseMode PanelCloseMode
        {
            get
            {
                return CloseMode.COMMON;
            }
        }

        public void Open(VoidNotPar callback = null)
        {
            if (!m_IsInit)
            {
                Init();
            }

            if (callback != null && !m_ListOpenCallback.Contains(callback))
            {
                m_ListOpenCallback.Add(callback);
            }

            if (m_ResState == ResState.UNLOAD)
            {
                m_ResState = ResState.LOADING;
                m_ResPath = string.Format("{0}/{1}", ResDefine.UI_PATH, PanelName);
                ResPool.Ins.Get(m_ResPath, LoadViewCallback);
            }
            else
            {
                UIMgr.Ins.StartCoroutine(ShowPanel(true));
            }
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

            UIMgr.Ins.StartCoroutine(ShowPanel(false));
        }

        private void Init()
        {
            m_ListOpenCallback = new List<VoidNotPar>();
            m_ListCloseCallback = new List<VoidNotPar>();
            m_ResState = ResState.UNLOAD;
            m_IsInit = true;
            OnInit();
        }

        private void LoadViewCallback(GameObject go)
        {
            this.gameObject = go;
            this.transform = go.transform;
            this.transform.SetParent(UIMgr.Ins.GetUILayer(PanelLayer), false);

            GameObject.DontDestroyOnLoad(this.gameObject);

            OnLoadViewCallback();
            UIMgr.Ins.StartCoroutine(ShowPanel(true));
        }

        private void Destroy()
        {
            if (PanelCloseMode == CloseMode.COMMON)
            {
                OnClose();
                return;
            }

            OnDestroy();

            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
                gameObject = null;
                transform = null;
            }

            if (PanelCloseMode == CloseMode.DESTROY)
            {
                ResMgr.Ins.UnloadAssetBundle(m_ResPath, true);
                m_ResPath = null;
                m_IsInit = false;
                m_Instance = null;
            }

            m_ResState = ResState.UNLOAD;
        }

        private IEnumerator ShowPanel(bool isShow)
        {
            yield return null;

            if (gameObject == null || m_IsShow == isShow)
            {
                yield break;
            }

            m_IsShow = isShow;
            gameObject.SetActive(isShow);

            if (m_IsShow)
            {
                PlayOpenAnim();
            }
            else
            {
                PlayCloseAnim();
            }
        }

        protected virtual void PlayOpenAnim()
        {
            for (int i = 0; i < m_ListOpenCallback.Count; i++)
            {
                m_ListOpenCallback[i]?.Invoke();
            }
            m_ListOpenCallback.Clear();
            OnAfterOpenHandle();
            UIMgr.Ins.AddPanel(this);
        }

        protected virtual void PlayCloseAnim()
        {
            for (int i = 0; i < m_ListCloseCallback.Count; i++)
            {
                m_ListCloseCallback[i]?.Invoke();
            }
            m_ListCloseCallback.Clear();
            OnBeforeCloseHandle();
            Destroy();
            UIMgr.Ins.RemovePanel(this);
        }

        protected abstract void OnInit();
        protected abstract void OnLoadViewCallback();
        protected abstract void OnAfterOpenHandle();
        protected abstract void OnBeforeCloseHandle();
        protected abstract void OnUpdate();
        protected abstract void OnClose();
        protected abstract void OnDestroy();

        private bool m_IsShow = false;
        private bool m_IsInit = false;
        private string m_ResPath = string.Empty;
        private List<VoidNotPar> m_ListOpenCallback = null;
        private List<VoidNotPar> m_ListCloseCallback = null;
        private ResState m_ResState = ResState.UNLOAD;
    }
}