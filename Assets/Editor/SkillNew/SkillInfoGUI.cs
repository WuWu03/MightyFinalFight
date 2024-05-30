using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData;

namespace SkillNew
{
    public class SkillInfoGUI : SkillBaseGUI
    {
        public SkillInfoGUI(EditorWindow window) : base(window)
        {
            m_ListKey = new List<GameFrameWork.Input.KeyType>();
            m_ListSkillEventType = new List<SkillEventType>();
            m_ListSkillEventContinuous = new List<bool>();
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

            m_ListSkillEvent = null;

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
                    m_ListSkillEventType.Clear();
                    m_ListSkillEventContinuous.Clear();
                    m_ListSkillEventNextSkill.Clear();

                    for (int i = 0; i < m_ListSkillEvent.Count; i++)
                    {
                        m_ListSkillEventType.Add(m_ListSkillEvent[i].skillEventType);
                        m_ListSkillEventContinuous.Add(m_ListSkillEvent[i].continuous);
                        m_ListSkillEventNextSkill.Add(m_ListSkillEvent[i].nextSkill);
                        SkillEditorHelper.UpdateSKilEventGUI(m_ListSkillEvent[i]);
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
                    () => {
                        SkillEditorHelper.currConfigData.skillFrameCount = m_SkillFrameCount;
                        SkillEditorHelper.SetShowNames();
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
                    }, 20);
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
                            SkillEditorHelper.UpdateSKilEventGUI(m_ListSkillEvent[i]);
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

            if (m_ListSkillEvent != null)
            {
                int removeIndex = -1;

                for (int i = 0; i < m_ListSkillEvent.Count; i++)
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

                        DrawField(() => { return m_ListSkillEventType[i] != m_ListSkillEvent[i].skillEventType; },
                              () => { m_ListSkillEventType[i] = (SkillEditorConfigData.SkillEventType)EditorGUILayout.EnumPopup("事件类型", m_ListSkillEventType[i]); },
                              () =>{
                                  if (m_ListSkillEventType[i] != m_ListSkillEvent[i].skillEventType)
                                  {
                                      bool hasSameEvent = false;

                                      for (int j = 0; j < m_ListSkillEvent.Count; j++)
                                      {
                                          if (m_ListSkillEvent[j].skillEventType != SkillEditorConfigData.SkillEventType.None &&
                                              m_ListSkillEventType[i] != SkillEditorConfigData.SkillEventType.None &&
                                              m_ListSkillEvent[j].skillEventType == m_ListSkillEventType[i])
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
                                          m_ListSkillEvent[i].skillEventType = m_ListSkillEventType[i];
                                          SkillEditorHelper.UpdateSKilEventGUI(m_ListSkillEvent[i]);
                                          this.m_EditorWindow.ShowNotification("更改成功");
                                      }
                                  }
                              }, 20, false);

                        SkillEditorHelper.DrawSKilEventlGUI(m_ListSkillEvent[i]);

                        DrawField(() => { return m_ListSkillEventContinuous[i] != m_ListSkillEvent[i].continuous; },
                            () => { m_ListSkillEventContinuous[i] = EditorGUILayout.Toggle("持续检测", m_ListSkillEventContinuous[i]); },
                            () => { m_ListSkillEvent[i].continuous = m_ListSkillEventContinuous[i]; }, 20);

                        EditorGUILayout.EndVertical();
                    });
                }

                if (removeIndex >= 0)
                {
                    m_ListSkillEvent.RemoveAt(removeIndex);
                    m_ListSkillEventType.RemoveAt(removeIndex);

                    if (m_ListSkillEvent.Count < 1)
                    {
                        SkillEditorHelper.currConfigData.dicSkillEvents.Remove(m_CurrFrame);
                        m_ListSkillEvent = null;
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
                    m_ListSkillEventType.Add(SkillEventType.None);
                }
                else
                {
                    m_EditorWindow.ShowNotification("帧索引异常");
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

        private List<SkillEvent> m_ListSkillEvent = null;
        private List<SkillEventType> m_ListSkillEventType = null;
        private List<bool> m_ListSkillEventContinuous = null;
        private List<int> m_ListSkillEventNextSkill = null;
        private int m_SkillFrameCount = 0;
        private int m_CurrFrame = 0;
        private List<GameFrameWork.Input.KeyType> m_ListKey = null;
        private Vector2 m_ScrollPos = Vector2.zero;
    }
}