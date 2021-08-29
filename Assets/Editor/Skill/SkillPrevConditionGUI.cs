using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillPrevConditionGUI : SkillGUI
{
    public SkillPrevConditionGUI(EditorWindow window) : base(window)
    {
        m_ListPrevCondition = new List<SkillConfigData.SkillPrevCondition>();
    }

    protected override void OnUpdateData()
    {
        base.OnUpdateData();
        CloneConditions();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
        EditorGUILayout.Space(10f);
        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

        for (int i = 0; i < SkillHelper.CurrConfigData.SkillPrevConditions.Length; i++)
        {
            GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label((i + 1).ToString() + ".");

                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    m_ListPrevCondition.RemoveAt(i);
                    SkillHelper.CurrConfigData.SkillPrevConditions = m_ListPrevCondition.ToArray();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                SkillConfigData.SkillPrevConditionType prevConditionType = SkillHelper.CurrConfigData.SkillPrevConditions[i].PrevConditionType;
                SkillConfigData.SkillPrevConditionType conditionType = (SkillConfigData.SkillPrevConditionType)EditorGUILayout.EnumPopup("ConditionType", prevConditionType);
                SkillHelper.CurrConfigData.SkillPrevConditions[i].PrevConditionType = conditionType;

                SkillHelper.CurrConfigData.SkillPrevConditions[i].IsRevert = EditorGUILayout.Toggle("IsRevert", SkillHelper.CurrConfigData.SkillPrevConditions[i].IsRevert);

                EditorGUILayout.BeginHorizontal();
                m_ListPrevCondition[i].Args = EditorGUILayout.TextField("Args", m_ListPrevCondition[i].Args);
                if (GUILayout.Button("更改", GUILayout.Width(100)))
                {
                    SkillHelper.CurrConfigData.SkillPrevConditions[i].Args = m_ListPrevCondition[i].Args;
                    ShowNotification("更改成功");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            });
        }
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("增加前置条件"))
        {
            m_ListPrevCondition.Add(new SkillConfigData.SkillPrevCondition());
            SkillHelper.CurrConfigData.SkillPrevConditions = m_ListPrevCondition.ToArray();
            SkillHelper.CurrConfigData.SkillPrevConditions[m_ListPrevCondition.Count - 1] = new SkillConfigData.SkillPrevCondition();
            return;
        }
    }

    private void CloneConditions()
    {
        m_ListPrevCondition.Clear();
        for (int i = 0; i < SkillHelper.CurrConfigData.SkillPrevConditions.Length; i++)
        {
            m_ListPrevCondition.Add(Clone(SkillHelper.CurrConfigData.SkillPrevConditions[i]));
        }
    }

    private SkillConfigData.SkillPrevCondition Clone(SkillConfigData.SkillPrevCondition source)
    {
        SkillConfigData.SkillPrevCondition newCondition = new SkillConfigData.SkillPrevCondition();
        newCondition.PrevConditionType = source.PrevConditionType;
        newCondition.IsRevert = source.IsRevert;
        newCondition.Args = source.Args;

        return newCondition;
    }

    private Vector2 m_ScrollPos = Vector2.zero;
    private List<SkillConfigData.SkillPrevCondition> m_ListPrevCondition = null;
}