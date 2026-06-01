using System;
using System.Text;
using WuWuFramework.UI;
using WuWuFramework.Utils;
using UnityEditor;

namespace WuWuFramework.Editor
{
    [CustomEditor(typeof(UIRefSetting))]
    public class UIRefSettingEditor : UnityEditor.Editor
    {
        private readonly StringBuilder m_HelpStringBuilder = new();
        private UIRefSetting m_UIRefSetting;

        private void OnEnable()
        {
            if (target is UIRefSetting uiRefSetting)
            {
                m_UIRefSetting = uiRefSetting;
                RefreshPath(uiRefSetting.moduleName, uiRefSetting.presenterName, uiRefSetting.viewName);
            }
        }

        public override void OnInspectorGUI()
        {
            m_HelpStringBuilder.Length = 0;
            serializedObject.Update();

            string moduleName = EditorGUILayout.TextField("Module Name", m_UIRefSetting.moduleName);
            string presenterName = EditorGUILayout.TextField("Presenter Name", m_UIRefSetting.presenterName);
            string viewName = EditorGUILayout.TextField("View Name", m_UIRefSetting.viewName);

            if (m_UIRefSetting.moduleName != moduleName)
            {
                EditorUtil.RegisterUndo(target, "设置改变：Module Name");
                m_UIRefSetting.moduleName = moduleName;
                RefreshPath(moduleName, presenterName, viewName);
            }

            if (m_UIRefSetting.presenterName != presenterName)
            {
                EditorUtil.RegisterUndo(target, "设置改变：Presenter Name");
                m_UIRefSetting.presenterName = presenterName;
                RefreshPath(moduleName, presenterName, viewName);
            }

            if (m_UIRefSetting.viewName != viewName)
            {
                EditorUtil.RegisterUndo(target, "设置改变：View Name");

                switch (m_UIRefSetting.uiType)
                {
                    case UIRefSetting.UIType.Panel:
                        viewName = viewName.EndsWith("Panel") ? viewName : viewName + "Panel";
                        break;
                    case UIRefSetting.UIType.View:
                        viewName = viewName.EndsWith("View") ? viewName : viewName + "View";
                        break;
                    case UIRefSetting.UIType.Item:
                        viewName = viewName.EndsWith("Item") ? viewName : viewName + "Item";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                m_UIRefSetting.viewName = viewName;
                RefreshPath(moduleName, presenterName, viewName);
            }

            if (m_UIRefSetting.uiType != UIRefSetting.UIType.Item)
            {
                UIRefSetting.UILayer uiLayer = (UIRefSetting.UILayer)EditorGUILayout.EnumPopup("UI Layer", m_UIRefSetting.uiLayer);

                if (m_UIRefSetting.uiLayer != uiLayer)
                {
                    EditorUtil.RegisterUndo(target, "设置改变：UI Layer");
                    m_UIRefSetting.uiLayer = uiLayer;
                }
            }

            UIRefSetting.UIType uiType = (UIRefSetting.UIType)EditorGUILayout.EnumPopup("UI Type", m_UIRefSetting.uiType);

            if (m_UIRefSetting.uiType != uiType)
            {
                EditorUtil.RegisterUndo(target, "设置改变：UI Type");
                m_UIRefSetting.uiType = uiType;
            }

            switch (uiType)
            {
                case UIRefSetting.UIType.Item:
                    UIEditorInit.DestroyPanelComponent();
                    break;
                default:
                    UIEditorInit.AddPanelComponent();
                    break;
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                m_HelpStringBuilder.AppendLine("1.脚本创建路径:");
                m_HelpStringBuilder.AppendLine("        UI视图脚本" + m_UIRefSetting.viewPath);

                if (m_UIRefSetting.uiType != UIRefSetting.UIType.Item)
                {
                    m_HelpStringBuilder.AppendLine("        UI设置脚本" + m_UIRefSetting.settingsPath);
                    m_HelpStringBuilder.AppendLine("        UI逻辑脚本" + m_UIRefSetting.presenterPath);
                }

                m_HelpStringBuilder.AppendLine("2.预制体创建路径:");
                m_HelpStringBuilder.AppendLine("        " + EditorMgr.GetWuWuFrameworkConfig().uiPrefabsPath);
                m_HelpStringBuilder.AppendLine();
            }
            else
            {
                EditorGUILayout.HelpBox("Empty Panel Name", MessageType.Error);
            }

            if (m_UIRefSetting.uiType != UIRefSetting.UIType.Item)
            {
                m_HelpStringBuilder.AppendLine("UI Layer: " + m_UIRefSetting.uiLayer);


                UIRefSetting.UIDestroyMode uiDestroyMode = (UIRefSetting.UIDestroyMode)EditorGUILayout.EnumPopup("Destroy Mode", m_UIRefSetting.uiDestroyMode);
                if (m_UIRefSetting.uiDestroyMode != uiDestroyMode)
                {
                    EditorUtil.RegisterUndo(target, "设置改变： Destroy Mode");
                    m_UIRefSetting.uiDestroyMode = uiDestroyMode;
                }

                m_HelpStringBuilder.AppendLine("Destroy Mode: " + m_UIRefSetting.uiDestroyMode);

                if (m_UIRefSetting.uiDestroyMode == UIRefSetting.UIDestroyMode.Delay)
                {
                    if (m_UIRefSetting.delayDestroyTime == 0)
                    {
                        m_UIRefSetting.delayDestroyTime = 10f;
                    }

                    EditorGUILayout.FloatField("Delay Destroy Time", m_UIRefSetting.delayDestroyTime);
                    m_HelpStringBuilder.Append("Delay Destroy Time: " + m_UIRefSetting.delayDestroyTime);
                }
                else
                {
                    m_UIRefSetting.delayDestroyTime = 0f;
                }
            }

            EditorGUILayout.HelpBox(m_HelpStringBuilder.ToString(), MessageType.Info, true);
            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshPath(string moduleName, string presenterName, string viewName)
        {
            WuWuFrameworkConfigWindowData windowData = WuWuFramework.Editor.EditorMgr.GetWuWuFrameworkConfig();

            if (string.IsNullOrEmpty(moduleName))
            {
                m_UIRefSetting.moduleName = "Module";
            }

            if (string.IsNullOrEmpty(presenterName))
            {
                m_UIRefSetting.presenterName = "Presenter";
            }

            if (string.IsNullOrEmpty(viewName))
            {
                m_UIRefSetting.viewName = "View";
            }

            m_UIRefSetting.viewPath = PathUtil.FormatPath(windowData.uiScriptsPath, moduleName, viewName, ".cs");
            m_UIRefSetting.settingsPath = PathUtil.FormatPath(windowData.uiScriptsPath, moduleName, viewName + "Settings", ".cs");
            m_UIRefSetting.presenterPath = PathUtil.FormatPath(windowData.uiScriptsPath, moduleName, presenterName, ".cs");
        }
    }
}