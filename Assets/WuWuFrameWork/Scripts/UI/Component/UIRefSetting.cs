using System;
using UnityEngine;

#if UNITY_EDITOR
namespace WuWuFramework.UI
{
    [ExecuteInEditMode]
    public class UIRefSetting : MonoBehaviour
    {
        public enum UIType : byte
        {
            Panel, //依附于其他界面的子界面
            View, //普通界面
            Item, //成员
        }

        public enum UIDestroyMode : byte
        {
            Always, //UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先打开的
            Immediately, //关闭时立即销毁
            Delay, //延迟一段时间销毁
            Eternal, //总是存于场景中, 除非主动销毁
        }

        public enum UILayer : byte
        {
            Scene,
            Bg,
            MainWindow,
            Window1,
            Window2,
            Tips,
            Guide,
            Message,
            Mask,
            Load,
        }

        [HideInInspector][SerializeField] private string m_ModuleName = string.Empty;

        public string moduleName
        {
            get { return m_ModuleName; }
            set
            {
                if (m_ModuleName != value)
                {
                    m_ModuleName = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                    RefreshScriptFolder();
                }
            }
        }

        [HideInInspector][SerializeField] private string m_PresenterName = string.Empty;
        public string presenterName
        {
            get { return m_PresenterName; }
            set
            {
                if (m_PresenterName != value)
                {
                    m_PresenterName = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                    RefreshScriptFolder();
                }
            }
        }

        [HideInInspector][SerializeField] private string m_ViewName = string.Empty;

        public string viewName
        {
            get { return m_ViewName; }
            set
            {
                if (m_ViewName != value)
                {
                    m_ViewName = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                    RefreshScriptFolder();
                }
            }
        }

        public UIDestroyMode uiDestroyMode = UIDestroyMode.Always;
        public UILayer uiLayer = UILayer.Window1;
        public UIType uiType = UIType.View;
        public float delayDestroyTime = 10;

        [HideInInspector] public string presenterPath;
        [HideInInspector] public string viewPath;
        [HideInInspector] public string settingsPath;

        public void RefreshScriptFolder()
        {
            string suffix = uiType switch
            {
                UIType.View => "View",
                UIType.Panel => "Panel",
                UIType.Item => "Item",
                _ => throw new ArgumentOutOfRangeException()
            };

            string tempViewName = m_ViewName.Replace("View", string.Empty).Replace("Panel", string.Empty).Replace("Item", string.Empty);
            m_ViewName = tempViewName + suffix;

            string tempPresenterName = m_PresenterName.Replace("Presenter", string.Empty);
            m_PresenterName = tempPresenterName + suffix + "Presenter";

            if (string.IsNullOrEmpty(m_ModuleName))
            {
                m_ModuleName = tempViewName;
            }
        }
    }
}
#endif