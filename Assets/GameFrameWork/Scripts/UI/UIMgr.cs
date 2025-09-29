using GameFrameWork.Pool;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class UIMgr : BaseMgr<UIMgr>
    {
        private class OpenPanelArgs : BaseEventArgs
        {
            public IPanel panel;
            public object arg;

            public static OpenPanelArgs Create(IPanel panel, object arg)
            {
                OpenPanelArgs waitLoadPanel = ReferencePool.Acquire<OpenPanelArgs>();
                waitLoadPanel.panel = panel;
                waitLoadPanel.arg = arg;
                return waitLoadPanel;
            }

            public override void Clear()
            {
                base.Clear();
                panel = null;
                arg = null;
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
            m_ListOpenPanel = new();
            m_ListAlways = new();
            m_ListDelayDestroy = new();
            m_ListPopPanel = new();
            m_QueueWaitLoadPanel = new();

            m_UIRoot = new("UIRoot");
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
            m_UICanvas.vertexColorAlwaysGammaSpace = true;

            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.referencePixelsPerUnit = 100f;
            canvasScaleAdapt.ScalerType = UICanvasScaleAdapt.Type.WidthOrHeight;
            inputModule.submitButton = "A";
            inputModule.cancelButton = "B";

            m_UIRoot.SetLayer("UI");

            Array layers = Enum.GetValues(typeof(PanelLayer));

            m_UILayerTransform = new RectTransform[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                GameObject layerGameObject = new(layers.GetValue(i).ToString());
                RectTransform layerRectTransform = layerGameObject.AddComponent<RectTransform>();
                Canvas layerCanvas = layerGameObject.GetOrAddComponent<Canvas>();

                layerRectTransform.anchoredPosition = Vector3.zero;
                layerRectTransform.sizeDelta = Vector2.zero;
                layerRectTransform.anchorMin = new Vector2(0, 0);
                layerRectTransform.anchorMax = new Vector2(1, 1);
                layerRectTransform.pivot = new Vector2(0.5f, 0.5f);
                layerRectTransform.SetParent(m_UICanvas.transform, false);

                layerGameObject.SetLayer("UI");

                layerCanvas.overrideSorting = true;
                layerCanvas.sortingOrder = (i + 1) * 1000;
                layerCanvas.vertexColorAlwaysGammaSpace = true;

                m_UILayerTransform[i] = layerRectTransform;
            }

            DontDestroyOnLoad(m_UIRoot);
        }

        protected override void OnUpdate()
        {
            if (m_QueueWaitLoadPanel.Count > 0)
            {
                Queue<OpenPanelArgs> queue = m_QueueWaitLoadPanel;

                lock (queue)
                {
                    OpenPanelArgs waitLoadPanel = m_QueueWaitLoadPanel.Dequeue();
                    string prefabName = StringUtil.Append(waitLoadPanel.panel.settings.panelName, ".prefab");
                    GameObjectPoolMgr.instance.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabsPath(), prefabName), OnLoadComplete, waitLoadPanel);
                }
            }

            if (m_ListDelayDestroy.Count > 0)
            {
                for (int i = m_ListDelayDestroy.Count - 1; i >= 0; i++)
                {
                    IPanel panel = m_ListDelayDestroy[i];
                    bool isDelayTimeOut = panel.settings.panelCloseMode == PanelCloseMode.DelayDestroy && panel.delayTime > 0f && Time.time - panel.delayTime >= 5f;

                    if (!isDelayTimeOut)
                    {
                        continue;
                    }
                    
                    GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject, true);
                    panel.Destroy();
                    m_ListDelayDestroy.Remove(panel);
                    m_ListPopPanel.Remove(panel);

                    if (m_CurrPopPanel == panel)
                    {
                        m_CurrPopPanel = null;
                    }
                }
            }

            if (m_ListAlways.Count > 1)
            {
                IPanel panel = m_ListAlways[0];
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject, true);
                panel.Destroy();
                m_ListAlways.Remove(panel);
                m_ListPopPanel.Remove(panel);

                if (m_CurrPopPanel == panel)
                {
                    m_CurrPopPanel = null;
                }
            }

            foreach (var panel in m_ListOpenPanel)
            {
                if (panel.isOpen)
                {
                    panel.Update();
                }
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach (var panel in m_ListPopPanel)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_ListOpenPanel)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_ListDelayDestroy)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_ListAlways)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            m_ListDelayDestroy.Clear();
            m_ListAlways.Clear();
            m_ListPopPanel.Clear();
            m_ListOpenPanel.Clear();
            m_QueueWaitLoadPanel.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_CanPopPanel = false;
            m_CurrPopPanel = null;
            m_ListDelayDestroy = null;
            m_ListAlways = null;
            m_ListPopPanel = null;
            m_ListOpenPanel = null;
            m_QueueWaitLoadPanel = null;
        }

        public RectTransform GetPanelLayer(PanelLayer layer)
        {
            return m_UILayerTransform[Convert.ToInt32(layer)];
        }

        public IPanel Open(string uiName, object arg = null)
        {
            return OpenPanel(uiName, arg);
        }

        public IPanel Get(string uiName)
        {
            return GetPanel(uiName);
        }

        public bool IsOpen(string panelName)
        {
            IPanel panel = GetPanel(panelName);
            return panel != null && panel.isOpen;
        }

        public void Close(string paneTypeName, bool isForceDestroy = false)
        {
            ClosePanel(paneTypeName, isForceDestroy);
        }

        public void Close(IPanel panel, bool isForceDestroy = false)
        {
            if (panel == null)
            {
                return;
            }

            ClosePanel(panel.settings.panelName, isForceDestroy);
        }

        private IPanel OpenPanel(string panelName, object arg)
        {
            System.Type panelType = GetPanelType(panelName);

            if (panelType == null)
            {
                return null;
            }

            IPanel panel = GetPanel(panelName);
            bool isNew = panel == null;

            if (isNew)
            {
                panel = Activator.CreateInstance(panelType) as IPanel;
            }

            if (!m_CanPopPanel && panel != null && panel.settings.panelType == PanelType.Root)
            {
                m_CanPopPanel = true;
            }

            if (m_CanPopPanel && panel != null && panel.settings.panelType != PanelType.Pop)
            {
                if (m_CurrPopPanel != null && m_CurrPopPanel != panel)
                {
                    m_ListPopPanel.Add(m_CurrPopPanel);
                    ClosePanel(m_CurrPopPanel, false, false);
                }

                m_CurrPopPanel = panel;
            }
            
            if (panel is { isInit: false })
            {
                m_QueueWaitLoadPanel.Enqueue(OpenPanelArgs.Create(panel, arg));
            }
            else if (panel is { isOpen: false })
            {
                panel.Open();
            }

            if (isNew)
            {
                m_ListOpenPanel.Add(panel);
            }

            m_ListAlways.Remove(panel);
            m_ListDelayDestroy.Remove(panel);

            return panel;
        }

        private IPanel GetPanel(string panelName)
        {
            System.Type panelType = GetPanelType(panelName);

            if (panelType == null)
            {
                return null;
            }

            foreach (var panel in m_ListOpenPanel)
            {
                if (panel.GetType() == panelType)
                {
                    return panel;
                }
            }

            return null;
        }

        private void ClosePanel(string panelTypeName, bool isForceDestroy)
        {
            IPanel panel = GetPanel(panelTypeName);
            ClosePanel(panel, isForceDestroy);
        }

        private void ClosePanel(IPanel panel, bool isForceDestroy, bool checkPopPanel = true)
        {
            if (panel == null)
            {
                return;
            }

            PanelType panelType = panel.settings.panelType;
            panel.Close();

            if (panel.settings.panelCloseMode == PanelCloseMode.Destroy || isForceDestroy)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject, true);
                panel.Destroy();
                m_ListOpenPanel.Remove(panel);
                m_ListPopPanel.Remove(panel);

                if (m_CurrPopPanel == panel)
                {
                    m_CurrPopPanel = null;
                }
            }
            else if (panel.settings.panelCloseMode == PanelCloseMode.DelayDestroy)
            {
                if (!m_ListDelayDestroy.Contains(panel))
                {
                    m_ListDelayDestroy.Add(panel);
                }
            }
            else if (panel.settings.panelCloseMode == PanelCloseMode.Always)
            {
                if (!m_ListAlways.Contains(panel))
                {
                    m_ListAlways.Add(panel);
                }
            }

            if (checkPopPanel && m_CanPopPanel && panelType != PanelType.Pop && m_ListPopPanel.Count > 0)
            {
                IPanel oldPanel = m_ListPopPanel[^1];
                oldPanel.Open();
                m_ListOpenPanel.Add(oldPanel);
                m_ListAlways.Remove(oldPanel);
                m_ListDelayDestroy.Remove(oldPanel);
                m_ListPopPanel.Remove(oldPanel);
                m_CurrPopPanel = oldPanel;
            }
        }

        private void OnLoadComplete(string assetPath, UnityEngine.Object obj, object arg)
        {
            if (arg is not OpenPanelArgs openPanelArgs)
            {
                return;
            }
            
            openPanelArgs.panel.Init(obj as GameObject, assetPath, openPanelArgs.arg);
            openPanelArgs.Release();
        }

        private System.Type GetPanelType(string panelName)
        {
            System.Type type = System.Type.GetType(panelName);

            if (type == null)
            {
                Log.LogError(panelName, "不存在");
            }

            return type;
        }

        private bool m_CanPopPanel = false;
        private IPanel m_CurrPopPanel = null;
        private List<IPanel> m_ListDelayDestroy = null;
        private List<IPanel> m_ListAlways = null;
        private List<IPanel> m_ListPopPanel = null;
        private List<IPanel> m_ListOpenPanel = null;
        private Queue<OpenPanelArgs> m_QueueWaitLoadPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}