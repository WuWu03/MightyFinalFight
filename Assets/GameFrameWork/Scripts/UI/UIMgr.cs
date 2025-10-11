using GameFrameWork.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public class UIMgr : BaseMgr<UIMgr>
    {
        private Canvas m_UICanvas = null;
        public Canvas uiCanvas
        {
            get
            {
                return m_UICanvas;
            }
        }
        
        private UnityEngine.Camera m_UICamera = null;
        public UnityEngine.Camera uiCamera
        {
            get
            {
                return m_UICamera;
            }
        }

        private bool m_CanPopPanel = false;
        private IView m_CurrPopView = null;
        private List<IView> m_DelayDestroyUIs = null;
        private List<IView> m_AlwaysUIs = null;
        private List<IView> m_PopViews = null;
        private List<IView> m_OpenViews = null;
        private RectTransform[] m_UILayers = null;
        private GameObject m_UIRoot = null;
        
        protected override void OnAwake()
        {
            m_OpenViews = new();
            m_AlwaysUIs = new();
            m_DelayDestroyUIs = new();
            m_PopViews = new();

            m_UIRoot = GameObject.Find("UIRoot");
            m_UICanvas = m_UIRoot.transform.Find("UICanvas").GetOrAddComponent<Canvas>();
            m_UICamera = m_UIRoot.transform.Find("UICamera").GetOrAddComponent<UnityEngine.Camera>();
            
            m_UILayers = new RectTransform[(int)UILayer.Load + 1];

            for (int i = 0; i < m_UILayers.Length; i++)
            {
                m_UILayers[i] = m_UICanvas.transform.GetChild(i).GetComponent<RectTransform>();
            }

            DontDestroyOnLoad(m_UIRoot);
        }

        protected override void OnUpdate()
        {
            if (m_DelayDestroyUIs.Count > 0)
            {
                for (int i = m_DelayDestroyUIs.Count - 1; i >= 0; i++)
                {
                    IView view = m_DelayDestroyUIs[i];
                    bool isDelayTimeOut = false;

                    if (view.settings.destroyMode == UIDestroyMode.Delay && view.delayTime > 0f)
                    {
                        isDelayTimeOut = Time.unscaledTime - view.delayTime >= view.settings.delayDestroyTime;
                    }

                    if (!isDelayTimeOut)
                    {
                        continue;
                    }

                    GameObjectPoolMgr.instance.Put(view.assetPath, view.gameObject, true);
                    view.Destroy();
                    m_DelayDestroyUIs.Remove(view);
                    m_PopViews.Remove(view);

                    if (m_CurrPopView == view)
                    {
                        m_CurrPopView = null;
                    }
                }
            }

            if (m_AlwaysUIs.Count > 1)
            {
                IView view = m_AlwaysUIs[0];
                GameObjectPoolMgr.instance.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_AlwaysUIs.Remove(view);
                m_PopViews.Remove(view);

                if (m_CurrPopView == view)
                {
                    m_CurrPopView = null;
                }
            }

            for (int i = 0; i < m_OpenViews.Count; i++)
            {
                IView view = m_OpenViews[i];
                if (view.isOpen)
                {
                    view.Update();
                }
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach (var panel in m_PopViews)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_OpenViews)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_DelayDestroyUIs)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_AlwaysUIs)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            m_DelayDestroyUIs.Clear();
            m_AlwaysUIs.Clear();
            m_PopViews.Clear();
            m_OpenViews.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_CanPopPanel = false;
            m_CurrPopView = null;
            m_DelayDestroyUIs = null;
            m_AlwaysUIs = null;
            m_PopViews = null;
            m_OpenViews = null;
        }

        public RectTransform GetPanelLayer(UILayer layer)
        {
            return m_UILayers[Convert.ToInt32(layer)];
        }

        public IView Open(string viewName, object arg = null)
        {
            Type viewType = GetViewType(viewName);
            return Open(viewType, arg);
        }

        public T Open<T>(object arg = null) where T : class, IView, new()
        {
            return Open(typeof(T), arg) as T;
        }
        
        public IView Open(Type panelType, object arg = null)
        {
            if (panelType == null)
            {
                return null;
            }

            IView view = Get(panelType);
            
            if (view != null)
            {
                view.Open(arg);
                return view;
            }
            
            view = Activator.CreateInstance(panelType) as IView;
            
            if (view == null)
            {
                return null;
            }
            
            m_OpenViews.Add(view);
            m_AlwaysUIs.Remove(view);
            m_DelayDestroyUIs.Remove(view);
            
            if (!m_CanPopPanel && view != null && view.settings.canPopUp)
            {
                m_CanPopPanel = true;
            }

            if (m_CanPopPanel && view != null && view.settings.canPopUp)
            {
                if (m_CurrPopView != null && m_CurrPopView != view)
                {
                    m_PopViews.Add(m_CurrPopView);
                    Close(m_CurrPopView, false, false);
                }

                m_CurrPopView = view;
            }

            view.Open(arg);
            return view;
        }
        
        public IView Get(string viewName)
        {
            System.Type viewType = GetViewType(viewName);
            return Get(viewType);
        }

        public T Get<T>() where T : class, IView, new()
        {
            return Get(typeof(T)) as T;
        }
        
        public IView Get(Type viewType)
        {
            if (viewType == null)
            {
                return null;
            }

            foreach (IView view in m_OpenViews)
            {
                if (view.GetType() == viewType)
                {
                    return view;
                }
            }

            return null;
        }
        
        public bool IsOpen(string viewName)
        {
            IView view = Get(viewName);
            return view is { isOpen: true };
        }

        public bool IsOpen<T>() where T : class, IView, new()
        {
            IView view = Get<T>();
            return view is { isOpen: true };
        }

        public bool IsOpen(Type viewType)
        {
            IView view = Get(viewType);
            return view is { isOpen: true };
        }
        
        public void Close(string viewName, bool isForceDestroy = false)
        {
            IView view = Get(viewName);
            Close(view, isForceDestroy);
        }

        public void Close<T>(bool isForceDestroy = false) where T : class, IView, new()
        {
            IView view = Get<T>();
            Close(view, isForceDestroy);
        }
        
        public void Close(Type viewType, bool isForceDestroy = false)
        {
            IView view = Get(viewType);
            Close(view, isForceDestroy);
        }
        
        public void Close(IView view, bool isForceDestroy = false, bool checkPopPanel = true)
        {
            if (view == null)
            {
                return;
            }

            view.Close();

            if (checkPopPanel && m_CanPopPanel && !view.settings.canPopUp && m_PopViews.Count > 0)
            {
                IView oldView = m_PopViews[^1];
                oldView.Open(null);
                m_OpenViews.Add(oldView);
                m_AlwaysUIs.Remove(oldView);
                m_DelayDestroyUIs.Remove(oldView);
                m_PopViews.Remove(oldView);
                m_CurrPopView = oldView;
            }
            
            if (view.settings.destroyMode == UIDestroyMode.Immediately || isForceDestroy)
            {
                GameObjectPoolMgr.instance.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_OpenViews.Remove(view);
                m_PopViews.Remove(view);

                if (m_CurrPopView == view)
                {
                    m_CurrPopView = null;
                }
            }
            else if (view.settings.destroyMode == UIDestroyMode.Delay)
            {
                if (!m_DelayDestroyUIs.Contains(view))
                {
                    m_DelayDestroyUIs.Add(view);
                }
            }
            else if (view.settings.destroyMode == UIDestroyMode.Always)
            {
                if (!m_AlwaysUIs.Contains(view))
                {
                    m_AlwaysUIs.Add(view);
                }
            }
        }

        public static void ResgistViewType<T>(string viewName) where T : class, IView, new()
        {
            s_ViewTypes.Add(viewName, typeof(T));
        }

        private System.Type GetViewType(string viewName)
        {
            if (s_ViewTypes.TryGetValue(viewName, out Type viewType))
            {
                return viewType;
            }

            Log.LogError(viewName, "不存在,请使用ResgistViewType方法进行类型注册");
            return null;
        }
        
        private static Dictionary<string,Type> s_ViewTypes = new Dictionary<string, Type>();
    }
}