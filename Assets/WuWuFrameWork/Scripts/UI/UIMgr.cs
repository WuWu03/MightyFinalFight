using System;
using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Pool;
using WuWuFramework.Utils;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.UI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIMgr : WuWuFrameworkModule, IUIMgr
    {
        private readonly UIRoot m_UIRoot;
        private readonly List<IUIView> m_DelayDestroyViews;
        private readonly List<IUIView> m_AlwaysViews;
        private readonly List<IUIView> m_PopViews;
        private readonly List<IUIView> m_OpenViews;
        private readonly List<IUIView> m_TempViews;
        private IUIView m_CurrPopView;
        private IGameObjectPoolMgr m_GameObjectPoolMgr;
        private bool m_CanPopView;
        private bool m_IsOpenViewsDirty;

        /// <summary>
        /// UI根节点
        /// </summary>
        public UIRoot uiRoot
        {
            get { return m_UIRoot; }
        }

        public UIMgr()
        {
            m_OpenViews = new();
            m_AlwaysViews = new();
            m_DelayDestroyViews = new();
            m_PopViews = new();
            m_TempViews = new();
            m_UIRoot = UnityObject.FindFirstObjectByType<UIRoot>();
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
            if (m_DelayDestroyViews.Count > 0)
            {
                for (int i = m_DelayDestroyViews.Count - 1; i >= 0; i++)
                {
                    IUIView view = m_DelayDestroyViews[i];
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
                    m_DelayDestroyViews.Remove(view);
                    m_PopViews.Remove(view);

                    if (m_CurrPopView == view)
                    {
                        m_CurrPopView = null;
                    }
                }
            }

            if (m_AlwaysViews.Count > 1)
            {
                IUIView view = m_AlwaysViews[0];
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_AlwaysViews.Remove(view);
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
                if (view is { isOpen: true })
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
            foreach (var view in m_PopViews)
            {
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject);
            }

            foreach (var view in m_OpenViews)
            {
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject);
            }

            foreach (var view in m_DelayDestroyViews)
            {
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject);
            }

            foreach (var view in m_AlwaysViews)
            {
                m_GameObjectPoolMgr.Put(view.assetPath, view.gameObject);
            }

            m_DelayDestroyViews.Clear();
            m_AlwaysViews.Clear();
            m_PopViews.Clear();
            m_OpenViews.Clear();
            m_TempViews.Clear();
        }

        public void SetMgr(IGameObjectPoolMgr gameObjectPoolMgr)
        {
            m_GameObjectPoolMgr = gameObjectPoolMgr;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="viewName">UI名字</param>
        /// <param name="arg">自定义参数</param>
        /// <returns>IPresenter</returns>
        public IUIView Open(string viewName, object arg = null)
        {
            Type viewType = UIFactory.GetViewType(viewName);
            return Open(viewType, arg);
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="arg">自定义参数</param>
        /// <typeparam name="T">UI类型</typeparam>
        /// <returns></returns>
        public T Open<T>(object arg = null) where T : class, IUIView, new()
        {
            return Open(typeof(T), arg) as T;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="viewType">UI类型</param>
        /// <param name="arg">自定义参数</param>
        /// <returns>IPresenter</returns>
        public IUIView Open(Type viewType, object arg = null)
        {
            if (viewType == null)
            {
                return null;
            }

            IUIView view = Get(viewType);

            if (view != null)
            {
                view.Open(arg);
                return view;
            }

            view = UIFactory.GetUIView(viewType);

            if (view == null)
            {
                throw new Exception(StringUtil.Append("创建 [", viewType.FullName, "] UI对象失败"));
            }

            view.SetMgr(this, m_GameObjectPoolMgr);
            m_OpenViews.Add(view);
            m_AlwaysViews.Remove(view);
            m_DelayDestroyViews.Remove(view);
            m_IsOpenViewsDirty = true;

            if (!m_CanPopView && view.settings.canPopUp)
            {
                m_CanPopView = true;
            }

            if (m_CanPopView && view.settings.canPopUp)
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

        public T Get<T>() where T : class, IUIView, new()
        {
            return Get(typeof(T)) as T;
        }

        public IUIView Get(Type viewType)
        {
            if (viewType == null)
            {
                return null;
            }

            foreach (IUIView view in m_OpenViews)
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
            IUIView view = Get(UIFactory.GetViewType(viewName));
            return view is { isOpen: true };
        }

        public bool IsOpen<T>() where T : class, IUIView, new()
        {
            IUIView view = Get<T>();
            return view is { isOpen: true };
        }

        public bool IsOpen(Type viewType)
        {
            IUIView view = Get(viewType);
            return view is { isOpen: true };
        }

        public void Close(string viewName, bool isForceDestroy = false)
        {
            IUIView view = Get(UIFactory.GetViewType(viewName));
            Close(view, isForceDestroy);
        }

        public void Close<T>(bool isForceDestroy = false) where T : class, IUIView, new()
        {
            IUIView view = Get<T>();
            Close(view, isForceDestroy);
        }

        public void Close(Type viewType, bool isForceDestroy = false)
        {
            IUIView view = Get(viewType);
            Close(view, isForceDestroy);
        }

        public void Close(IUIView view, bool isForceDestroy = false, bool checkPopView = true)
        {
            if (view == null)
            {
                return;
            }

            view.Close();

            if (checkPopView && m_CanPopView && !view.settings.canPopUp && m_PopViews.Count > 0)
            {
                IUIView oldView = m_PopViews[^1];
                oldView.Open(null);
                m_OpenViews.Add(oldView);
                m_AlwaysViews.Remove(oldView);
                m_DelayDestroyViews.Remove(oldView);
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
                if (!m_DelayDestroyViews.Contains(view))
                {
                    m_DelayDestroyViews.Add(view);
                }
            }
            else if (view.settings.destroyMode == UIDestroyMode.Always)
            {
                if (!m_AlwaysViews.Contains(view))
                {
                    m_AlwaysViews.Add(view);
                }
            }
        }
    }
}