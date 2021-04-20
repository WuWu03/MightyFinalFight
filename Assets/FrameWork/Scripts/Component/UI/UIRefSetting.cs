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
        BG,
        MainPanel,
        FirstLevel,
        SecondLevel,
        ThirdLevel,
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
    public string ScriptFolder
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
                RefreshPanelFolder();
            }
        }
    }

    [HideInInspector] [SerializeField] 
    private string m_PanelName = string.Empty;
    public string PanelName
    {
        get { return m_PanelName; }
        set
        {
            if (m_PanelName != value)
            {
                m_PanelName = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                RefreshPanelFolder();
                RefreshPrefabFolder();
            }
        }
    }

    [HideInInspector] [SerializeField] 
    private string m_PrefabFolder = string.Empty;
    public string PrefabFolder
    {
        get { return m_PrefabFolder; }
        set
        {
            if (m_PrefabFolder != value)
            {
                m_PrefabFolder = value;
                RefreshPrefabFolder();
            }
        }
    }

    [HideInInspector][SerializeField]
    private ExoprtScriptType m_ScriptType;
    public ExoprtScriptType ScriptType
    {
        get { return m_ScriptType; }
        set
        {
            if (m_ScriptType != value)
            {
                m_ScriptType = value;
                RefreshPanelFolder();
            }
        }
    }

    public Type PanelType = Type.Normal;
    public PreLoadType PanelPreLoadType = PreLoadType.None;
    public CloseMode PanelCloseMode = CloseMode.Always;
    public Layer PanelLayer = Layer.FirstLevel;
    public bool IsCustomLayer = false;
    public float UnLoadTime = 10;

    [HideInInspector] public string PanelPath;
    [HideInInspector] public string PanelComponentPath;
    [HideInInspector] public string PanelPrefabPath;


    public void RefreshPanelFolder()
    {
        if (!m_PanelName.EndsWith("Panel"))
        {
            m_PanelName = m_PanelName + "Panel";
        }

        string extension = ScriptType == ExoprtScriptType.CSharp ? "cs" : "lua";
        string scriptPath = ScriptType == ExoprtScriptType.CSharp ? DefaultCSharpScriptFolder : DefaultLuaScriptFolder;

        if (string.IsNullOrEmpty(m_ScriptFolder))
        {
            m_ScriptFolder = scriptPath;
        }
        else
        {
            if(m_ScriptFolder.Contains(DefaultCSharpScriptFolder) && ScriptType != ExoprtScriptType.CSharp)
            {
                m_ScriptFolder = m_ScriptFolder.Replace(DefaultCSharpScriptFolder, DefaultLuaScriptFolder);
            }

            if (m_ScriptFolder.Contains(DefaultLuaScriptFolder) && ScriptType != ExoprtScriptType.Lua)
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

        PanelPath = string.Format(m_ScriptFolder + "{0}.{1}", m_PanelName, extension);
        PanelComponentPath = string.Format(m_ScriptFolder + "{0}Component.{1}", m_PanelName, extension);
    }

    public void RefreshPrefabFolder()
    {
        if (string.IsNullOrEmpty(m_PrefabFolder))
        {
            m_PrefabFolder = DefaultPrefabFolder;
        }

        if (!m_PrefabFolder.StartsWith("Assets/"))
        {
            m_PrefabFolder = "Assets/" + m_PrefabFolder;
        }

        if (!m_PrefabFolder.EndsWith("/"))
        {
            m_PrefabFolder += "/";
        }

        PanelPrefabPath = string.Format(m_PrefabFolder + "{0}.prefab", m_PanelName);
    }

    private const string DefaultCSharpScriptFolder = "Assets/Scripts/UI/";
    private const string DefaultLuaScriptFolder = "Assets/Lua/UI/";
    private const string DefaultPrefabFolder = "Assets/ArtResources/UI/Prefabs/";
}
#endif