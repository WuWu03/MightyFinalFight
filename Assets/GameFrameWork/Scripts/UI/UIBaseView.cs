using GameFrameWork.Event;
using System;
using System.Collections.Generic;
using GameFrameWork.Pool;
using GameFrameWork.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFrameWork.UI
{
    public abstract class UIBaseView<C, S> : IView where C : UIBaseComponent, new() where S : UIBaseSettings, new()
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

        private UIBaseSettings m_Settings;
        public UIBaseSettings settings
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
        
        private bool m_IsShow;
        public bool isShow
        {
            get
            {
                return m_IsShow;
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
        private object m_Arg = null;
        protected UIBaseView()
        {
            m_AssetPath = string.Empty;
            m_IsOpen = false;
            m_DelayTime = 0f;
            m_IsShow = false;
            m_Settings = new S();
            m_Component = new C();
        }

        public void Open(object arg)
        {
            if (m_IsOpen)
            {
                Show();
                return;
            }

            if (arg != null)
            {
                m_Arg = arg;
            }
            
            m_IsOpen = true;
            string prefabName = StringUtil.Append(m_Settings.name, ".prefab");
            GameObjectPoolMgr.instance.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabsPath(), prefabName), OnLoadComplete);
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
            m_DicHandler = null;
            m_Component = null;
            m_Settings = null;
            m_Arg = null;
        }
        
        private void OnLoadComplete(string assetPath, Object uiGameObject, object arg)
        {
            m_GameObject = uiGameObject as GameObject;
            m_AssetPath = assetPath;
            
            if (m_GameObject is not null)
            {
                m_Transform = m_GameObject.transform;
                m_Component.InitComponent(m_GameObject.GetComponent<UIRefRoot>());
                m_GameObject.SetLayer(LayerName.UI);
                m_Transform.SetParent(UIMgr.instance.GetPanelLayer(settings.Layer), false);
            }
            
            OnOpen(m_Arg);
            Show();
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
            UIMgr.instance.Close(m_Settings.name);
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