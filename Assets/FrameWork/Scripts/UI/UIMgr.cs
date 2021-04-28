using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class UIMgr : BaseMgr<UIMgr>
    {
        public enum Type
        {
            Root,//根界面（主界面）
            Normal,//一般界面
            Pop,//弹出界面
        }

        public enum Layer
        {
            BG,
            MainPanel,
            FirstLevel,
            SecondLevel,
            ThirdLevel,
        }

        public enum CloseMode
        {
            Always = 1,         // UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先关闭的
            Destroy = 2,        // 关闭时立即销毁
            DelayDestroy = 3,   // 延迟一段时间销毁
            Eternal = 4,        // 总是存于场景中, 除非主动销毁
        }

        private class WaitLoadPanel
        {
            public BasePanel Panel;
            public string PanelName;
            public object[] Param;

            public WaitLoadPanel(BasePanel panel,string panelName,object[] param)
            {
                this.Panel = panel;
                this.PanelName = panelName;
                this.Param = param;
            }
        }

        protected override void OnAwake()
        {
            m_ListOpenPanel = new List<BasePanel>();
            m_ListAlways = new List<BasePanel>();
            m_StackMutexPanel = new Stack<BasePanel>();
            m_QueueDelayDestroy = new Queue<BasePanel>();
            m_QueueWaitLoadPanel = new Queue<WaitLoadPanel>();

            m_UIRoot = new GameObject("UIRoot");
            m_UICanvas = new GameObject("UICanvas").GetOrAddComponent<Canvas>();
            m_UICamera = new GameObject("UICamera").GetOrAddComponent<UnityEngine.Camera>();
            m_EventSystem = new GameObject("EventSystem").GetOrAddComponent<EventSystem>();

            CanvasScaler canvasScaler = m_UICanvas.gameObject.GetOrAddComponent<CanvasScaler>();
            UICanvasScaleAdapt canvasScaleAdapt = m_UICanvas.gameObject.GetOrAddComponent<UICanvasScaleAdapt>();
            StandaloneInputModule inputModule = m_EventSystem.gameObject.GetOrAddComponent<StandaloneInputModule>();

            m_UICamera.transform.SetParent(m_UIRoot.transform, false);
            m_UICanvas.transform.SetParent(m_UIRoot.transform, false);
            m_EventSystem.transform.SetParent(m_UIRoot.transform, false);

            m_UICamera.clearFlags = CameraClearFlags.SolidColor;
            m_UICamera.backgroundColor = Color.black;
            m_UICamera.cullingMask = LayerMask.GetMask("UI");
            m_UICamera.orthographic = true;
            m_UICamera.orthographicSize = 5;
            m_UICamera.nearClipPlane = -1000;
            m_UICamera.farClipPlane = 1000;
            m_UICamera.depth = 0;

            m_UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
            m_UICanvas.worldCamera = m_UICamera;
            m_UICanvas.planeDistance = 100;

            canvasScaler.referenceResolution = new Vector2(1280, 720);
            canvasScaler.referencePixelsPerUnit = 100f;
            canvasScaleAdapt.ScalerType = UICanvasScaleAdapt.Type.WidthOrHeight;
            inputModule.submitButton = "A";
            inputModule.cancelButton = "B";

            m_UIRoot.SetLayer("UI");

            Array layers = Enum.GetValues(typeof(Layer));

            m_UILayerTransform = new RectTransform[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                m_UILayerTransform[i] = new GameObject(Enum.GetValues(typeof(Layer)).GetValue(i).ToString()).AddComponent<RectTransform>();
                m_UILayerTransform[i].gameObject.GetOrAddComponent<Canvas>().overrideSorting = true;
                m_UILayerTransform[i].gameObject.GetOrAddComponent<Canvas>().sortingOrder = i * 1000;
                m_UILayerTransform[i].gameObject.GetOrAddComponent<GraphicRaycaster>();
                m_UILayerTransform[i].anchoredPosition = Vector3.zero;
                m_UILayerTransform[i].sizeDelta = Vector2.zero;
                m_UILayerTransform[i].anchorMin = new Vector2(0, 0);
                m_UILayerTransform[i].anchorMax = new Vector2(1, 1);
                m_UILayerTransform[i].pivot = new Vector2(0.5f, 0.5f);
                m_UILayerTransform[i].SetParent(m_UICanvas.transform, false);
                m_UILayerTransform[i].gameObject.SetLayer("UI");
            }

            DontDestroyOnLoad(m_UIRoot);
        }

        public Transform GetUILayer(Layer layer)
        {
            return m_UILayerTransform[Convert.ToInt32(layer)];
        }

        public void Open<T>(params object[] param) where T : BasePanel, new()
        {
            RealOpen(typeof(T).Name, param);
        }

        public void Open(string panelName,params object[] param)
        {
            RealOpen(panelName, param);
        }

        public T GetPanel<T>() where T : BasePanel
        {
            BasePanel panel = RealGet(typeof(T).Name);
            if (panel == null)
            {
                return null;
            }
            return panel as T;
        }

        public BasePanel GetPanel(string panelName)
        {
            BasePanel panel = RealGet(panelName);
            if (panel == null)
            {
                return null;
            }
            return panel;
        }

        public bool IsPanelOpen<T>()
        {
            BasePanel panel = RealGet(typeof(T).Name);
            return panel != null && panel.IsOpen;
        }

        public bool IsPanelOpen(string panelName)
        {
            BasePanel panel = RealGet(panelName);
            return panel != null && panel.IsOpen;
        }

        public void Close<T>(bool isForceDestroy = false) where T:BasePanel
        {
            RealClose(typeof(T).Name,isForceDestroy);
        }

        public void Close(string panelName,bool isForceDestroy = false)
        {
            RealClose(panelName, isForceDestroy);
        }

        public void Close(BasePanel panel,bool isForceDestroy = false)
        {
            if(panel == null)
            {
                return;
            }

            RealClose(panel.PanelName, isForceDestroy);
        }

        private void RealOpen(string panelName, object[] param)
        {
            BasePanel openPanel = OpenPanel(panelName, param);

            if(openPanel == null || openPanel.PanelType == Type.Pop)
            {
                return;
            }
            
            if(m_StackMutexPanel.Count > 0)
            {
                ClosePanel(m_StackMutexPanel.Peek().PanelName, false);
            }

            m_StackMutexPanel.Push(openPanel);
        }

        private void RealClose(string panelName,bool isForceDestroy)
        {
            BasePanel closePanel = ClosePanel(panelName, isForceDestroy);

            if (closePanel == null)
            {
                return;
            }

            if (closePanel.PanelType == Type.Pop || m_StackMutexPanel.Count < 1)
            {
                return;
            }

            if (m_StackMutexPanel.Peek().PanelName.Equals(panelName))
            {
                m_StackMutexPanel.Pop();
            }

            if (m_StackMutexPanel.Count > 0)
            {
                BasePanel lastPanel = m_StackMutexPanel.Peek();
                lastPanel.Open();
                m_ListOpenPanel.Add(lastPanel);
            }
        }

        private BasePanel RealGet(string panelName)
        {
            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                if (m_ListOpenPanel[i].PanelName.Equals(panelName))
                {
                    return m_ListOpenPanel[i];
                }
            }

            return null;
        }

        private BasePanel OpenPanel(string panelName, object[] param)
        {
            System.Type type = System.Type.GetType(panelName);

            if (type == null)
            {
                Log.Debugger.LogError("Panel is invalid!");
                return null;
            }

            BasePanel panel = RealGet(panelName);

            if (panel != null && panel.IsOpen)
            {
                return null;
            }

            if (panel == null)
            {
                panel = Activator.CreateInstance(type) as BasePanel;
                m_ListOpenPanel.Add(panel);
            }

            if (!panel.IsInit)
            {
                m_QueueWaitLoadPanel.Enqueue(new WaitLoadPanel(panel, panelName, param));
            }
            else if (!panel.IsOpen)
            {
                panel.Open();
            }

            return panel;
        }

        private BasePanel ClosePanel(string panelName, bool isForceDestroy)
        {
            BasePanel panel = RealGet(panelName);

            if (panel == null)
            {
                return null;
            }

            panel.Close();
 
            if (panel.PanelCloseMode == CloseMode.DelayDestroy)
            {
                Queue<BasePanel> queue = m_QueueDelayDestroy;
                lock (queue)
                {
                    m_QueueDelayDestroy.Enqueue(panel);
                }

                m_ListOpenPanel.Remove(panel);
            }

            if (panel.PanelCloseMode == CloseMode.Always && !m_ListAlways.Contains(panel))
            {
                List<BasePanel> list = m_ListAlways;
                lock (list)
                {
                    m_ListAlways.Add(panel);
                }
            }

            if (panel.PanelCloseMode == CloseMode.Destroy || isForceDestroy)
            {
                panel.Destroy();
                Destroy(panel.gameObject);
                m_ListOpenPanel.Remove(panel);
            }

            return panel;
        }

        private void OnResComplete(GameObject go,object[] param)
        {
            WaitLoadPanel wait = (param[0] as WaitLoadPanel);
            wait.Panel.Init(go, wait.Param);
        }

        private void Update()
        {
            if(m_QueueWaitLoadPanel.Count > 0)
            {
                WaitLoadPanel waitLoadPanel = null;
                Queue<WaitLoadPanel> queue = m_QueueWaitLoadPanel;

                lock (queue)
                {
                    waitLoadPanel = m_QueueWaitLoadPanel.Dequeue();
                    UITools.LoadUI(waitLoadPanel.PanelName, OnResComplete, waitLoadPanel);
                }
            }

            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                if (m_ListOpenPanel[i].IsOpen)
                    m_ListOpenPanel[i].Update();
            }

            if (m_QueueDelayDestroy.Count > 0 && m_QueueDelayDestroy.Peek().IsDelayTimeOut)
            {
                BasePanel panel = null;
                Queue<BasePanel> queue = m_QueueDelayDestroy;

                lock (queue)
                {
                    panel = m_QueueDelayDestroy.Dequeue();
                    panel.Destroy();
                    Destroy(panel.gameObject);
                }
            }

            if(m_ListAlways.Count > 10)
            {
                BasePanel panel = null;
                List<BasePanel> list = m_ListAlways;

                lock (list)
                {
                    panel = m_ListAlways[0];
                    m_ListAlways.RemoveAt(0);
                    m_ListOpenPanel.Remove(panel);
                    panel.Destroy();
                    Destroy(panel.gameObject); 
                }
            }
        }

        protected override void OnShutDown()
        {
            m_StackMutexPanel.Clear();
            m_ListOpenPanel.Clear();
            m_QueueDelayDestroy.Clear();
            m_ListAlways.Clear();
        }

        private Queue<BasePanel> m_QueueDelayDestroy = null;
        private List<BasePanel> m_ListAlways = null;
        private Queue<WaitLoadPanel> m_QueueWaitLoadPanel = null;
        private Stack<BasePanel> m_StackMutexPanel = null;
        private List<BasePanel> m_ListOpenPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}