using GameFrameWork.Editor;
using GameFrameWork.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SkillEditorHelper
{
    public static SkillConfigData currConfigData
    {
        get
        {
            if (!HasData())
            {
                return null;
            }

            return m_SkillConfig.listDatas[m_CurrSelectIndex];
        }
    }

    public static List<SkillConfigData> skillDatas
    {
        get
        {
            if (!HasData())
            {
                return null;
            }

            return m_SkillConfig.listDatas;
        }
    }

    public static int currSelectIndex
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

    public static string currShowName
    {
        get
        {
            return m_ShowNames[m_CurrSelectIndex];
        }
    }

    public static string[] showNames
    {
        get
        {
            return m_ShowNames;
        }
    }

    public static GUIStyle selectButtonOnStyle
    {
        get
        {
            return m_SelectButtonOnStyle;
        }
    }

    public static GUIStyle selectButtonStyle
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

        string configDataPath = PathUtil.GetAssetPath(GameFrameWork.Editor.EditorMgr.GetGameFrameWorkConfig().configDataPath + "SkillConfig.asset");
        string configDataFullPath = PathUtil.GetAssetFullPath(configDataPath);

        if (!File.Exists(configDataFullPath))
        {
            EditorMgr.CreateSkillData();
        }

        m_SkillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>(configDataPath);

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
    public static void AddData(string name)
    {
        SkillConfigData skillConfigData = new SkillConfigData();
        skillConfigData.Name = name;
        skillConfigData.Key = new SkillConfigData.SkillKey();
        skillConfigData.Key.Keys = new GameFrameWork.Input.KeyType[0];
        skillConfigData.SkillPrevConditions = new SkillConfigData.SkillPrevCondition[0];
        skillConfigData.SkillEffects = new SkillConfigData.SkillEffect[0];
        m_SkillConfig.AddData(skillConfigData);
        m_CurrSelectIndex = m_SkillConfig.listDatas.Count - 1;
        SetShowNames();
    }

    public static void RemoveData()
    {
        if (!HasData() || m_CurrSelectIndex < 0 || m_CurrSelectIndex >= m_SkillConfig.listDatas.Count)
        {
            return;
        }

        m_SkillConfig.listDatas.RemoveAt(m_CurrSelectIndex);

        if(m_CurrSelectIndex >= m_SkillConfig.listDatas.Count)
        {
            m_CurrSelectIndex = m_SkillConfig.listDatas.Count - 1;
        }
    }

    public static bool HasData()
    {
        if (m_SkillConfig == null || m_SkillConfig.listDatas == null || m_SkillConfig.listDatas.Count < 1)
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

        for (int i = 0; i < m_SkillConfig.listDatas.Count; i++)
        {
            string name = string.IsNullOrEmpty(m_SkillConfig.listDatas[i].Name) ? "未命名" : m_SkillConfig.listDatas[i].Name;
            temp.Add(name);
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

    public static void Release()
    {
        m_SkillConfig = null;
    }

    private static int m_CurrSelectIndex = 0;
    private static string[] m_ShowNames = null;
    private static GUIStyle m_IndexLabelStyle = null;
    private static GUIStyle m_SelectButtonOnStyle = null;
    private static GUIStyle m_SelectButtonStyle = null;
    private static SkillConfig m_SkillConfig = null;
}
