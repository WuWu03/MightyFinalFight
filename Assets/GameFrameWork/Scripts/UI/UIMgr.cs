using GameFrameWork.Pool;
using System;
using System.Collections.Generic;
using GameFrameWork.Event;
using GameFrameWork.Utils;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.UI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIMgr : GameFrameWorkModule,IUIMgr
    {
        private readonly Canvas m_UICanvas;
        private readonly UnityEngine.Camera m_UICamera;
        private readonly List<IView> m_DelayDestroyUIs;
        private readonly List<IView> m_AlwaysUIs;
        private readonly List<IView> m_PopViews;
        private readonly List<IView> m_OpenViews;
        private readonly List<IView> m_TempViews;
        private readonly RectTransform[] m_UILayers;
        private readonly Dictionary<string, Type> s_ViewTypes = new();
        private IView m_CurrPopView;
        private IGameObjectPoolMgr m_GameObjectPoolMgr;
        private IEventMgr m_EventMgr;
        private bool m_CanPopPanel;
        private bool m_IsOpenViewsDirty;
        
        public UIMgr()
        {
            m_OpenViews = new();
            m_AlwaysUIs = new();
            m_DelayDestroyUIs = new();
            m_PopViews = new();
            m_TempViews = new();
            GameObject uiRoot = GameObject.Find("UIRoot");
            m_UICanvas = uiRoot.transform.Find("UICanvas").GetOrAddComponent<Canvas>();
            m_UICamera = uiRoot.transform.Find("UICamera").GetOrAddComponent<UnityEngine.Camera>();
            m_UILayers = new RectTransform[(int)UILayer.Load + 1];

            for (int i = 0; i < m_UILayers.Length; i++)
            {
                m_UILayers[i] = m_UICanvas.transform.GetChild(i).GetComponent<RectTransform>();
            }
            
            UnityObject.DontDestroyOnLoad(uiRoot);
        }
        
        /// <summary>
        /// UI画布
        /// </summary>
        public Canvas uiCanvas
        {
            get
            {
                return m_UICanvas;
            }
        }
        
        /// <summary>
        /// UI相机
        /// </summary>
        public UnityEngine.Camera uiCamera
        {
            get
            {
                return m_UICamera;
            }
        }
        
        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        /// <param name="time"></param>
        /// <param name="unscaledTime"></param>
        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
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

                    m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject, true);
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
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_AlwaysUIs.Remove(view);
                m_PopViews.Remove(view);

                if (m_CurrPopView == view)
                {
                    m_CurrPopView = null;
                }
            }

            if (m_IsOpenViewsDirty)
            {
                m_TempViews.Clear();
                m_TempViews.AddRange(m_OpenViews);
                m_IsOpenViewsDirty = false;
            }

            foreach (var view in m_TempViews)
            {
                if (view is { isOpen : true })
                {
                    view.Update();
                }
            }
        }
        
        /// <summary>
        /// 销毁
        /// </summary>
        public override void Shutdown()
        {
            foreach (var panel in m_PopViews)
            {
                m_GameObjectPoolMgr.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_OpenViews)
            {
                m_GameObjectPoolMgr.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_DelayDestroyUIs)
            {
                m_GameObjectPoolMgr.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_AlwaysUIs)
            {
                m_GameObjectPoolMgr.Put(panel.assetPath, panel.gameObject);
            }

            m_DelayDestroyUIs.Clear();
            m_AlwaysUIs.Clear();
            m_PopViews.Clear();
            m_OpenViews.Clear();
            m_TempViews.Clear();
        }

        public void SetMgr(IGameObjectPoolMgr gameObjectPoolMgr, IEventMgr eventMgr)
        {
            m_GameObjectPoolMgr = gameObjectPoolMgr;
            m_EventMgr = eventMgr;
        }

        /// <summary>
        /// 获取层级
        /// </summary>
        /// <param name="layer">层级类型</param>
        /// <returns></returns>
        public RectTransform GetLayer(UILayer layer)
        {
            return m_UILayers[Convert.ToInt32(layer)];
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="viewName">UI名字</param>
        /// <param name="arg">自定义参数</param>
        /// <returns>IView</returns>
        public IView Open(string viewName, object arg = null)
        {
            Type viewType = GetViewType(viewName);
            return Open(viewType, arg);
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="arg">自定义参数</param>
        /// <typeparam name="T">UI类型</typeparam>
        /// <returns></returns>
        public T Open<T>(object arg = null) where T : class, IView, new()
        {
            return Open(typeof(T), arg) as T;
        }
        
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="viewType">UI类型</param>
        /// <param name="arg">自定义参数</param>
        /// <returns>IView</returns>
        public IView Open(Type viewType, object arg = null)
        {
            if (viewType == null)
            {
                return null;
            }

            IView view = Get(viewType);
            
            if (view != null)
            {
                view.Open(arg);
                return view;
            }
            
            view = Activator.CreateInstance(viewType) as IView;
            
            if (view == null)
            {
                throw new Exception(StringUtil.Append("创建 [", viewType.FullName, "] UI对象失败"));
            }

            view.SetMgr(this, m_GameObjectPoolMgr, m_EventMgr);
            m_OpenViews.Add(view);
            m_AlwaysUIs.Remove(view);
            m_DelayDestroyUIs.Remove(view);
            m_IsOpenViewsDirty = true;
            
            if (!m_CanPopPanel && view.settings.canPopUp)
            {
                m_CanPopPanel = true;
            }

            if (m_CanPopPanel && view.settings.canPopUp)
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
            Type viewType = GetViewType(viewName);
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
                m_IsOpenViewsDirty = true;
            }
            
            if (view.settings.destroyMode == UIDestroyMode.Immediately || isForceDestroy)
            {
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_OpenViews.Remove(view);
                m_PopViews.Remove(view);
                m_IsOpenViewsDirty = true;
                
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


        public void RegisterViewType<T>(string viewName) where T : class, IView, new()
        {
            s_ViewTypes.Add(viewName, typeof(T));
        }
        
        private Type GetViewType(string viewName)
        {
            if (s_ViewTypes.TryGetValue(viewName, out Type viewType))
            {
                return viewType;
            }

            throw new Exception(StringUtil.Append("[", viewName, "] 不存在,请使用RegisterViewType方法进行类型注册"));
        }
    }
}