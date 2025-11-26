using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class EditorMgr
    {
        static EditorMgr() { }

        public static GameFrameWorkConfigWindowData GetGameFrameWorkConfig()
        {
            GameFrameWorkConfigWindowData config = AssetDatabase.LoadAssetAtPath<GameFrameWorkConfigWindowData>(EditorPathUtil.GameFrameWorkConfigWindowDataPath);
            return config;
        }

        [MenuItem("GameFrameWork/Start Up &1", false, 0)]
        public static void GameFrameWorkStartUp()
        {
            CreateEntryScript();
        
            Rect rect = new(0, 0, 600, 300);
            EditorWindow window = EditorWindow.GetWindowWithRect<GameFrameWorkConfigWindow>(rect);
            window.Show();
        }
        
        /// <summary>
        /// 创建框架启动脚本
        /// </summary>
        private static void CreateEntryScript()
        {
            string[] entryScript = EditorUtil.GetAssemblyTypeNames("GameFrameWork.GameFrameWorkEntry", true, "GameFrameWorkEntry");
        
            if (entryScript == null || entryScript.Length < 1)
            {
                StringBuilder sb = new();
                sb.AppendLine("using GameFrameWork;");
                sb.AppendLine("using UnityEngine;");
                sb.AppendLine();
                sb.AppendLine("public class GameEntry : GameFrameWorkEntry");
                sb.AppendLine("{");
                sb.AppendLine("\tprotected override void OnInit(GameObject manager)");
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine();
                sb.AppendLine("\tprotected override void OnStartGame()");
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine();
                sb.AppendLine("\tprotected override void OnExit()");
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine();
                sb.Append("}");
        
                Utils.FileUtil.VerifyDirectory(EditorPathUtil.EditorScriptFullPath);
                File.WriteAllText(EditorPathUtil.EntryScriptFullPath, sb.ToString());
                AssetDatabase.Refresh();
            }
        }
        
        /// <summary>
        /// 主要用于第一次启动Unity编辑器和编译完成后，检查是否已经设置框架启动场景
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            int isShowMainScene = EditorPrefs.GetInt("unity_editor_show_main_scene", 0);
        
            if (isShowMainScene == 0)
            {
                EditorApplication.update += CheckIsInit;
            }
        
            EditorApplication.wantsToQuit += ApplicationWantsToQuit;
        }
        
        private static void CheckIsInit()
        {
            if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
            {
                return;
            }
        
            EditorSceneManager.sceneOpened += CheckEntryScene;
            EditorApplication.update -= CheckIsInit;
            CheckEntryScene();
        }
        
        private static void CheckEntryScene(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            EditorSceneManager.sceneOpened -= CheckEntryScene;
            CheckEntryScene();
        }
        
        private static void CheckEntryScene()
        {
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            int isShowMainScene = EditorPrefs.GetInt("unity_editor_show_main_scene", 0);
        
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                EditorPrefs.SetInt("unity_editor_show_main_scene", 1);
                GameFrameWorkStartUp();
                return;
            }
        
            if (isShowMainScene == 0)
            {
                EditorPrefs.SetInt("unity_editor_show_main_scene", 1);
                GoToGameFrameWorkEntryScene();
            }
        }
        
        /// <summary>
        /// 跳转到框架启动场景
        /// </summary>
        public static void GoToGameFrameWorkEntryScene()
        {
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                return;
            }
        
            if (!EditorSceneManager.GetActiveScene().path.Equals(config.entryScene))
            {
                EditorSceneManager.OpenScene(config.entryScene);
            }
        
            Type[] entryTypes = EditorUtil.GetAssemblyTypes("GameFrameWork.GameFrameWorkEntry", "GameFrameWorkEntry");
        
            if (entryTypes == null || entryTypes.Length < 1)
            {
                return;
            }
        
            GameObject entry = GameObject.Find("GameEntry");
            if (entry == null)
            {
                entry = new GameObject("GameEntry");
                entry.AddComponent(entryTypes[0]); 
            }
            
            GameObject uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(EditorPathUtil.EditorUIRootPath));
            }
        }
        
        private static bool ApplicationWantsToQuit()
        {
            EditorPrefs.SetInt("unity_editor_show_main_scene", 0);
            return true;
        }

        [MenuItem("GameFrameWork/UI列表 &2", false, 101)]
        public static void OpenUIListWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<UIListWindow>();
            window.Show();
        }

        [MenuItem("GameFrameWork/AssetBundle编辑器 &3", false, 102)]
        public static void AssetBundleEditor()
        {
            Rect wr = new(0, 0, 700, 800);
            EditorWindow window = EditorWindow.GetWindowWithRect(typeof(AssetBundleWindow), wr);
            window.Show();
        }

        [MenuItem("GameFrameWork/行为树编辑器 &4", false, 103)]
        public static void BehaviourTreeEditor()
        {
            EditorWindow window = EditorWindow.GetWindow<BehaviourTreeWindow>();
            window.Show();
        }

        [MenuItem("GameFrameWork/工具/切图工具", false, 104)]
        public static void OpenSpriteSpliterTool()
        {
            Rect rect = new(0, 0, 600, 300);
            EditorWindow window = EditorWindow.GetWindowWithRect<SpriteSplitTool>(rect);
            window.Show();
        }

        [MenuItem("GameFrameWork/工具/PlayerPrefs工具", false, 105)]
        public static void OpenPlayerPrefsTool()
        {
            Rect rect = new(0, 0, 600, 300);
            EditorWindow window = EditorWindow.GetWindowWithRect<PlayerPrefsTool>(rect);
            window.Show();
        }

        [MenuItem("GameFrameWork/Build/Build Game", false, 106)]
        public static void BuildGame()
        {
            BuildGame(false);

        }

        [MenuItem("GameFrameWork/Build/Build Game Log", false, 107)]
        public static void BuildGameLog()
        {
            BuildGame(true);
        }

        private static void BuildGame(bool openLog)
        {
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("提示", "没有设置框架启动场景，点击确认前往设置", "确认"))
                {
                    GameFrameWorkStartUp();
                }

                return;
            }

            if (!UnityEditor.EditorUtility.DisplayDialog("提示", "点击确认开始打包", "确认", "取消"))
            {
                return;
            }

            bool isLoadFromAssetBundle = config.isLoadFromAssetBundle;
            bool isOpenLog = config.isOpenLog;

            config.isLoadFromAssetBundle = true;
            config.isOpenLog = openLog;

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.Refresh();
            BuildTool.Build(EditorUserBuildSettings.activeBuildTarget, config.buildPath);

            config = GetGameFrameWorkConfig();
            config.isLoadFromAssetBundle = isLoadFromAssetBundle;
            config.isOpenLog = isOpenLog;
        }

        [MenuItem("GameFrameWork/EditorDemo/Tab", false, 1001)]
        public static void TabDemoWinow()
        {
            Rect wr = new(0, 0, 600, 600);
            TabDemo window = EditorWindow.GetWindowWithRect<TabDemo>(wr, true, "Unity Tab标签");
            window.Show();
        }

        [MenuItem("GameFrameWork/EditorDemo/Styles&Icons", false, 1002)]
        public static void BuiltInDemo()
        {
            BuiltInDemo window = EditorWindow.GetWindow<BuiltInDemo>();
            window.Show();
        }

        [MenuItem("GameFrameWork/EditorDemo/SplitView")]
        public static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow<SplitViewDemo>();
            window.Show();
        }

        [MenuItem("Assets/创建艺术字", false, 0)]
        public static void CreateFont()
        {
            FontMaker.CreateMyFontSprite();
        }

        [MenuItem("Assets/创建UI图集", false, 1)]
        public static void CreateSpriteAtlas()
        {
            SpriteAtlasPacker window = EditorWindow.GetWindow<SpriteAtlasPacker>();
            window.Show();
        }

        [MenuItem("Assets/添加场景到打包列表", false, 2)]
        public static void AddScene()
        {
            if (Selection.objects.Length > 0)
            {
                List<EditorBuildSettingsScene> sceneList = new();
                sceneList.AddRange(EditorBuildSettings.scenes);
                bool isExist = false;

                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    string assetPath = AssetDatabase.GetAssetPath(Selection.objects[i]);

                    if (!Path.GetExtension(assetPath).Equals(".unity"))
                    {
                        continue;
                    }

                    isExist = true;
                    EditorBuildSettingsScene editorBuildSettings = new(assetPath, true);
                    sceneList.Add(editorBuildSettings);
                }

                if (sceneList.Count > 0)
                {
                    EditorBuildSettings.scenes = sceneList.ToArray();
                    AssetDatabase.Refresh();
                }
                if (isExist)
                {
                    UnityEditor.EditorUtility.DisplayDialog("提示", "添加成功", "确定");
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("提示", "沒有选中任何场景文件，请选择场景文件再进行操作", "确定");
                }
            }
        }

        [MenuItem("Assets/复制路径", false, 3)]
        private static void CopyAssetsPath()
        {
            if (Selection.activeObject == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "没有选中任何物体", "确定");
                return;
            }

            EditorUtil.CopyTextEditor(AssetDatabase.GetAssetPath(Selection.activeObject));
        }


        [MenuItem("GameObject/复制路径", false, 0)]
        private static void CopyGameObjectPath()
        {
            if (Selection.activeGameObject == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "没有选中任何物体", "确定");
                return;
            }

            StringBuilder sb = new();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                string nodePath = EditorUtil.GetNodePath(Selection.gameObjects[i].transform);

                sb.Append("\"");
                sb.Append(nodePath);
                sb.Append("\"");

                if (i < Selection.gameObjects.Length - 1)
                {
                    sb.Append("\n");
                }
            }

            EditorUtil.CopyTextEditor(sb.ToString());
        }
    }
}