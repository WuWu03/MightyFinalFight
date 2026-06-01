using System;
using WuWuFramework.Pool;
using UnityEngine;

namespace WuWuFramework.UI
{
    public interface IUIMgr
    {
        public Canvas uiCanvas { get; }
        public UnityEngine.Camera uiCamera { get; }
        public void SetMgr(IGameObjectPoolMgr gameObjectPoolMgr);
        public RectTransform GetLayer(UILayer layer);
        public IUIView Open(string viewName, object arg = null);
        public T Open<T>(object arg = null) where T : class, IUIView, new();
        public IUIView Open(Type viewType, object arg = null);
        public T Get<T>() where T : class, IUIView, new();
        public IUIView Get(Type viewType);
        public bool IsOpen(string viewName);
        public bool IsOpen<T>() where T : class, IUIView, new();
        public bool IsOpen(Type viewType);
        public void Close(string viewName, bool isForceDestroy = false);
        public void Close<T>(bool isForceDestroy = false) where T : class, IUIView, new();
        public void Close(Type viewType, bool isForceDestroy = false);
        public void Close(IUIView view, bool isForceDestroy = false, bool checkPopPanel = true);
    }
}