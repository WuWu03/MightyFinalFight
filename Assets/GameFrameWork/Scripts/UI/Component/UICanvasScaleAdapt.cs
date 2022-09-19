using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class UICanvasScaleAdapt : MonoBehaviour
    {
        public static float GameScreenRatio = 1280f / 720f;

        /// <summary>
        /// 方式1 方式2 等同于CanvasScale上面的
        /// 方式3 如果宽高比小于标准 则按宽适配 如果宽高比大于标准 则按高
        /// </summary>
        public enum Type
        {
            Width = 1,
            Height = 2,
            WidthOrHeight = 3,
        }

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

        // Use this for initialization
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
            float diff = GameScreenRatio / realScreenRatio;

            switch (m_ScalerType)
            {
                case Type.Width:
                    m_CanvasScale.matchWidthOrHeight = 0;
                    break;
                case Type.Height:
                    m_CanvasScale.matchWidthOrHeight = 1;
                    break;
                case Type.WidthOrHeight:
                    if (diff < 1)
                    {
                        m_CanvasScale.matchWidthOrHeight = 1;
                    }
                    else
                    {
                        m_CanvasScale.matchWidthOrHeight = 0;
                    }
                    break;
                default:
                    break;
            }
        }

        private CanvasScaler m_CanvasScale;
        private Type m_ScalerType = Type.WidthOrHeight;
    }
}