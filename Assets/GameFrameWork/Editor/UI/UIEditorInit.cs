using GameFrameWork.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class UIEditorInit
    {
        private static UnityEngine.SceneManagement.Scene scene
        {
            get
            {
                return SceneManager.GetActiveScene();
            }
        }

        public static UIRefSetting uiRefSetting
        {
            get
            {
                if (s_UIRefSetting == null)
                {
                    GameObject gameObject = GameObject.Find("UI Scene Setting");
                    s_UIRefSetting = gameObject.GetComponent<UIRefSetting>();
                }

                return s_UIRefSetting;
            }
        }

        static UIEditorInit()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI = null;
            EditorApplication.hierarchyWindowItemOnGUI = HierarchyWindowItemOnGUI;

            s_CSharpExporter = new CSharpExporter();
            s_LuaExporter = new LuaExporter();
        }

        public static void NewUIScene()
        {
            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("创建新的UI场景", "NewPanel", "unity", "Save Scene as...", EditorPathUtil.GetUIScenesPath());

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
      
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            UnityObject root = UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<UnityObject>(EditorPathUtil.editorUIRootPath));
            root.name = "UIRoot";

            UIRefSetting settings = new GameObject("UI Scene Setting").AddComponent<UIRefSetting>();
            settings.panelName = Path.GetFileNameWithoutExtension(path);
            settings.prefabFolder = PathUtil.GetUIPrefabPath();
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
            EditorMgr.RefreshUIMenuItem();
        }

        private static void DuringSceneGUI(SceneView scnView)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && IsUIScene())
            {
                AppConfig appGo = GameObject.FindAnyObjectByType<AppConfig>();
                if (appGo != null)
                {
                    GameObject.DestroyImmediate(appGo.gameObject);
                }

                GUI.color = Color.green;
                Handles.BeginGUI();

                if (GUI.Button(new Rect(10, 10, 150f, 30f), "生成预制体"))
                {
                    string exportPath = ExportUIPrefab(true);
                    if (string.IsNullOrEmpty(exportPath)) return;

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(exportPath);
                }

                GUI.color = Color.red;
                if (GUI.Button(new Rect(10, 50, 150f, 30f), "生成预制体(不生成代码)"))
                {
                    string exportPath = ExportUIPrefab(false);
                    if (string.IsNullOrEmpty(exportPath)) return;

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(exportPath);
                }

                GUI.color = Color.white;

                if (GUI.Button(new Rect(30, 90, 110f, 30f), "复制引用到剪切板"))
                {
                    CopyRefStr();
                    UnityEngine.Event.current.Use();
                }

                if (GUI.Button(new Rect(30, 130, 110f, 30f), "添加引用"))
                {
                    AddUIRef();
                    UnityEngine.Event.current.Use();
                }

                Handles.EndGUI();
            }
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !IsUIScene())
            {
                return;
            }

            GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject == null || gameObject.transform.parent == null)
            {
                return;
            }

            if (gameObject.name[0] >= '0' && gameObject.name[0] <= '9')
            {
                char[] objName = gameObject.name.ToCharArray();
                int index = gameObject.name[0];
                index += 49;
                objName[0] = (char)index;
                gameObject.name = new string(objName);
            }

            UIRef component = gameObject.GetComponent<UIRef>();

            if (component == null)
            {
                return;
            }

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            GUIStyle labelStyle2 = new GUIStyle(EditorStyles.label);

            labelStyle.normal.textColor = Color.green;
            labelStyle2.normal.textColor = Color.yellow;

            float x = selectionRect.x + selectionRect.width - 15f;
            float y = selectionRect.y + 2f;
            float width = 15f;
            float height = selectionRect.height;

            GUI.Label(new Rect(x, y, width, height), "*", component.isCopyRefStr ? labelStyle2 : labelStyle);
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
            uiRef.componentName = typeof(GameObject).Name;
            uiRef.useDefaultName = true;
            uiRef.SetName(obj.name);
        }

        private static bool ExportUIRef()
        {
            if (uiRefSetting == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(uiRefSetting.panelName) || uiRefSetting.panelName.Contains("/"))
            {
                UnityEngine.Debug.LogError("界面名字未设置正确");
                Selection.activeGameObject = uiRefSetting.gameObject;
                return false;
            }

            GameObject gameObject = GameObject.Find("UIRoot/UICanvas/Panel");
            UIRefRoot rootComponent = gameObject.GetComponent<UIRefRoot>();
            UIRef[] components = gameObject.GetComponentsInChildren<UIRef>(true);

            List<UIRef> retList = new List<UIRef>();
            HashSet<string> repeat = new HashSet<string>();
            List<UnityObject> listComponent = new List<UnityObject>();

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (component.isCopyRefStr || component.isLayoutItemVariable) continue;

                string name = component.GetName();
                if (!repeat.Add(name))
                {
                    string errorStr = string.Concat(new string[]
                    {
                        "有重复的引用名称 => ",
                        component.refName,
                        "; 引用对象=>",
                        EditorUtil.GetHierarchy(component.gameObject),
                        "; ",
                        EditorUtil.GetHierarchy(component.gameObject)
                    });

                    UnityEngine.Debug.LogError(errorStr);
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }

                if (string.IsNullOrEmpty(component.componentName) || component.componentName == typeof(Transform).Name)
                {
                    listComponent.Add(component.transform);
                }
                else if (component.componentName == typeof(RectTransform).Name)
                {
                    listComponent.Add(component.GetComponent<RectTransform>());
                }
                else if (component.componentName == typeof(GameObject).Name)
                {
                    listComponent.Add(component.gameObject);
                }
                else
                {
                    listComponent.Add(component.GetComponent(component.componentName));
                }

                retList.Add(component);
            }

            rootComponent.objects = listComponent.ToArray();
            UnityEditor.EditorUtility.SetDirty(rootComponent);

            if (uiRefSetting.scriptType == UIRefSetting.ExoprtScriptType.CSharp)
            {
                s_CSharpExporter.Export(retList.ToArray(), uiRefSetting);
            }
            else
            {
                s_LuaExporter.Export(retList.ToArray(), uiRefSetting);
            }

            return true;
        }

        private static bool CopyRefStr()
        {
            if(!CanExprot())
            {
                return false;
            }

            if (uiRefSetting == null)
            {
                return false;
            }

            GameObject root = GameObject.Find("UIRoot");
            GameObject panel = root.transform.Find("UICanvas/Panel").gameObject;

            UIRef[] components = panel.GetComponentsInChildren<UIRef>(true);
            List<UIRef> listRef = new List<UIRef>();
            HashSet<string> hashSet = new HashSet<string>();

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (!component.isCopyRefStr) continue;

                string name = component.GetName();
                string path = EditorUtil.GetHierarchy(component.gameObject);

                if (!hashSet.Add(name))
                {
                    UnityEngine.Debug.LogError("有重复的引用名称 => " + component.refName + "; 引用对象=>" + path);
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }
            }

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].isLayoutItemVariable) continue;
                listRef.Add(components[i]);
            }

            string value = string.Empty;

            if (uiRefSetting.scriptType == UIRefSetting.ExoprtScriptType.CSharp)
            {
                value = s_CSharpExporter.CopyRef(listRef.ToArray());
            }
            else if (uiRefSetting.scriptType == UIRefSetting.ExoprtScriptType.Lua)
            {
                value = s_LuaExporter.CopyRef(listRef.ToArray());
            }

            if (string.IsNullOrEmpty(value))
            {
                UnityEngine.Debug.LogWarning("没有需要导出到剪切板的对象");
            }
            else
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("已复制到剪切板"));
                GUIUtility.systemCopyBuffer = value;
            }

            return true;
        }

        private static string ExportUIPrefab(bool generateCode)
        {
            if (!CanExprot()) return null;
            if (generateCode)
            {
                 if(!ExportUIRef()) return null;
            }

            string path = uiRefSetting.panelPrefabPath;

            if (File.Exists(path))
            {
                if (!UnityEditor.EditorUtility.DisplayDialog("存在资源", "已经存在资源 " + path + " 是否替换", "替换", "取消"))
                {
                    return null;
                }
            }

            GameObject root = GameObject.Find("UIRoot");
            GameObject panel = root.transform.Find("UICanvas/Panel").gameObject;
            Utilities.FileUtil.VerifyDirectory(Path.GetDirectoryName(path));

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

        private static bool IsUIScene()
        {
            return GameObject.FindObjectOfType<UIRefSetting>() != null;
        }

        private static bool CanExprot()
        {
            if (!IsUIScene())
            {
                UnityEngine.Debug.LogError("当前场景不是UI场景 Scene => " + scene.name);
                return false;
            }

            GameObject root = GameObject.Find("UIRoot");

            if (root == null || root.transform.Find("UICanvas/Panel") == null)
            {
                UnityEngine.Debug.LogError(root + "场景UI资源错误 Scene => " + scene.name + "===没有Panel对象");
                return false;
            }

            return true;
        }

        private static UIRefSetting s_UIRefSetting;
        private static IExporter s_CSharpExporter = null;
        private static IExporter s_LuaExporter = null;
    }
}