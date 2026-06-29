using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WuWuFramework.UI;
using UnityObject = UnityEngine.Object;
using WuWuFileUtil = WuWuFramework.Utils.FileUtil;
using WuWuPathUtil = WuWuFramework.Utils.PathUtil;

namespace WuWuFramework.Editor
{
    [InitializeOnLoad]
    public static class UIEditorInit
    {
        private static UnityEngine.SceneManagement.Scene scene
        {
            get { return SceneManager.GetActiveScene(); }
        }

        private static UIRefSetting s_UIRefSetting;
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

        private static GameObject s_Panel;
        public static GameObject panel
        {
            get
            {
                if (s_Panel == null)
                {
                    s_Panel = GameObject.Find("UIRoot/UICanvas/Panel");
                }

                return s_Panel;
            }
        }
        
        private static readonly IUIScriptsExporter s_CSharpExporter;

        static UIEditorInit()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            s_CSharpExporter = new CSharpUIScriptsExporter();
        }

        public static bool CanCreateUIScene(string uiName)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return false;
            }

            WuWuFrameworkConfigWindowData config = EditorMgr.GetWuWuFrameworkConfig();

            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                if (EditorUtility.DisplayDialog("提示", "未设置UI目录，点击确定前往设置", "确定"))
                {
                    EditorMgr.WuWuFrameworkStartUp();
                }

                return false;
            }

            string uiPath = WuWuPathUtil.FormatPath(config.uiScenesPath, uiName + ".unity");

            if (File.Exists(uiPath))
            {
                UnityEngine.SceneManagement.Scene activeScene = SceneManager.GetActiveScene();

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
            WuWuFrameworkConfigWindowData config = EditorMgr.GetWuWuFrameworkConfig();
            string uiPath = WuWuPathUtil.FormatPath(config.uiScenesPath, uiName + ".unity");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            UnityObject root = UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<UnityObject>(EditorPathUtil.EditorUIRootScenePath));
            root.name = "UIRoot";
            UIRefSetting settings = new GameObject("UI Scene Setting").AddComponent<UIRefSetting>();
            settings.viewName = Path.GetFileNameWithoutExtension(uiName);
            settings.transform.SetAsLastSibling();

            if (root is not GameObject rootObj)
            {
                return;
            }

            rootObj.transform.SetAsLastSibling();
            s_Panel = new("Panel");
            s_Panel.AddComponent<RectTransform>();
            s_Panel.transform.SetParent(rootObj.transform.Find("UICanvas"), false);
            s_Panel.AddComponent<UIRefRoot>();
            AddPanelComponent();
            EditorSceneManager.SaveScene(scene, uiPath);
            AssetDatabase.Refresh();
            Selection.activeGameObject = settings.gameObject;
        }

        public static void AddPanelComponent()
        {
            if (panel == null || panel.GetComponent<Canvas>() != null)
            {
                return;
            }
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector3.zero;
            rect.sizeDelta = Vector2.zero;
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            panel.AddComponent<Canvas>().vertexColorAlwaysGammaSpace = true;
            panel.AddComponent<GraphicRaycaster>();
        }

        public static void DestroyPanelComponent()
        {
            if (panel == null || panel.GetComponent<Canvas>() == null)
            {
                return;
            }
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector3.zero;
            rect.sizeDelta = Vector2.zero;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            Object.DestroyImmediate(panel.GetComponent<GraphicRaycaster>());
            Object.DestroyImmediate(panel.GetComponent<Canvas>());
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

            GameObject gameObject = EditorUtility.EntityIdToObject(instanceID) as GameObject;

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
                if (gameObject.GetComponentInChildren<UIRef>() != null)
                {
                    DrawStar(Color.white, selectionRect);
                }

                return;
            }

            DrawStar(component.isCopyRefStr ? Color.yellow : Color.green, selectionRect);
        }

        private static void DrawStar(Color color, Rect rect)
        {
            GUIStyle labelStyle = new(EditorStyles.label);
            labelStyle.normal.textColor = color;
            float x = rect.x + rect.width - 15f;
            float y = rect.y + 2f;
            float width = 15f;
            float height = rect.height;
            GUI.Label(new Rect(x, y, width, height), "*", labelStyle);
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
                Debug.LogError("界面名字未设置正确");
                Selection.activeGameObject = uiRefSetting.gameObject;
                return false;
            }

            GameObject gameObject = GameObject.Find("UIRoot/UICanvas/Panel");
            UIRefRoot[] uiRefRoots = gameObject.GetComponentsInChildren<UIRefRoot>(true);
            List<UIRef> uiRefs = new();

            foreach (UIRefRoot uiRefRoot in uiRefRoots)
            {
                GenUIRefRootObjs(uiRefRoot, uiRefs);
            }

            EditorUtility.SetDirty(gameObject);
            s_CSharpExporter.Export(uiRefs.ToArray(), uiRefSetting);
            return true;
        }

        private static bool GenUIRefRootObjs(UIRefRoot uiRefRoot, List<UIRef> rootUIRefs)
        {
            UIRef[] uiRefs = uiRefRoot.GetComponentsInChildren<UIRef>(true);
            HashSet<string> repeat = new();
            List<UnityObject> listComponent = new();
            bool rootIsListItem = false;

            foreach (var uiRef in uiRefs)
            {
                if (!uiRef.IsScrollList() && !uiRef.IsStaticList())
                {
                    uiRef.isList = false;
                }

                if (!uiRef.IsListItemVariable())
                {
                    uiRef.isListItemVariable = false;
                }

                if (!uiRef.IsListItem())
                {
                    uiRef.isListItem = false;

                    if (uiRef.gameObject == uiRefRoot.gameObject)
                    {
                        Object.DestroyImmediate(uiRefRoot);
                        return true;
                    }
                }

                if (!rootIsListItem && uiRef.gameObject == uiRefRoot.gameObject && uiRef.isListItem)
                {
                    rootIsListItem = true;
                }

                bool isListItem = uiRef.isListItem;
                bool isListItemVariable = !rootIsListItem && uiRef.isListItemVariable;

                if (uiRef.isCopyRefStr || isListItem || isListItemVariable)
                {
                    continue;
                }

                string name = uiRef.GetName();
                if (!repeat.Add(name))
                {
                    string errorStr = string.Concat(
                        "有重复的引用名称 => ",
                        uiRef.refName,
                        "; 引用对象=>",
                        EditorUtil.GetHierarchy(uiRef.gameObject),
                        "; ",
                        EditorUtil.GetHierarchy(uiRef.gameObject)
                    );

                    Debug.LogError(errorStr);
                    Selection.activeGameObject = uiRef.gameObject;
                    return false;
                }

                if (string.IsNullOrEmpty(uiRef.componentName) || uiRef.componentName == nameof(Transform))
                {
                    listComponent.Add(uiRef.transform);
                }
                else if (uiRef.componentName == nameof(RectTransform))
                {
                    listComponent.Add(uiRef.GetComponent<RectTransform>());
                }
                else if (uiRef.componentName == nameof(GameObject))
                {
                    listComponent.Add(uiRef.gameObject);
                }
                else
                {
                    listComponent.Add(uiRef.GetComponent(uiRef.componentFullName));
                }

                rootUIRefs.Add(uiRef);
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
            List<UIRef> listRef = new();
            HashSet<string> hashSet = new();

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
                    Debug.LogError("有重复的引用名称 => " + component.refName + "; 引用对象=>" + path);
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }
            }

            foreach (var component in components)
            {
                if (component.isListItemVariable) continue;
                listRef.Add(component);
            }

            string value = s_CSharpExporter.CopyRef(listRef.ToArray());

            if (string.IsNullOrEmpty(value))
            {
                Debug.LogWarning("没有需要导出到剪切板的对象");
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

            string path = WuWuPathUtil.FormatPath(EditorMgr.GetWuWuFrameworkConfig().uiPrefabsPath, s_UIRefSetting.viewName, ".prefab");

            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog("存在资源", "已经存在资源 " + path + " 是否替换", "替换", "取消"))
                {
                    return null;
                }
            }

            GameObject root = GameObject.Find("UIRoot");
            GameObject panel = root.transform.Find("UICanvas/Panel").gameObject;
            WuWuFileUtil.VerifyDirectory(Path.GetDirectoryName(path));
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(panel, path, out bool isSuccess);

            if (!isSuccess)
            {
                return null;
            }

            UIRef[] components = prefab.GetComponentsInChildren<UIRef>(true);

            foreach (var component in components)
            {
                UnityObject.DestroyImmediate(component, true);
            }

            EditorSceneManager.SaveScene(scene);
            return path;
        }

        private static bool IsUIScene()
        {
            return UnityObject.FindAnyObjectByType<UIRefSetting>() != null;
        }

        private static bool CanExport()
        {
            if (!IsUIScene())
            {
                Debug.LogError("当前场景不是UI场景 Scene => " + scene.name);
                return false;
            }

            GameObject root = GameObject.Find("UIRoot");

            if (root == null || root.transform.Find("UICanvas/Panel") == null)
            {
                Debug.LogError(root + "场景UI资源错误 Scene => " + scene.name + "===没有Panel对象");
                return false;
            }

            return true;
        }
    }
}