using UnityEngine;
using System.Text;
using UnityEditor;

[CustomEditor(typeof(UIRefSetting))]
public class UISettingEditor : Editor
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

        string panelName = EditorGUILayout.TextField("Panel Name", m_UIRefSetting.PanelName);
        if (m_UIRefSetting.PanelName != panelName)
        {
            FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Panel Name");
            m_UIRefSetting.PanelName = panelName;
        }

        string scriptFolder = EditorGUILayout.TextField("Script Folder", m_UIRefSetting.ScriptFolder);
        if (m_UIRefSetting.ScriptFolder != scriptFolder)
        {
            FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Folder Name");
            m_UIRefSetting.ScriptFolder = scriptFolder;
        }

        string prefabFolder = EditorGUILayout.TextField("Prefab Folder", m_UIRefSetting.PrefabFolder);
        if (m_UIRefSetting.PrefabFolder != prefabFolder)
        {
            FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Res Folder Name");
            m_UIRefSetting.PrefabFolder = prefabFolder;
        }

        if (!string.IsNullOrEmpty(panelName))
        {
            m_SBHelp.AppendLine("1.View and Controller will be Created at:");
            m_SBHelp.AppendLine("        " + m_UIRefSetting.PanelPath);
            m_SBHelp.AppendLine("        " + m_UIRefSetting.PanelCtrlPath);
            m_SBHelp.AppendLine("2.Prefab will be Created at:");
            m_SBHelp.AppendLine("        " + m_UIRefSetting.PanelPrefabPath);
            m_SBHelp.AppendLine();
        }
        else
        {
            EditorGUILayout.HelpBox("Empty Panel Name", MessageType.Error);
        }

        SerializedProperty type = FrameWorkEditorMgr.DrawProperty("Panel Type", serializedObject, "PanelType");
        SerializedProperty scripType = FrameWorkEditorMgr.DrawProperty("Script Type", serializedObject, "ScriptType");

        if (type.intValue != (int)UIRefSetting.Type.Root)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty isCustom = FrameWorkEditorMgr.DrawProperty(null, serializedObject, "IsCustomLayer");
            if (isCustom.boolValue)
            {
                FrameWorkEditorMgr.DrawProperty("", serializedObject, "PanelLayer");
                m_SBHelp.AppendLine("Panel Layer: " + m_UIRefSetting.PanelLayer);
            }
            else
            {
                UIRefSetting.Layer layer = type.intValue == 1 ? UIRefSetting.Layer.MainPanel : UIRefSetting.Layer.FirstLevel;
                m_SBHelp.AppendLine("Panel Layer: " + layer);
            }
            EditorGUILayout.EndHorizontal();

            UIRefSetting.ExoprtScriptType scriptType = (UIRefSetting.ExoprtScriptType)EditorGUILayout.EnumPopup("Script Type", m_UIRefSetting.ScriptType);
            if (m_UIRefSetting.ScriptType != scriptType)
            {
                FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Script Type");
                m_UIRefSetting.ScriptType = scriptType;
            }

            UIRefSetting.CloseMode closeMode = (UIRefSetting.CloseMode)EditorGUILayout.EnumPopup("Close Mode", m_UIRefSetting.PanelCloseMode);
            if (m_UIRefSetting.PanelCloseMode != closeMode)
            {
                FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Close Mode");
                m_UIRefSetting.PanelCloseMode = closeMode;
            }


            FrameWorkEditorMgr.DrawProperty("PreLoad Type", serializedObject, "PanelPreLoadType");

            m_SBHelp.AppendLine("PreLoad Type: " + m_UIRefSetting.PanelPreLoadType);
            m_SBHelp.AppendLine("Close Mode: " + m_UIRefSetting.PanelCloseMode);

            if (m_UIRefSetting.PanelCloseMode == UIRefSetting.CloseMode.DelayDestroy)
            {
                FrameWorkEditorMgr.DrawProperty("UnLoad Time", serializedObject, "UnLoadTime");
                m_SBHelp.Append("UnLoad Time: " + m_UIRefSetting.UnLoadTime);
            }
        }
        else
        {
            m_SBHelp.AppendLine("UI Layer: Root");
            m_SBHelp.AppendLine("Pre Load: True");
            m_SBHelp.Append("Close Mode: Eternal");
        }

        EditorGUILayout.HelpBox(m_SBHelp.ToString(), MessageType.None);

        serializedObject.ApplyModifiedProperties();
    }

    private StringBuilder m_SBHelp = new StringBuilder();

    private UIRefSetting m_UIRefSetting;
}
