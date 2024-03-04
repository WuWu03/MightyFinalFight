using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public class SkillPrevConditionGUI : SkillGUI
    {
        public SkillPrevConditionGUI(EditorWindow window) : base(window)
        {
            m_ListPrevCondition = new List<SkillEditorConfigData.SkillPrevCondition>();
        }

        protected override void OnUpdateData()
        {
            base.OnUpdateData();
            CloneConditions();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (SkillEditorHelper.currConfigData.SkillPrevConditions != null)
            {
                EditorGUILayout.Space(10f);
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                int removeIndex = -1;

                for (int i = 0; i < SkillEditorHelper.currConfigData.SkillPrevConditions.Length; i++)
                {
                    GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                    {
                        EditorGUILayout.BeginVertical();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label((i + 1).ToString() + ".");

                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            removeIndex = i;
                        }

                        EditorGUILayout.EndHorizontal();

                        SkillEditorConfigData.SkillPrevConditionType prevConditionType = SkillEditorHelper.currConfigData.SkillPrevConditions[i].prevConditionType;
                        SkillEditorConfigData.SkillPrevConditionType conditionType = (SkillEditorConfigData.SkillPrevConditionType)EditorGUILayout.EnumPopup("ConditionType", prevConditionType);
                        SkillEditorHelper.currConfigData.SkillPrevConditions[i].prevConditionType = conditionType;
                        SkillEditorHelper.currConfigData.SkillPrevConditions[i].isRevert = EditorGUILayout.Toggle("IsRevert", SkillEditorHelper.currConfigData.SkillPrevConditions[i].isRevert);

                        EditorGUILayout.BeginHorizontal();
                        m_ListPrevCondition[i].args = EditorGUILayout.TextField("Args", m_ListPrevCondition[i].args);
                        if (GUILayout.Button("更改", GUILayout.Width(100)))
                        {
                            SkillEditorHelper.currConfigData.SkillPrevConditions[i].args = m_ListPrevCondition[i].args;
                            m_EditorWindow.ShowNotification("更改成功");
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                    });
                }

                if (removeIndex >= 0)
                {
                    m_ListPrevCondition.RemoveAt(removeIndex);
                    SkillEditorHelper.currConfigData.SkillPrevConditions = CommonUtil.DeleteElement(SkillEditorHelper.currConfigData.SkillPrevConditions, removeIndex);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();
            }

            if (GUILayout.Button("增加前置条件"))
            {
                m_ListPrevCondition.Add(new SkillEditorConfigData.SkillPrevCondition());
                SkillEditorHelper.currConfigData.SkillPrevConditions = CommonUtil.AddElement(SkillEditorHelper.currConfigData.SkillPrevConditions, new SkillEditorConfigData.SkillPrevCondition());
            }
        }

        private void CloneConditions()
        {
            if (SkillEditorHelper.currConfigData == null || SkillEditorHelper.currConfigData.SkillPrevConditions == null)
            {
                return;
            }

            m_ListPrevCondition.Clear();

            for (int i = 0; i < SkillEditorHelper.currConfigData.SkillPrevConditions.Length; i++)
            {
                m_ListPrevCondition.Add(Clone(SkillEditorHelper.currConfigData.SkillPrevConditions[i]));
            }
        }

        private SkillEditorConfigData.SkillPrevCondition Clone(SkillEditorConfigData.SkillPrevCondition source)
        {
            SkillEditorConfigData.SkillPrevCondition newCondition = new SkillEditorConfigData.SkillPrevCondition();
            newCondition.prevConditionType = source.prevConditionType;
            newCondition.isRevert = source.isRevert;
            newCondition.args = source.args;

            return newCondition;
        }

        private Vector2 m_ScrollPos = Vector2.zero;
        private List<SkillEditorConfigData.SkillPrevCondition> m_ListPrevCondition = null;
    }
}