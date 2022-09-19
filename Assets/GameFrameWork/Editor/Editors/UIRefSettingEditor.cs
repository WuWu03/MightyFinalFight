using UnityEngine;
using System.Text;
using UnityEditor;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(UIRefSetting))]
    public class UIRefSettingEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            m_UIRefSetting = target as UIRefSetting;
            m_UIRefSetting.RefreshPanelFolder();
            m_UIRefSetting.RefreshPrefabFolder();
        }

        public override void OnInspectorGUI()
        {
            m_SBHelp.Length = 0;
            serializedObject.Update();

            string panelName = EditorGUILayout.TextField("Panel Name", m_UIRefSetting.panelName);
            if (m_UIRefSetting.panelName != panelName)
            {
                EditorUtil.RegisterUndo(target, "Change UIRefSetting Panel Name");
                m_UIRefSetting.panelName = panelName;
            }

            string scriptFolder = EditorGUILayout.TextField("Script Folder", m_UIRefSetting.scriptFolder);
            if (m_UIRefSetting.scriptFolder != scriptFolder)
            {
                EditorUtil.RegisterUndo(target, "Change UIRefSetting Folder Name");
                m_UIRefSetting.scriptFolder = scriptFolder;
            }

            string prefabFolder = EditorGUILayout.TextField("Prefab Folder", m_UIRefSetting.prefabFolder);
            if (m_UIRefSetting.prefabFolder != prefabFolder)
            {
                EditorUtil.RegisterUndo(target, "Change UIRefSetting Res Folder Name");
                m_UIRefSetting.prefabFolder = prefabFolder;
            }

            UIRefSetting.ExoprtScriptType scriptType = (UIRefSetting.ExoprtScriptType)EditorGUILayout.EnumPopup("Script Type", m_UIRefSetting.scriptType);
            if (m_UIRefSetting.scriptType != scriptType)
            {
                EditorUtil.RegisterUndo(target, "Change UIRefSetting Script Type");
                m_UIRefSetting.scriptType = scriptType;
            }

            UIRefSetting.Type panelType = (UIRefSetting.Type)EditorGUILayout.EnumPopup("Panel Type", m_UIRefSetting.panelType);
            if (m_UIRefSetting.panelType != panelType)
            {
                EditorUtil.RegisterUndo(target, "Change UIRefSetting Panel Type");
                m_UIRefSetting.panelType = panelType;
            }

            if (!string.IsNullOrEmpty(panelName))
            {
                m_SBHelp.AppendLine("1.View and Controller will be Created at:");
                m_SBHelp.AppendLine("        " + m_UIRefSetting.panelPath);
                m_SBHelp.AppendLine("        " + m_UIRefSetting.panelComponentPath);
                m_SBHelp.AppendLine("2.Prefab will be Created at:");
                m_SBHelp.AppendLine("        " + m_UIRefSetting.panelPrefabPath);
                m_SBHelp.AppendLine();
            }
            else
            {
                EditorGUILayout.HelpBox("Empty Panel Name", MessageType.Error);
            }

            if (panelType != UIRefSetting.Type.Root)
            {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty isCustom = EditorUtil.DrawProperty(null, serializedObject, "isCustomLayer");
                if (isCustom.boolValue)
                {
                    EditorUtil.DrawProperty("", serializedObject, "panelLayer");
                    m_SBHelp.AppendLine("Panel Layer: " + m_UIRefSetting.panelLayer);
                }
                else
                {
                    if (panelType == UIRefSetting.Type.Normal) m_UIRefSetting.panelLayer = UIRefSetting.Layer.FirstLevel;
                    if (panelType == UIRefSetting.Type.Pop) m_UIRefSetting.panelLayer = UIRefSetting.Layer.SecondLevel;
                    m_SBHelp.AppendLine("Panel Layer: " + m_UIRefSetting.panelLayer);
                }
                EditorGUILayout.EndHorizontal();


                if (panelType != UIRefSetting.Type.Pop)
                {
                    UIRefSetting.CloseMode closeMode = (UIRefSetting.CloseMode)EditorGUILayout.EnumPopup("Close Mode", m_UIRefSetting.panelCloseMode);
                    if (m_UIRefSetting.panelCloseMode != closeMode)
                    {
                        EditorUtil.RegisterUndo(target, "Change UIRefSetting Close Mode");
                        m_UIRefSetting.panelCloseMode = closeMode;
                    }
                }
                else
                {
                    m_UIRefSetting.panelCloseMode = UIRefSetting.CloseMode.Destroy;
                }

                EditorUtil.DrawProperty("PreLoad Type", serializedObject, "panelPreLoadType");

                m_SBHelp.AppendLine("PreLoad Type: " + m_UIRefSetting.panelPreLoadType);
                m_SBHelp.AppendLine("Close Mode: " + m_UIRefSetting.panelCloseMode);

                if (m_UIRefSetting.panelCloseMode == UIRefSetting.CloseMode.DelayDestroy)
                {
                    if (m_UIRefSetting.unLoadTime == 0) m_UIRefSetting.unLoadTime = 10f;
                    SerializedProperty unLoadTime = EditorUtil.DrawProperty("UnLoad Time", serializedObject, "unLoadTime");
                    m_SBHelp.Append("UnLoad Time: " + m_UIRefSetting.unLoadTime);
                }
                else
                {
                    m_UIRefSetting.unLoadTime = 0f;
                }
            }
            else
            {
                m_SBHelp.AppendLine("UI Layer: MainPanel");
                m_SBHelp.AppendLine("Pre Load: True");
                m_SBHelp.Append("Close Mode: Eternal");
                m_UIRefSetting.panelCloseMode = UIRefSetting.CloseMode.Eternal;
                m_UIRefSetting.panelLayer = UIRefSetting.Layer.MainPanel;
                m_UIRefSetting.unLoadTime = 0f;
            }

            EditorGUILayout.HelpBox(m_SBHelp.ToString(), MessageType.None);

            serializedObject.ApplyModifiedProperties();
        }

        private StringBuilder m_SBHelp = new StringBuilder();

        private UIRefSetting m_UIRefSetting;
    }
}