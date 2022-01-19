using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class CSharpExporter : BaseExporter
    {
        public override void Export(UIRef[] uiRefs, UIRefSetting setting)
        {
            ExportComponent(uiRefs, setting);
            ExportPanel(uiRefs, setting);
        }

        public override string CopyRef(UIRef[] uiRefs)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < uiRefs.Length; i++)
            {
                if (!uiRefs[i].IsCopyRefStr) continue;
                sb.AppendFormat("public {0} {1} ", uiRefs[i].ComponentName, uiRefs[i].GetName());
                sb.Append("{ get; private set; };\n");
                sb.AppendFormat("{0} = root.Objects[{1}] as {2}\n", uiRefs[i].GetName(), i, uiRefs[i].ComponentName);
            }
            return sb.ToString();
        }

        private void ExportComponent(UIRef[] uiRefs, UIRefSetting setting)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}**************************************/\n", Year, Month, Day, Hour, Minute);
            sb.AppendLine("/**Create By GQY****************************************/");
            sb.AppendLine("/**工具生成，请勿修改************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendFormat("public class {0}Component : BasePanelComponent\n", setting.PanelName);
            sb.AppendLine("{");

            List<UIRef> layoutRefList = new List<UIRef>();
            List<UIRef> normalRefList = new List<UIRef>();

            for (int i = 0; i < uiRefs.Length; i++)
            {
                if (uiRefs[i].IsLayoutContent() && uiRefs[i].IsLayout)
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
                sb.AppendFormat("\tpublic {0} {1}", uiRef.ComponentName, uiRef.GetName());
                sb.Append(" { get; private set; }\n");
            }

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                string itemName = layoutRefList[i].GetName() + "Item";
                string itemVarableName = layoutRefList[i].GetName() + "GroupView";
                string layoutName = layoutRefList[i].IsLoopScroll ? "LayoutGroupLoopView" : "LayoutGroupView";
                sb.AppendFormat("\tpublic {0}<{1}> {2}", layoutName, itemName, itemVarableName);
                sb.Append(" { get; private set; }\n");
            }

            sb.AppendLine();
            sb.AppendFormat("\tpublic {0}Component(UIRefRoot root) : base(root)", setting.PanelName);
            sb.Append(" { }\n\n");
            sb.AppendLine("\tprotected override void InitComponent(UIRefRoot root)");
            sb.AppendLine("\t{");

            for (int i = 0; i < normalRefList.Count; i++)
            {
                UIRef uiRef = normalRefList[i];
                sb.AppendFormat("\t\t{0} = root.Objects[{1}] as {2};\n", uiRef.GetName(), i, uiRef.ComponentName);
            }

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                string itemName = layoutRefList[i].GetName() + "Item";
                string itemVarableName = layoutRefList[i].GetName() + "GroupView";
                string layoutName = layoutRefList[i].IsLoopScroll ? "LayoutGroupLoopView" : "LayoutGroupView";
                sb.AppendFormat("\t\t{0} = new {1}<{2}>();\n", itemVarableName, layoutName, itemName);
            }

            sb.AppendLine("\t}");

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                GenCSharpLayout(layoutRefList[i], sb);
            }

            sb.Append("}");
            FileUitl.VerifyDirectory(setting.ScriptFolder);
            FileUitl.CreateTextFile(setting.PanelComponentPath, sb.ToString());
        }

        private void ExportPanel(UIRef[] uiRefs, UIRefSetting setting)
        {
            if (FileUitl.FileExists(setting.PanelPath)) return;
            StringBuilder sb = new StringBuilder();

            string layerName = Enum.GetName(typeof(UIRefSetting.Layer), setting.PanelLayer);
            string closeModeName = Enum.GetName(typeof(UIRefSetting.CloseMode), setting.PanelCloseMode);
            string typeName = Enum.GetName(typeof(UIRefSetting.Type), setting.PanelType);
            float unLoadTime = setting.UnLoadTime;

            sb.Clear();
            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}****************************************/\n", Year, Month, Day, Hour, Minute);
            sb.AppendLine("/**Create By GQY****************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using DG.Tweening;");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendLine();
            sb.AppendFormat("public class {0} : BasePanel", setting.PanelName);
            sb.AppendLine("\n{");
            sb.Append("\tpublic override string PanelName { get { " + string.Format("return \"{0}\"", setting.PanelName) + "; } }\n");
            sb.Append("\tpublic override float PanelUnLoadTime { get { " + string.Format("return {0}f", unLoadTime) + "; } }\n");
            sb.Append("\tpublic override UIMgr.Type PanelType { get { " + string.Format("return UIMgr.Type.{0}", typeName) + "; } }\n");
            sb.Append("\tpublic override UIMgr.Layer PanelLayer { get { " + string.Format("return UIMgr.Layer.{0}", layerName) + "; } }\n");
            sb.Append("\tpublic override UIMgr.CloseMode PanelCloseMode { get { " + string.Format("return UIMgr.CloseMode.{0}", closeModeName) + "; } }\n");
            sb.AppendLine();
            sb.AppendLine("\tprotected override void OnInit(object[] param)");
            sb.AppendLine("\t{");
            sb.AppendFormat("\t\tm_Component = new {0}Component(UIRefRoot);\n", setting.PanelName);
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
            sb.AppendFormat("\tprivate {0}Component m_Component = null;\n", setting.PanelName);
            sb.Append("}");
            FileUitl.VerifyDirectory(setting.ScriptFolder);
            FileUitl.CreateTextFile(setting.PanelPath, sb.ToString());
        }

        private static void GenCSharpLayout(UIRef uiRef, StringBuilder sb)
        {
            UIRef[] itemRefs = uiRef.GetComponentsInChildren<UIRef>(true);
            UIRef itemRef = null;
            string itemName = string.Empty;

            for (int i = 0; i < itemRefs.Length; i++)
            {
                if (itemRefs[i].IsLayoutItem)
                {
                    itemRef = itemRefs[i];
                    itemName = itemRefs[i].gameObject.name;
                    break;
                }
            }

            if(itemRef == null)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0} : LayoutGroupViewItem\n", uiRef.GetName() + "Item");
            sb.AppendLine("\t{");

            for (int i = 0; i < itemRefs.Length; i++)
            {
                if (!itemRefs[i].IsLayoutItemVariable) continue;
                sb.AppendFormat("\t\tpublic {0} {1} = null;\n", itemRefs[i].ComponentName, itemRefs[i].GetName());
            }

            sb.AppendLine("\t\tprotected override void OnCreate(GameObject go)");
            sb.AppendLine("\t\t{");

            for (int i = 0; i < itemRefs.Length; i++)
            {
                if (!itemRefs[i].IsLayoutItemVariable) continue;
                string path = EditorUtility.GetHierarchy(itemRefs[i].gameObject);
                path = path.Substring(path.LastIndexOf(itemName) + itemName.Length + 1).Replace(@"\", "/");
                if (itemRefs[i].ComponentName.Equals("GameObject"))
                {
                    sb.AppendFormat("\t\t\t{0} = transform.Find(\"{1}\").gameObject;\n", itemRefs[i].GetName(), path);
                }
                else
                {
                    sb.AppendFormat("\t\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();\n", itemRefs[i].GetName(), path, itemRefs[i].ComponentName);
                }
            }

            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
        }

        private string GetComment(UIRef uiRef)
        {
            string objPath = EditorUtility.GetHierarchy(uiRef.gameObject);
            string comment = objPath.Substring("UIRoot/UICanvas/Panel".Length + 1).Replace("\\", "/") + "," + uiRef.ComponentName;

            if (!string.IsNullOrEmpty(uiRef.Desc))
            {
                comment = comment + "[" + uiRef.Desc + "]";
            }

            return comment;
        }
    }
}
