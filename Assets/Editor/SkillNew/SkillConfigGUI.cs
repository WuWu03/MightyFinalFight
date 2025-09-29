using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData;

namespace SkillNew
{
    public class SkillConfigGUI : SkillBaseGUI
    {
        public SkillConfigGUI(EditorWindow window) : base(window)
        {
            m_ListKey = new List<GameFrameWork.Input.KeyType>();
            m_SkillEventTypes = new List<SkillEventType>();
            m_SkillEventContinuouses = new List<bool>();
            m_ListSkillEventNextSkill = new List<int>();
        }

        protected override void OnUpdateData()
        {
            if (!SkillEditorHelper.HasData())
            {
                return;
            }

            m_SkillFrameCount = SkillEditorHelper.currConfigData.skillFrameCount;
            m_CurrFrame = Mathf.Min(1, m_SkillFrameCount);

            m_SkillEvents = null;

            if (m_CurrFrame > 0)
            {
                if (SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out var list))
                {
                    m_SkillEvents = list.ToList();
                }
                else
                {
                    m_SkillEvents = null;
                }

                if (m_SkillEvents != null && m_SkillEvents.Count > 0)
                {
                    m_SkillEventTypes.Clear();
                    m_SkillEventContinuouses.Clear();
                    m_ListSkillEventNextSkill.Clear();

                    for (int i = 0; i < m_SkillEvents.Count; i++)
                    {
                        m_SkillEventTypes.Add(m_SkillEvents[i].skillEventType);
                        m_SkillEventContinuouses.Add(m_SkillEvents[i].continuous);
                        m_ListSkillEventNextSkill.Add(m_SkillEvents[i].nextSkill);
                        SkillEditorHelper.UpdateSKilEventGUI(m_SkillEvents[i]);
                    }
                }
            }

            m_ListKey.Clear();
        }

        protected override void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                GUILayout.Label(SkillEditorHelper.currShowName, SkillEditorHelper.indexLabelStyle);
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                DrawField(() => { return m_SkillFrameCount != SkillEditorHelper.currConfigData.skillFrameCount; },
                    () => { m_SkillFrameCount = EditorGUILayout.IntField("技能帧数", m_SkillFrameCount); },
                    () =>
                    {
                        SkillEditorHelper.currConfigData.skillFrameCount = m_SkillFrameCount;
                        SkillEditorHelper.SetShowNames();
                        int[] deletes = SkillEditorHelper.currConfigData.dicSkillEvents.Keys.Where(x => x > m_SkillFrameCount).ToArray();

                        foreach (int delete in deletes)
                        {
                            SkillEditorHelper.currConfigData.dicSkillEvents.Remove(delete);
                        }

                        if (SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out var list))
                        {
                            m_SkillEvents = list.ToList();
                        }
                        else
                        {
                            m_SkillEvents = null;
                        }
                    }, 20);
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                int frameIndex = EditorGUILayout.IntSlider("当前帧", m_CurrFrame, Mathf.Min(1, SkillEditorHelper.currConfigData.skillFrameCount), SkillEditorHelper.currConfigData.skillFrameCount);
                if (frameIndex != m_CurrFrame)
                {
                    if (SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(frameIndex, out var list))
                    {
                        m_SkillEvents = list.ToList();
                    }
                    else
                    {
                        m_SkillEvents = null;
                    }

                    if (m_SkillEvents != null)
                    {
                        for (int i = 0; i < m_SkillEvents.Count; i++)
                        {
                            SkillEditorHelper.UpdateSKilEventGUI(m_SkillEvents[i]);
                        }
                    }

                    m_CurrFrame = frameIndex;
                }
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.LabelField("技能事件列表");
            });

            EditorGUILayout.BeginVertical();

            if (m_SkillEvents != null)
            {
                int removeIndex = -1;

                for (int i = 0; i < m_SkillEvents.Count; i++)
                {
                    GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                    {
                        EditorGUILayout.BeginVertical();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField((i + 1).ToString());
                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            if (UnityEditor.EditorUtility.DisplayDialog("提示", "确定删除该事件？", "确定", "取消"))
                            {
                                removeIndex = i;
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        DrawField(() => { return m_SkillEventTypes[i] != m_SkillEvents[i].skillEventType; },
                            () => { m_SkillEventTypes[i] = (SkillEventType)EditorGUILayout.EnumPopup("事件类型", m_SkillEventTypes[i]); },
                            () =>
                            {
                                if (m_SkillEventTypes[i] != m_SkillEvents[i].skillEventType)
                                {
                                    bool hasSameEvent = false;

                                    for (int j = 0; j < m_SkillEvents.Count; j++)
                                    {
                                        if (m_SkillEvents[j] == m_SkillEvents[i])
                                        {
                                            continue;
                                        }

                                        if (m_SkillEvents[j].skillEventType != SkillEventType.None &&
                                            m_SkillEvents[i].skillEventType != SkillEventType.None &&
                                            m_SkillEvents[j].skillEventType == m_SkillEvents[i].skillEventType)
                                        {
                                            hasSameEvent = true;
                                            break;
                                        }
                                    }

                                    if (hasSameEvent)
                                    {
                                        m_EditorWindow.ShowNotification(new GUIContent("每种事件类型在同一帧只能出现一次"));
                                    }
                                    else
                                    {
                                        m_SkillEvents[i].skillEventType = m_SkillEventTypes[i];
                                        SkillEditorHelper.UpdateSKilEventGUI(m_SkillEvents[i]);
                                        m_EditorWindow.ShowNotification(new GUIContent("更改成功"));
                                    }
                                }
                            }, 20, false);

                        SkillEditorHelper.DrawSKilEventlGUI(m_SkillEvents[i]);

                        DrawField(() => { return m_SkillEventContinuouses[i] != m_SkillEvents[i].continuous; },
                            () => { m_SkillEventContinuouses[i] = EditorGUILayout.Toggle("持续检测", m_SkillEventContinuouses[i]); },
                            () => 
                            {
                                m_SkillEvents[i].continuous = m_SkillEventContinuouses[i];
                                m_SkillEventContinuouses[i] = m_SkillEventContinuouses[i];
                            }, 20);

                        EditorGUILayout.EndVertical();
                    });
                }

                if (removeIndex >= 0)
                {
                    m_SkillEvents.RemoveAt(removeIndex);
                    m_SkillEventTypes.RemoveAt(removeIndex);
                    m_SkillEventContinuouses.RemoveAt(removeIndex);

                    if (m_SkillEvents.Count < 1)
                    {
                        SkillEditorHelper.currConfigData.dicSkillEvents.Remove(m_CurrFrame);
                        m_SkillEvents = null;
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
                        m_SkillEvents = list.ToList();
                        SkillEditorHelper.currConfigData.dicSkillEvents.Add(m_CurrFrame, list);
                    }

                    m_SkillEvents.Add(new SkillEditorConfigData.SkillEvent());
                    m_SkillEventTypes.Add(SkillEventType.None);
                    m_SkillEventContinuouses.Add(false);
                }
                else
                {
                    m_EditorWindow.ShowNotification(new GUIContent("帧索引异常"));
                }
            }

            EditorGUILayout.EndVertical();

            int removeKeyIndex = -1;

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("技能按键");

                if (SkillEditorHelper.currConfigData.skillKey != null)
                {
                    SkillEditorHelper.currConfigData.skillKey.addTrigger = EditorGUILayout.Toggle("加入触发", SkillEditorHelper.currConfigData.skillKey.addTrigger);

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
                }

                if (GUILayout.Button("增加按键"))
                {
                    m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                    SkillEditorHelper.currConfigData.skillKey.keys = m_ListKey.ToArray();
                }

                EditorGUILayout.EndVertical();
            });

            if (removeKeyIndex >= 0)
            {
                m_ListKey.RemoveAt(removeKeyIndex);
                SkillEditorHelper.currConfigData.skillKey.keys = m_ListKey.ToArray();
                removeKeyIndex = -1;
            }

            EditorGUILayout.EndScrollView();
        }

        private List<SkillEvent> m_SkillEvents = null;
        private List<SkillEventType> m_SkillEventTypes = null;
        private List<bool> m_SkillEventContinuouses = null;
        private List<int> m_ListSkillEventNextSkill = null;
        private int m_SkillFrameCount = 0;
        private int m_CurrFrame = 0;
        private List<GameFrameWork.Input.KeyType> m_ListKey = null;
        private Vector2 m_ScrollPos = Vector2.zero;
    }
}