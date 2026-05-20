using System;
using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.UI
{
    public interface IUIMgr
    {
        public Canvas uiCanvas { get; }
        public UnityEngine.Camera uiCamera { get; }
        public void SetMgr(IGameObjectPoolMgr gameObjectPoolMgr);
        public RectTransform GetLayer(UILayer layer);
        public IPresenter Open(string viewName, object arg = null);
        public T Open<T>(object arg = null) where T : class, IPresenter, new();
        public IPresenter Open(Type viewType, object arg = null);
        public IPresenter Get(string viewName);
        public T Get<T>() where T : class, IPresenter, new();
        public IPresenter Get(Type viewType);
        public bool IsOpen(string viewName);
        public bool IsOpen<T>() where T : class, IPresenter, new();
        public bool IsOpen(Type viewType);
        public void Close(string viewName, bool isForceDestroy = false);
        public void Close<T>(bool isForceDestroy = false) where T : class, IPresenter, new();
        public void Close(Type viewType, bool isForceDestroy = false);
        public void Close(IPresenter view, bool isForceDestroy = false, bool checkPopPanel = true);
    }
}