using GameFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SkillHelper
{
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

        m_IndexLabelStyle = new GUIStyle(GUI.skin.label);
        m_IndexLabelStyle.alignment = TextAnchor.MiddleLeft;
        m_IndexLabelStyle.fontSize = 18;
        m_IndexLabelStyle.fontStyle = FontStyle.Bold;
        m_IndexLabelStyle.fixedHeight = 20;

        SetShowNames();
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

    private static int m_CurrSelectIndex = 0;
    private static string[] m_ShowNames = null;
    private static GUIStyle m_IndexLabelStyle = null;
    private static SkillConfig m_SkillConfig = null;
}
