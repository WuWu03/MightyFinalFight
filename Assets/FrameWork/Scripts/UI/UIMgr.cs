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
            Destroy = 2,        //关闭时立即销毁
            DelayDestroy = 3,   // 延迟一段时间销毁
            Eternal = 4,        // 总是存于场景中, 除非主动销毁
        }

        private void Awake()
        {
            m_ListOpenPanel = new List<BasePanelCtrl>();
            m_StackMutexPanel = new Stack<BasePanelCtrl>();
            m_QueueDelayDestroy = new Queue<BasePanelCtrl>();
            m_QueueAlways = new Queue<BasePanelCtrl>();

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

            GameObject.DontDestroyOnLoad(m_UIRoot);
        }

        public void Open<T>(VoidNotPar callback = null, params object[] param) where T : BasePanel, new()
        {
            InnerOpen(typeof(T).Name, callback, param);
        }

        public void Open(string panelName, VoidNotPar callback = null, params object[] param)
        {
            InnerOpen(panelName, callback, param);
        }

        public T GetPanel<T>() where T : BasePanelCtrl
        {
            return InnerGet(typeof(T).Name.Replace("Ctrl", "")) as T;
        }

        public T GetPanel<T>(string panelName) where T : BasePanelCtrl
        {
            return InnerGet(panelName) as T;
        }

        public bool IsPanelOpen<T>()
        {
            BasePanelCtrl ctrl = InnerGet(typeof(T).Name);
            return ctrl != null && ctrl.IsOpen;
        }

        public bool IsPanelOpen(string panelName)
        {
            BasePanelCtrl ctrl = InnerGet(panelName);
            return ctrl != null && ctrl.IsOpen;
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


        private void InnerOpen(string panelName, VoidNotPar callback, object[] param)
        {
            System.Type type = System.Type.GetType(panelName + "Ctrl");

            if (type == null)
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

            if (ctrl.Panel.PanelType == Type.Pop) return;

            if (m_StackMutexPanel.Count > 0)
            {
                m_StackMutexPanel.Peek().Close(null);
            }

            m_StackMutexPanel.Push(ctrl);
        }

        private void InnerClose(string panelName, VoidNotPar callback)
        {
            BasePanelCtrl ctrl = InnerGet(panelName);

            if (ctrl == null) return;

            ctrl.Close(callback);

            if (ctrl.Panel.PanelCloseMode == CloseMode.DelayDestroy)
            {
                Queue<BasePanelCtrl> queue = m_QueueDelayDestroy;
                lock(queue)
                {
                    m_QueueDelayDestroy.Enqueue(ctrl);
                }
            }

            if(ctrl.Panel.PanelCloseMode == CloseMode.Always)
            {
                Queue<BasePanelCtrl> queue = m_QueueAlways;
                lock (queue)
                {
                    m_QueueAlways.Enqueue(ctrl);
                }
            }

            if (ctrl.Panel.PanelCloseMode == CloseMode.Destroy)
            {
                m_ListOpenPanel.Remove(ctrl);
            }

            if (ctrl.Panel.PanelType == Type.Pop) return;
            if (m_StackMutexPanel.Count < 1) return;

            BasePanelCtrl top = m_StackMutexPanel.Pop();

            if (m_StackMutexPanel.Count < 1)
            {
                if (top.Panel.PanelType == Type.Root)
                    m_StackMutexPanel.Push(top);
                return;
            }

            m_StackMutexPanel.Peek().Open();
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

        private void Update()
        {
            for (int i = 0; i < m_ListOpenPanel.Count; i++)
            {
                if (m_ListOpenPanel[i].IsOpen)
                    m_ListOpenPanel[i].Update();
            }

            if (m_QueueDelayDestroy.Count > 0 && m_QueueDelayDestroy.Peek().IsDelayTimeOut)
            {
                BasePanelCtrl ctrl = null;
                Queue<BasePanelCtrl> queue = m_QueueDelayDestroy;

                lock (queue)
                {
                    ctrl = m_QueueDelayDestroy.Dequeue();
                    ctrl.Destroy(true);
                    m_ListOpenPanel.Remove(ctrl);
                }
            }

            if(m_QueueAlways.Count > 10)
            {
                BasePanelCtrl ctrl = null;
                Queue<BasePanelCtrl> queue = m_QueueAlways;

                lock (queue)
                {
                    ctrl = m_QueueAlways.Dequeue();
                    ctrl.Destroy(true);
                    m_ListOpenPanel.Remove(ctrl);
                }
            }
        }

        public override void ShutDown()
        {
            m_StackMutexPanel.Clear();
            m_ListOpenPanel.Clear();
            m_QueueDelayDestroy.Clear();
            m_QueueAlways.Clear();
        }

        private Queue<BasePanelCtrl> m_QueueDelayDestroy = null;
        private Queue<BasePanelCtrl> m_QueueAlways = null;
        private Stack<BasePanelCtrl> m_StackMutexPanel = null;
        private List<BasePanelCtrl> m_ListOpenPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}