using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityObject = UnityEngine.Object;
using GameFrameWork.Utility;

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

            m_CSharpExporter = new CSharpExporter();
            m_LuaExporter = new LuaExporter();
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

                if (GUI.Button(new Rect(0f, (float)(Screen.height - 70), 70f, 30f), "生成预制体"))
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

            if (gameObject.name[0] >= '0' && gameObject.name[0] <= '9')
            {
                char[] objName = gameObject.name.ToCharArray();
                int index = gameObject.name[0];
                index += 49;
                objName[0] = (char)index;
                gameObject.name = new string(objName);
            }

            UIRef component = gameObject.GetComponent<UIRef>();
            if (component == null) return;

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            GUIStyle labelStyle2 = new GUIStyle(EditorStyles.label);

            labelStyle.normal.textColor = Color.green;
            labelStyle2.normal.textColor = Color.yellow;

            float x = selectionRect.x + selectionRect.width - 15f;
            float y = selectionRect.y + 2f;
            float width = 15f;
            float height = selectionRect.height;

            GUI.Label(new Rect(x, y, width, height), "*", component.IsCopyRefStr ? labelStyle2 : labelStyle);
        }

        private static bool CopyRefStr()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogError("没有选中节点对象");
                return false;
            }

            string currObjPath = EditorUtility.GetHierarchy(Selection.activeGameObject);
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
                string path = EditorUtility.GetHierarchy(component.gameObject);
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

            UIRef uiRef = obj.AddComponent<UIRef>();
            uiRef.ComponentName = typeof(GameObject).Name;
            uiRef.UseDefaultName = true;
            uiRef.SetName(obj.name);
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
                        EditorUtility.GetHierarchy(component.gameObject),
                        "; ",
                        EditorUtility.GetHierarchy(component.gameObject)
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
                m_CSharpExporter.Export(components, setting);
            }
            else
            {
                m_LuaExporter.Export(components, setting);
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
            FileUitl.VerifyDirectory(Path.GetDirectoryName(path));

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

        private static IExporter m_CSharpExporter = null;
        private static IExporter m_LuaExporter = null;
    }
}