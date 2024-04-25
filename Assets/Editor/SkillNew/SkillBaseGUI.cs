using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData;

namespace SkillNew
{
    public class SkillBaseGUI : SkillGUI
    {
        public SkillBaseGUI(EditorWindow window) : base(window)
        {
            m_ListKey = new List<GameFrameWork.Input.KeyType>();
            m_ListSillEventGUI = new List<SkillGUI>();
            m_ListDeleteSkillEvent = new List<int>();
        }

        protected override void OnUpdateData()
        {
            if(!SkillEditorHelper.HasData())
            {
                return;
            }

            m_SkillFrameCount = SkillEditorHelper.currConfigData.skillFrameCount;
            m_CurrFrame = Mathf.Min(1, m_SkillFrameCount);

            if (m_CurrFrame > 0)
            {
                if (SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out var list))
                {
                    m_ListSkillEvent = list.ToList();
                }
                else
                {
                    m_ListSkillEvent = null;
                }

                if (m_ListSkillEvent != null && m_ListSkillEvent.Count > 0)
                {
                    m_ListSillEventGUI.Clear();

                    for (int i = 0; i < m_ListSkillEvent.Count; i++)
                    {
                        SkillEventGUI gui = SkillEditorHelper.GetSKillGUI(m_ListSkillEvent[i].eventType);

                        if(gui != null)
                        {
                            gui.UpdateSkillEvent(m_ListSkillEvent[i]);
                        }
                    }
                }
            }

            m_ListKey.Clear();
        }

        protected override void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            GUILayout.Label(SkillEditorHelper.currShowName, SkillEditorHelper.indexLabelStyle);

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginHorizontal();
                bool isModify = SkillEditorHelper.currConfigData.skillFrameCount != m_SkillFrameCount;
                m_SkillFrameCount = EditorGUILayout.IntField("技能帧数", m_SkillFrameCount, SkillEditorHelper.GetTextFieldStyle(isModify));

                if (GUILayout.Button("更改", GUILayout.Width(100)))
                {
                    SkillEditorHelper.currConfigData.skillFrameCount = m_SkillFrameCount;
                    SkillEditorHelper.SetShowNames();
                    m_EditorWindow.ShowNotification("更改成功");

                    int[] deletes = SkillEditorHelper.currConfigData.dicSkillEvents.Keys.Where(x => x > m_SkillFrameCount).ToArray();

                    foreach (int delete in deletes)
                    {
                        SkillEditorHelper.currConfigData.dicSkillEvents.Remove(delete);
                    }

                    if (SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out var list))
                    {
                        m_ListSkillEvent = list.ToList();
                    }
                    else
                    {
                        m_ListSkillEvent = null;
                    }
                }

                EditorGUILayout.EndHorizontal();
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                int frameIndex = EditorGUILayout.IntSlider("当前帧", m_CurrFrame, Mathf.Min(1, SkillEditorHelper.currConfigData.skillFrameCount), SkillEditorHelper.currConfigData.skillFrameCount);
                if (frameIndex != m_CurrFrame)
                {
                    if (SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(frameIndex, out var list))
                    {
                        m_ListSkillEvent = list.ToList();
                    }
                    else
                    {
                        m_ListSkillEvent = null;
                    }

                    if (m_ListSkillEvent != null)
                    {
                        for (int i = 0; i < m_ListSkillEvent.Count; i++)
                        {
                            SkillEventGUI gui = SkillEditorHelper.GetSKillGUI(m_ListSkillEvent[i].eventType);

                            if (gui != null)
                            {
                                gui.UpdateSkillEvent(m_ListSkillEvent[i]);
                            }
                        }
                    }

                    m_CurrFrame = frameIndex;
                }
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("技能事件列表");
                if (m_ListSkillEvent != null)
                {
                    m_ListDeleteSkillEvent.Clear();

                    for (int i = 0; i < m_ListSkillEvent.Count; i++)
                    {
                        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                        {
                            EditorGUILayout.BeginVertical();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField((i + 1).ToString());
                            if (GUILayout.Button("x", GUILayout.Width(20)))
                            {
                                if(UnityEditor.EditorUtility.DisplayDialog("提示", "确定删除该事件？", "确定","取消"))
                                {
                                    m_ListDeleteSkillEvent.Add(i);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            Enum eventTypeValue = EditorGUILayout.EnumPopup("事件类型", m_ListSkillEvent[i].eventType);

                            SkillEditorConfigData.SkillEventType skillEventType = (SkillEditorConfigData.SkillEventType)eventTypeValue;
                            SkillEventGUI gui = SkillEditorHelper.GetSKillGUI(m_ListSkillEvent[i].eventType);

                            if (skillEventType != m_ListSkillEvent[i].eventType)
                            {
                                bool hasSameEvent = false;

                                for (int j = 0; j < m_ListSkillEvent.Count; j++)
                                {
                                    if (m_ListSkillEvent[j].eventType != SkillEditorConfigData.SkillEventType.None &&
                                        skillEventType != SkillEditorConfigData.SkillEventType.None &&
                                        m_ListSkillEvent[j].eventType == skillEventType)
                                    {
                                        hasSameEvent = true;
                                        break;
                                    }
                                }

                                if (hasSameEvent)
                                {
                                    this.m_EditorWindow.ShowNotification("每种事件类型在同一帧只能出现一次");
                                }
                                else
                                {
                                    m_ListSkillEvent[i].eventType = skillEventType;

                                    gui = SkillEditorHelper.GetSKillGUI(m_ListSkillEvent[i].eventType);

                                    if (gui != null)
                                    {
                                        gui.UpdateSkillEvent(m_ListSkillEvent[i]);
                                    }
                                }
                            }

                            if (gui != null)
                            {
                                gui.Draw();
                            }

                            EditorGUILayout.EndVertical();
                        });
                    }

                    if (m_ListDeleteSkillEvent.Count > 0)
                    {
                        for (int i = 0; i < m_ListDeleteSkillEvent.Count; i++)
                        {
                            m_ListSkillEvent.RemoveAt(m_ListDeleteSkillEvent[i]);
                        }
                    }
                }


                if (GUILayout.Button("增加技能事件"))
                {
                    if (m_CurrFrame > 0)
                    {
                        if (!SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out var list))
                        {
                            list = new SerializableList<SkillEditorConfigData.SkillEvent>();
                            m_ListSkillEvent = list.ToList();
                            SkillEditorHelper.currConfigData.dicSkillEvents.Add(m_CurrFrame, list);
                        }

                        m_ListSkillEvent.Add(new SkillEditorConfigData.SkillEvent());
                    }
                    else
                    {
                        m_EditorWindow.ShowNotification("帧索引异常");
                    }
                }

                EditorGUILayout.EndVertical();
            });

      

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("技能按键");

                if (SkillEditorHelper.currConfigData.skillKey != null)
                {
                    SkillEditorHelper.currConfigData.skillKey.addTrigger = EditorGUILayout.Toggle("加入触发", SkillEditorHelper.currConfigData.skillKey.addTrigger);

                    int removeKeyIndex = -1;

                    for (int i = 0; i < SkillEditorHelper.currConfigData.skillKey.keys.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        SkillEditorHelper.currConfigData.skillKey.keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(SkillEditorHelper.currConfigData.skillKey.keys[i]);
                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            removeKeyIndex = i;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (removeKeyIndex >= 0)
                    {
                        m_ListKey.RemoveAt(removeKeyIndex);
                        SkillEditorHelper.currConfigData.skillKey.keys = m_ListKey.ToArray();
                        removeKeyIndex = -1;
                    }
                }

                if (GUILayout.Button("增加按键"))
                {
                    m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                    SkillEditorHelper.currConfigData.skillKey.keys = m_ListKey.ToArray();
                }

                EditorGUILayout.EndVertical();
            });

            EditorGUILayout.EndScrollView();
        }

        private List<SkillEditorConfigData.SkillEvent> m_ListSkillEvent = null;
        private List<int> m_ListDeleteSkillEvent = null;
        private List<SkillGUI> m_ListSillEventGUI = null;
        private int m_SkillFrameCount = 0;
        private int m_CurrFrame = 0;
        private List<GameFrameWork.Input.KeyType> m_ListKey = null;
        private Vector2 m_ScrollPos = Vector2.zero;
    }
}