using System;
using UnityEngine;
using WuWuFramework.Camera;

namespace WuWuFramework.UI
{
    public class UIRoot : MonoBehaviour
    {
        public RectTransform[] uiLayers;
        public Canvas uiCanvas;
        public UnityEngine.Camera uiCamera;

        private void Awake()
        {
            WuWuFrameworkMgr.GetModule<ICameraMgr>().AddUICamera(uiCamera);
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 获取层级
        /// </summary>
        /// <param name="layer">层级类型</param>
        /// <returns></returns>
        public RectTransform GetLayer(UILayer layer)
        {
            return uiLayers[Convert.ToInt32(layer)];
        }
    }
}