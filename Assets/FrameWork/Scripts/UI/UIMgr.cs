using FrameWork.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIMgr : BaseMgr<UIMgr>
    {    
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
            Destroy = 2,        //关闭时立即销毁
            DelayDestroy = 3,   // 延迟一段时间销毁
            Eternal = 4,        // 总是存于场景中, 除非主动销毁
        }

        private void Awake()
        {
            m_DicPanelMap = new Dictionary<string, Type>();
            m_ListOpenPanel = new List<BasePanelCtrl>();
            m_QueueMutexPanel = new Queue<BasePanelCtrl>();
            m_UIRoot = new GameObject("UIRoot");
            m_UICanvas = new GameObject("UICanvas", typeof(GraphicRaycaster)).GetOrAddComponent<Canvas>();
            m_UICamera = new GameObject("UICamera").GetOrAddComponent<UnityEngine.Camera>();
            m_EventSystem = new GameObject("EventSystem").GetOrAddComponent<EventSystem>();
            m_UICanvas.gameObject.AddComponent<GraphicRaycaster>();

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
                m_UILayerTransform[i].anchoredPosition = Vector3.zero;
                m_UILayerTransform[i].sizeDelta = Vector2.zero;
                m_UILayerTransform[i].anchorMin = new Vector2(0, 0);
                m_UILayerTransform[i].anchorMax = new Vector2(1, 1);
                m_UILayerTransform[i].pivot = new Vector2(0.5f, 0.5f);
                m_UILayerTransform[i].SetParent(m_UICanvas.transform, false);
            }

            GameObject.DontDestroyOnLoad(m_UIRoot);
        }

        public void Open<T>(VoidNotPar callback = null,params object[] param) where T:BasePanel,new()
        {
            InnerOpen(typeof(T).Name,callback, param);
        }

        public void Open(string panelName,VoidNotPar callback = null, params object[] param)
        {
            InnerOpen(panelName, callback, param);
        }

        public BasePanelCtrl GetPanel<T>()
        {
            return InnerGet(typeof(T).Name);
        }

        public BasePanelCtrl GetPanel(string panelName)
        {
            return InnerGet(panelName);
        }

        public void Close<T>(VoidNotPar callback = null)
        {
            InnerClose(typeof(T).Name, callback);
        }

        public void Close(string panelName,VoidNotPar callback = null)
        {
            InnerClose(panelName, callback);
        }

        public Transform GetUILayer(Layer layer)
        {
            return m_UILayerTransform[Convert.ToInt32(layer)];
        }

        public void AddPanelMap<T>(string panelName) where T : BasePanelCtrl
        {
            m_DicPanelMap.Add(panelName, typeof(T));
        }

        public void AddPanelMap(string panelName,Type type)
        {
            if (typeof(BasePanelCtrl) == type)
                m_DicPanelMap.Add(panelName, type);
        }

        private void InnerOpen(string panelName, VoidNotPar callback, object[] param)
        {
            Type type = null;

            if (!m_DicPanelMap.TryGetValue(panelName, out type))
            {
                Debug.LogError("Panel is invalid!");
                return;
            }

            BasePanelCtrl ctrl = InnerGet(panelName);
            if (ctrl != null && ctrl.IsOpen) return;

            if (ctrl == null)
            {
                ctrl = Activator.CreateInstance(type) as BasePanelCtrl;
                m_ListOpenPanel.Add(ctrl);
            }

            ctrl.Open(callback, param);

            if (!IsMutex(ctrl.Panel.PanelLayer)) return;
            if (m_QueueMutexPanel.Count < 1) return;
            if (m_QueueMutexPanel.Peek().Equals(ctrl)) return;

            InnerClose(m_QueueMutexPanel.Peek().Panel.PanelName, null);
            m_QueueMutexPanel.Enqueue(ctrl);
        }

        private void InnerClose(string panelName,VoidNotPar callback)
        {
            BasePanelCtrl ctrl = InnerGet(panelName);

            if (ctrl == null) return;
            
            if (m_QueueMutexPanel.Count > 0 && m_QueueMutexPanel.Contains(ctrl) && IsMutex(ctrl.Panel.PanelLayer))
            {
                if (m_QueueMutexPanel.Peek().Panel.PanelLayer != Layer.MainPanel)
                {
                    m_QueueMutexPanel.Dequeue();
                }

                if (m_QueueMutexPanel.Count > 0)
                    m_QueueMutexPanel.Peek().Open();
            }

            ctrl.Close(callback);

            if (ctrl.Panel.PanelCloseMode == CloseMode.Destroy)
                m_ListOpenPanel.Remove(ctrl);
        }

        private BasePanelCtrl InnerGet(string panelName)
        {
            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                if (m_ListOpenPanel[i].Panel.PanelName.Equals(panelName))
                {
                    return m_ListOpenPanel[i];
                }
            }

            return null;
        }

        private bool IsMutex(Layer layer)
        {
            for (int i = 0; i < m_MutexLayers.Length; i++)
            {
                if (layer == m_MutexLayers[i])
                {
                    return true;
                }
            }

            return false;
        }

        private void Update()
        {
            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                m_ListOpenPanel[i].Update();
            }
        }

        public override void ShutDown()
        {
            m_DicPanelMap.Clear();
            m_QueueMutexPanel.Clear();
            m_ListOpenPanel.Clear();
        }

        private Layer[] m_MutexLayers = new Layer[]
        {
            Layer.MainPanel,
            Layer.FirstLevel
        };

        private Dictionary<string, Type> m_DicPanelMap = null;
        private Queue<BasePanelCtrl> m_QueueMutexPanel = null;
        private List<BasePanelCtrl> m_ListOpenPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}