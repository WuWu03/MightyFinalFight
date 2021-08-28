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
    }

    private void OnGUI()
    {
        InitConfig();
        MainGUI();
    }

    private void InitConfig()
    {
        m_ListPrevCondition = new List<SkillConfigData.SkillPrevCondition>();
       
        m_CurrSelectIndex = 0;
        m_SkillGUIs = new SkillGUI[3];
        m_SkillGUIs[0] = new SkillBaseGUI(m_CurrConfigData);
        m_SkillGUIs[1] = new SkillPrevConditionGUI();
        m_SkillGUIs[2] = new SkillEffectGUI();

        SetCurrData();
    }

    private void MainGUI()
    {
        int selectIndex = EditorGUILayout.Popup("选择处理项", m_CurrSelectIndex, SkillHelper.ShowNames);
        if (m_CurrSelectIndex != selectIndex)
        {
            m_CurrSelectIndex = selectIndex;
            SetCurrData();
        }

        EditorGUILayout.BeginHorizontal();
        m_AddName = EditorGUILayout.TextField("增加项", m_AddName);
        if (GUILayout.Button("增加", GUILayout.Width(100)))
        {
            AddData();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(SkillHelper.ShowNames[m_CurrSelectIndex], SkillHelper.IndexLabelStyle);

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("x"))
        {
            RemoveData();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

        EditorGUILayout.Space(10);

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.LabelField("SkillPrevConditions");
        });

        for (int i = 0; i < m_CurrConfigData.SkillPrevConditions.Length; i++)
        {
            GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label((i + 1).ToString() + ".");

                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    m_ListPrevCondition.RemoveAt(i);
                    m_CurrConfigData.SkillPrevConditions = m_ListPrevCondition.ToArray();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                SkillConfigData.SkillPrevConditionType prevConditionType = m_CurrConfigData.SkillPrevConditions[i].PrevConditionType;
                SkillConfigData.SkillPrevConditionType conditionType = (SkillConfigData.SkillPrevConditionType)EditorGUILayout.EnumPopup("ConditionType", prevConditionType);
                m_CurrConfigData.SkillPrevConditions[i].PrevConditionType = conditionType;

                m_CurrConfigData.SkillPrevConditions[i].IsRevert = EditorGUILayout.Toggle("IsRevert", m_CurrConfigData.SkillPrevConditions[i].IsRevert);

                EditorGUILayout.BeginHorizontal();
                m_ListPrevCondition[i].Args = EditorGUILayout.TextField("Args", m_ListPrevCondition[i].Args);
                if (GUILayout.Button("更改", GUILayout.Width(100)))
                {
                    m_CurrConfigData.SkillPrevConditions[i].Args = m_ListPrevCondition[i].Args;
                    ShowNotification(new GUIContent("更改成功"));
                }
                EditorGUILayout.EndHorizontal();

                if (i == m_CurrConfigData.SkillPrevConditions.Length - 1)
                {
                    if (GUILayout.Button("增加前置条件"))
                    {
                        m_ListPrevCondition.Add(new SkillConfigData.SkillPrevCondition());
                        m_CurrConfigData.SkillPrevConditions = m_ListPrevCondition.ToArray();
                        return;
                    }
                }

                EditorGUILayout.EndVertical();
            });
        }

        EditorGUILayout.Space(10);

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.LabelField("SkillEffects");
        });

        EditorGUILayout.EndScrollView();
    }



 

    private void AddData()
    {

    }

    private void RemoveData()
    {

    }

    private void SetCurrData()
    {
        m_CurrConfigData = m_SkillConfig.Datas[m_CurrSelectIndex];
        
        //m_ListPrevCondition.Clear();

        //m_ListPrevCondition.AddRange(m_CurrConfigData.SkillPrevConditions);
    }

   

    private List<SkillConfigData.SkillPrevCondition> m_ListPrevCondition = null;

    private string m_AddName = string.Empty;
    private int m_CurrSelectIndex = 0;


    private SkillConfig m_SkillConfig = null;
    private SkillConfigData m_CurrConfigData = null;
    private SkillGUI[] m_SkillGUIs = null;
    private Vector2 m_ScrollPos = Vector2.zero;

}
