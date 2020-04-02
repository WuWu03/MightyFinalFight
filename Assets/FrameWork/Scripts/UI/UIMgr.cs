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
        public enum UILayer
        {
            BG,
            MainPanel,
            FirstLevel,
            SecondLevel,
            ThirdLevel,
        }

        protected override void Awake()
        {
            m_DicSprite = new Dictionary<string, Sprite>();
            m_QueueOpenPanel = new Queue<BasePanel>();
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

            Array layers = Enum.GetValues(typeof(UILayer));

            m_UILayerTransform = new RectTransform[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                m_UILayerTransform[i] = new GameObject(Enum.GetValues(typeof(UILayer)).GetValue(i).ToString()).AddComponent<RectTransform>();
                m_UILayerTransform[i].anchoredPosition = Vector3.zero;
                m_UILayerTransform[i].sizeDelta = Vector2.zero;
                m_UILayerTransform[i].anchorMin = new Vector2(0, 0);
                m_UILayerTransform[i].anchorMax = new Vector2(1, 1);
                m_UILayerTransform[i].pivot = new Vector2(0.5f, 0.5f);
                m_UILayerTransform[i].SetParent(m_UICanvas.transform, false);
            }

            GameObject.DontDestroyOnLoad(m_UIRoot);
        }

        public void AddPanel(BasePanel panel)
        {
            if(m_QueueOpenPanel.Contains(panel))
            {
                return;
            }

            for (int i = 0; i < m_MutexLayers.Length; i++)
            {
                if (panel.PanelLayer.Equals(m_MutexLayers))
                {
                    m_QueueOpenPanel.Enqueue(panel);
                    break;
                } 
            }
        }

        public void RemovePanel(BasePanel panel)
        {
            if (!m_QueueOpenPanel.Contains(panel))
            {
                return;
            }

            m_QueueOpenPanel.Dequeue();
            
            if(m_QueueOpenPanel.Count >0)
            {
                m_QueueOpenPanel.Peek().Open();
            }
        }

        public BasePanel GetCurrPanel()
        {
            return m_QueueOpenPanel.Peek();
        }

        public Transform GetUILayer(UILayer layer)
        {
            return m_UILayerTransform[Convert.ToInt32(layer)];
        }

        public void SetIconSprite(string path, Image renderer)
        {
            Sprite sprite = null;

            if (m_DicSprite.TryGetValue(path, out sprite))
            {
                renderer.sprite = sprite;
                return;
            }

            Action<UnityEngine.Object> action = delegate (UnityEngine.Object obj)
            {
                renderer.sprite = obj as Sprite;

                if (!m_DicSprite.ContainsKey(path))
                {
                    m_DicSprite.Add(path, renderer.sprite);
                }
            };

            string loadPath = string.Format("{0}/{1}", ResDefine.ICON_PATH, path);
            ResMgr.Ins.LoadAsset(loadPath, action, true, typeof(Sprite));
        }

        protected override void Update()
        {

        }

        public override void ShutDown()
        {
            throw new NotImplementedException();
        }

        private UILayer[] m_MutexLayers = new UILayer[]
        {
            UILayer.FirstLevel
        };

        private Queue<BasePanel> m_QueueOpenPanel = null;
        private RectTransform[] m_UILayerTransform = null;
        private Dictionary<string, Sprite> m_DicSprite = null;
        private GameObject m_UIRoot = null;
        private Canvas m_UICanvas = null;
        private EventSystem m_EventSystem = null;
        private UnityEngine.Camera m_UICamera = null;
    }
}