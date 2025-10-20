using System;
using System.Text;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEditor;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(UIRefSetting))]
    public class UIRefSettingEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            m_UIRefSetting = target as UIRefSetting;
            RefreshPath(m_UIRefSetting.moduleName, m_UIRefSetting.viewName);
        }

        public override void OnInspectorGUI()
        {
            m_SBHelp.Length = 0;
            serializedObject.Update();

            string viewName = EditorGUILayout.TextField("View Name", m_UIRefSetting.viewName);
            string moduleName = EditorGUILayout.TextField("Module Name", m_UIRefSetting.moduleName);

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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                m_UIRefSetting.viewName = viewName;
                RefreshPath(moduleName, viewName);
            }

            if (m_UIRefSetting.moduleName != moduleName)
            {
                EditorUtil.RegisterUndo(target, "设置改变：Module Name");
                m_UIRefSetting.moduleName = moduleName;
                RefreshPath(moduleName, viewName);
            }

            UIRefSetting.UILayer uiLayer = (UIRefSetting.UILayer)EditorGUILayout.EnumPopup("UI Layer", m_UIRefSetting.uiLayer);
            if (m_UIRefSetting.uiLayer != uiLayer)
            {
                EditorUtil.RegisterUndo(target, "设置改变：UI Layer");
                m_UIRefSetting.uiLayer = uiLayer;
            }

            UIRefSetting.UIType uiType =
                (UIRefSetting.UIType)EditorGUILayout.EnumPopup("UI Type", m_UIRefSetting.uiType);
            if (m_UIRefSetting.uiType != uiType)
            {
                EditorUtil.RegisterUndo(target, "设置改变：UI Type");
                m_UIRefSetting.uiType = uiType;
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                m_SBHelp.AppendLine("1.脚本创建路径:");
                m_SBHelp.AppendLine("        UI逻辑脚本" + m_UIRefSetting.viewPath);
                m_SBHelp.AppendLine("        UI视图脚本" + m_UIRefSetting.componentPath);
                m_SBHelp.AppendLine("        UI设置脚本" + m_UIRefSetting.settingsPath);
                m_SBHelp.AppendLine("2.预制体创建路径:");
                m_SBHelp.AppendLine("        " + EditorMgr.GetGameFrameWorkConfig().uiPrefabsPath);
                m_SBHelp.AppendLine();
            }
            else
            {
                EditorGUILayout.HelpBox("Empty Panel Name", MessageType.Error);
            }

            m_SBHelp.AppendLine("UI Layer: " + m_UIRefSetting.uiLayer);

            UIRefSetting.UIDestroyMode uiDestroyMode = (UIRefSetting.UIDestroyMode)EditorGUILayout.EnumPopup("Destroy Mode", m_UIRefSetting.uiDestroyMode);
            if (m_UIRefSetting.uiDestroyMode != uiDestroyMode)
            {
                EditorUtil.RegisterUndo(target, "设置改变： Destroy Mode");
                m_UIRefSetting.uiDestroyMode = uiDestroyMode;
            }

            m_SBHelp.AppendLine("Destroy Mode: " + m_UIRefSetting.uiDestroyMode);

            if (m_UIRefSetting.uiDestroyMode == UIRefSetting.UIDestroyMode.Delay)
            {
                if (m_UIRefSetting.delayDestroyTime == 0)
                {
                    m_UIRefSetting.delayDestroyTime = 10f;
                }

                EditorGUILayout.FloatField("Delay Destroy Time", m_UIRefSetting.delayDestroyTime);
                m_SBHelp.Append("Delay Destroy Time: " + m_UIRefSetting.delayDestroyTime);
            }
            else
            {
                m_UIRefSetting.delayDestroyTime = 0f;
            }

            EditorGUILayout.HelpBox(m_SBHelp.ToString(), MessageType.None);
            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshPath(string moduleName, string viewName)
        {
            GameFrameWorkConfigWindowData windowData = GameFrameWork.Editor.EditorMgr.GetGameFrameWorkConfig();
            
            if (string.IsNullOrEmpty(moduleName))
            {
                m_UIRefSetting.moduleName = "Module";
            }
            
            if (string.IsNullOrEmpty(viewName))
            {
                m_UIRefSetting.viewName = "View";    
            }
            
            m_UIRefSetting.viewPath = PathUtil.FormatPath(windowData.uiScriptsPath, moduleName, viewName, ".cs");
            m_UIRefSetting.componentPath = PathUtil.FormatPath(windowData.uiScriptsPath, moduleName, viewName + "Component", ".cs");
            m_UIRefSetting.settingsPath = PathUtil.FormatPath(windowData.uiScriptsPath, moduleName, viewName + "Settings", ".cs");
        }

        private StringBuilder m_SBHelp = new();
        private UIRefSetting m_UIRefSetting;
    }
}