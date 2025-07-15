using System;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class UIRefSetting : MonoBehaviour
{
    public enum Type
    {
        Root = 1,       // 根UI, 必定: 预加载, 位于导航, 常驻场景, 无法导航至另一个界面.
        Normal = 2,     // 一般窗口, 默认: 加入导航; 常驻场景;
        Pop = 3,        // 弹窗, 默认: 不加入导航; 立即销毁;
    }

    public enum CloseMode
    {
        Always = 1,         //UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先打开的
        Destroy = 2,        //关闭时立即销毁
        DelayDestroy = 3,   //延迟一段时间销毁
        Eternal = 4         //总是存于场景中, 除非主动销毁
    }

    public enum Layer
    {
        Layer1,
        Layer2,
        Layer3,
        Layer4,
        Layer5,
        Layer6,
        Layer7,
        Layer8,
    }

    public enum PreLoadType
    {
        None = 0,
        LoginPreLoad,//登录后，进入主界面前预加载
        CourtPreLoad,//进入球场前预加载
    }

    public enum ExoprtScriptType
    {
        CSharp,
        Lua,
    }

    [HideInInspector] [SerializeField] 
    private string m_ScriptFolder = string.Empty;
    public string scriptFolder
    {
        get 
        {
            return m_ScriptFolder;
        }
        set
        {
            if (m_ScriptFolder != value)
            {
                m_ScriptFolder = value;
                RefreshScriptFolder();
            }
        }
    }

    [HideInInspector] [SerializeField] 
    private string m_PanelName = string.Empty;
    public string panelName
    {
        get { return m_PanelName; }
        set
        {
            if (m_PanelName != value)
            {
                m_PanelName = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                RefreshScriptFolder();
            }
        }
    }

    [HideInInspector][SerializeField]
    private ExoprtScriptType m_ScriptType;
    public ExoprtScriptType scriptType
    {
        get { return m_ScriptType; }
        set
        {
            if (m_ScriptType != value)
            {
                m_ScriptType = value;
                RefreshScriptFolder();
            }
        }
    }

    public Type panelType = Type.Normal;
    public PreLoadType panelPreLoadType = PreLoadType.None;
    public CloseMode panelCloseMode = CloseMode.Always;
    public Layer panelLayer = Layer.Layer3;
    public bool isCustomLayer = false;
    public float unLoadTime = 10;

    [HideInInspector] public string panelPath;
    [HideInInspector] public string panelComponentPath;
    [HideInInspector] public string panelSettingsPath;


    public void RefreshScriptFolder()
    {
        if (!m_PanelName.EndsWith("Panel"))
        {
            m_PanelName = m_PanelName + "Panel";
        }

        string extension = scriptType == ExoprtScriptType.CSharp ? "cs" : "lua";
        string scriptPath = scriptType == ExoprtScriptType.CSharp ? DefaultCSharpScriptFolder : DefaultLuaScriptFolder;

        if (string.IsNullOrEmpty(m_ScriptFolder))
        {
            m_ScriptFolder = scriptPath;
        }
        else
        {
            if(m_ScriptFolder.Contains(DefaultCSharpScriptFolder) && scriptType != ExoprtScriptType.CSharp)
            {
                m_ScriptFolder = m_ScriptFolder.Replace(DefaultCSharpScriptFolder, DefaultLuaScriptFolder);
            }

            if (m_ScriptFolder.Contains(DefaultLuaScriptFolder) && scriptType != ExoprtScriptType.Lua)
            {
                m_ScriptFolder = m_ScriptFolder.Replace(DefaultLuaScriptFolder, DefaultCSharpScriptFolder);
            }
        }

        if (!m_ScriptFolder.StartsWith("Assets/"))
        {
            m_ScriptFolder = "Assets/" + m_ScriptFolder;
        }

        if (!m_ScriptFolder.EndsWith("/"))
        {
            m_ScriptFolder += "/";
        }

        if (!m_ScriptFolder.EndsWith(m_PanelName + "/"))
        {
            m_ScriptFolder += m_PanelName + "/";
        }

        panelPath = string.Format(m_ScriptFolder + "{0}.{1}", m_PanelName, extension);
        panelComponentPath = string.Format(m_ScriptFolder + "{0}Component.{1}", m_PanelName, extension);
        panelSettingsPath = string.Format(m_ScriptFolder + "{0}Settings.{1}", m_PanelName, extension);
    }

    private const string DefaultCSharpScriptFolder = "Assets/Scripts/UI/";
    private const string DefaultLuaScriptFolder = "Assets/Lua/UI/";
}
#endif