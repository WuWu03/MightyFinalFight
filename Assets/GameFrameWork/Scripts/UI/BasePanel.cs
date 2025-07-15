using GameFrameWork.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BasePanel<T> : IPanel where T : BasePanelComponent, new()
    {
        public GameObject gameObject { get; private set; }

        public Transform transform { get; private set; }

        public BasePanelSettings settings
        {
            get
            {
                if (m_Settings == null)
                {
                    m_Settings = this.CreatePanelParam("Settings") as BasePanelSettings;
                }

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

        public bool isInit
        {
            get
            {
                return m_IsInit;
            }
        }

        public float delayTime
        {
            get
            {
                return m_DelayTime;
            }
        }

        public void Init(GameObject uiGameObject, string assetPath, object[] param)
        {
            gameObject = uiGameObject;
            transform = gameObject.transform;
            m_AssetPath = assetPath;
            m_Component = this.CreatePanelParam("Component") as T;
            m_DicHandler = new Dictionary<int, List<EventHandler<GameEventArgs>>>();

            if (m_Settings == null)
            {
                m_Settings = this.CreatePanelParam("Settings") as BasePanelSettings;
            }

            m_Component.InitComponent(gameObject.GetComponent<UIRefRoot>());
            gameObject.SetLayer(LayerName.UI);
            transform.SetParent(UIMgr.instance.GetUILayer(m_Settings.panelLayer), false);

            OnInit(param);
            m_IsInit = true;
            Open();
        }

        public void Open()
        {
            m_IsOpen = true;
            m_DelayTime = 0;
            gameObject.SetActiveSelf(!m_IsHide);
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

            if (gameObject != null)
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

            if (gameObject != null)
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

        protected abstract void OnInit(object[] param);
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

        private bool m_IsOpen = false;
        private bool m_IsInit = false;
        private bool m_IsHide = false;
        private float m_DelayTime = 0f;
        private string m_AssetPath = string.Empty;

        private Dictionary<int, List<EventHandler<GameEventArgs>>> m_DicHandler = null;
        private BasePanelSettings m_Settings = null;

        protected T m_Component = null;
    }
}