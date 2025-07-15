using GameFrameWork.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillEditorWindow : EditorWindow
{
    private void OnEnable()
    {
        SkillEditorHelper.InitConfig();
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
        SkillEditorHelper.SaveConfig();
        SkillEditorHelper.Release();
    }

    private void OnGUI()
    {
        SkillEditorHelper.InitGUIStyle();
        MainGUI();
    }

    private void MainGUI()
    {
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < m_TabNames.Length; i++)
        {
            if (GUILayout.Button(m_TabNames[i], i == m_CurrPage ? SkillEditorHelper.selectButtonOnStyle : SkillEditorHelper.selectButtonStyle))
            {
                m_CurrPage = i;
                m_SkillGUIs[m_CurrPage].UpdateData();
                break;
            }
        }
        EditorGUILayout.EndHorizontal();

        int selectIndex = EditorGUILayout.Popup("选择处理项", SkillEditorHelper.currSelectIndex, SkillEditorHelper.showNames);

        if (selectIndex != SkillEditorHelper.currSelectIndex)
        {
            SkillEditorHelper.currSelectIndex = selectIndex;
            m_SkillGUIs[m_CurrPage].UpdateData();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        m_AddName = EditorGUILayout.TextField("增加项", m_AddName);
        if (GUILayout.Button("增加", GUILayout.Width(100)))
        {
            SkillEditorHelper.AddData(m_AddName);
            m_SkillGUIs[m_CurrPage].UpdateData();
            return;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("删除当前项"))
        {
            SkillEditorHelper.RemoveData();
            m_SkillGUIs[m_CurrPage].UpdateData();
            return;
        }

        if (SkillEditorHelper.currConfigData != null)
        {
            m_SkillGUIs[m_CurrPage].Draw();
        }
    }

    private int m_CurrPage = 0;
    private string m_AddName = string.Empty;
    private SkillGUI[] m_SkillGUIs = null;
    private string[] m_TabNames = new string[] { "SkillBaseConfig", "SkillPrevConditionConfig", "SkillEffectConfig" };
}
