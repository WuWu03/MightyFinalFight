using System;
using UnityEngine;
using WuWuFramework.Camera;

namespace WuWuFramework.UI
{
    public class UIRoot : MonoBehaviour
    {
        private Canvas m_UICanvas;
        private UnityEngine.Camera m_UICamera;
        private RectTransform[] m_UILayers;

        public Canvas uiCanvas
        {
            get { return m_UICanvas; }
        }

        public UnityEngine.Camera uiCamera
        {
            get { return m_UICamera; }
        }

        private void Awake()
        {
            string[] uiLayerNames = Enum.GetNames(typeof(UILayer));
            m_UICanvas = transform.Find("UICanvas").GetOrAddComponent<Canvas>();
            m_UICamera = transform.Find("UICamera").GetOrAddComponent<UnityEngine.Camera>();
            m_UILayers = new RectTransform[uiLayerNames.Length];

            for (int i = 0; i < uiLayerNames.Length; i++)
            {
                m_UILayers[i] = m_UICanvas.transform.Find(uiLayerNames[i]).GetComponent<RectTransform>();
            }

            WuWuFrameworkMgr.GetModule<ICameraMgr>().AddUICamera(m_UICamera);
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 获取层级
        /// </summary>
        /// <param name="layer">层级类型</param>
        /// <returns></returns>
        public RectTransform GetLayer(UILayer layer)
        {
            return m_UILayers[Convert.ToInt32(layer)];
        }
    }
}