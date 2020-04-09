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

    public enum CloseStrategy
    {
        Default = 0,
        Always = 1,         //UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先打开的
        Destroy = 2,        //关闭时立即销毁
        DelayDestroy = 3,   //延迟一段时间销毁
        Eternal = 4         //总是存于场景中, 除非主动销毁
    }

    public enum UILayer
    {
        Lowest = 0,     // 存放更低一级UI
        Root = 1,       // 存放root类型UI
        General = 2,    // 存放normal, pop类型UI
        Highest = 3,    // 高于normal, pop类型UI, 一般一些通用弹窗
        Special = 4     // 存放特殊UI类型: 链接等待UIWait, 引导界面等. 他们之间的层级用UGUI一套层级管理. 
    }

    public enum UIPreLoad
    {
        None = 0,
        LoginPreLoad,//登录后，进入主界面前预加载
        CourtPreLoad,//进入球场前预加载
    }

    public enum UIExoprtScriptType
    {
        CSharp,
        Lua,
    }

    [HideInInspector] [SerializeField] string _folder = string.Empty;
    public string folder
    {
        get { return _folder; }
        set
        {
            if (_folder != value)
            {
                _folder = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                CalPath();
            }
        }
    }

    [HideInInspector] [SerializeField] string _panelName = string.Empty;
    public string panelName
    {
        get { return _panelName; }
        set
        {
            if (_panelName != value)
            {
                _panelName = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|/|\\\\", "");
                CalPath();
                CalResPath();
            }
        }
    }

    [HideInInspector] [SerializeField] string _resFolder = string.Empty;
    public string resFolder
    {
        get { return _resFolder; }
        set
        {
            if (_resFolder != value)
            {
                _resFolder = System.Text.RegularExpressions.Regex.Replace(value, "\\.|\\s|", "");
                CalResPath();
            }
        }
    }


    [HideInInspector] [SerializeField] CloseStrategy _strategy;
    public CloseStrategy strategy
    {
        get { return _strategy; }
        set
        {
            if (_strategy != value)
            {                                                                                                                                           
                _strategy = value;
                CalRealStartegy();
            }
        }
    }

    public Type type = Type.Normal;
    public UIExoprtScriptType scriptType = UIExoprtScriptType.CSharp;
    public UIPreLoad preLoad = UIPreLoad.None;
    public bool customLayer = false;
    public UILayer layer = UILayer.General;
    public float unloadTime = 10;

    [HideInInspector] public CloseStrategy realStartegy;
    [HideInInspector] public string _panelPath;
    [HideInInspector] public string _ctrlPath;
    [HideInInspector] public string _resPath;

    public void CalPath()
    {
        string ctrlPath = _panelName;
        string f = folder;

        if (_panelName.EndsWith("View"))
        {
            if (string.IsNullOrEmpty(f))
                f = _panelName.Substring(0, _panelName.Length - 4);
        }
        else if (_panelName.EndsWith("Panel"))
        {
            if (string.IsNullOrEmpty(f))
                f = _panelName.Substring(0, _panelName.Length - 5);

            ctrlPath = _panelName.Substring(0, _panelName.Length - 5);
        }
        else
        {
            if (string.IsNullOrEmpty(f))
                f = _panelName;
        }

        ctrlPath += "Ctrl";

        _panelPath = string.Format("Assets/LuaScripts/Game/GUI/{0}/{1}.lua", f, _panelName);
        _ctrlPath = string.Format("Assets/LuaScripts/Game/GUI/{0}/{1}.lua", f, ctrlPath);
    }

    public void CalResPath()
    {
        if (!string.IsNullOrEmpty(_resFolder))
        {
            _resPath = string.Format("Assets/Res/GUI/{0}/{1}.prefab", _resFolder, _panelName);
        }
        else
        {
            if (_panelName.Contains("View"))
            {
                _resPath = string.Format("Assets/Res/GUI/Views/{0}/{1}.prefab", _panelName, _panelName);
            }
            else
            {
                _resPath = string.Format("Assets/Res/GUI/{0}/{1}.prefab", _panelName, _panelName);
            }
        }        
    }

    public void CalRealStartegy()
    {
        if (_strategy == CloseStrategy.Default)
        {
            switch(type)
            {
                case Type.Root:
                case Type.Normal: realStartegy = CloseStrategy.Always; break;
                case Type.Pop: realStartegy = CloseStrategy.Destroy; break;
            }
        }
        else
        {
            realStartegy = _strategy;
        }
    }

    //form: panelName, layerIndex, type, preload, closeStraegy, unloadTime, resFloder
    public string CreateParameters()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("\"" + panelName + "\"");
        sb.Append(", ");

        if (type == Type.Root)
        {
            sb.Append("1, UIManager.Type.Root, true, UIManager.CloseStrategy.Eternal");
        }
        else
        {
            sb.AppendInt(customLayer ? (int)layer : 2);
            sb.Append(", ");

            sb.Append("UIManager.Type." + type.ToString());
            sb.Append(", ");

            sb.AppendInt((int)preLoad);
            sb.Append(", ");

            if (realStartegy == CloseStrategy.DelayDestroy)
            {
                sb.Append("UIManager.CloseStrategy.DelayDestroy");
                sb.Append(", ");

                sb.Append(unloadTime.ToString("0.00"));
            }
            else
            {
                sb.Append("UIManager.CloseStrategy." + realStartegy.ToString());
            }
        }

        if (!string.IsNullOrEmpty(_resFolder))
        {
            if (realStartegy == CloseStrategy.DelayDestroy)
            {
                sb.Append(", ");
                sb.Append(string.Format("\"{0}\"", _resFolder));
            }
            else
            {
                sb.Append(", 0, ");
                sb.Append(string.Format("\"{0}\"", _resFolder));
            }
        }

        return sb.ToString();
    }
}
#endif