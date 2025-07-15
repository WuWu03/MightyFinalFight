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
            public IPanel panel;
            public object[] param;

            public static WaitLoadPanel Create(IPanel panel, object[] param)
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
            m_ListOpenPanel = new List<IPanel>();
            m_ListAlways = new List<IPanel>();
            m_ListDelayDestroy = new List<IPanel>();
            m_ListPopPanel = new List<IPanel>();
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
            m_UICanvas.vertexColorAlwaysGammaSpace = true;

            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.referencePixelsPerUnit = 100f;
            canvasScaleAdapt.ScalerType = UICanvasScaleAdapt.Type.WidthOrHeight;
            inputModule.submitButton = "A";
            inputModule.cancelButton = "B";

            m_UIRoot.SetLayer("UI");

            Array layers = Enum.GetValues(typeof(Layer));

            m_UILayerTransform = new RectTransform[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                GameObject layerGameObject = new GameObject(layers.GetValue(i).ToString());
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

        public Transform GetUILayer(Layer layer)
        {
            return m_UILayerTransform[Convert.ToInt32(layer)];
        }

        public IPanel Open(string panelTypeName, params object[] args)
        {
            return OpenPanel(panelTypeName, args);
        }

        public IPanel Get(string panelTypeName)
        {
            return GetPanel(panelTypeName);
        }

        public bool IsOpen(string panelTypeName)
        {
            IPanel panel = GetPanel(panelTypeName);
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

        private IPanel OpenPanel(string panelTypeName, object[] args)
        {
            System.Type type = GetPanelType(panelTypeName);

            if (type == null)
            {
                return null;
            }

            IPanel panel = GetPanel(panelTypeName) ;
            bool isNew = panel == null;

            if (isNew)
            {
                panel = Activator.CreateInstance(type) as IPanel;
            }

            if (!m_CanPopPanel && panel.settings.panelType == Type.Root)
            {
                m_CanPopPanel = true;
            }

            if (m_CanPopPanel && panel.settings.panelType != Type.Pop)
            {
                if (m_CurrPopPanel != null && m_CurrPopPanel != panel)
                {
                    m_ListPopPanel.Add(m_CurrPopPanel);
                    ClosePanel(m_CurrPopPanel, false, false);
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

            if (isNew)
            {
                m_ListOpenPanel.Add(panel);
            }

            m_ListAlways.Remove(panel);
            m_ListDelayDestroy.Remove(panel);

            return panel;
        }

        private IPanel GetPanel(string paneTypelName)
        {
            System.Type panelType = GetPanelType(paneTypelName);

            if (panelType == null)
            {
                return null;
            }

            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                IPanel panel = m_ListOpenPanel[i];

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

            panel.Close();

            if (panel.settings.panelCloseMode == CloseMode.Destroy || isForceDestroy)
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
            else if (panel.settings.panelCloseMode == CloseMode.DelayDestroy)
            {
                if (!m_ListDelayDestroy.Contains(panel))
                {
                    m_ListDelayDestroy.Add(panel);
                }
            }
            else if (panel.settings.panelCloseMode == CloseMode.Always)
            {
                if (!m_ListAlways.Contains(panel))
                {
                    m_ListAlways.Add(panel);
                }
            }

            if (checkPopPanel && m_CanPopPanel && panel.settings.panelType != Type.Pop && m_ListPopPanel.Count > 0)
            {
                IPanel oldPanel = m_ListPopPanel[m_ListPopPanel.Count - 1];
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
            waitLoadPanel.panel.Init(obj as GameObject, assetPath, waitLoadPanel.param);
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
                    string prefabName = StringUtil.Append(waitLoadPanel.panel.settings.panelName, ".prefab");
                    GameObjectPoolMgr.instance.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabsPath(), prefabName), OnLoadComplete, waitLoadPanel);
                }
            }

            if (m_ListDelayDestroy.Count > 0)
            {
                for (int i = m_ListDelayDestroy.Count - 1; i >= 0; i++)
                {
                    IPanel panel = m_ListDelayDestroy[i];
                    bool isDelayTimeOut = panel.settings.panelCloseMode == CloseMode.DelayDestroy && panel.delayTime > 0f && Time.time - panel.delayTime >= 5f;

                    if (isDelayTimeOut)
                    {
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

            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                IPanel panel = m_ListOpenPanel[i];
                if (panel.isOpen)
                {
                    panel.Update();
                }
            }
        }

        private System.Type GetPanelType(string panelTypeName)
        {
            System.Type type = System.Type.GetType(panelTypeName);

            if (type == null)
            {
                Log.LogError(panelTypeName, "不存在");
            }

            return type;
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            for (int i = 0; i < m_ListPopPanel.Count; i++)
            {
                GameObjectPoolMgr.instance.Put(m_ListPopPanel[i].assetPath, m_ListPopPanel[i].gameObject);
            }

            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                GameObjectPoolMgr.instance.Put(m_ListOpenPanel[i].assetPath, m_ListOpenPanel[i].gameObject);
            }

            for (int i = 0; i < m_ListDelayDestroy.Count; i++)
            {
                GameObjectPoolMgr.instance.Put(m_ListDelayDestroy[i].assetPath, m_ListDelayDestroy[i].gameObject);
            }

            for (int i = 0; i < m_ListAlways.Count; i++)
            {
                GameObjectPoolMgr.instance.Put(m_ListAlways[i].assetPath, m_ListAlways[i].gameObject);
            }

            m_ListPopPanel.Clear();
            m_ListOpenPanel.Clear();
            m_ListDelayDestroy.Clear();
            m_ListAlways.Clear();
        }

        private bool m_CanPopPanel = false;
        private IPanel m_CurrPopPanel = null;
        private List<IPanel> m_ListDelayDestroy = null;
        private List<IPanel> m_ListAlways = null;
        private List<IPanel> m_ListPopPanel = null;
        private List<IPanel> m_ListOpenPanel = null;
        private Queue<WaitLoadPanel> m_QueueWaitLoadPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}