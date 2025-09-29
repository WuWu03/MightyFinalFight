using GameFrameWork.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BasePanel<C, S> : IPanel where C : BasePanelComponent, new() where S : BasePanelSettings, new()
    {
        private GameObject m_GameObject;
        public GameObject gameObject
        {
            get
            {
                return m_GameObject;
            } 
        }
        
        private Transform m_Transform;
        public Transform transform
        {
            get
            {
                return m_Transform;
            }
        }

        private BasePanelSettings m_Settings;
        public BasePanelSettings settings
        {
            get
            {
                return m_Settings;
            }
        }

        private string m_AssetPath;
        public string assetPath
        {
            get
            {
                return m_AssetPath;
            }
        }
        
        private bool m_IsOpen = false;
        public bool isOpen
        {
            get
            {
                return m_IsOpen;
            }
        }
        
        private bool m_IsInit;
        public bool isInit
        {
            get
            {
                return m_IsInit;
            }
        }

        private bool m_IsHide;
        public bool isHide
        {
            get
            {
                return m_IsHide;
            }
        }
        
        private float m_DelayTime;
        public float delayTime
        {
            get
            {
                return m_DelayTime;
            }
        }
        
        private C m_Component;
        protected C component
        {
            get
            {
                return m_Component;
            }
        }
        
        private Dictionary<int, List<EventHandler<GameEventArgs>>> m_DicHandler = new();
        
        protected BasePanel()
        {
            m_AssetPath = string.Empty;
            m_IsOpen = false;
            m_IsInit = false;
            m_DelayTime = 0f;
            m_IsHide = false;
            m_Settings = new S();
            m_Component = new C();
        }

        public void Init(GameObject uiGameObject, string assetPath, object arg)
        {
            m_GameObject = uiGameObject;
            m_Transform = gameObject.transform;
            m_AssetPath = assetPath;
            m_Component.InitComponent(gameObject.GetComponent<UIRefRoot>());
            m_GameObject.SetLayer(LayerName.UI);
            m_Transform.SetParent(UIMgr.instance.GetPanelLayer(settings.panelLayer), false);
            
            OnInit(arg);
            m_IsInit = true;
            Open();
        }

        public void Open()
        {
            m_IsOpen = true;
            m_DelayTime = 0;
            m_GameObject.SetActiveSelf(!m_IsHide);
            OnOpen();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Close()
        {
            m_IsOpen = false;
            m_DelayTime = Time.unscaledTime;

            foreach (KeyValuePair<int, List<EventHandler<GameEventArgs>>> kvp in m_DicHandler)
            {
                foreach (EventHandler<GameEventArgs> handler in kvp.Value)
                {
                    EventMgr.instance.UnSubscribe(kvp.Key, handler);
                }
            }

            m_DicHandler.Clear();
            gameObject.SetActiveSelf(false);
            OnClose();
        }

        public void Show()
        {
            if (!m_IsHide)
            {
                return;
            }

            m_IsHide = false;

            if (gameObject is not null)
            {
                gameObject.SetActiveSelf(true);
            }
        }

        public void Hide()
        {
            if (m_IsHide)
            {
                return;
            }

            m_IsHide = true;

            if (gameObject is not null)
            {
                gameObject.SetActiveSelf(false);
            }
        }

        public void Destroy()
        {
            OnDestroy();
            m_DicHandler.Clear();
            m_IsOpen = false;
            m_IsInit = false;
            m_IsHide = false;
            m_DelayTime = 0f;
            m_AssetPath = string.Empty;
            m_DicHandler = null;
            m_Component = null;
            m_Settings = null;
        }

        protected void AddEvent(int eventId, EventHandler<GameEventArgs> handler)
        {
            if (handler == null)
            {
                Log.LogError("事件 [", eventId.ToString(), "] 的回调函数为空");
                return;
            }

            if (m_DicHandler.TryGetValue(eventId, out List<EventHandler<GameEventArgs>> list))
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
                list = new List<EventHandler<GameEventArgs>>
                {
                    handler
                };

                m_DicHandler.Add(eventId, list);
            }

            EventMgr.instance.Subscribe(eventId, handler);
        }

        protected void RemoveEvent(int eventId, EventHandler<GameEventArgs> handler)
        {
            if (m_DicHandler.TryGetValue(eventId, out List<EventHandler<GameEventArgs>> list))
            {
                list.Remove(handler);
            }
            EventMgr.instance.UnSubscribe(eventId, handler);
        }

        protected void CloseSelf()
        {
            UIMgr.instance.Close(m_Settings.panelName);
        }

        protected abstract void OnInit(object arg);
        protected abstract void OnOpen();
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