using UnityEngine;
using UnityEngine.UI;

namespace WuWuFramework.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class UICanvasScaleAdapt : MonoBehaviour
    {
        public enum Type
        {
            Width = 1,
            Height = 2,
            WidthOrHeight = 3,
        }

        
        [SerializeField] private Type m_ScalerType = Type.WidthOrHeight;
        public Type ScalerType
        {
            get
            {
                return m_ScalerType;
            }
            set
            {
                m_ScalerType = value;
                UpdateScaleType();
            }
        }

        private CanvasScaler m_CanvasScale;
        
        private void Awake()
        {
            m_CanvasScale = GetComponent<CanvasScaler>();
            m_CanvasScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            m_CanvasScale.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            UpdateScaleType();
        }

        private void UpdateScaleType()
        {
            float realScreenRatio = (float)Screen.width / (float)Screen.height;
            float currScreenRatio = m_CanvasScale.referenceResolution.x / m_CanvasScale.referenceResolution.y;
            float diff = currScreenRatio / realScreenRatio;

            switch (m_ScalerType)
            {
                case Type.Width:
                    m_CanvasScale.matchWidthOrHeight = 0;
                    break;
                case Type.Height:
                    m_CanvasScale.matchWidthOrHeight = 1;
                    break;
                case Type.WidthOrHeight:
                    m_CanvasScale.matchWidthOrHeight = diff < 1 ? 1 : 0;
                    break;
                default:
                    break;
            }
        }
    }
}