using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityObject = UnityEngine.Object;
using FrameWork;

[InitializeOnLoad]
public class UIUtility
{
    private static string PanelContent =
@"local $LuaClass = BaseClass(BasePanel)

--------------------------------------------------------------------
--------- 以下是自动生成的代码，所有手动更改都是无效的---------------------
function $LuaClass:OnInit(go)
    local targetObject = go:GetComponent(""UIRefRoot"").Objects
$RefContent
    self:InnerInit()
end
--- END ------------------------------------------------------------
--------------------------------------------------------------------

function $LuaClass:InnerInit()

end

return $LuaClass.New($Params)";

    private static string PanelRefContent =
@"    local targetObject = go:GetComponent(""UIRefRoot"").Objects
$RefContent
    self:InnerInit()
end";

    private static string CtrlContent =
@"local $PanelLuaClass = require ""$PanelLuaPath""

local $LuaClass = BaseCtrl.New($PanelLuaClass)

function $LuaClass:OnInit(obj)

end

return $LuaClass";

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

    private static void DuringSceneGUI(SceneView scnView)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && UIUtility.IsUIScene())
        {
            GUI.color = Color.green;
            Handles.BeginGUI();

            if (GUI.Button(new Rect(0f, (float)(Screen.height - 30 - 40), 70f, 30f), "生成预制体"))
            {
                string text = UIUtility.ExportUIPrefab(true);
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                //Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(text);
            }

            GUI.color = Color.white;
            if (GUI.Button(new Rect((float)(Screen.width - 200), (float)(Screen.height - 70), 100f, 30f), "输出引用到剪切板"))
            {
                UIUtility.ExportClipboardRef();
                Event.current.Use();
            }
            if (GUI.Button(new Rect((float)(Screen.width - 70), (float)(Screen.height - 70), 70f, 30f), "添加引用"))
            {
                UIUtility.AddUIRef();
                Event.current.Use();
            }
            Handles.EndGUI();
        }
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || !UIUtility.IsUIScene()) return;

        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null || gameObject.transform.parent == null) return;

        UIRef component = gameObject.GetComponent<UIRef>();
        if (component == null) return;

        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        GUIStyle labelStyle2 = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.green;
        labelStyle2.normal.textColor = Color.yellow;

        GUI.Label(new Rect(selectionRect.x + selectionRect.width - 15f, selectionRect.y + 2f, 15f,
            selectionRect.height), "*", component.OutputClipBoard ? labelStyle2 : labelStyle);
    }

    private static bool ExportClipboardRef()
    {
        bool result;
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("没有选中节点对象");
            result = false;
        }
        else
        {
            string hierarchy = FrameWorkEditorMgr.GetHierarchy(Selection.activeGameObject);
            if (!hierarchy.StartsWith("UIRoot\\UICanvas\\Panel"))
            {
                Debug.LogError("应该选择Panel下面的对象作为根节点");
                result = false;
            }
            else
            {
                UIRef[] components = Selection.activeGameObject.GetComponentsInChildren<UIRef>(true);
                HashSet<string> hashSet = new HashSet<string>();
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < components.Length; i++)
                {
                    UIRef component = components[i];
                    if (!component.OutputClipBoard) continue;

                    string name = component.GetName();
                    string text = FrameWorkEditorMgr.GetHierarchy(component.gameObject);
                    if (!hashSet.Add(name))
                    {
                        Debug.LogError("有重复的引用名称 => " + component.Name + "; 引用对象=>" + text);
                        Selection.activeGameObject = component.gameObject;
                        result = false;
                        return result;
                    }
                    string value;
                    if (text == hierarchy)
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
                        text = text.Substring(hierarchy.Length + 1).Replace("\\", "/");
                        if (string.IsNullOrEmpty(component.ComponentName) || component.ComponentName == "Transform")
                        {
                            value = string.Format("{0} = targetTrans:Find(\"{1}\");", name, text);
                        }
                        else if (component.ComponentName == "GameObject")
                        {
                            value = string.Format("{0} = targetTrans:Find(\"{1}\").gameObject;", name, text);
                        }
                        else
                        {
                            value = string.Format("{0} = targetTrans:Find(\"{1}\"):GetComponent(\"{2}\");", name, text, component.ComponentName);
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
                result = true;
            }
        }
        return result;
    }

    private static void WriteAllText(string filePath, string text)
    {
        File.WriteAllText(filePath, new UTF8Encoding(false).GetString(Encoding.UTF8.GetBytes(text)));
    }

    private static bool ExportUIRef()
    {
        UIRefSetting mUIRefSetting = UIUtility.UIRefSetting;
        bool result;
        if (mUIRefSetting == null) return false;
        
        if (string.IsNullOrEmpty(mUIRefSetting.panelName)|| mUIRefSetting.panelName.Contains("/"))
        {
            Debug.LogError("界面名字未设置正确");
            Selection.activeGameObject = mUIRefSetting.gameObject;
            return false;
        }
        else
        {
            StringBuilder content = new StringBuilder();

            GameObject gameObject = GameObject.Find("UIRoot/UICanvas/Panel");
            UIRefRoot rootComponent = gameObject.GetComponent<UIRefRoot>();
            UIRef[] components = gameObject.GetComponentsInChildren<UIRef>(true);
            Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
            List<UnityObject> list = new List<UnityObject>();
            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (component.OutputClipBoard) continue;

                string name = component.GetName();
                if (dictionary.ContainsKey(name))
                {
                    Debug.LogError(string.Concat(new string[]
                    {
                        "有重复的引用名称 => ",
                        component.Name,
                        "; 引用对象=>",
                        FrameWorkEditorMgr.GetHierarchy(component.gameObject),
                        "; ",
                        FrameWorkEditorMgr.GetHierarchy(dictionary[name].gameObject)
                    }));
                    Selection.activeGameObject = component.gameObject;
                    return false;
                }

                dictionary.Add(name, component.gameObject);
                UnityEngine.Object item;
                string str;
                if (string.IsNullOrEmpty(component.ComponentName) || component.ComponentName == typeof(Transform).Name)
                {
                    item = component.transform;
                    str = "Transform";
                }
                else if(component.ComponentName == typeof(RectTransform).Name)
                {
                    item = component.GetComponent<RectTransform>();
                    str = "RectTransform";
                }
                else if (component.ComponentName == typeof(GameObject).Name)
                {
                    item = component.gameObject;
                    str = "GameObject";
                }
                else
                {
                    item = component.GetComponent(component.ComponentName);
                    str = component.ComponentName;
                }
                list.Add(item);
                string text = FrameWorkEditorMgr.GetHierarchy(component.gameObject).Substring("UIRoot/UICanvas/Panel".Length + 1).Replace("\\", "/") + ", " + str;
                if (!string.IsNullOrEmpty(component.Desc))
                {
                    text = component.Desc + "(" + text + ")";
                }
                content.Append("\t-- ").Append(text).Append("\n");
                content.Append("\t").Append(string.Format("self.{0} = targetObject[{1}]", name, list.Count - 1)).Append("\n");
            }

            rootComponent.Objects = list.ToArray();
            EditorUtility.SetDirty(rootComponent);

            string panelPath = mUIRefSetting._panelPath;
            string ctrlPath = mUIRefSetting._ctrlPath;

            if (!File.Exists(panelPath))
            {
                //ABPath.VerifyDirectory(Path.GetDirectoryName(panelPath));

                string className = Path.GetFileNameWithoutExtension(panelPath);
                StringBuilder head = new StringBuilder();
                head.Append(PanelContent);

                head.Replace("$LuaClass", Path.GetFileNameWithoutExtension(panelPath));
                head.Replace("$RefContent", content.ToString());
                head.Replace("$Params", mUIRefSetting.CreateParameters());

                UIUtility.WriteAllText(panelPath, head.ToString());
            }
            else
            {
                List<string> list2 = new List<string>();
                string[] array = File.ReadAllLines(panelPath);
                bool? flag = new bool?(false);
                string[] array2 = array;
                for (int j = 0; j < array2.Length; j++)
                {
                    string text6 = array2[j];
                    if (!flag.HasValue)
                    {
                        list2.Add(text6);
                    }
                    else if (flag.Value)
                    {
                        if (text6 == "end")
                        {
                            flag = null;
                        }
                    }
                    else
                    {
                        list2.Add(text6);
                        if (Regex.Match(text6, "function(\\s+)(\\w+):OnInit\\(go\\)").Success)
                        {
                            flag = new bool?(true);
                            list2.Add(PanelRefContent.Replace("$RefContent", content.ToString()));
                        }
                    }
                }

                list2[list2.Count - 1] = string.Format("return {0}.New({1})",
                    Path.GetFileNameWithoutExtension(panelPath), mUIRefSetting.CreateParameters());
                UIUtility.WriteAllText(panelPath, string.Join("\n", list2.ToArray()));
            }

            if (!File.Exists(ctrlPath))
            {
                StringBuilder panelPathDot = new StringBuilder(panelPath);
                panelPathDot.Replace("Assets/LuaScripts/", "");
                panelPathDot.Replace(".lua", "");
                panelPathDot.Replace("/", ".");

                StringBuilder controlContent = new StringBuilder(CtrlContent);
                controlContent.Replace("$PanelLuaClass", Path.GetFileNameWithoutExtension(panelPath));
                controlContent.Replace("$PanelLuaPath", panelPathDot.ToString());
                controlContent.Replace("$LuaClass", Path.GetFileNameWithoutExtension(ctrlPath));

                UIUtility.WriteAllText(ctrlPath, controlContent.ToString());
            }
            result = true;
        }
        return result;
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
        if (obj != null)
        {
            UIRef uiRef = obj.GetOrAddComponent<UIRef>();
            uiRef.ComponentName = typeof(GameObject).Name;

            uiRef.UseObjName = true;
            uiRef.SetObjName(obj.name);
        }
    }

    private static bool IsUIScene()
    {
        return GameObject.FindObjectOfType<UIRefSetting>() != null;
    }

    private static string GetDefaultName(string name)
    {
        int num = 1;
        string arg = name;
        string arg2 = string.Empty;
        int num2 = name.LastIndexOf('.');
        if (num2 != -1)
        {
            arg = name.Substring(0, num2);
            arg2 = name.Substring(num2);
        }
        string text = name;
        while (File.Exists(text))
        {
            text = string.Format("{0} {1}{2}", arg, num++, arg2);
        }
        return text;
    }

    private static string ExportUIPrefab(bool showExist = true)
    {
        string result;
        if (!IsUIScene())
        {
            Debug.LogError("当前场景不是UI场景 Scene => " + scene.name);
            result = null;
        }
        else
        {
            GameObject gameObject = GameObject.Find("UIRoot");
            if (gameObject == null || gameObject.transform.Find("UICanvas/Panel") == null)
            {
                Debug.LogError(gameObject + "场景UI资源错误 Scene => " + scene.name + "===没有Panel对象");
                return null;
            }

            if (!UIUtility.ExportUIRef())
            {
                result = null;
            }
            else
            {
                string path = UIRefSetting._resPath;

                if (File.Exists(path))
                {
                    if (!EditorUtility.DisplayDialog("存在资源", "已经存在资源 " + path + " 是否替换", "替换", "取消"))
                    {
                        result = null;
                        return result;//
                    }
                }

                GameObject gameObject2 = gameObject.transform.Find("Panel").gameObject;
                //ABPath.VerifyDirectory(Path.GetDirectoryName(path));
                GameObject gameObject3;
                if (File.Exists(path))
                {
                    gameObject3 = PrefabUtility.ReplacePrefab(gameObject2, AssetDatabase.LoadMainAssetAtPath(path), 0);
                }
                else
                {
                    gameObject3 = PrefabUtility.CreatePrefab(path, gameObject2);
                }

                UIRef[] componentsInChildren = gameObject3.GetComponentsInChildren<UIRef>(true);
                UIRef[] array = componentsInChildren;
                for (int i = 0; i < array.Length; i++)
                {
                    UIRef uIRef = array[i];
                    UnityEngine.Object.DestroyImmediate(uIRef, true);
                }
                EditorSceneManager.SaveScene(scene);
                result = path;
            }
        }
        return result;
    }

    public static void NewUIScene()
    {
        string path = EditorUtility.SaveFilePanelInProject("创建新的UI场景", "NewUI", "unity", "Save Scene as...");

        if (string.IsNullOrEmpty(path)) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        GameObject.DestroyImmediate(GameObject.Find("Main Camera"));
        GameObject.DestroyImmediate(GameObject.Find("Directional Light"));

        UnityObject root = UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<UnityObject>("Assets/FrameWork/UI/UIRoot.prefab"));
        root.name = "UIRoot";

        UIRefSetting settings = new GameObject("UI Scene Setting").AddComponent<UIRefSetting>();
        settings.panelName = Path.GetFileNameWithoutExtension(path);
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
}
