using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityObject = UnityEngine.Object;
using GameFrameWork.Utils;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public class UIUtility
    {
        private static Scene scene { get { return SceneManager.GetActiveScene(); } }

        private static UIRefSetting m_UIRefSetting;
        public static UIRefSetting UIRefSetting
        {
            get
            {
                if (m_UIRefSetting == null)
                {
                    GameObject gameObject = GameObject.Find("UI Scene Setting");
                    m_UIRefSetting = gameObject.GetComponent<UIRefSetting>();
                }
                return m_UIRefSetting;
            }
        }

        static UIUtility()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI = null;
            EditorApplication.hierarchyWindowItemOnGUI = HierarchyWindowItemOnGUI;
        }

        public static void NewUIScene()
        {
            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("创建新的UI场景", "NewPanel", "unity", "Save Scene as...");

            if (string.IsNullOrEmpty(path)) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            UnityObject root = UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<UnityObject>("Assets/FrameWork/UI/UIRoot.prefab"));
            root.name = "UIRoot";

            UIRefSetting settings = new GameObject("UI Scene Setting").AddComponent<UIRefSetting>();
            settings.PanelName = Path.GetFileNameWithoutExtension(path);
            settings.transform.SetAsLastSibling();

            GameObject rootObj = root as GameObject;
            rootObj.transform.SetAsLastSibling();
            GameObject panel = new GameObject("Panel");
            RectTransform rect = panel.AddComponent<RectTransform>();
            panel.gameObject.AddComponent<UIRefRoot>();
            rect.anchoredPosition = Vector3.zero;
            rect.sizeDelta = Vector2.zero;
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.SetParent(rootObj.transform.Find("UICanvas"), false);

            EditorSceneManager.SaveScene(scene, path);
            Selection.activeGameObject = settings.gameObject;
        }

        private static void DuringSceneGUI(SceneView scnView)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && UIUtility.IsUIScene())
            {
                GUI.color = Color.green;
                Handles.BeginGUI();

                if (GUI.Button(new Rect(0f, (float)(Screen.height - 30 - 40), 70f, 30f), "生成预制体"))
                {
                    string exportPath = ExportUIPrefab(true);
                    if (string.IsNullOrEmpty(exportPath)) return;

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(exportPath);
                }

                GUI.color = Color.white;
                if (GUI.Button(new Rect((float)(Screen.width - 200), (float)(Screen.height - 70), 110f, 30f), "复制引用到剪切板"))
                {
                    CopyRefStr();
                    UnityEngine.Event.current.Use();
                }
                if (GUI.Button(new Rect((float)(Screen.width - 70), (float)(Screen.height - 70), 70f, 30f), "添加引用"))
                {
                    AddUIRef();
                    UnityEngine.Event.current.Use();
                }
                Handles.EndGUI();
            }
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !UIUtility.IsUIScene()) return;

            GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null || gameObject.transform.parent == null) return;

            UIRef component = gameObject.GetComponent<UIRef>();
            if (component == null) return;

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            GUIStyle labelStyle2 = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.green;
            labelStyle2.normal.textColor = Color.yellow;

            GUI.Label(new Rect(selectionRect.x + selectionRect.width - 15f, selectionRect.y + 2f, 15f,
                selectionRect.height), "*", component.IsCopyRefStr ? labelStyle2 : labelStyle);
        }

        private static bool CopyRefStr()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogError("没有选中节点对象");
                return false;
            }

            string currObjPath = EditorMgr.GetHierarchy(Selection.activeGameObject);
            if (!currObjPath.StartsWith("UIRoot\\UICanvas\\Panel"))
            {
                Debug.LogError("应该选择Panel下面的对象作为根节点");
                return false;
            }

            UIRef[] components = Selection.activeGameObject.GetComponentsInChildren<UIRef>(true);
            HashSet<string> hashSet = new HashSet<string>();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (!component.IsCopyRefStr) continue;

                string name = component.GetName();
                string path = EditorMgr.GetHierarchy(component.gameObject);
                if (!hashSet.Add(name))
                {
                    Debug.LogError("有重复的引用名称 => " + component.Name + "; 引用对象=>" + path);
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }
                string value;
                if (path == currObjPath)
                {
                    if (string.IsNullOrEmpty(component.ComponentName) || component.ComponentName == "Transform")
                    {
                        value = string.Format("{0} = targetTrans;", name);
                    }
                    else if (component.ComponentName == "GameObject")
                    {
                        value = string.Format("{0} = targetTrans.gameObject;", name);
                    }
                    else
                    {
                        value = string.Format("{0} = targetTrans:GetComponent(\"{1}\");", name, component.ComponentName);
                    }
                }
                else
                {
                    path = path.Substring(currObjPath.Length + 1).Replace("\\", "/");
                    if (string.IsNullOrEmpty(component.ComponentName) || component.ComponentName == "Transform")
                    {
                        value = string.Format("{0} = targetTrans:Find(\"{1}\");", name, path);
                    }
                    else if (component.ComponentName == "GameObject")
                    {
                        value = string.Format("{0} = targetTrans:Find(\"{1}\").gameObject;", name, path);
                    }
                    else
                    {
                        value = string.Format("{0} = targetTrans:Find(\"{1}\"):GetComponent(\"{2}\");", name, path, component.ComponentName);
                    }
                }
                stringBuilder.Append("\t").AppendLine(value);
            }

            if (stringBuilder.Length == 0)
            {
                Debug.LogWarning("没有需要导出到剪切板的对象");
            }
            else
            {
                stringBuilder.Insert(0, "{\n\tmTargetTrans = targetTrans;\n");
                stringBuilder.Append("}");
                Debug.Log(stringBuilder.ToString());
                GUIUtility.systemCopyBuffer = stringBuilder.ToString();
            }

            return true;
        }

        private static void AddUIRef()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject obj = gameObjects[i];
                AddUIRef(obj);
            }
        }

        private static void AddUIRef(GameObject obj)
        {
            if (obj == null) return;

            UIRef[] components = obj.GetComponents<UIRef>();
            UIRef uiRef = obj.AddComponent<UIRef>();
            uiRef.ComponentName = typeof(GameObject).Name;
            if (components.Length == 0)
            {
                uiRef.UseObjName = true;
                uiRef.SetObjName(obj.name);
            }
            else
            {
                uiRef.UseObjName = false;
                uiRef.SetName(obj.name);
            }
        }

        private static bool IsUIScene()
        {
            return GameObject.FindObjectOfType<UIRefSetting>() != null;
        }

        private static bool ExportUIRef()
        {
            UIRefSetting setting = UIUtility.UIRefSetting;
            if (setting == null) return false;

            if (string.IsNullOrEmpty(setting.PanelName) || setting.PanelName.Contains("/"))
            {
                Debug.LogError("界面名字未设置正确");
                Selection.activeGameObject = setting.gameObject;
                return false;
            }

            GameObject gameObject = GameObject.Find("UIRoot/UICanvas/Panel");
            UIRefRoot rootComponent = gameObject.GetComponent<UIRefRoot>();
            UIRef[] components = gameObject.GetComponentsInChildren<UIRef>(true);
            HashSet<string> repeat = new HashSet<string>();

            List<UnityObject> listComponent = new List<UnityObject>();

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (component.IsCopyRefStr) continue;

                string name = component.GetName();
                if (!repeat.Add(name))
                {
                    Debug.LogError(string.Concat(new string[]
                    {
                    "有重复的引用名称 => ",
                    component.Name,
                    "; 引用对象=>",
                    EditorMgr.GetHierarchy(component.gameObject),"; ",
                    EditorMgr.GetHierarchy(component.gameObject)
                    }));
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }

                //if (component.IsLayoutItem) continue;
                if (string.IsNullOrEmpty(component.ComponentName) || component.ComponentName == typeof(Transform).Name)
                {
                    listComponent.Add(component.transform);
                }
                else if (component.ComponentName == typeof(RectTransform).Name)
                {
                    listComponent.Add(component.GetComponent<RectTransform>());
                }
                else if (component.ComponentName == typeof(GameObject).Name)
                {
                    listComponent.Add(component.gameObject);
                }
                else
                {
                    listComponent.Add(component.GetComponent(component.ComponentName));
                }
            }

            rootComponent.Objects = listComponent.ToArray();
            UnityEditor.EditorUtility.SetDirty(rootComponent);

            if (setting.ScriptType == UIRefSetting.ExoprtScriptType.CSharp)
            {
                ExportCSharp(components, setting);
            }
            else
            {
                ExportLua(components);
            }
            return true;
        }

        private static string ExportUIPrefab(bool showExist = true)
        {
            if (!IsUIScene())
            {
                Debug.LogError("当前场景不是UI场景 Scene => " + scene.name);
                return null;
            }

            GameObject root = GameObject.Find("UIRoot");
            if (root == null || root.transform.Find("UICanvas/Panel") == null)
            {
                Debug.LogError(root + "场景UI资源错误 Scene => " + scene.name + "===没有Panel对象");
                return null;
            }

            if (!ExportUIRef()) return null;

            string path = UIRefSetting.PanelPrefabPath;

            if (File.Exists(path))
            {
                if (!UnityEditor.EditorUtility.DisplayDialog("存在资源", "已经存在资源 " + path + " 是否替换", "替换", "取消"))
                {
                    return null;
                }
            }

            GameObject panel = root.transform.Find("UICanvas/Panel").gameObject;
            IOUtil.VerifyDirectory(Path.GetDirectoryName(path));

            bool isSuccess;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(panel, path, out isSuccess);

            if (!isSuccess) return null;

            UIRef[] components = prefab.GetComponentsInChildren<UIRef>(true);

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                UnityEngine.Object.DestroyImmediate(component, true);
            }

            EditorSceneManager.SaveScene(scene);
            return path;
        }

        private static void ExportCSharp(UIRef[] uiRefs, UIRefSetting setting)
        {
            StringBuilder sb = new StringBuilder();

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;

            string layerName = Enum.GetName(typeof(UIRefSetting.Layer), setting.PanelLayer);
            string closeModeName = Enum.GetName(typeof(UIRefSetting.CloseMode), setting.PanelCloseMode);
            string typeName = Enum.GetName(typeof(UIRefSetting.Type), setting.PanelType);
            float unLoadTime = setting.UnLoadTime;

            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}**************************************/\n", year, month, day, hour, minute);
            sb.AppendLine("/**Create By GQY****************************************/");
            sb.AppendLine("/**工具生成，请勿修改************************************/");
            sb.AppendLine("/*******************************************************/");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using GameFrameWork.UI;");
            sb.AppendFormat("public class {0}Component:BasePanelComponent\n", setting.PanelName);
            sb.AppendLine("{");

            List<UIRef> layoutRefList = new List<UIRef>();
            List<UIRef> normalRefList = new List<UIRef>();

            for (int i = 0; i < uiRefs.Length; i++)
            {
                if (uiRefs[i].IsLayoutContent())
                {
                    layoutRefList.Add(uiRefs[i]);
                    normalRefList.Add(uiRefs[i]);
                }
                else if (!uiRefs[i].IsLayoutItemVariable)
                {
                    normalRefList.Add(uiRefs[i]);
                }
            }

            for (int i = 0; i < normalRefList.Count; i++)
            {
                UIRef uiRef = normalRefList[i];
                sb.Append("\t//").Append(GetComment(uiRef));
                sb.AppendLine();
                sb.AppendFormat("\tpublic {0} {1}", uiRef.ComponentName, uiRef.GetName());
                sb.Append(" { get; private set;}\n");
            }

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                string itemName = layoutRefList[i].GetName() + "Item";
                string itemVarableName = layoutRefList[i].GetName() + "GroupView";
                string layoutName = layoutRefList[i].IsLoopLayout ? "LayoutGroupLoopView" : "LayoutGroupView";
                sb.AppendFormat("\tpublic {0}<{1}> {2}", layoutName, itemName, itemVarableName);
                sb.Append(" { get; private set;}\n");
            }
            sb.AppendLine();
            sb.AppendFormat("\tpublic {0}Component(UIRefRoot root) : base(root)", setting.PanelName);
            sb.Append(" { }\n");
            sb.AppendLine("\tprotected override void InitComponent(UIRefRoot root)");
            sb.AppendLine("\t{");

            for (int i = 0; i < normalRefList.Count; i++)
            {
                int objIndex = i;
                UIRef uiRef = normalRefList[i];
                for (int j = 0; j < uiRefs.Length; j++)
                {
                    if (uiRefs[j] == uiRef)
                    {
                        objIndex = j;
                        break;
                    }
                }
                sb.AppendFormat("\t\t{0} = root.Objects[{1}] as {2};\n", uiRef.GetName(), objIndex, uiRef.ComponentName);
            }

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                string itemName = layoutRefList[i].GetName() + "Item";
                string itemVarableName = layoutRefList[i].GetName() + "GroupView";
                string layoutName = layoutRefList[i].IsLoopLayout ? "LayoutGroupLoopView" : "LayoutGroupView";
                sb.AppendFormat("\t\t{0} = new {1}<{2}>();\n", itemVarableName, layoutName, itemName);
            }

            sb.AppendLine("\t}");

            for (int i = 0; i < layoutRefList.Count; i++)
            {
                GenCSharpLayout(layoutRefList[i], sb);
            }

            sb.Append("}");
            IOUtil.VerifyDirectory(setting.ScriptFolder);
            IOUtil.CreateTextFile(setting.PanelComponentPath, sb.ToString());

            if (File.Exists(setting.PanelPath)) return;
            sb.Clear();
            sb.AppendLine("/*******************************************************/");
            sb.AppendFormat("/**{0}-{1}-{2} {3}:{4}****************************************/\n", year, month, day, hour, minute);
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
            IOUtil.VerifyDirectory(setting.ScriptFolder);
            IOUtil.CreateTextFile(setting.PanelPath, sb.ToString());
        }

        private static void GenCSharpLayout(UIRef uiRef, StringBuilder sb)
        {
            UIRef[] itemRefs = uiRef.GetComponentsInChildren<UIRef>(true);


            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0} : LayoutGroupViewItem\n", uiRef.GetName() + "Item");
            sb.AppendLine("\t{");

            string itemName = string.Empty;

            for (int i = 0; i < itemRefs.Length; i++)
            {
                if (itemRefs[i].IsLayoutItem)
                {
                    itemName = itemRefs[i].name;
                    break;
                }
            }

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
                string path = EditorMgr.GetHierarchy(itemRefs[i].gameObject);
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

        private static bool ExportLua(UIRef[] uiRefs)
        {
            return false;
        }

        private static string GetComment(UIRef uiRef)
        {
            string objPath = EditorMgr.GetHierarchy(uiRef.gameObject);
            string comment = objPath.Substring("UIRoot/UICanvas/Panel".Length + 1).Replace("\\", "/") + "," + uiRef.ComponentName;

            if (!string.IsNullOrEmpty(uiRef.Desc))
            {
                comment = comment + "[" + uiRef.Desc + "]";
            }

            return comment;
        }
    }
}