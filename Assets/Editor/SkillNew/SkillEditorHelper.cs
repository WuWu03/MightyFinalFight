using System;
using GameFrameWork.Serialize;
using System.Collections.Generic;
using System.IO;
using GameFrameWork.Input;
using GameFrameWork.Utils;
using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public static class SkillEditorHelper
    {
        public static SkillEditorConfigData CurrConfigData
        {
            get
            {
                if (!HasData())
                {
                    return null;
                }

                return m_SkillEditorConfig.listDatas[m_CurrSelectIndex];
            }
        }

        public static int currSelectIndex
        {
            get
            {
                return m_CurrSelectIndex;
            }
            set
            {
                m_CurrSelectIndex = value;
            }
        }

        public static string currShowType
        {
            get
            {
                return Path.GetDirectoryName(m_ShowNames[m_CurrSelectIndex]);
            }
        }

        public static string currShowName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(m_ShowNames[m_CurrSelectIndex]);
            }
        }

        public static string[] showNames
        {
            get
            {
                return m_ShowNames;
            }
        }

        public static GUIStyle selectButtonOnStyle
        {
            get
            {
                return m_SelectButtonOnStyle;
            }
        }

        public static GUIStyle selectButtonStyle
        {
            get
            {
                return m_SelectButtonStyle;
            }
        }

        public static GUIStyle indexLabelStyle
        {
            get
            {
                return m_IndexLabelStyle;
            }
        }

        public static void InitConfig(EditorWindow window)
        {
            string fileName = "SkillEditorConfig";
            string ext = ".asset";
            string path = Application.dataPath + "/Editor/Config/";

            if (!File.Exists(path + fileName + ext))
            {
                GameFrameWork.Editor.EditorUtil.CreateConfigData<SkillEditorConfig, SkillEditorConfigData>(fileName, ext, path);
            }

            if (m_SkillEditorConfig == null)
            {
                m_SkillEditorConfig = AssetDatabase.LoadAssetAtPath<SkillEditorConfig>("Assets/Editor/Config/" + fileName + ext);
            }

            m_DicSkillEventGUI = new()
            {
                {SkillEditorConfigData.SkillEventType.AnimEvent,new SkillAnimEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.AudioEvent,new SkillAudioEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.TargetTransformEvent,new SkillTransformEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.SelfTransformEvent,new SkillTransformEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.TargetPhysicsEvent,new SkillPhysicsEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.SelfPhysicsEvent,new SkillPhysicsEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.BulletEvent,new SkillBulletEventGUI(window) },
            };

            m_CurrSelectIndex = 0;
            SetShowNames();
        }


        public static void InitGUIStyle()
        {
            if (m_IndexLabelStyle != null)
            {
                return;
            }

            m_IndexLabelStyle = new(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                fixedHeight = 20
            };

            m_SelectButtonOnStyle = new("flow node 1")
            {
                stretchWidth = true,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 15f
            };

            m_SelectButtonStyle = new("flow node 0")
            {
                stretchWidth = true,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 15f
            };
        }

        public static void AddData(string name)
        {
            SkillEditorConfigData skillConfigData = new()
            {
                skillName = name,
                skillKey = new()
                {
                    keys = Array.Empty<KeyType>(),
                },
                dicSkillSelectors = new(),
                dicSkillEvents = new(),
                skillPrevConditions = Array.Empty<SkillEditorConfigData.SkillPrevCondition>()
            };
            
            m_SkillEditorConfig.AddData(skillConfigData);
            m_CurrSelectIndex = m_SkillEditorConfig.listDatas.Count - 1;
            SetShowNames();
        }

        public static void RemoveData()
        {
            if (!HasData() || m_CurrSelectIndex < 0 || m_CurrSelectIndex >= m_SkillEditorConfig.listDatas.Count)
            {
                return;
            }

            m_SkillEditorConfig.listDatas.RemoveAt(m_CurrSelectIndex);

            if (m_CurrSelectIndex >= m_SkillEditorConfig.listDatas.Count)
            {
                m_CurrSelectIndex = m_SkillEditorConfig.listDatas.Count - 1;
            }
        }

        public static bool HasData()
        {
            if (m_SkillEditorConfig == null || m_SkillEditorConfig.listDatas == null || m_SkillEditorConfig.listDatas.Count < 1)
            {
                return false;
            }

            return true;
        }

        public static void SetShowNames()
        {
            if (!HasData())
            {
                return;
            }

            List<string> temp = new List<string>();

            for (int i = 0; i < m_SkillEditorConfig.listDatas.Count; i++)
            {
                bool hasName = !string.IsNullOrEmpty(m_SkillEditorConfig.listDatas[i].skillName);

                if (!hasName)
                {
                    m_SkillEditorConfig.listDatas[i].skillName = "未命名";
                }

                temp.Add(m_SkillEditorConfig.listDatas[i].skillName);
            }

            m_ShowNames = temp.ToArray();
        }

        public static void UpdateSKilEventGUI(SkillEditorConfigData.SkillEvent skillEvent)
        {
            if (m_DicSkillEventGUI.TryGetValue(skillEvent.skillEventType, out SkillEventGUI skillGUI))
            {
                skillGUI.ResetEvent();
                skillGUI.UpdateSkillEvent(skillEvent);
            }
        }

        public static void DrawSKilEventGUI(SkillEditorConfigData.SkillEvent skillEvent)
        {
            if (m_DicSkillEventGUI.TryGetValue(skillEvent.skillEventType, out SkillEventGUI skillGUI))
            {
                skillGUI.Draw();
            }
        }

        public static void SaveConfig()
        {
            UnityEditor.EditorUtility.SetDirty(m_SkillEditorConfig);
        }

        private static string[] m_ShowNames = null;
        private static int m_CurrSelectIndex = -1;
        private static Dictionary<SkillEditorConfigData.SkillEventType, SkillEventGUI> m_DicSkillEventGUI = null;
        private static SkillEditorConfig m_SkillEditorConfig = null;
        private static GUIStyle m_IndexLabelStyle = null;
        private static GUIStyle m_SelectButtonOnStyle = null;
        private static GUIStyle m_SelectButtonStyle = null;
    }
}