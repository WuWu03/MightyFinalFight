using GameFrameWork.Serialize;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public static class SkillEditorHelper
    {
        public static SkillEditorConfigData currConfigData
        {
            get
            {
                if (!HasData())
                {
                    return null;
                }

                return m_SkillEditorConfig.Datas[m_CurrSelectIndex];
            }
        }

        public static List<SkillEditorConfigData> skillDatas
        {
            get
            {
                if (!HasData())
                {
                    return null;
                }

                return m_SkillEditorConfig.Datas;
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

            m_DicSkillEventGUI = new Dictionary<SkillEditorConfigData.SkillEventType, SkillEventGUI>()
            {
                {SkillEditorConfigData.SkillEventType.AnimEvent,new SkillAnimEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.AudioEvent,new SkillAudioEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.TargetTransformEvent,new SkillTransformEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.SelfTransformEvent,new SkillTransformEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.TargetPhysicsEvent,new SkillPhysicsEventGUI(window) },
                {SkillEditorConfigData.SkillEventType.SelfPhysicsEvent,new SkillPhysicsEventGUI(window) },
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

            m_IndexLabelStyle = new GUIStyle(GUI.skin.label);
            m_IndexLabelStyle.alignment = TextAnchor.MiddleLeft;
            m_IndexLabelStyle.fontSize = 18;
            m_IndexLabelStyle.fontStyle = FontStyle.Bold;
            m_IndexLabelStyle.fixedHeight = 20;
 
            m_SelectButtonOnStyle = new GUIStyle("flow node 1");
            m_SelectButtonOnStyle.stretchWidth = true;
            m_SelectButtonOnStyle.alignment = TextAnchor.MiddleCenter;
            m_SelectButtonOnStyle.contentOffset = new Vector2(0, -15f);
            m_SelectButtonOnStyle.fixedHeight = 15f;

            m_SelectButtonStyle = new GUIStyle("flow node 0");
            m_SelectButtonStyle.stretchWidth = true;
            m_SelectButtonStyle.alignment = TextAnchor.MiddleCenter;
            m_SelectButtonStyle.contentOffset = new Vector2(0, -15f);
            m_SelectButtonStyle.fixedHeight = 15f;
        }

        public static void AddData(string name)
        {
            SkillEditorConfigData skillConfigData = new SkillEditorConfigData();
            skillConfigData.skillName = name;
            skillConfigData.skillKey = new SkillEditorConfigData.SkillKey();
            skillConfigData.skillKey.keys = new GameFrameWork.Input.KeyType[0];
            skillConfigData.dicSkillSelectors = new SerializableDictionary<int, SkillEditorConfigData.SkillSelector>();
            skillConfigData.dicSkillEvents = new SerializableDictionary<int, SerializableList<SkillEditorConfigData.SkillEvent>>();
            skillConfigData.skillPrevConditions = new SkillEditorConfigData.SkillPrevCondition[0];
            m_SkillEditorConfig.AddData(skillConfigData);
            m_CurrSelectIndex = m_SkillEditorConfig.Datas.Count - 1;
            SetShowNames();
        }

        public static void RemoveData()
        {
            if (!HasData() || m_CurrSelectIndex < 0 || m_CurrSelectIndex >= m_SkillEditorConfig.Datas.Count)
            {
                return;
            }

            m_SkillEditorConfig.Datas.RemoveAt(m_CurrSelectIndex);

            if (m_CurrSelectIndex >= m_SkillEditorConfig.Datas.Count)
            {
                m_CurrSelectIndex = m_SkillEditorConfig.Datas.Count - 1;
            }
        }

        public static bool HasData()
        {
            if (m_SkillEditorConfig == null || m_SkillEditorConfig.Datas == null || m_SkillEditorConfig.Datas.Count < 1)
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

            for (int i = 0; i < m_SkillEditorConfig.Datas.Count; i++)
            {
                bool hasName = !string.IsNullOrEmpty(m_SkillEditorConfig.Datas[i].skillName);

                if (!hasName)
                {
                    m_SkillEditorConfig.Datas[i].skillName = "Î´ÃüÃû";
                }

                temp.Add(m_SkillEditorConfig.Datas[i].skillName);
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

        public static void DrawSKilEventlGUI(SkillEditorConfigData.SkillEvent skillEvent)
        {
            if (m_DicSkillEventGUI.TryGetValue(skillEvent.skillEventType, out SkillEventGUI skillGUI))
            {
                skillGUI.Draw();
            }
        }

        public static void ShowNotification(this EditorWindow window, string content)
        {
            window.ShowNotification(new GUIContent(content));
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