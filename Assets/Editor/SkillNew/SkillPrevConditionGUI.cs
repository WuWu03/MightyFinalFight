using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData;

namespace SkillNew
{
    public class SkillPrevConditionGUI : SkillBaseGUI
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

            if (SkillEditorHelper.currConfigData.skillPrevConditions != null)
            {
                EditorGUILayout.Space(10f);
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                int removeIndex = -1;
                SkillPrevCondition[] skillPrevConditions = SkillEditorHelper.currConfigData.skillPrevConditions;

                for (int i = 0; i < skillPrevConditions.Length; i++)
                {
                    SkillPrevCondition skillPrevCondition = skillPrevConditions[i];

                    GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                    {
                        EditorGUILayout.BeginVertical();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label((i + 1).ToString() + ".");

                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            if (UnityEditor.EditorUtility.DisplayDialog("提示", "确定删除该事件？", "确定", "取消"))
                            {
                                removeIndex = i;
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        DrawField(() => { return m_ListPrevCondition[i].prevConditionType != skillPrevCondition.prevConditionType; },
                            () => { m_ListPrevCondition[i].prevConditionType = (SkillPrevConditionType)EditorGUILayout.EnumPopup("条件类型", m_ListPrevCondition[i].prevConditionType); },
                            () => { skillPrevCondition.prevConditionType = m_ListPrevCondition[i].prevConditionType; });

                        DrawField(() => { return m_ListPrevCondition[i].hpLimit != skillPrevCondition.hpLimit; },
                            () => { m_ListPrevCondition[i].hpLimit = EditorGUILayout.IntField("血量限制", m_ListPrevCondition[i].hpLimit); },
                            () => { skillPrevCondition.hpLimit = m_ListPrevCondition[i].hpLimit; });

                        DrawField(() => { return m_ListPrevCondition[i].isRevert != skillPrevCondition.isRevert; },
                            () => { m_ListPrevCondition[i].isRevert = EditorGUILayout.Toggle("条件反转", m_ListPrevCondition[i].isRevert); },
                            () => { skillPrevCondition.isRevert = m_ListPrevCondition[i].isRevert; });


                        EditorGUILayout.EndVertical();
                    });
                }

                if (removeIndex >= 0)
                {
                    m_ListPrevCondition.RemoveAt(removeIndex);
                    SkillEditorHelper.currConfigData.skillPrevConditions = CommonUtil.DeleteElement(SkillEditorHelper.currConfigData.skillPrevConditions, removeIndex);
                    removeIndex = -1;
                }

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();
            }

            if (GUILayout.Button("增加前置条件"))
            {
                m_ListPrevCondition.Add(new SkillPrevCondition());
                SkillEditorHelper.currConfigData.skillPrevConditions = CommonUtil.AddElement(SkillEditorHelper.currConfigData.skillPrevConditions, new SkillPrevCondition());
            }
        }

        private void CloneConditions()
        {
            if (SkillEditorHelper.currConfigData == null || SkillEditorHelper.currConfigData.skillPrevConditions == null)
            {
                return;
            }

            m_ListPrevCondition.Clear();

            SkillPrevCondition[] skillPrevConditions = SkillEditorHelper.currConfigData.skillPrevConditions;
            for (int i = 0; i < skillPrevConditions.Length; i++)
            {
                m_ListPrevCondition.Add(Clone(skillPrevConditions[i]));
            }
        }

        private SkillPrevCondition Clone(SkillPrevCondition source)
        {
            SkillPrevCondition newCondition = new SkillPrevCondition
            {
                prevConditionType = source.prevConditionType,
                isRevert = source.isRevert,
                hpLimit = source.hpLimit
            };

            return newCondition;
        }

        private Vector2 m_ScrollPos = Vector2.zero;
        private List<SkillPrevCondition> m_ListPrevCondition = null;
    }
}