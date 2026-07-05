using System;
using System.IO;
using System.Text;
using WuWuFramework.UI;
using WuWuFramework.Utils;

namespace WuWuFramework.Editor
{
    public class CSharpUIScriptsExporter : BaseUIScriptsExporter
    {
        public override void Export(UIRef[] uiRefs, UIRefSetting setting)
        {
            ExportView(uiRefs, setting);

            if (setting.uiType != UIRefSetting.UIType.Item)
            {
                ExportViewSettings(setting);
                ExportPresenter(setting);
            }

            ExportUIMapping();
        }

        public override string CopyRef(UIRef[] uiRefs)
        {
            StringBuilder sb = new();
            for (int i = 0; i < uiRefs.Length; i++)
            {
                if (!uiRefs[i].isCopyRefStr)
                {
                    continue;
                }

                sb.AppendFormat("public {0} {1} ", uiRefs[i].componentName, uiRefs[i].GetName());
                sb.Append("{ get; private set; };\r\n");
                sb.AppendFormat("{0} = root.Objects[{1}] as {2}\r\n", uiRefs[i].GetName(), i, uiRefs[i].componentName);
            }
            return sb.ToString();
        }

        private void ExportView(UIRef[] uiRefs, UIRefSetting setting)
        {
            StringBuilder sb = new();

            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: {0} 模块 {1} 视图\r\n", setting.moduleName, setting.viewName);
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: " + Author);
            sb.AppendLine(" * @Note: 工具生成，请勿修改");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using TMPro;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using WuWuFramework.UI;");
            sb.AppendLine();
            sb.AppendFormat("public class {0} : UIBaseView<{0}, {0}Presenter, {0}Settings>", setting.viewName);
            sb.AppendLine("\r\n{");

            foreach (var uiRef in uiRefs)
            {
                if (!uiRef.isListItem && !uiRef.isListItemVariable)
                {
                    sb.Append("\t//").Append(GetComment(uiRef));
                    sb.AppendLine();
                    sb.AppendFormat("\tpublic {0} {1}", uiRef.componentName, uiRef.GetName());
                    sb.Append(" { get; private set; }\r\n");
                }
            }

            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnInitView(UIRefRoot root)");
            sb.AppendLine("\t{");

            for (int i = 0; i < uiRefs.Length; i++)
            {
                UIRef uiRef = uiRefs[i];

                if (uiRef.isList)
                {
                    sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
                    sb.AppendFormat("\t\t{0}?.Init<{1}>();\r\n", uiRef.GetName(), uiRef.GetName(true) + "Item");
                }
                else if (!uiRef.isListItem && !uiRef.isListItemVariable)
                {
                    sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
                }
            }

            sb.AppendLine("\t}");

            foreach (var uiRef in uiRefs)
            {
                if (uiRef.isList)
                {
                    ExportLayout(uiRef, sb);
                }
            }

            sb.Append("}");
            FileUtil.CreateTextFile(setting.viewPath, sb.ToString());
        }

        private void ExportViewSettings(UIRefSetting setting)
        {
            StringBuilder sb = new();

            string layerName = Enum.GetName(typeof(UIRefSetting.UILayer), setting.uiLayer);
            string destroyModeName = Enum.GetName(typeof(UIRefSetting.UIDestroyMode), setting.uiDestroyMode);
            string canPopUp = "false";
            float delayDestroyTime = setting.delayDestroyTime;

            if (setting.uiType == UIRefSetting.UIType.View)
            {
                canPopUp = setting.uiLayer switch
                {
                    UIRefSetting.UILayer.MainWindow => "true",
                    UIRefSetting.UILayer.Window1 => "true",
                    _ => "false"
                };
            }

            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: {0} 模块 {1} 视图设置\r\n", setting.moduleName, setting.viewName);
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: " + Author);
            sb.AppendLine(" * @Note: 工具生成，请勿修改");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("using WuWuFramework.UI;");
            sb.AppendLine();
            sb.AppendFormat("public class {0}Settings : UIBaseViewSettings", setting.viewName);
            sb.AppendLine("\r\n{");
            sb.Append("\tpublic override string prefabName { get { " + $"return \"{setting.viewName}.prefab\"" + "; } }\r\n");
            sb.Append("\tpublic override float delayDestroyTime { get { " + $"return {delayDestroyTime}f" + "; } }\r\n");
            sb.Append("\tpublic override bool canPopUp { get { " + $"return {canPopUp}" + "; } }\r\n");
            sb.Append("\tpublic override UILayer layer { get { " + $"return UILayer.{layerName}" + "; } }\r\n");
            sb.Append("\tpublic override UIDestroyMode destroyMode { get { " + $"return UIDestroyMode.{destroyModeName}" + "; } }\r\n");
            sb.Append("}");
            FileUtil.CreateTextFile(setting.settingsPath, sb.ToString());
        }

        private void ExportPresenter(UIRefSetting setting)
        {
            if (File.Exists(setting.presenterPath))
            {
                return;
            }

            StringBuilder sb = new();

            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: {0} 模块 {1} 视图展示器\r\n", setting.moduleName, setting.viewName);
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: " + Author);
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("using WuWuFramework.UI;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendFormat("public class {0} : UIBaseViewPresenter<{0}>", setting.presenterName);
            sb.AppendLine("\r\n{");
            sb.AppendLine("\tprotected override void OnOpen(object arg)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnShow(object arg)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnUpdate()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnHide()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnClose()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnDestroy()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.Append("}");
            FileUtil.CreateTextFile(setting.presenterPath, sb.ToString());
        }


        private void ExportUIMapping()
        {
            StringBuilder sb = new();
            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: UI工厂\r\n");
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: " + Author);
            sb.AppendLine(" * @Note: 工具生成，请勿修改");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("namespace WuWuFramework.UI");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic static partial class UIFactory");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tstatic UIFactory()");
            sb.AppendLine("\t\t{");

            string[] uiTypeNames = EditorUtil.GetAssemblyTypeNames("WuWuFramework.UI.IUIView", false, "UIBaseView");
            for (int i = 0; i < uiTypeNames.Length; i++)
            {
                sb.AppendFormat("\t\t\ts_Factories.Add(typeof({0}), CreateUIView<{0}>);", uiTypeNames[i]);
                sb.AppendLine();
            }
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.Append("}");
            WuWuFrameworkConfigWindowData windowData = WuWuFramework.Editor.EditorMgr.GetWuWuFrameworkConfig();
            FileUtil.CreateTextFile(PathUtil.FormatPath(windowData.uiScriptsPath, "UIFactory.cs"), sb.ToString());
        }

        private void ExportLayout(UIRef uiRef, StringBuilder sb)
        {
            UIRef[] itemUIRefs = uiRef.GetComponentsInChildren<UIRef>(true);
            UIRef tempUIRef = null;

            foreach (var itemUIRef in itemUIRefs)
            {
                if (itemUIRef.isListItem)
                {
                    tempUIRef = itemUIRef;
                    break;
                }
            }

            if (tempUIRef == null)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0} : {1}\r\n", uiRef.GetName(true) + "Item", "BaseListItem");
            sb.AppendLine("\t{");

            foreach (var variableUIRef in itemUIRefs)
            {
                if (!variableUIRef.isListItemVariable)
                {
                    continue;
                }

                sb.AppendFormat("\t\t//{0}\r\n", GetComment(variableUIRef));
                sb.AppendFormat("\t\tpublic {0} {1} ", variableUIRef.componentName, variableUIRef.GetName());
                sb.Append("{get; private set;}\r\n");
            }

            sb.AppendLine("\t\tprotected override void OnCreate(GameObject go)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tUIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();");

            int itemIndex = 0;

            foreach (var itemUIRef in itemUIRefs)
            {
                if (!itemUIRef.isListItemVariable)
                {
                    continue;
                }

                sb.AppendFormat("\t\t\t{0} = uiRefRoot.objects[{1}] as {2};\r\n", itemUIRef.GetName(), itemIndex, itemUIRef.componentName);
                itemIndex++;
            }

            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
        }

        private string GetComment(UIRef uiRef)
        {
            string objPath = EditorUtil.GetHierarchy(uiRef.gameObject);
            string comment = objPath.Substring("UIRoot/UICanvas/Panel".Length + 1).Replace("\\", "/") + "," + uiRef.componentName;

            if (!string.IsNullOrEmpty(uiRef.desc))
            {
                comment = comment + "[" + uiRef.desc + "]";
            }

            return comment;
        }
    }
}
