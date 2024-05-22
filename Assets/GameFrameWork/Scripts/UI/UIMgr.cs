using GameFrameWork.Pool;
using GameFrameWork.Resources;
using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Application;

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
            Layer1,
            Layer2,
            Layer3,
            Layer4,
            Layer5,
            Layer6,
            Layer7,
            Layer8,
        }

        public enum CloseMode
        {
            Always = 1,         // UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先关闭的
            Destroy = 2,        // 关闭时立即销毁
            DelayDestroy = 3,   // 延迟一段时间销毁
            Eternal = 4,        // 总是存于场景中, 除非主动销毁
        }

        private class WaitLoadPanel : IReference
        {
            public BasePanel panel;
            public object[] param;

            public static WaitLoadPanel Create(BasePanel panel, object[] param)
            {
                WaitLoadPanel waitLoadPanel = ReferencePool.Acquire<WaitLoadPanel>();
                waitLoadPanel.panel = panel;
                waitLoadPanel.param = param;
                return waitLoadPanel;
            }

            public WaitLoadPanel()
            {
            }

            public void Clear()
            {
                panel = null;
                param = null;
            }
        }

        public UnityEngine.Camera uiCamera
        {
            get
            {
                return m_UICamera;
            }
        }

        protected override void OnAwake()
        {
            m_ListOpenPanel = new List<BasePanel>();
            m_ListAlways = new List<BasePanel>();
            m_ListDelayDestroy = new List<BasePanel>();
            m_ListPopPanel = new List<BasePanel>();
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

            m_UICamera.clearFlags = CameraClearFlags.Depth;
            m_UICamera.backgroundColor = Color.black;
            m_UICamera.cullingMask = LayerMask.GetMask("UI");
            m_UICamera.orthographic = true;
            m_UICamera.orthographicSize = 5;
            m_UICamera.nearClipPlane = -1000;
            m_UICamera.farClipPlane = 1000;
            m_UICamera.depth = 100;

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

        public T Open<T>(params object[] args) where T : BasePanel, new()
        {
            BasePanel panel = RealOpen(typeof(T).Name, args);

            if (panel == null)
            {
                return default(T);
            }

            return panel as T;
        }

        public BasePanel Open(string panelName, params object[] args)
        {
            return RealOpen(panelName, args);
        }

        public T Get<T>() where T : BasePanel
        {
            BasePanel panel = GetPanel(typeof(T).Name);

            if (panel == null)
            {
                return null;
            }

            return panel as T;
        }

        public BasePanel Get(string panelName)
        {
            BasePanel panel = GetPanel(panelName);

            if (panel == null)
            {
                return null;
            }

            return panel;
        }

        public bool IsOpen<T>()
        {
            BasePanel panel = GetPanel(typeof(T).Name);
            return panel != null && panel.isOpen;
        }

        public bool IsOpen(string panelName)
        {
            BasePanel panel = GetPanel(panelName);
            return panel != null && panel.isOpen;
        }

        public void Close<T>(bool isForceDestroy = false) where T : BasePanel
        {
            ClosePanel(typeof(T).Name, isForceDestroy);
        }

        public void Close(string panelName, bool isForceDestroy = false)
        {
            ClosePanel(panelName, isForceDestroy);
        }

        public void Close(BasePanel panel, bool isForceDestroy = false)
        {
            if (panel == null)
            {
                return;
            }

            ClosePanel(panel.panelName, isForceDestroy);
        }

        private BasePanel RealOpen(string panelName, object[] args)
        {
            System.Type type = System.Type.GetType(panelName);

            if (type == null)
            {
                Log.LogError(panelName, "不存在");
                return null;
            }

            BasePanel panel = GetPanel(panelName);
            bool isNew = panel == null;

            if (isNew)
            {
                panel = Activator.CreateInstance(type) as BasePanel;
            }

            if (!m_CanPopPanel && panel.panelType == Type.Root)
            {
                m_CanPopPanel = true;
            }

            if (m_CanPopPanel && panel.panelType != Type.Pop)
            {
                if (m_CurrPopPanel != null && m_CurrPopPanel != panel)
                {
                    m_ListPopPanel.Add(m_CurrPopPanel);
                    ClosePanel(m_CurrPopPanel, false);
                }

                m_CurrPopPanel = panel;
            }

            if (!panel.isInit)
            {
                m_QueueWaitLoadPanel.Enqueue(WaitLoadPanel.Create(panel, args));
            }
            else if (!panel.isOpen)
            {
                panel.Open();
            }

            if(isNew)
            {
                m_ListOpenPanel.Add(panel);
            }

            m_ListAlways.Remove(panel);
            m_ListDelayDestroy.Remove(panel);

            return panel;
        }

        private BasePanel GetPanel(string panelName)
        {
            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                if (m_ListOpenPanel[i].panelName.Equals(panelName))
                {
                    return m_ListOpenPanel[i];
                }
            }

            return null;
        }

        private void ClosePanel(string panelName, bool isForceDestroy)
        {
            BasePanel panel = GetPanel(panelName);
            ClosePanel(panel, isForceDestroy);
        }

        private void ClosePanel(BasePanel panel, bool isForceDestroy, bool isPop = true)
        {
            if (panel == null)
            {
                return;
            }

            panel.Close();

            if (panel.panelCloseMode == CloseMode.Destroy || isForceDestroy)
            {
                panel.Destroy();
                GameObjectPool.instance.Put(panel.assetPath, panel.gameObject, true);
                m_ListOpenPanel.Remove(panel);
                m_ListPopPanel.Remove(panel);

                if (m_CurrPopPanel == panel)
                {
                    m_CurrPopPanel = null;
                }
            }
            else if (panel.panelCloseMode == CloseMode.DelayDestroy)
            {
                if (!m_ListDelayDestroy.Contains(panel))
                {
                    m_ListDelayDestroy.Add(panel);
                }
            }
            else if (panel.panelCloseMode == CloseMode.Always)
            {
                if (!m_ListAlways.Contains(panel))
                {
                    m_ListAlways.Add(panel);
                }
            }

            if (m_CanPopPanel && isPop && panel.panelType != Type.Pop && m_ListPopPanel.Count > 0)
            {
                BasePanel oldPanel = m_ListPopPanel[m_ListPopPanel.Count- 1];
                oldPanel.Open();
                m_ListOpenPanel.Add(oldPanel);
                m_ListAlways.Remove(oldPanel);
                m_ListDelayDestroy.Remove(oldPanel);
                m_ListPopPanel.Remove(oldPanel);
                m_CurrPopPanel = oldPanel;
            }
        }

        private void OnLoadComplete(string assetPath, UnityEngine.Object obj, object[] param)
        {
            WaitLoadPanel waitLoadPanel = (param[0] as WaitLoadPanel);
            waitLoadPanel.panel.Init(obj as GameObject, PathUtil.FormatPath(PathUtil.GetUIPrefabPath(), waitLoadPanel.panel.panelName), waitLoadPanel.param);
            ReferencePool.ReleaseReference(waitLoadPanel);
        }

        protected override void OnUpdate()
        {
            if (m_QueueWaitLoadPanel.Count > 0)
            {
                WaitLoadPanel waitLoadPanel = null;
                Queue<WaitLoadPanel> queue = m_QueueWaitLoadPanel;

                lock (queue)
                { 
                    waitLoadPanel = m_QueueWaitLoadPanel.Dequeue();
                    string prefabName = StringUtil.Format(waitLoadPanel.panel.panelName, ".prefab");
                    GameObjectPool.instance.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabPath(), prefabName), OnLoadComplete, waitLoadPanel);
                }
            }

            if (m_ListDelayDestroy.Count > 0)
            {
                for (int i = m_ListDelayDestroy.Count - 1; i >= 0; i++)
                {
                    BasePanel panel = m_ListDelayDestroy[i];

                    if (panel.isDelayTimeOut)
                    {
                        panel.Destroy();
                        GameObjectPool.instance.Put(panel.assetPath, panel.gameObject, true);
                        m_ListOpenPanel.Remove(panel);
                        m_ListDelayDestroy.Remove(panel);
                        m_ListPopPanel.Remove(panel);

                        if(m_CurrPopPanel == panel)
                        {
                            m_CurrPopPanel = null;
                        }
                    }
                }
            }

            if (m_ListAlways.Count > 1)
            {
                BasePanel panel = m_ListAlways[0];
                panel.Destroy();
                GameObjectPool.instance.Put(panel.assetPath, panel.gameObject, true);
                m_ListAlways.Remove(panel);
                m_ListOpenPanel.Remove(panel);
                m_ListPopPanel.Remove(panel);

                if (m_CurrPopPanel == panel)
                {
                    m_CurrPopPanel = null;
                }
            }

            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                if (m_ListOpenPanel[i].isOpen)
                {
                    m_ListOpenPanel[i].Update();
                }
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            for (int i = 0; i < m_ListPopPanel.Count; i++)
            {
                GameObjectPool.instance.Put(m_ListPopPanel[i].assetPath, m_ListPopPanel[i].gameObject);
            }

            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                GameObjectPool.instance.Put(m_ListOpenPanel[i].assetPath, m_ListOpenPanel[i].gameObject);
            }

            for (int i = 0; i < m_ListDelayDestroy.Count; i++)
            {
                GameObjectPool.instance.Put(m_ListDelayDestroy[i].assetPath, m_ListDelayDestroy[i].gameObject);
            }

            for (int i = 0; i < m_ListAlways.Count; i++)
            {
                GameObjectPool.instance.Put(m_ListAlways[i].assetPath, m_ListAlways[i].gameObject);
            }

            m_ListPopPanel.Clear();
            m_ListOpenPanel.Clear();
            m_ListDelayDestroy.Clear();
            m_ListAlways.Clear();
        }

        private bool m_CanPopPanel = false;
        private BasePanel m_CurrPopPanel = null;
        private List<BasePanel> m_ListDelayDestroy = null;
        private List<BasePanel> m_ListAlways = null;
        private List<BasePanel> m_ListPopPanel = null;
        private List<BasePanel> m_ListOpenPanel = null;
        private Queue<WaitLoadPanel> m_QueueWaitLoadPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}