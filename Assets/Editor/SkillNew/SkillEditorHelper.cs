using GameFrameWork.Editor;
using System.Collections;
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

        public static GUIStyle textFiledNormalStyle
        {
            get
            {
                return m_TextFiledNormalStyle;
            }
        }

        public static GUIStyle textFieldModifyStyle
        {
            get
            {
                return m_TextFiledModifyStyle;
            }
        }

        public static void InitConfig()
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
            m_CurrSelectIndex = 0;

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

            m_TextFiledNormalStyle = new GUIStyle(EditorStyles.textField);
            m_TextFiledModifyStyle = new GUIStyle(EditorStyles.textField);
            m_TextFiledModifyStyle.normal.textColor = Color.red;
            m_TextFiledModifyStyle.active.textColor = Color.red;
            m_TextFiledModifyStyle.hover.textColor = Color.red;
            m_TextFiledModifyStyle.focused.textColor = Color.red;
        }

        public static GUIStyle GetTextFieldStyle(bool isModify = false)
        {
            if (isModify)
            {
                return m_TextFiledModifyStyle;
            }

            return m_TextFiledNormalStyle;
        }

        public static void AddData(string name)
        {
            SkillEditorConfigData skillConfigData = new SkillEditorConfigData();
            skillConfigData.skillName = name;
            skillConfigData.Key = new SkillEditorConfigData.SkillKey();
            skillConfigData.Key.keys = new GameFrameWork.Input.KeyType[0];
            skillConfigData.dicSkillSelectors = new Dictionary<int, SkillEditorConfigData.SkillSelector>();
            skillConfigData.dicSkillEvents = new Dictionary<int, List<SkillEditorConfigData.SkillEvent>>();
            skillConfigData.SkillPrevConditions = new SkillEditorConfigData.SkillPrevCondition[0];
            //skillConfigData.SkillEffects = new SkillEditorConfigData.SkillEffect[0];
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
                string name = string.IsNullOrEmpty(m_SkillEditorConfig.Datas[i].skillName) ? "Î´ÃüÃû" : m_SkillEditorConfig.Datas[i].skillName;
                temp.Add(name);
            }

            m_ShowNames = temp.ToArray();
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
        private static SkillEditorConfig m_SkillEditorConfig = null;
        private static GUIStyle m_IndexLabelStyle = null;
        private static GUIStyle m_SelectButtonOnStyle = null;
        private static GUIStyle m_SelectButtonStyle = null;
        private static GUIStyle m_TextFiledNormalStyle = null;
        private static GUIStyle m_TextFiledModifyStyle = null;
    }
}