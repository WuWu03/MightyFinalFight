using GameFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SkillHelper
{
    public static SkillConfigData CurrConfigData
    {
        get
        {
            if (!HasData())
            {
                return null;
            }
            return m_SkillConfig.Datas[m_CurrSelectIndex];
        }
    }

    public static int CurrSelectIndex
    {
        get
        {
            return m_CurrSelectIndex;
        }
        set
        {
            m_CurrSelectIndex = value;
        }
    }

    public static string CurrShowName
    {
        get
        {
            return m_ShowNames[m_CurrSelectIndex].Substring(m_ShowNames[m_CurrSelectIndex].IndexOf(".") + 1);
        }
    }

    public static string[] ShowNames
    {
        get
        {
            return m_ShowNames;
        }
    }

    public static GUIStyle SelectButtonOnStyle
    {
        get
        {
            return m_SelectButtonOnStyle;
        }
    }

    public static GUIStyle SelectButtonStyle
    {
        get
        {
            return m_SelectButtonStyle;
        }
    }

    public static GUIStyle IndexLabelStyle
    {
        get
        {
            return m_IndexLabelStyle;
        }
    }

    public static void InitConfig()
    {
        if (m_SkillConfig != null)
        {
            return;
        }

        if (!File.Exists(PathUtil.ConfigDataDefaultFullPath + "SkillData.asset"))
        {
            EditorMgr.CreateSkillData();
        }

        m_SkillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>(PathUtil.ConfigDataDefaultPath + "SkillData.asset");

   

        SetShowNames();
    }

    public static void InitGUIStyle()
    {
        if(m_IndexLabelStyle != null)
        {
            return;
        }

        m_IndexLabelStyle = new GUIStyle(GUI.skin.label);
        m_IndexLabelStyle.alignment = TextAnchor.MiddleLeft;
        m_IndexLabelStyle.fontSize = 18;
        m_IndexLabelStyle.fontStyle = FontStyle.Bold;
        m_IndexLabelStyle.fixedHeight = 20;
        m_CurrSelectIndex = 0;

        m_SelectButtonOnStyle = new GUIStyle("flow node 1");
        m_SelectButtonOnStyle.stretchWidth = true;
        m_SelectButtonOnStyle.alignment = TextAnchor.MiddleCenter;
        m_SelectButtonOnStyle.contentOffset = new Vector2(0, -15f);
        m_SelectButtonOnStyle.fixedHeight = 15f;

        m_SelectButtonStyle = new GUIStyle("flow node 0");
        m_SelectButtonStyle.stretchWidth = true;
        m_SelectButtonStyle.alignment = TextAnchor.MiddleCenter;
        m_SelectButtonStyle.contentOffset = new Vector2(0, -15f);
        m_SelectButtonStyle.fixedHeight = 15f;
    }
    public static void AddData()
    {

    }

    public static void RemoveData()
    {

    }

    public static bool HasData()
    {
        if (m_SkillConfig == null || m_SkillConfig.Datas == null || m_SkillConfig.Datas.Count < 1)
        {
            return false;
        }

        return true;
    }

    public static void SetShowNames()
    {
        if (!HasData())
        {
            return;
        }

        List<string> temp = new List<string>();

        for (int i = 0; i < m_SkillConfig.Datas.Count; i++)
        {
            string name = string.IsNullOrEmpty(m_SkillConfig.Datas[i].Name) ? "未命名" : m_SkillConfig.Datas[i].Name;
            temp.Add((i + 1).ToString() + "." + name);
        }

        m_ShowNames = temp.ToArray();
    }

    public static void SetSelectIndex(int index)
    {
        m_CurrSelectIndex = index;
    }

    public static void SaveConfig()
    {
        UnityEditor.EditorUtility.SetDirty(m_SkillConfig);
    }

    private static int m_CurrSelectIndex = 0;
    private static string[] m_ShowNames = null;
    private static GUIStyle m_IndexLabelStyle = null;
    private static GUIStyle m_SelectButtonOnStyle = null;
    private static GUIStyle m_SelectButtonStyle = null;
    private static SkillConfig m_SkillConfig = null;
}
