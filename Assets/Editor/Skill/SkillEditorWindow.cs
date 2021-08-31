using GameFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillEditorWindow : EditorWindow
{
    private void OnEnable()
    {
        SkillHelper.InitConfig();
        m_CurrPage = 0;
        m_SkillGUIs = new SkillGUI[3];
        m_SkillGUIs[0] = new SkillBaseGUI(this);
        m_SkillGUIs[1] = new SkillPrevConditionGUI(this);
        m_SkillGUIs[2] = new SkillEffectGUI(this);
        m_SkillGUIs[0].UpdateData();
        m_SkillGUIs[1].UpdateData();
        m_SkillGUIs[2].UpdateData();
    }

    public void OnDisable()
    {
        SkillHelper.SaveConfig();
    }

    private void OnGUI()
    {
        SkillHelper.InitGUIStyle();
        MainGUI();
    }

    private void MainGUI()
    {
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < m_TabNames.Length; i++)
        {
            if (GUILayout.Button(m_TabNames[i], i == m_CurrPage ? SkillHelper.SelectButtonOnStyle : SkillHelper.SelectButtonStyle))
            {
                m_CurrPage = i;
                m_SkillGUIs[m_CurrPage].UpdateData();
                return;
            }
        }
        EditorGUILayout.EndHorizontal();

        int selectIndex = EditorGUILayout.Popup("选择处理项", SkillHelper.CurrSelectIndex, SkillHelper.ShowNames);

        if (selectIndex != SkillHelper.CurrSelectIndex)
        {
            SkillHelper.CurrSelectIndex = selectIndex;
            m_SkillGUIs[m_CurrPage].UpdateData();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        m_AddName = EditorGUILayout.TextField("增加项", m_AddName);
        if (GUILayout.Button("增加", GUILayout.Width(100)))
        {
            SkillHelper.AddData(m_AddName);
            m_SkillGUIs[m_CurrPage].UpdateData();
            return;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("删除当前项"))
        {
            SkillHelper.RemoveData();
            m_SkillGUIs[m_CurrPage].UpdateData();
            return;
        }

        if (SkillHelper.CurrConfigData != null)
        {
            m_SkillGUIs[m_CurrPage].Draw();
        }
    }

    private int m_CurrPage = 0;
    private string m_AddName = string.Empty;
    private SkillGUI[] m_SkillGUIs = null;
    private string[] m_TabNames = new string[] { "SkillBaseConfig", "SkillPrevConditionConfig", "SkillEffectConfig" };
}
