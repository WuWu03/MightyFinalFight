using GameFrameWork.Event;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BasePanel
    {
        public GameObject gameObject { get; private set; }

        public Transform transform { get; private set; }

        public BasePanelSettings settings
        {
            get
            {
                if(m_Settings == null)
                {
                    m_Settings = CreatePanelSetting();
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

        public bool isDelayTimeOut
        {
            get
            {
                return m_Settings.panelCloseMode == UIMgr.CloseMode.DelayDestroy && m_DelayTime > 0f && Time.time - m_DelayTime >= 5f;
            }
        }

        public void Init(GameObject go, string assetPath, object[] param)
        {
            gameObject = go;
            transform = go.transform;
            m_UIRefRoot = go.GetComponent<UIRefRoot>();
            m_AssetPath = assetPath;
            m_DicHandler = new Dictionary<int, List<EventHandler<GameEventArgs>>>();
            m_Component = CreatePanelComponent();
            gameObject.SetLayer(LayerName.UI);
            transform.SetParent(UIMgr.instance.GetUILayer(m_Settings.panelLayer), false);

            OnInit(param);
            m_IsInit = true;
            Open();

            if (m_IsHide)
            {
                gameObject.SetActive(false);
            }
        }

        public void Open()
        {
            m_IsOpen = true;
            gameObject.SetActive(true);
            m_DelayTime = 0;
            OnOpen();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Close()
        {
            m_IsOpen = false;
            gameObject.SetActive(false);
            m_DelayTime = Time.unscaledTime;

            foreach (KeyValuePair<int, List<EventHandler<GameEventArgs>>> kvp in m_DicHandler)
            {
                foreach (EventHandler<GameEventArgs> handler in kvp.Value)
                {
                    EventMgr.instance.UnSubscribe(kvp.Key, handler);
                }
            }

            m_DicHandler.Clear();
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
                gameObject.SetActive(true);
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
                gameObject.SetActive(false);
            }
        }

        public T GetComponent<T>()
        {
            if (gameObject != null)
            {
                return gameObject.GetComponent<T>();
            }

            return default;
        }

        public void Destroy()
        {
            m_DicHandler.Clear();
            m_IsOpen = false;
            m_IsInit = false;
            m_IsHide = false;
            m_DelayTime = 0f;
            m_AssetPath = string.Empty;
            m_Component = null;
            m_Settings = null;
            m_UIRefRoot = null;
            m_DicHandler = null;
            OnDestroy();
        }

        private BasePanelComponent CreatePanelComponent()
        {
            string componentTypeName = StringUtil.Format(GetType().Name, "Component");
            Type componentType = GetPaneInfoType(componentTypeName);

            if (componentType != null)
            {
                return Activator.CreateInstance(componentType, new object[] { m_UIRefRoot }) as BasePanelComponent;
            }

            return null;
        }

        private BasePanelSettings CreatePanelSetting()
        {
            string settingsTypeName = StringUtil.Format(GetType().Name, "Settings");
            Type settingsType = GetPaneInfoType(settingsTypeName);

            if (settingsType != null)
            {
                return Activator.CreateInstance(settingsType) as BasePanelSettings;
            }

            return null;
        }

        private Type GetPaneInfoType(string typeName)
        {
            Type type = Type.GetType(typeName);

            if (type == null)
            {
                Log.LogError(typeName, "不存在");
                return null;
            }

            return type;
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

        protected void AddEvent(int eventId, EventHandler<GameEventArgs> handler)
        {
            if (m_DicHandler.TryGetValue(eventId, out List<EventHandler<GameEventArgs>> list))
            {
                if (list.Contains(handler))
                {
                    Log.LogError("事件 [", eventId, "] 已经存在，不能重复添加");
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

        protected void CloseSelf()
        {
            UIMgr.instance.Close(m_Settings.panelName);
        }

        protected T GetPanelComponent<T>() where T : BasePanelComponent
        {
            return m_Component as T;
        }

        private bool m_IsOpen = false;
        private bool m_IsInit = false;
        private bool m_IsHide = false;
        private float m_DelayTime = 0f;
        private string m_AssetPath = string.Empty;
        private BasePanelComponent m_Component = null;
        private BasePanelSettings m_Settings = null;
        private UIRefRoot m_UIRefRoot = null;
        private Dictionary<int, List<EventHandler<GameEventArgs>>> m_DicHandler = null;
    }
}