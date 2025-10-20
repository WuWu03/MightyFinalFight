using GameFrameWork.Utils;
using System.Collections.Generic;
using System.IO;
using GameFrameWork.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

        private static UIRefSetting s_UIRefSetting;
        private static IUIScriptsExporter s_CSharpExporter;
        
        static UIEditorInit()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI = null;
            EditorApplication.hierarchyWindowItemOnGUI = HierarchyWindowItemOnGUI;
            s_CSharpExporter = new CSharpUIScriptsExporter();
        }

        public static bool CanCreateUIScene(string uiName)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return false;
            }

            GameFrameWorkConfigWindowData config = EditorMgr.GetGameFrameWorkConfig();

            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("提示", "未设置UI目录，点击确定前往设置", "确定"))
                {
                    EditorMgr.GameFrameWorkStartUp();
                }

                return false;
            }

            string uiPath = PathUtil.FormatPath(config.uiScenesPath, uiName + ".unity");

            if (File.Exists(uiPath))
            {
                UnityEngine.SceneManagement.Scene activeScene = EditorSceneManager.GetActiveScene();

                if (activeScene.path == uiPath)
                {
                    EditorUtility.DisplayDialog("新建UI", "当前已位于 [" + uiName + "] 场景中", "确定");
                }
                else if (EditorUtility.DisplayDialog("新建UI", "UI [" + uiName + "] 已存在，是否跳转？", "确定", "取消"))
                {
                    EditorSceneManager.OpenScene(uiPath);
                }

                return false;
            }

            if (!EditorUtility.DisplayDialog("新建UI", "是否创建UI [" + uiName + "]？", "确定", "取消"))
            {
                return false;
            }

            return true;
        }

        public static void NewUIScene(string uiName)
        {
            GameFrameWorkConfigWindowData config = EditorMgr.GetGameFrameWorkConfig();
            string uiPath = PathUtil.FormatPath(config.uiScenesPath, uiName + ".unity");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            UnityObject root = UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<UnityObject>(EditorPathUtil.editorUIRootScenePath));
            root.name = "UIRoot";
            UIRefSetting settings = new GameObject("UI Scene Setting").AddComponent<UIRefSetting>();
            settings.viewName = Path.GetFileNameWithoutExtension(uiName);
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
            panel.AddComponent<Canvas>().vertexColorAlwaysGammaSpace = true;
            panel.AddComponent<GraphicRaycaster>();
            EditorSceneManager.SaveScene(scene, uiPath);
            AssetDatabase.Refresh();
            Selection.activeGameObject = settings.gameObject;
        }

        private static void DuringSceneGUI(SceneView scnView)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && IsUIScene())
            {
                GUI.color = Color.green;
                Handles.BeginGUI();

                if (GUI.Button(new Rect(10, 10, 150f, 30f), "生成预制体"))
                {
                    string exportPath = ExportUIPrefab(true);
                    if (string.IsNullOrEmpty(exportPath))
                    {
                        return;
                    }

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(exportPath);
                }

                GUI.color = Color.red;
                if (GUI.Button(new Rect(10, 50, 150f, 30f), "生成预制体(不生成代码)"))
                {
                    string exportPath = ExportUIPrefab(false);
                    if (string.IsNullOrEmpty(exportPath))
                    {
                        return;
                    }

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

            foreach (var gameObject in gameObjects)
            {
                AddUIRef(gameObject);
            }
        }

        private static void AddUIRef(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            UIRef uiRef = obj.AddComponent<UIRef>();
            uiRef.componentName = nameof(GameObject);
            uiRef.useDefaultName = true;
            uiRef.SetName(obj.name);
        }

        private static bool ExportUIRef()
        {
            if (uiRefSetting == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(uiRefSetting.viewName) || uiRefSetting.viewName.Contains("/"))
            {
                UnityEngine.Debug.LogError("界面名字未设置正确");
                Selection.activeGameObject = uiRefSetting.gameObject;
                return false;
            }

            GameObject gameObject = GameObject.Find("UIRoot/UICanvas/Panel");
            UIRefRoot[] uiRefRoots = gameObject.GetComponentsInChildren<UIRefRoot>(true);
            List<UIRef> retList = new();

            foreach (UIRefRoot uiRefRoot in uiRefRoots)
            {
                UIRef rootRef = uiRefRoot.GetComponent<UIRef>();
                GenUIRefRootObjs(uiRefRoot, rootRef != null && rootRef.isListItem, retList);
            }

            UnityEditor.EditorUtility.SetDirty(gameObject);
            s_CSharpExporter.Export(retList.ToArray(), uiRefSetting);
            return true;
        }

        private static bool GenUIRefRootObjs(UIRefRoot uiRefRoot, bool isLayoutItem, List<UIRef> uiRefs)
        {
            UIRef[] components = uiRefRoot.GetComponentsInChildren<UIRef>(true);
            HashSet<string> repeat = new();
            List<UnityObject> listComponent = new();
            int startIndex = isLayoutItem ? 1 : 0;

            for (int i = startIndex; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (component.isCopyRefStr || (!isLayoutItem && component.IsListItemVariable))
                {
                    continue;
                }

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

                uiRefs.Add(component);
            }

            uiRefRoot.objects = listComponent.ToArray();
            return true;
        }

        private static bool CopyRefStr()
        {
            if (!CanExport())
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

            foreach (var component in components)
            {
                if (!component.isCopyRefStr)
                {
                    continue;
                }

                string name = component.GetName();
                string path = EditorUtil.GetHierarchy(component.gameObject);

                if (!hashSet.Add(name))
                {
                    UnityEngine.Debug.LogError("有重复的引用名称 => " + component.refName + "; 引用对象=>" + path);
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }
            }

            foreach (var component in components)
            {
                if (component.IsListItemVariable) continue;
                listRef.Add(component);
            }

            string value = s_CSharpExporter.CopyRef(listRef.ToArray());
            
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
            if (!CanExport())
            {
                return null;
            }

            if (generateCode)
            {
                if (!ExportUIRef())
                {
                    return null;
                }
            }

            string path = PathUtil.FormatPath(EditorMgr.GetGameFrameWorkConfig().uiPrefabsPath, s_UIRefSetting.viewName, ".prefab");

            if (File.Exists(path))
            {
                if (!UnityEditor.EditorUtility.DisplayDialog("存在资源", "已经存在资源 " + path + " 是否替换", "替换", "取消"))
                {
                    return null;
                }
            }

            GameObject root = GameObject.Find("UIRoot");
            GameObject panel = root.transform.Find("UICanvas/Panel").gameObject;
            Utils.FileUtil.VerifyDirectory(Path.GetDirectoryName(path));

            bool isSuccess;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(panel, path, out isSuccess);

            if (!isSuccess)
            {
                return null;
            }

            UIRef[] components = prefab.GetComponentsInChildren<UIRef>(true);

            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component, true);
            }

            EditorSceneManager.SaveScene(scene);
            return path;
        }

        private static bool IsUIScene()
        {
            return UnityEngine.Object.FindAnyObjectByType<UIRefSetting>() != null;
        }

        private static bool CanExport()
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
    }
}