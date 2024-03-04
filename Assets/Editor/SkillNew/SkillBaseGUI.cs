using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public class SkillBaseGUI : SkillGUI
    {
        public SkillBaseGUI(EditorWindow window) : base(window)
        {
            m_ListKey = new List<GameFrameWork.Input.KeyType>();
        }

        protected override void OnUpdateData()
        {
            if(!SkillEditorHelper.HasData())
            {
                return;
            }

            m_SkillFrameCount = SkillEditorHelper.currConfigData.skillFrameCount;
            m_CurrFrame = Mathf.Min(1, m_SkillFrameCount);

            if(m_CurrFrame > 0)
            {
                SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(m_CurrFrame, out m_SkillEventList);
            }

            m_ListKey.Clear();
            //    m_CurrId = SkillEditorHelper.currConfigData.Id;
            //    m_CurrName = Path.GetFileNameWithoutExtension(SkillEditorHelper.currShowName);
            //    m_CurrType = Path.GetDirectoryName(SkillEditorHelper.currShowName);
            //    m_CurrLevel = SkillEditorHelper2.currConfigData.Level;
            //    m_CurrAnimName = SkillEditorHelper2.currConfigData.AnimationName;
            //    m_CurrHurtSound = SkillEditorHelper2.currConfigData.HurtSound;
            //    m_EnternalTriggerTime = SkillEditorHelper2.currConfigData.EnternalTiggerTime;
            //    m_AnimSpeed = SkillEditorHelper2.currConfigData.AnimSpeed;
            //    m_AnimTime = SkillEditorHelper2.currConfigData.AnimTime;
            //    m_Exp = SkillEditorHelper2.currConfigData.EXP;

            //    if (SkillEditorHelper2.currConfigData.Key.Keys != null)
            //    {
            //        m_ListKey.AddRange(SkillEditorHelper2.currConfigData.Key.Keys);
            //    }
        }

        protected override void OnGUI()
        {
            EditorGUILayout.Space(10f);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            GUILayout.Label(SkillEditorHelper.currShowName, SkillEditorHelper.indexLabelStyle);

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_CurrId = EditorGUILayout.IntField("Id", m_CurrId);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.Id = m_CurrId;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_CurrLevel = EditorGUILayout.IntField("Level", m_CurrLevel);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.Level = m_CurrLevel;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});


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
                }

                EditorGUILayout.EndHorizontal();
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                int frameIndex = EditorGUILayout.IntSlider("当前帧", m_CurrFrame, Mathf.Min(1, SkillEditorHelper.currConfigData.skillFrameCount), SkillEditorHelper.currConfigData.skillFrameCount);
                if (frameIndex != m_CurrFrame)
                {
                    SkillEditorHelper.currConfigData.dicSkillEvents.TryGetValue(frameIndex, out m_SkillEventList);
                    m_CurrFrame = frameIndex;
                }
            });

            if(m_SkillEventList != null)
            {
                for (int i = 0; i < m_SkillEventList.Count; i++)
                {
                    GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                    {
                        m_SkillEventList[i].eventType = (SkillEditorConfigData.SkillEventType)EditorGUILayout.EnumPopup("事件类型", m_SkillEventList[i].eventType);

                    });
                }
            }

            if (GUILayout.Button("增加技能事件"))
            {
                if(m_CurrFrame > 0)
                {
                    if (m_SkillEventList == null)
                    {
                        m_SkillEventList = new List<SkillEditorConfigData.SkillEvent>();
                        SkillEditorHelper.currConfigData.dicSkillEvents.Add(m_CurrFrame, m_SkillEventList);
                    }

                    m_SkillEventList.Add(new SkillEditorConfigData.SkillEvent());
                }
                else
                {
                    m_EditorWindow.ShowNotification("帧索引异常");
                }
            }

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(()
            //    =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_CurrAnimName = EditorGUILayout.TextField("AnimName", m_CurrAnimName);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.AnimationName = m_CurrAnimName;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_CurrHurtSound = EditorGUILayout.TextField("HurtSound", m_CurrHurtSound);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.HurtSound = m_CurrHurtSound;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_EnternalTriggerTime = EditorGUILayout.FloatField("EnternalTriggerTime", m_EnternalTriggerTime);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.EnternalTiggerTime = m_EnternalTriggerTime;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_AnimSpeed = EditorGUILayout.FloatField("AnimSpeed", m_AnimSpeed);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.AnimSpeed = m_AnimSpeed;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_AnimTime = EditorGUILayout.IntField("AnimTime", m_AnimTime);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.AnimTime = m_AnimTime;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    m_Exp = EditorGUILayout.IntField("EXP", m_Exp);

            //    if (GUILayout.Button("更改", GUILayout.Width(100)))
            //    {
            //        SkillEditorHelper2.currConfigData.EXP = m_Exp;
            //        ShowNotification("更改成功");
            //    }
            //    EditorGUILayout.EndHorizontal();
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    SkillEditorHelper2.currConfigData.Type = (SkillConfigData.SkillType)EditorGUILayout.EnumPopup("SkillType", SkillEditorHelper2.currConfigData.Type);
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    SkillEditorHelper2.currConfigData.DeployerType = (SkillConfigData.SkillDeployerType)EditorGUILayout.EnumPopup("SkillDeployerType", SkillEditorHelper2.currConfigData.DeployerType);
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    SkillEditorHelper2.currConfigData.TriggerType = (SkillConfigData.SkillTriggerType)EditorGUILayout.EnumPopup("SkillTriggerType", SkillEditorHelper2.currConfigData.TriggerType);
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    SkillEditorHelper2.currConfigData.IsInEffectPlaySound = EditorGUILayout.Toggle("EffectPlaySound", SkillEditorHelper2.currConfigData.IsInEffectPlaySound);
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    SkillEditorHelper2.currConfigData.CanChangeDir = EditorGUILayout.Toggle("CanChangeDir", SkillEditorHelper2.currConfigData.CanChangeDir);
            //});

            //GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            //{
            //    SkillEditorHelper2.currConfigData.CanMove = EditorGUILayout.Toggle("CanMove", SkillEditorHelper2.currConfigData.CanMove);
            //});

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("SkillKey");

                if (SkillEditorHelper.currConfigData.Key != null)
                {
                    SkillEditorHelper.currConfigData.Key.addTrigger = EditorGUILayout.Toggle("AddTrigger", SkillEditorHelper.currConfigData.Key.addTrigger);

                    int removeKeyIndex = -1;

                    for (int i = 0; i < SkillEditorHelper.currConfigData.Key.keys.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        SkillEditorHelper.currConfigData.Key.keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(SkillEditorHelper.currConfigData.Key.keys[i]);
                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            removeKeyIndex = i;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (removeKeyIndex >= 0)
                    {
                        m_ListKey.RemoveAt(removeKeyIndex);
                        SkillEditorHelper.currConfigData.Key.keys = m_ListKey.ToArray();
                        removeKeyIndex = -1;
                    }
                }

                if (GUILayout.Button("增加按键"))
                {
                    m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                    SkillEditorHelper.currConfigData.Key.keys = m_ListKey.ToArray();
                }

                //if (GUILayout.Button("默认重力"))
                //{
                //    for (int i = 0; i < SkillEditorHelper.skillDatas.Count; i++)
                //    {
                //        for (int j = 0; j < SkillEditorHelper.skillDatas[i].SkillEffects.Length; j++)
                //        {
                //            SkillEditorHelper.skillDatas[i].SkillEffects[j].Gravity = 1f;
                //        }
                //    };
                //}

                EditorGUILayout.EndVertical();
            });

            EditorGUILayout.EndScrollView();
        }

        private int m_CurrId = 0;
        private int m_CurrLevel = 0;
        private string m_CurrType = string.Empty;
        private string m_CurrName = string.Empty;
        private string m_CurrAnimName = string.Empty;
        private string m_CurrHurtSound = string.Empty;
        private float m_EnternalTriggerTime = 0;
        private float m_AnimSpeed = 0;
        private int m_AnimTime = 0;
        private int m_Exp = 0;
        private List<SkillEditorConfigData.SkillEvent> m_SkillEventList = null;
        private int m_SkillFrameCount = 0;
        private int m_CurrFrame = 0;
        private List<GameFrameWork.Input.KeyType> m_ListKey = null;
        private Vector2 m_ScrollPos = Vector2.zero;
    }
}