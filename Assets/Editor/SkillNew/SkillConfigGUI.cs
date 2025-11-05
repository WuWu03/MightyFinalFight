using System.Collections.Generic;
using System.Linq;
using GameFrameWork.Serialize;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData;

namespace SkillNew
{
    public class SkillConfigGUI : SkillBaseGUI
    {
        private SerializableList<SkillEvent> m_SkillEvents;
        private readonly List<SkillEventType> m_SkillEventTypes;
        private readonly List<bool> m_SkillEventContinuous;
        private readonly List<int> m_ListSkillEventNextSkill;
        private readonly List<GameFrameWork.Input.KeyType> m_ListKey;
        private int m_SkillFrameCount;
        private int m_CurrFrame;
        private Vector2 m_ScrollPos = Vector2.zero;
        
        public SkillConfigGUI(EditorWindow window) : base(window)
        {
            m_ListKey = new List<GameFrameWork.Input.KeyType>();
            m_SkillEventTypes = new List<SkillEventType>();
            m_SkillEventContinuous = new List<bool>();
            m_ListSkillEventNextSkill = new List<int>();
        }

        protected override void OnUpdateData()
        {
            if (!SkillEditorHelper.HasData())
            {
                return;
            }

            m_SkillFrameCount = SkillEditorHelper.CurrConfigData.skillFrameCount;
            m_CurrFrame = Mathf.Min(1, m_SkillFrameCount);
            m_SkillEvents = null;

            if (m_CurrFrame > 0)
            {
                m_SkillEvents = SkillEditorHelper.CurrConfigData.dicSkillEvents.GetValueOrDefault(m_CurrFrame);

                if (m_SkillEvents is { Count: > 0 })
                {
                    m_SkillEventTypes.Clear();
                    m_SkillEventContinuous.Clear();
                    m_ListSkillEventNextSkill.Clear();

                    foreach (var skillEvent in m_SkillEvents)
                    {
                        m_SkillEventTypes.Add(skillEvent.skillEventType);
                        m_SkillEventContinuous.Add(skillEvent.continuous);
                        m_ListSkillEventNextSkill.Add(skillEvent.nextSkill);
                        SkillEditorHelper.UpdateSKilEventGUI(skillEvent);
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
                DrawField(() => { return m_SkillFrameCount != SkillEditorHelper.CurrConfigData.skillFrameCount; },
                    () => { m_SkillFrameCount = EditorGUILayout.IntField("技能帧数", m_SkillFrameCount); },
                    () =>
                    {
                        SkillEditorHelper.CurrConfigData.skillFrameCount = m_SkillFrameCount;
                        SkillEditorHelper.SetShowNames();
                        int[] deletes = SkillEditorHelper.CurrConfigData.dicSkillEvents.Keys.Where(x => x > m_SkillFrameCount).ToArray();

                        foreach (int delete in deletes)
                        {
                            SkillEditorHelper.CurrConfigData.dicSkillEvents.Remove(delete);
                        }

                        m_SkillEvents = SkillEditorHelper.CurrConfigData.dicSkillEvents.GetValueOrDefault(m_CurrFrame);
                    });
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                int frameIndex = EditorGUILayout.IntSlider("当前帧", m_CurrFrame, Mathf.Min(1, SkillEditorHelper.CurrConfigData.skillFrameCount), SkillEditorHelper.CurrConfigData.skillFrameCount);
                if (frameIndex != m_CurrFrame)
                {
                    m_SkillEvents = SkillEditorHelper.CurrConfigData.dicSkillEvents.GetValueOrDefault(frameIndex);

                    if (m_SkillEvents is { Count: > 0 })
                    {
                        foreach (var skillEvent in m_SkillEvents)
                        {
                            SkillEditorHelper.UpdateSKilEventGUI(skillEvent);
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
                    int currentIndex = i;
                    GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                    {
                        EditorGUILayout.BeginVertical();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField((currentIndex + 1).ToString());
                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            if (EditorUtility.DisplayDialog("提示", "确定删除该事件？", "确定", "取消"))
                            {
                                removeIndex = currentIndex;
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        DrawField(() => { return m_SkillEventTypes[currentIndex] != m_SkillEvents[currentIndex].skillEventType; },
                            () => { m_SkillEventTypes[currentIndex] = (SkillEventType)EditorGUILayout.EnumPopup("事件类型", m_SkillEventTypes[currentIndex]); },
                            () =>
                            {
                                if (m_SkillEventTypes[currentIndex] != m_SkillEvents[currentIndex].skillEventType)
                                {
                                    bool hasSameEvent = false;

                                    foreach (var tempSkillEvent in m_SkillEvents)
                                    {
                                        if (tempSkillEvent == m_SkillEvents[currentIndex])
                                        {
                                            continue;
                                        }

                                        if (tempSkillEvent.skillEventType != SkillEventType.None && 
                                            m_SkillEvents[currentIndex].skillEventType != SkillEventType.None &&
                                            tempSkillEvent.skillEventType == m_SkillEvents[currentIndex].skillEventType)
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
                                        m_SkillEvents[currentIndex].skillEventType = m_SkillEventTypes[currentIndex];
                                        SkillEditorHelper.UpdateSKilEventGUI(m_SkillEvents[currentIndex]);
                                        m_EditorWindow.ShowNotification(new GUIContent("更改成功"));
                                    }
                                }
                            }, 20, false);

                        SkillEditorHelper.DrawSKilEventGUI(m_SkillEvents[currentIndex]);

                        DrawField(() => { return m_SkillEventContinuous[currentIndex] != m_SkillEvents[currentIndex].continuous; },
                            () => { m_SkillEventContinuous[currentIndex] = EditorGUILayout.Toggle("持续检测", m_SkillEventContinuous[currentIndex]); },
                            () => 
                            {
                                m_SkillEvents[currentIndex].continuous = m_SkillEventContinuous[currentIndex];
                                m_SkillEventContinuous[currentIndex] = m_SkillEventContinuous[currentIndex];
                            }, 20);

                        EditorGUILayout.EndVertical();
                    });
                }

                if (removeIndex >= 0)
                {
                    m_SkillEvents.RemoveAt(removeIndex);
                    m_SkillEventTypes.RemoveAt(removeIndex);
                    m_SkillEventContinuous.RemoveAt(removeIndex);

                    if (m_SkillEvents.Count < 1)
                    {
                        SkillEditorHelper.CurrConfigData.dicSkillEvents.Remove(m_CurrFrame);
                        m_SkillEvents = null;
                    }
                }
            }

            if (GUILayout.Button("增加技能事件"))
            {
                if (m_CurrFrame > 0)
                {
                    if (!SkillEditorHelper.CurrConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out var list))
                    {
                        list = new();
                        SkillEditorHelper.CurrConfigData.dicSkillEvents.Add(m_CurrFrame, list);
                    }
                    
                    m_SkillEvents = list;
                    m_SkillEvents.Add(new SkillEvent());
                    m_SkillEventTypes.Add(SkillEventType.None);
                    m_SkillEventContinuous.Add(false);
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

                if (SkillEditorHelper.CurrConfigData.skillKey != null)
                {
                    SkillEditorHelper.CurrConfigData.skillKey.addTrigger = EditorGUILayout.Toggle("加入触发", SkillEditorHelper.CurrConfigData.skillKey.addTrigger);

                    for (int i = 0; i < SkillEditorHelper.CurrConfigData.skillKey.keys.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        SkillEditorHelper.CurrConfigData.skillKey.keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(SkillEditorHelper.CurrConfigData.skillKey.keys[i]);
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
                    SkillEditorHelper.CurrConfigData.skillKey.keys = m_ListKey.ToArray();
                }

                EditorGUILayout.EndVertical();
            });

            if (removeKeyIndex >= 0)
            {
                m_ListKey.RemoveAt(removeKeyIndex);
                SkillEditorHelper.CurrConfigData.skillKey.keys = m_ListKey.ToArray();
                removeKeyIndex = -1;
            }

            EditorGUILayout.EndScrollView();
        }
    }
}