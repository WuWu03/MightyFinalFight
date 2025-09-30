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
        private UnityEngine.Camera m_UICamera = null;
        public UnityEngine.Camera uiCamera
        {
            get
            {
                return m_UICamera;
            }
        }

        private bool m_CanPopPanel = false;
        private IView m_CurrPopView = null;
        private List<IView> m_DelayDestroyPanels = null;
        private List<IView> m_AlwaysPanels = null;
        private List<IView> m_PopPanels = null;
        private List<IView> m_OpenPanels = null;
        private RectTransform[] m_UILayers = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        
        protected override void OnAwake()
        {
            m_OpenPanels = new();
            m_AlwaysPanels = new();
            m_DelayDestroyPanels = new();
            m_PopPanels = new();

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

            Array layers = Enum.GetValues(typeof(UILayer));

            m_UILayers = new RectTransform[layers.Length];

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

                m_UILayers[i] = layerRectTransform;
            }

            DontDestroyOnLoad(m_UIRoot);
        }

        protected override void OnUpdate()
        {
            if (m_DelayDestroyPanels.Count > 0)
            {
                for (int i = m_DelayDestroyPanels.Count - 1; i >= 0; i++)
                {
                    IView view = m_DelayDestroyPanels[i];
                    bool isDelayTimeOut = false;

                    if (view.settings.CloseMode == UICloseMode.DelayDestroy && view.delayTime > 0f)
                    {
                        isDelayTimeOut = Time.time - view.delayTime >= view.settings.unLoadTime;
                    }

                    if (!isDelayTimeOut)
                    {
                        continue;
                    }

                    GameObjectPoolMgr.instance.Put(view.assetPath, view.gameObject, true);
                    view.Destroy();
                    m_DelayDestroyPanels.Remove(view);
                    m_PopPanels.Remove(view);

                    if (m_CurrPopView == view)
                    {
                        m_CurrPopView = null;
                    }
                }
            }

            if (m_AlwaysPanels.Count > 1)
            {
                IView view = m_AlwaysPanels[0];
                GameObjectPoolMgr.instance.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_AlwaysPanels.Remove(view);
                m_PopPanels.Remove(view);

                if (m_CurrPopView == view)
                {
                    m_CurrPopView = null;
                }
            }

            foreach (var panel in m_OpenPanels)
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

            foreach (var panel in m_PopPanels)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_OpenPanels)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_DelayDestroyPanels)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            foreach (var panel in m_AlwaysPanels)
            {
                GameObjectPoolMgr.instance.Put(panel.assetPath, panel.gameObject);
            }

            m_DelayDestroyPanels.Clear();
            m_AlwaysPanels.Clear();
            m_PopPanels.Clear();
            m_OpenPanels.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_CanPopPanel = false;
            m_CurrPopView = null;
            m_DelayDestroyPanels = null;
            m_AlwaysPanels = null;
            m_PopPanels = null;
            m_OpenPanels = null;
        }

        public RectTransform GetPanelLayer(UILayer layer)
        {
            return m_UILayers[Convert.ToInt32(layer)];
        }

        public IView Open(string uiName, object arg = null)
        {
            return OpenPanel(uiName, arg);
        }

        public IView Get(string uiName)
        {
            return GetPanel(uiName);
        }

        public bool IsOpen(string panelName)
        {
            IView view = GetPanel(panelName);
            return view != null && view.isOpen;
        }

        public void Close(string paneTypeName, bool isForceDestroy = false)
        {
            ClosePanel(paneTypeName, isForceDestroy);
        }

        public void Close(IView view, bool isForceDestroy = false)
        {
            if (view == null)
            {
                return;
            }

            ClosePanel(view.settings.name, isForceDestroy);
        }

        private IView OpenPanel(string panelName, object arg)
        {
            System.Type panelType = GetPanelType(panelName);

            if (panelType == null)
            {
                return null;
            }

            IView view = GetPanel(panelName);
 
            if (view == null)
            {
                view = Activator.CreateInstance(panelType) as IView;
            }

            if (view == null)
            {
                return null;
            }
            
            m_OpenPanels.Add(view);
            m_AlwaysPanels.Remove(view);
            m_DelayDestroyPanels.Remove(view);
            
            if (!m_CanPopPanel && view != null && view.settings.canPopUp)
            {
                m_CanPopPanel = true;
            }

            if (m_CanPopPanel && view != null && view.settings.canPopUp)
            {
                if (m_CurrPopView != null && m_CurrPopView != view)
                {
                    m_PopPanels.Add(m_CurrPopView);
                    ClosePanel(m_CurrPopView, false, false);
                }

                m_CurrPopView = view;
            }

            view.Open(arg);
            return view;
        }

        private IView GetPanel(string panelName)
        {
            System.Type panelType = GetPanelType(panelName);

            if (panelType == null)
            {
                return null;
            }

            foreach (var panel in m_OpenPanels)
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
            IView view = GetPanel(panelTypeName);
            ClosePanel(view, isForceDestroy);
        }

        private void ClosePanel(IView view, bool isForceDestroy, bool checkPopPanel = true)
        {
            if (view == null)
            {
                return;
            }

            view.Close();

            if (view.settings.CloseMode == UICloseMode.Destroy || isForceDestroy)
            {
                GameObjectPoolMgr.instance.Put(view.assetPath, view.gameObject, true);
                view.Destroy();
                m_OpenPanels.Remove(view);
                m_PopPanels.Remove(view);

                if (m_CurrPopView == view)
                {
                    m_CurrPopView = null;
                }
            }
            else if (view.settings.CloseMode == UICloseMode.DelayDestroy)
            {
                if (!m_DelayDestroyPanels.Contains(view))
                {
                    m_DelayDestroyPanels.Add(view);
                }
            }
            else if (view.settings.CloseMode == UICloseMode.Always)
            {
                if (!m_AlwaysPanels.Contains(view))
                {
                    m_AlwaysPanels.Add(view);
                }
            }

            if (checkPopPanel && m_CanPopPanel && !view.settings.canPopUp && m_PopPanels.Count > 0)
            {
                IView oldView = m_PopPanels[^1];
                oldView.Open(null);
                m_OpenPanels.Add(oldView);
                m_AlwaysPanels.Remove(oldView);
                m_DelayDestroyPanels.Remove(oldView);
                m_PopPanels.Remove(oldView);
                m_CurrPopView = oldView;
            }
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


    }
}