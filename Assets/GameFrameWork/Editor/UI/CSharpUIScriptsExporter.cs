using GameFrameWork.UI;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.Editor
{
    public class CSharpUIScriptsExporter : BaseUIScriptsExporter
    {
        public override void Export(UIRef[] uiRefs, UIRefSetting setting)
        {
            ExportComponent(uiRefs, setting);
            ExportPanelSettings(setting);
            ExportPanel(setting);
        }

        public override string CopyRef(UIRef[] uiRefs)
        {
            StringBuilder sb = new StringBuilder();
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

            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}*************************************/\r\n", year, month, day, hour, minute);
            sb.AppendLine("/**Create By WuWu***************************************/");
            sb.AppendLine("/**工具生成，请勿修改************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using TMPro;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine();

            sb.AppendFormat("public class {0}Component : BasePanelComponent", setting.panelName);
            sb.AppendLine("\r\n{");

            List<UIRef> layoutRefList = new List<UIRef>();
            List<UIRef> normalRefList = new List<UIRef>();

            for (int i = 0; i < uiRefs.Length; i++)
            {
                if (uiRefs[i].isLayout)
                {
                    layoutRefList.Add(uiRefs[i]);
                }
                else if (!uiRefs[i].isLayoutItem && !uiRefs[i].isLayoutItemVariable)
                {
                    normalRefList.Add(uiRefs[i]);
                }
            }

            for (int i = 0; i < uiRefs.Length; i++)
            {
                UIRef uiRef = uiRefs[i];

                if (uiRef.isLayout)
                {
                    if (uiRef.IsScollLayoutGroupView())
                    {
                        string layoutGroupName = uiRef.GetName() + "GroupView";
                        sb.AppendFormat("\tpublic ScrollLayoutGroupView {0}", layoutGroupName);
                        sb.Append(" { get; private set; }\r\n");
                    }
                    else if(uiRef.IsLayoutGroupView())
                    {
                        string layoutItemName = uiRef.GetName(true) + "Item";
                        string layoutGroupName = uiRef.GetName() + "GroupView";
                        sb.AppendFormat("\tpublic LayoutGroupView<{0}> {1}", layoutItemName, layoutGroupName);
                        sb.Append(" { get; private set; }\r\n");
                    }
                }
                else if (!uiRef.isLayoutItem && !uiRef.isLayoutItemVariable)
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

                if(uiRef.isLayout)
                {
                    if (uiRef.IsScollLayoutGroupView())
                    {
                        string layoutGroupName = uiRef.GetName() + "GroupView";
                        string layoutItemNameUpper = uiRef.GetName(true) + "Item";
                        string layoutItemNameLower = uiRef.GetName(false) + "Item";
                        int itemIndex = -1;
                        UIRef itemUIRef = null;

                        for (int j = 0; j < uiRefs.Length; j++)
                        {
                            if (uiRefs[j].isLayoutItem)
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

                        sb.AppendFormat("\t\t{0} {1} = root.objects[{2}] as {3};\r\n", itemUIRef.componentName, itemUIRef.GetName(), itemIndex, itemUIRef.componentName);
                        sb.AppendFormat("\t\t{0} = root.objects[{1}] as ScrollLayoutGroupView;\r\n", layoutGroupName, i, layoutItemNameUpper);
                        sb.AppendFormat("\t\t{0}.Init<{1}>({2});\r\n", layoutGroupName, layoutItemNameUpper, itemUIRef.GetName());
                    }
                    else if (uiRef.IsLayoutGroupView())
                    {
                        string layoutGroupName = uiRef.GetName() + "GroupView";
                        string layoutItemNameUpper = uiRef.GetName(true) + "Item";
                        string layoutItemNameLower = uiRef.GetName(false) + "Item";
                        int itemIndex = -1;
                        UIRef itemUIRef = null;

                        for (int j = 0; j < uiRefs.Length; j++)
                        {
                            if (uiRefs[j].isLayoutItem && uiRefs[j].transform.parent == uiRef.transform)
                            {
                                itemUIRef = uiRefs[j];
                                itemIndex = j;
                                break;
                            }
                        }

                        sb.AppendFormat("\t\t{0} {1} = root.objects[{2}] as {3};\r\n", uiRef.componentName, uiRef.GetName(), i, uiRef.componentName);
                        sb.AppendFormat("\t\t{0} {1} = root.objects[{2}] as {3};\r\n", itemUIRef.componentName, itemUIRef.GetName(), itemIndex, itemUIRef.componentName);
                        sb.AppendFormat("\t\t{0} = new LayoutGroupView<{1}>({2},{3});\r\n", layoutGroupName, layoutItemNameUpper, uiRef.GetName(), itemUIRef.GetName());
                    }
                }
                else if (!uiRef.isLayoutItem && !uiRef.isLayoutItemVariable)
                {
                    sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
                }
            }

            sb.AppendLine("\t}");

            for (int i = 0; i < uiRefs.Length; i++)
            {
                UIRef uiRef = uiRefs[i];
                if (uiRef.isLayout)
                {
                    ExportLayout(uiRef, sb);
                }
            }

            sb.Append("}");
            FileUtil.VerifyDirectory(setting.scriptFolder);
            FileUtil.CreateTextFile(setting.panelComponentPath, sb.ToString());
        }

        private void ExportPanelSettings(UIRefSetting setting)
        {
            StringBuilder sb = new StringBuilder();

            string layerName = Enum.GetName(typeof(UIRefSetting.Layer), setting.panelLayer);
            string closeModeName = Enum.GetName(typeof(UIRefSetting.CloseMode), setting.panelCloseMode);
            string typeName = Enum.GetName(typeof(UIRefSetting.Type), setting.panelType);
            float unLoadTime = setting.unLoadTime;

            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}*************************************/\r\n", year, month, day, hour, minute);
            sb.AppendLine("/**Create By WuWu***************************************/");
            sb.AppendLine("/**工具生成，请勿修改************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine();

            sb.AppendFormat("public class {0}Settings : BasePanelSettings", setting.panelName);
            sb.AppendLine("\r\n{");

            sb.Append("\tpublic override string panelName { get { " + string.Format("return \"{0}\"", setting.panelName) + "; } }\r\n");
            sb.Append("\tpublic override float panelUnLoadTime { get { " + string.Format("return {0}f", unLoadTime) + "; } }\r\n");
            sb.Append("\tpublic override PanelType panelType { get { " + string.Format("return PanelType.{0}", typeName) + "; } }\r\n");
            sb.Append("\tpublic override PanelLayer panelLayer { get { " + string.Format("return PanelLayer.{0}", layerName) + "; } }\r\n");
            sb.Append("\tpublic override PanelCloseMode panelCloseMode { get { " + string.Format("return PanelCloseMode.{0}", closeModeName) + "; } }\r\n");
            sb.Append("}");
            FileUtil.VerifyDirectory(setting.scriptFolder);
            FileUtil.CreateTextFile(setting.panelSettingsPath, sb.ToString());
        }

        private void ExportPanel(UIRefSetting setting)
        {
            if (File.Exists(setting.panelPath))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Clear();
            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}****************************************/\r\n", year, month, day, hour, minute);
            sb.AppendLine("/**Create By GQY****************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendFormat("public class {0} : BasePanel<{1}Component, {2}Settings>", setting.panelName, setting.panelName, setting.panelName);
            sb.AppendLine("\r\n{");
            sb.AppendLine("\tprotected override void OnInit(object arg)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnOpen()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnUpdate()");
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
            FileUtil.VerifyDirectory(setting.scriptFolder);
            FileUtil.CreateTextFile(setting.panelPath, sb.ToString());
        }

        private void ExportLayout(UIRef uiRef, StringBuilder sb)
        {
            UIRef[] childrenItemRefs = uiRef.GetComponentsInChildren<UIRef>(true);
            UIRef itemRef = null;

            for (int i = 0; i < childrenItemRefs.Length; i++)
            {
                if (childrenItemRefs[i].isLayoutItem)
                {
                    itemRef = childrenItemRefs[i];
                    break;
                }
            }

            if(itemRef == null)
            {
                return;
            }

            string layoutViewItemClassName = string.Empty;

            if (uiRef.IsScollLayoutGroupView())
            {
                layoutViewItemClassName = "ScrollLayoutGroupViewItem";
            }
            else if (uiRef.IsLayoutGroupView())
            {
                layoutViewItemClassName = "LayoutGroupViewItem";
            }

            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0} : {1}\r\n", uiRef.GetName(true) + "Item", layoutViewItemClassName);
            sb.AppendLine("\t{");

            for (int i = 0; i < childrenItemRefs.Length; i++)
            {
                if (!childrenItemRefs[i].isLayoutItemVariable)
                {
                    continue;
                }
                sb.AppendFormat("\t\tpublic {0} {1} = null;\r\n", childrenItemRefs[i].componentName, childrenItemRefs[i].GetName());
            }

            sb.AppendLine("\t\tprotected override void OnCreate(GameObject go)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tUIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();");

            int itemIndex = 0;
            for (int i = 0; i < childrenItemRefs.Length; i++)
            {
                if (!childrenItemRefs[i].isLayoutItemVariable)
                {
                    continue;
                }

                sb.AppendFormat("\t\t\t{0} = uiRefRoot.objects[{1}] as {2};\r\n", childrenItemRefs[i].GetName(), itemIndex, childrenItemRefs[i].componentName);
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
