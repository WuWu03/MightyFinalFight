using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;

namespace GameFrameWork.Editor
{
    public class CSharpExporter : BaseExporter
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
                if (!uiRefs[i].isCopyRefStr) continue;
                sb.AppendFormat("public {0} {1} ", uiRefs[i].componentName, uiRefs[i].GetName());
                sb.Append("{ get; private set; };\r\n");
                sb.AppendFormat("{0} = root.Objects[{1}] as {2}\r\n", uiRefs[i].GetName(), i, uiRefs[i].componentName);
            }
            return sb.ToString();
        }

        private void ExportComponent(UIRef[] uiRefs, UIRefSetting setting)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}*************************************/\r\n", year, month, day, hour, minute);
            sb.AppendLine("/**Create By WuWu***************************************/");
            sb.AppendLine("/**工具生成，请勿修改************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
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
                if (uiRefs[i].IsLayoutContent() && uiRefs[i].isLayout)
                {
                    layoutRefList.Add(uiRefs[i]);
                }

                normalRefList.Add(uiRefs[i]);
            }

            for (int i = 0; i < normalRefList.Count; i++)
            {
                UIRef uiRef = normalRefList[i];
                sb.Append("\t//").Append(GetComment(uiRef));
                sb.AppendLine();
                sb.AppendFormat("\tpublic {0} {1}", uiRef.componentName, uiRef.GetName());
                sb.Append(" { get; private set; }\r\n");
            }

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                if (IsScrollLayout(layoutRefList[i]))
                {
                    continue;
                }

                string layoutName = "LayoutGroupView";
                string itemName = layoutRefList[i].GetName(true) + "Item";
                string itemVarableName = layoutRefList[i].GetName() + "GroupView";
                sb.AppendFormat("\tpublic {0}<{1}> {2}", layoutName, itemName, itemVarableName);
                sb.Append(" { get; private set; }\r\n");
            }

            sb.AppendLine();
            sb.AppendFormat("\tpublic {0}Component(UIRefRoot root) : base(root)", setting.panelName);
            sb.Append(" { }\r\n\r\n");
            sb.AppendLine("\tprotected override void InitComponent(UIRefRoot root)");
            sb.AppendLine("\t{");

            for (int i = 0; i < normalRefList.Count; i++)
            {
                UIRef uiRef = normalRefList[i];
                sb.AppendFormat("\t\t{0} = root.objects[{1}] as {2};\r\n", uiRef.GetName(), i, uiRef.componentName);
            }

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                if (IsScrollLayout(layoutRefList[i]))
                {
                    continue;
                }

                string itemVarableName = layoutRefList[i].GetName() + "GroupView";
                string layoutName = "LayoutGroupView";
                string itemName = layoutRefList[i].GetName(true) + "Item";
                sb.AppendFormat("\t\t{0} = new {1}<{2}>();\r\n", itemVarableName, layoutName, itemName);
            }

            sb.AppendLine("\t}");

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                GenCSharpLayout(layoutRefList[i], sb);
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
            sb.Append("\tpublic override UIMgr.Type panelType { get { " + string.Format("return UIMgr.Type.{0}", typeName) + "; } }\r\n");
            sb.Append("\tpublic override UIMgr.Layer panelLayer { get { " + string.Format("return UIMgr.Layer.{0}", layerName) + "; } }\r\n");
            sb.Append("\tpublic override UIMgr.CloseMode panelCloseMode { get { " + string.Format("return UIMgr.CloseMode.{0}", closeModeName) + "; } }\r\n");
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
            sb.AppendFormat("public class {0} : BasePanel", setting.panelName);
            sb.AppendLine("\r\n{");

            sb.AppendLine("\tprotected override void OnInit(BasePanelComponent panelComponent, object[] param)");
            sb.AppendLine("\t{");
            sb.AppendFormat("\t\tm_Component = panelComponent as {0}Component;\r\n", setting.panelName);
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
            sb.AppendLine();
            sb.AppendFormat("\tprivate {0}Component m_Component = null;\r\n", setting.panelName);
            sb.Append("}");
            FileUtil.VerifyDirectory(setting.scriptFolder);
            FileUtil.CreateTextFile(setting.panelPath, sb.ToString());
        }

        private void GenCSharpLayout(UIRef uiRef, StringBuilder sb)
        {
            UIRef[] childrenItemRefs = uiRef.GetComponentsInChildren<UIRef>(true);
            UIRef itemRef = null;
            string itemName = string.Empty;

            for (int i = 0; i < childrenItemRefs.Length; i++)
            {
                if (childrenItemRefs[i].isLayoutItem)
                {
                    itemRef = childrenItemRefs[i];
                    itemName = childrenItemRefs[i].gameObject.name;
                    break;
                }
            }

            if(itemRef == null)
            {
                return;
            }

            string layoutViewItemClassName = IsScrollLayout(uiRef) ? "ScrollLayoutGroupViewItem" : "LayoutGroupViewItem";
            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0} : {1}\r\n", uiRef.GetName(true) + "Item", layoutViewItemClassName);
            sb.AppendLine("\t{");

            for (int i = 0; i < childrenItemRefs.Length; i++)
            {
                if (!childrenItemRefs[i].isLayoutItemVariable) continue;
                sb.AppendFormat("\t\tpublic {0} {1} = null;\r\n", childrenItemRefs[i].componentName, childrenItemRefs[i].GetName());
            }

            sb.AppendLine("\t\tprotected override void OnCreate(GameObject go)");
            sb.AppendLine("\t\t{");

            for (int i = 0; i < childrenItemRefs.Length; i++)
            {
                if (!childrenItemRefs[i].isLayoutItemVariable) continue;
                string path = EditorUtil.GetHierarchy(childrenItemRefs[i].gameObject);
                path = path.Substring(path.LastIndexOf(itemName) + itemName.Length + 1).Replace(@"\", "/");

                if (childrenItemRefs[i].componentName.Equals("GameObject"))
                {
                    sb.AppendFormat("\t\t\t{0} = transform.Find(\"{1}\").gameObject;\r\n", childrenItemRefs[i].GetName(), path);
                }
                else
                {
                    sb.AppendFormat("\t\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();\r\n", childrenItemRefs[i].GetName(), path, childrenItemRefs[i].componentName);
                }
            }

            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
        }

        private bool IsScrollLayout(UIRef uiRef)
        {
            ScrollRect parentScroll = uiRef.GetComponentInParent<ScrollRect>();

            if (parentScroll != null)
            {
                UIRef parentUIRef = parentScroll.GetComponent<UIRef>();

                if (parentUIRef != null && parentUIRef.isScrollLayout)
                {
                    return true;
                }
            }

            return false;
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
