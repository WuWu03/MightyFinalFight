using GameFrameWork.Utils;
using System;
using System.IO;
using System.Text;
using GameFrameWork.UI;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class CSharpUIScriptsExporter : BaseUIScriptsExporter
    {
        public override void Export(UIRef[] uiRefs, UIRefSetting setting)
        {
            ExportComponent(uiRefs, setting);
            ExportViewSettings(setting);
            ExportView(setting);
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

        private void ExportComponent(UIRef[] uiRefs, UIRefSetting setting)
        {
            StringBuilder sb = new();

            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: {0} 模块 {1} 界面组件\r\n", setting.moduleName, setting.viewName);
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: WuWu");
            sb.AppendLine(" * @Note: 工具生成，请勿修改");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using TMPro;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine();
            sb.AppendFormat("public class {0}Component : UIBaseComponent", setting.viewName);
            sb.AppendLine("\r\n{");
            
            foreach (var uiRef in uiRefs)
            {
                if (!uiRef.isListItem && !uiRef.IsListItemVariable)
                {
                    sb.Append("\t//").Append(GetComment(uiRef));
                    sb.AppendLine();
                    sb.AppendFormat("\tpublic {0} {1}", uiRef.componentName, uiRef.GetName());
                    sb.Append(" { get; private set; }\r\n");
                }
            }

            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnInitComponent(UIRefRoot root)");
            sb.AppendLine("\t{");

            for (int i = 0; i < uiRefs.Length; i++)
            {
                UIRef uiRef = uiRefs[i];

                if(uiRef.IsList)
                {
                    if (uiRef.IsScrollList())
                    {
                        int itemIndex = -1;
                        UIRef itemUIRef = null;

                        for (int j = 0; j < uiRefs.Length; j++)
                        {
                            if (uiRefs[j].isListItem)
                            {
                                Transform current = uiRefs[j].transform.parent;

                                while (current != null)
                                {
                                    if (current == uiRef.transform)
                                    {
                                        itemUIRef = uiRefs[j];
                                        itemIndex = j;
                                        break;
                                    }
                                    current = current.parent;
                                }
                            }

                            if (itemIndex > 0)
                            {
                                break;
                            }
                        }
                        
                        sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
                        sb.AppendFormat("\t\t{0} {1} = root.objects[{2}] as {3};\r\n", itemUIRef.componentName, uiRef.GetName() + "Item", itemIndex, itemUIRef.componentName);
                        sb.AppendFormat("\t\t{0}.Init<{1}>({2});\r\n", uiRef.GetName(), uiRef.GetName(true) + "Item", uiRef.GetName() + "Item");
                    }
                    else if (uiRef.IsStaticList())
                    {
                        int itemIndex = -1;
                        UIRef itemUIRef = null;

                        for (int j = 0; j < uiRefs.Length; j++)
                        {
                            if (uiRefs[j].isListItem && uiRefs[j].transform.parent == uiRef.transform)
                            {
                                itemUIRef = uiRefs[j];
                                itemIndex = j;
                                break;
                            }
                        }

                        sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
                        sb.AppendFormat("\t\t{0} {1} = root.objects[{2}] as {3};\r\n", itemUIRef.componentName, uiRef.GetName() + "Item", itemIndex, itemUIRef.componentName);
                        sb.AppendFormat("\t\t{0}.Init<{1}>({2}.gameObject , {3});\r\n", uiRef.GetName(), uiRef.GetName(true) + "Item", uiRef.GetName(), uiRef.GetName() + "Item");
                    }
                }
                else if (!uiRef.isListItem && !uiRef.IsListItemVariable)
                {
                    sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
                }
            }

            sb.AppendLine("\t}");

            foreach (var uiRef in uiRefs)
            {
                if (uiRef.IsList)
                {
                    ExportLayout(uiRef, sb);
                }
            }

            sb.Append("}");
            FileUtil.CreateTextFile(setting.componentPath, sb.ToString());
        }

        private void ExportViewSettings(UIRefSetting setting)
        {
            StringBuilder sb = new();

            string layerName = Enum.GetName(typeof(UIRefSetting.UILayer), setting.uiLayer);
            string destroyModeName = Enum.GetName(typeof(UIRefSetting.UIDestroyMode), setting.uiDestroyMode);
            string canPopUp = string.Empty;
            float delayDestroyTime = setting.delayDestroyTime;

            if (setting.uiType == UIRefSetting.UIType.View)
            {
                canPopUp = setting.uiLayer switch
                {
                    UIRefSetting.UILayer.MainWindow => "true",
                    UIRefSetting.UILayer.Window1 => "true",
                    UIRefSetting.UILayer.Window2 => "true",
                    _ => "false"
                };
            }

            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: {0} 模块 {1} 界面组件\r\n", setting.moduleName, setting.viewName);
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: WuWu");
            sb.AppendLine(" * @Note: 工具生成，请勿修改");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine();
            sb.AppendFormat("public class {0}Settings : UIBaseSettings", setting.viewName);
            sb.AppendLine("\r\n{");

            sb.Append("\tpublic override string prefabName { get { " + $"return \"{setting.viewName}.prefab\"" + "; } }\r\n");
            sb.Append("\tpublic override float delayDestroyTime { get { " + $"return {delayDestroyTime}f" + "; } }\r\n");
            sb.Append("\tpublic override bool canPopUp { get { " + $"return {canPopUp}" + "; } }\r\n");
            sb.Append("\tpublic override UILayer layer { get { " + $"return UILayer.{layerName}" + "; } }\r\n");
            sb.Append("\tpublic override UIDestroyMode destroyMode { get { " + $"return UIDestroyMode.{destroyModeName}" + "; } }\r\n");
            sb.Append("}");
            FileUtil.CreateTextFile(setting.settingsPath, sb.ToString());
        }

        private void ExportView(UIRefSetting setting)
        {
            if (File.Exists(setting.viewPath))
            {
                return;
            }

            StringBuilder sb = new();
            
            sb.AppendLine("/*");
            sb.AppendFormat(" * @Desc: {0} 模块 {1} 界面视图\r\n", setting.moduleName, setting.viewName);
            sb.AppendFormat(" * @Date: {0}-{1}-{2} {3}:{4}:{5}\r\n", year, month, day, hour, minute, second);
            sb.AppendLine(" * @Author: WuWu");
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendFormat("public class {0} : UIBaseView<{1}Component, {2}Settings>", setting.viewName, setting.viewName, setting.viewName);
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
            FileUtil.CreateTextFile(setting.viewPath, sb.ToString());
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

            if(tempUIRef == null)
            {
                return;
            }

            string listItemClassName = string.Empty;

            if (uiRef.IsScrollList())
            {
                listItemClassName = "ScrollListItem";
            }
            else if (uiRef.IsStaticList())
            {
                listItemClassName = "StaticListItem";
            }

            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0} : {1}\r\n", uiRef.GetName(true) + "Item", listItemClassName);
            sb.AppendLine("\t{");

            foreach (var variableUIRef in itemUIRefs)
            {
                if (!variableUIRef.IsListItemVariable)
                {
                    continue;
                }
                
                sb.AppendFormat("\t\t//{0}\r\n",GetComment(variableUIRef));
                sb.AppendFormat("\t\tpublic {0} {1} ", variableUIRef.componentName, variableUIRef.GetName());
                sb.Append("{get; private set;}\r\n");
            }

            sb.AppendLine("\t\tprotected override void OnCreate(GameObject go)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tUIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();");

            int itemIndex = 0;
            
            foreach (var itemUIRef in itemUIRefs)
            {
                if (!itemUIRef.IsListItemVariable)
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
