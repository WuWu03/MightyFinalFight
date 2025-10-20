using System;
using GameFrameWork.Event;
using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.UI
{
    public interface IUIMgr
    {
        public Canvas uiCanvas { get; }
        public UnityEngine.Camera uiCamera { get; }
        public void SetMgr(IGameObjectPoolMgr gameObjectPoolMgr, IEventMgr eventMgr);
        public RectTransform GetLayer(UILayer layer);
        public IView Open(string viewName, object arg = null);
        public T Open<T>(object arg = null) where T : class, IView, new();
        public IView Open(Type viewType, object arg = null);
        public IView Get(string viewName);
        public T Get<T>() where T : class, IView, new();
        public IView Get(Type viewType);
        public bool IsOpen(string viewName);
        public bool IsOpen<T>() where T : class, IView, new();
        public bool IsOpen(Type viewType);
        public void Close(string viewName, bool isForceDestroy = false);
        public void Close<T>(bool isForceDestroy = false) where T : class, IView, new();
        public void Close(Type viewType, bool isForceDestroy = false);
        public void Close(IView view, bool isForceDestroy = false, bool checkPopPanel = true);
    }
}