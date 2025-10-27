using GameFrameWork.Event;
using System;
using System.Collections.Generic;
using GameFrameWork.Pool;
using GameFrameWork.Utils;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using EventArg = GameFrameWork.Event.EventArg;

namespace GameFrameWork.UI
{
    public abstract class UIBaseView<C, S> : IView where C : UIBaseComponent, new() where S : UIBaseSettings, new()
    {
        private readonly Dictionary<uint, List<EventHandler<EventArg>>> m_DicHandler = new();
        private GameObject m_GameObject;
        private Transform m_Transform;
        private UIBaseSettings m_Settings;
        private IUIMgr m_UIMgr;
        private IGameObjectPoolMgr m_GameObjectPoolMgr;
        private IEventMgr m_EventMgr;
        private C m_Component;
        
        private string m_AssetPath;
        private bool m_IsOpen;
        private bool m_IsShow;
        private float m_DelayTime;
        private object m_Arg;
        private bool m_IsLoading;
        
        public UIBaseView()
        {
            m_AssetPath = string.Empty;
            m_IsOpen = false;
            m_DelayTime = 0f;
            m_IsShow = false;
            m_Settings = new S();
            m_Component = new C();
        }
        

        public GameObject gameObject
        {
            get
            {
                return m_GameObject;
            } 
        }

        public Transform transform
        {
            get
            {
                return m_Transform;
            }
        }
        
        public UIBaseSettings settings
        {
            get
            {
                return m_Settings;
            }
        }
        
        public string assetPath
        {
            get
            {
                return m_AssetPath;
            }
        }
        
        public bool isOpen
        {
            get
            {
                return m_IsOpen;
            }
        }
        
        public bool isShow
        {
            get
            {
                return m_IsShow;
            }
        }
        
        public float delayTime
        {
            get
            {
                return m_DelayTime;
            }
        }
        
        protected C component
        {
            get
            {
                return m_Component;
            }
        }
        
        public void SetMgr(IUIMgr uiMgr, IGameObjectPoolMgr gameObjectPoolMgr, IEventMgr eventMgr)
        {
            m_UIMgr = uiMgr;
            m_GameObjectPoolMgr = gameObjectPoolMgr;
            m_EventMgr = eventMgr;
        }

        public void Open(object arg)
        {
            if (m_IsLoading)
            {
                return;
            }
            
            if (arg != null)
            {
                m_Arg = arg;
            }

            if (m_IsOpen)
            {
                Show();
                return;
            }
            
            m_IsLoading = true;
            m_GameObjectPoolMgr.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabsPath(), m_Settings.prefabName), OnLoadComplete);
        }

        public void Update()
        {
            OnUpdate();
        }
        
        public void Close()
        {
            m_IsOpen = false;
            m_DelayTime = Time.unscaledTime;

            foreach (KeyValuePair<uint, List<EventHandler<EventArg>>> kvp in m_DicHandler)
            {
                foreach (EventHandler<EventArg> handler in kvp.Value)
                {
                    m_EventMgr.UnSubscribe(kvp.Key, handler);
                }
            }

            m_DicHandler.Clear();
            Hide();
            OnClose();
        }
        
        public void Show()
        {
            if (m_IsShow)
            {
                return;
            }

            m_IsShow = true;

            if (gameObject is not null)
            {
                gameObject.SetActiveSelf(true);
            }
            
            OnShow(m_Arg);
        }

        public void Hide()
        {
            if (!m_IsShow)
            {
                return;
            }

            m_IsShow = false;

            if (gameObject is not null)
            {
                gameObject.SetActiveSelf(false);
            }
            
            OnHide();
        }

        public void Destroy()
        {
            OnDestroy();
            m_DicHandler.Clear();
            m_IsOpen = false;
            m_IsShow = false;
            m_DelayTime = 0f;
            m_AssetPath = string.Empty;
            m_Component = null;
            m_Settings = null;
            m_Arg = null;
            m_UIMgr = null;
            m_EventMgr = null;
        }
        
        private void OnLoadComplete(string assetPath, UnityObject uiGameObject, object arg)
        {
            m_GameObject = uiGameObject as GameObject;
            m_AssetPath = assetPath;
            m_IsOpen = true;
            m_IsLoading = false;
            
            if (m_GameObject is not null)
            {
                m_Transform = m_GameObject.transform;
                m_Component.InitComponent(m_GameObject.GetComponent<UIRefRoot>());
                m_GameObject.SetLayer(LayerName.UI);
                m_Transform.SetParent(m_UIMgr.GetLayer(settings.layer), false);
            }
            
            OnOpen(m_Arg);
            Show();
        }

        protected void AddEvent(uint eventId, EventHandler<EventArg> handler)
        {
            if (handler == null)
            {
                Log.LogError("事件 [", eventId.ToString(), "] 的回调函数为空");
                return;
            }

            if (m_DicHandler.TryGetValue(eventId, out List<EventHandler<EventArg>> list))
            {
                if (list.Contains(handler))
                {
                    Log.LogError("事件 [", eventId.ToString(), "] 重复订阅");
                    return;
                }

                list.Add(handler);
            }
            else
            {
                list = new List<EventHandler<EventArg>>
                {
                    handler
                };

                m_DicHandler.Add(eventId, list);
            }

            m_EventMgr.Subscribe(eventId, handler);
        }

        protected void RemoveEvent(uint eventId, EventHandler<EventArg> handler)
        {
            if (m_DicHandler.TryGetValue(eventId, out List<EventHandler<EventArg>> list))
            {
                list.Remove(handler);
            }
            
            m_EventMgr.UnSubscribe(eventId, handler);
        }
        
        protected void CloseSelf()
        {
            m_UIMgr.Close(this);
        }
        
        protected abstract void OnOpen(object arg);
        protected abstract void OnShow(object arg);
        protected abstract void OnHide();
        protected abstract void OnUpdate();
        protected abstract void OnClose();
        protected abstract void OnDestroy();
        protected virtual void OnJoyStickUp() { }
        protected virtual void OnJoyStickLeft() { }
        protected virtual void OnJoyStickDown() { }
        protected virtual void OnJoyStickRight() { }
        protected virtual void OnButtonA() { }
        protected virtual void OnButtonB() { }
        protected virtual void OnButtonX() { }
        protected virtual void OnButtonY() { }
    }
}