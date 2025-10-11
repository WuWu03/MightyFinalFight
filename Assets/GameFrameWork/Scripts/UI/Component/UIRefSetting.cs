using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class UIRefSetting : MonoBehaviour
{
    public enum UIType : byte
    {
        Panel,//依附于其他界面的子界面
        View,//普通界面
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

    [HideInInspector] [SerializeField] 
    private string m_ModuleName = string.Empty;
    public string moduleName
    {
        get 
        {
            return m_ModuleName;
        }
        set
        {
            if (m_ModuleName != value)
            {
                m_ModuleName =  System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                RefreshScriptFolder();
            }
        }
    }

    [HideInInspector] [SerializeField] 
    private string m_ViewName = string.Empty;
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

    [HideInInspector] public string viewPath;
    [HideInInspector] public string componentPath;
    [HideInInspector] public string settingsPath;
    
    public void RefreshScriptFolder()
    {
        string suffix = uiType == UIType.View ? "View" : "Panel";
        string tempViewName = m_ViewName.Replace("View", string.Empty).Replace("Panel", string.Empty);
        m_ViewName = tempViewName + suffix;
        
        if (string.IsNullOrEmpty(m_ModuleName))
        {
            m_ModuleName = tempViewName;
        }
    }
}
#endif