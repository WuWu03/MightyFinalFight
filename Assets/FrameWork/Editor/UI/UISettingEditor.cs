using UnityEngine;
using System.Text;
using UnityEditor;

[CustomEditor(typeof(UIRefSetting))]
public class UISettingEditor : Editor
{
    private StringBuilder help = new StringBuilder();

    private UIRefSetting setting;

    private void OnEnable()
    {
        setting = target as UIRefSetting;
        setting.CalPath();
        setting.CalResPath();
        setting.CalRealStartegy();
    }

    public override void OnInspectorGUI()
    {
        help.Length = 0;
        serializedObject.Update();

        string panelName = EditorGUILayout.TextField("Panel Name", setting.panelName);
        if (setting.panelName != panelName)
        {
            FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Panel Name");
            setting.panelName = panelName;
        }

        string folder = EditorGUILayout.TextField("Folder", setting.folder);
        if (setting.folder != folder)
        {
            FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Folder Name");
            setting.folder = folder;
        }

        string resFolder = EditorGUILayout.TextField("Res Folder", setting.resFolder);
        if (setting.resFolder != resFolder)
        {
            FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Res Folder Name");
            setting.resFolder = resFolder;
        }

        if (panelName != string.Empty)
        {
            help.AppendLine("View and Controller will be Created at:");
            help.AppendLine("  " + setting._panelPath);
            help.AppendLine("  " + setting._ctrlPath);

            help.AppendLine("Resources will be Created at:");
            help.AppendLine("  " + setting._resPath);
            help.AppendLine();
        }
        else
        {
            EditorGUILayout.HelpBox("Empty Panel Name", MessageType.Error);
        }

        SerializedProperty type = FrameWorkEditorMgr.DrawProperty("Type of UI", serializedObject, "type");
        SerializedProperty scripType = FrameWorkEditorMgr.DrawProperty("Script Type", serializedObject, "scriptType");

        if (type.intValue != (int)UIRefSetting.Type.Root)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty isCustom = FrameWorkEditorMgr.DrawProperty(null, serializedObject, "customLayer");
            if (isCustom.boolValue)
            {
                FrameWorkEditorMgr.DrawProperty("", serializedObject, "layer");
                help.AppendLine("UI Layer: " + setting.layer);
            }
            else
            {
                UIRefSetting.UILayer layer = type.intValue == 1 ? UIRefSetting.UILayer.Root : UIRefSetting.UILayer.General;
                help.AppendLine("UI Layer: " + layer);
            }
            EditorGUILayout.EndHorizontal();

            UIRefSetting.CloseStrategy strategy = (UIRefSetting.CloseStrategy)EditorGUILayout.EnumPopup("Close Strategy", setting.strategy);            
            if (setting.strategy != strategy)
            {
                FrameWorkEditorMgr.RegisterUndo(target, "Change UIRefSetting Close Strategy");
                setting.strategy = strategy;
            }

            setting.CalRealStartegy();

            FrameWorkEditorMgr.DrawProperty("Is Preload", serializedObject, "preLoad");

            help.AppendLine("Pre Load: " + setting.preLoad);
            help.AppendLine("Close Strategy: " + setting.realStartegy);
          
            if (setting.realStartegy == UIRefSetting.CloseStrategy.DelayDestroy)
            {
                FrameWorkEditorMgr.DrawProperty("Unload Time", serializedObject, "unloadTime");
                help.Append("Detroy Delay: " + setting.unloadTime);
            }
        }
        else
        {
            help.AppendLine("UI Layer: Root");
            help.AppendLine("Pre Load: True");
            help.Append("Close Strategy: Eternal");
        }

        EditorGUILayout.HelpBox(help.ToString(), MessageType.None);

        serializedObject.ApplyModifiedProperties();
    }
}
